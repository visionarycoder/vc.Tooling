using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.Generators.Common.Builders;
using Vc.Generators.Common.Extensions;
using Vc.Generators.Common.Models;
using Vc.Generators.Common.Diagnostics;

namespace Vc.Generators.Domain;

[Generator(LanguageNames.CSharp)]
public sealed class VcStateMachineGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                static (ctx, ct) => Transform(ctx, ct))
            .Where(static m => m is not null)!;

        var compilationAndModels = context.CompilationProvider.Combine(candidates.Collect());

        context.RegisterSourceOutput(compilationAndModels, static (spc, pair) =>
        {
            var (compilation, models) = pair;
            if (models.IsDefaultOrEmpty)
            {
                return;
            }

            foreach (var model in models)
            {
                EmitStateMachine(spc, compilation, model);
            }
        });
    }

    private static StateMachineModel? Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (context.Node is not ClassDeclarationSyntax classDecl)
        {
            return null;
        }

        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl, ct) as INamedTypeSymbol;
        if (symbol is null)
        {
            return null;
        }

        if (!symbol.HasAttribute("StateMachineAttribute") &&
            !symbol.HasAttribute("StateMachine"))
        {
            return null;
        }

        var ns = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        var typeName = symbol.Name;

        var states = ImmutableArray.CreateBuilder<StateModel>();
        var transitions = ImmutableArray.CreateBuilder<TransitionModel>();

        foreach (var member in symbol.GetTypeMembers())
        {
            if (!member.HasAttribute("StateAttribute") &&
                !member.HasAttribute("State"))
            {
                continue;
            }

            var attr = member.GetAttribute("StateAttribute") ?? member.GetAttribute("State");
            var nameArg = attr?.ConstructorArguments.Length > 0
                ? attr.Value.ConstructorArguments[0].Value as string
                : null;

            var isInitial = GetNamedBool(attr, "IsInitial");
            var isFinal = GetNamedBool(attr, "IsFinal");

            states.Add(new StateModel(
                member.Name,
                nameArg ?? member.Name,
                isInitial,
                isFinal));
        }

        foreach (var member in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (!member.HasAttribute("TransitionAttribute") &&
                !member.HasAttribute("Transition"))
            {
                continue;
            }

            var attr = member.GetAttribute("TransitionAttribute") ?? member.GetAttribute("Transition");
            if (attr is null || attr.ConstructorArguments.Length < 2)
            {
                continue;
            }

            var from = attr.Value.ConstructorArguments[0].Value as string;
            var to = attr.Value.ConstructorArguments[1].Value as string;

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                continue;
            }

            transitions.Add(new TransitionModel(
                from!,
                to!,
                member.Name,
                member.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                member.IsAsync));
        }

        return new StateMachineModel(
            ns,
            typeName,
            states.ToImmutable(),
            transitions.ToImmutable());
    }

    private static bool GetNamedBool(AttributeData? attr, string name)
    {
        if (attr is null)
        {
            return false;
        }

        foreach (var kvp in attr.NamedArguments)
        {
            if (string.Equals(kvp.Key, name, StringComparison.OrdinalIgnoreCase) &&
                kvp.Value.Value is bool b)
            {
                return b;
            }
        }

        return false;
    }

    private static void EmitStateMachine(SourceProductionContext context, Compilation compilation, StateMachineModel model)
    {
        if (model.States.IsDefaultOrEmpty)
        {
            var diag = Diagnostic.Create(
                VcGeneratorDiagnostics.InvalidAttributeUsage,
                Location.None,
                "StateMachine must declare at least one [State].");
            context.ReportDiagnostic(diag);
            return;
        }

        var fileName = $"{model.TypeName}.StateMachine.g.cs";

        var sb = new IndentedStringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(model.Namespace))
        {
            sb.AppendLine($"namespace {model.Namespace};");
            sb.AppendLine();
        }

        var enumName = $"{model.TypeName}State";
        var machineClassName = $"{model.TypeName}StateMachine";

        // Enum
        sb.AppendLine($"public enum {enumName}");
        sb.AppendLine("{");
        sb.Indent();
        for (var i = 0; i < model.States.Length; i++)
        {
            var state = model.States[i];
            var comma = i < model.States.Length - 1 ? "," : string.Empty;
            sb.AppendLine($"{state.SymbolName}{comma}");
        }

        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();

        // State machine class
        sb.AppendLine($"public static class {machineClassName}");
        sb.AppendLine("{");
        sb.Indent();

        EmitInitialState(sb, enumName, model);
        sb.AppendLine();
        EmitFinalStates(sb, enumName, model);
        sb.AppendLine();
        EmitTransitionMap(sb, enumName, model);
        sb.AppendLine();
        EmitCanTransition(sb, enumName);
        sb.AppendLine();
        EmitTryTransition(sb, enumName);
        sb.AppendLine();
        EmitAsyncHandlers(sb, enumName, model);

        sb.Unindent();
        sb.AppendLine("}");

        context.AddSource(fileName, SourceText.From(sb.ToString(), System.Text.Encoding.UTF8));
    }

    private static void EmitInitialState(IndentedStringBuilder sb, string enumName, StateMachineModel model)
    {
        var initial = model.States.FirstOrDefault(s => s.IsInitial) ?? model.States[0];
        sb.AppendLine($"public static {enumName} InitialState => {enumName}.{initial.SymbolName};");
    }

    private static void EmitFinalStates(IndentedStringBuilder sb, string enumName, StateMachineModel model)
    {
        sb.AppendLine($"public static System.Collections.Generic.IReadOnlySet<{enumName}> FinalStates {{ get; }} =");
        sb.AppendLine("    new System.Collections.Generic.HashSet<" + enumName + ">");
        sb.AppendLine("    {");
        sb.Indent();

        foreach (var state in model.States.Where(s => s.IsFinal))
        {
            sb.AppendLine($"{enumName}.{state.SymbolName},");
        }

        sb.Unindent();
        sb.AppendLine("    };");
    }

    private static void EmitTransitionMap(IndentedStringBuilder sb, string enumName, StateMachineModel model)
    {
        sb.AppendLine($"private static readonly System.Collections.Generic.Dictionary<{enumName}, System.Collections.Generic.HashSet<{enumName}>> _transitions = new()");
        sb.AppendLine("{");
        sb.Indent();

        var grouped = model.Transitions
            .GroupBy(t => t.From, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var group in grouped)
        {
            sb.AppendLine($"[{enumName}.{group.Key}] = new System.Collections.Generic.HashSet<{enumName}>");
            sb.AppendLine("{");
            sb.Indent();

            foreach (var transition in group)
            {
                sb.AppendLine($"{enumName}.{transition.To},");
            }

            sb.Unindent();
            sb.AppendLine("},");
        }

        sb.Unindent();
        sb.AppendLine("};");
    }

    private static void EmitCanTransition(IndentedStringBuilder sb, string enumName)
    {
        sb.AppendLine($"public static bool CanTransition({enumName} from, {enumName} to)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("if (!_transitions.TryGetValue(from, out var targets))");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("return false;");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("return targets.Contains(to);");
        sb.Unindent();
        sb.AppendLine("}");
    }

    private static void EmitTryTransition(IndentedStringBuilder sb, string enumName)
    {
        sb.AppendLine($"public static bool TryTransition({enumName} from, {enumName} to, out {enumName} result)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("if (!CanTransition(from, to))");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("result = from;");
        sb.AppendLine("return false;");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("result = to;");
        sb.AppendLine("return true;");
        sb.AppendLine("}");
        sb.Unindent();
        sb.AppendLine("}");
    }

    private static void EmitAsyncHandlers(IndentedStringBuilder sb, string enumName, StateMachineModel model)
    {
        if (model.Transitions.IsDefaultOrEmpty)
        {
            return;
        }

        sb.AppendLine($"public static async System.Threading.Tasks.Task<{enumName}> ExecuteAsync(object instance, {enumName} from, {enumName} to, System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("if (!CanTransition(from, to))");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("return from;");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("var type = instance.GetType();");
        sb.AppendLine("var methodName = GetHandlerName(from, to);");
        sb.AppendLine("var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);");
        sb.AppendLine("if (method is null)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("return to;");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("var result = method.Invoke(instance, method.GetParameters().Length == 1");
        sb.AppendLine("    ? new object?[] { cancellationToken }");
        sb.AppendLine("    : System.Array.Empty<object?>());");
        sb.AppendLine();
        sb.AppendLine("if (result is System.Threading.Tasks.Task task)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("await task.ConfigureAwait(false);");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("return to;");
        sb.Unindent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine($"private static string GetHandlerName({enumName} from, {enumName} to)");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("return $\"On{from}To{to}\";");
        sb.Unindent();
        sb.AppendLine("}");
    }

    private sealed record StateMachineModel(
        string Namespace,
        string TypeName,
        ImmutableArray<StateModel> States,
        ImmutableArray<TransitionModel> Transitions);

    private sealed record StateModel(
        string SymbolName,
        string DisplayName,
        bool IsInitial,
        bool IsFinal);

    private sealed record TransitionModel(
        string From,
        string To,
        string HandlerName,
        string ReturnType,
        bool IsAsync);
}
