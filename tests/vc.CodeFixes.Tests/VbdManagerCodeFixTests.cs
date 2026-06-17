using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.Analyzers.Design.Vbd;
using Vc.CodeFixes.Design.Vbd;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

public sealed class VbdManagerCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForUnstableContract()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public OrderDto BuildOrder() => new();

                public void Orchestrate()
                {
                    BuildOrder();
                }
            }

            public sealed class OrderDto
            {
            }
            """;

        var document = await CreateDocumentAsync(source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document);
        var diagnostic = Assert.Single(diagnostics.Where(d => d.Id == DiagnosticIds.VbdManagerVaultUnstableContract));

        var provider = new VbdManagerCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdManagerVaultUnstableContract, updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForFeatureSpecificLogic()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void ExecuteFeatureV2()
                {
                }
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.VbdManagerVaultFeatureSpecificLogic,
                "Manager contains feature-specific logic",
                "Manager method '{0}' appears feature-specific; extract to policy/strategy.",
                "VbdManager",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            methodNode.Identifier.GetLocation(),
            methodNode.Identifier.Text);

        var provider = new VbdManagerFeatureCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdManagerVaultFeatureSpecificLogic, updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForMissingOrchestration()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void Handle()
                {
                }
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.VbdManagerVaultMissingOrchestration,
                "Manager missing orchestration",
                "Manager type '{0}' has no orchestration-style method calls.",
                "VbdManager",
                DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            classNode.Identifier.GetLocation(),
            classNode.Identifier.Text);

        var provider = new VbdManagerOrchestrationCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdManagerVaultMissingOrchestration, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "OrderManager.cs", SourceText.From(source));
    }

    private static async Task<IReadOnlyList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Document document)
    {
        var compilation = await document.Project.GetCompilationAsync();
        return await compilation!.WithAnalyzers([new VbdManagerAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}