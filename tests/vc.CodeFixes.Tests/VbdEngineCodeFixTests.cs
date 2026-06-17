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

public sealed class VbdEngineCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForInfrastructureAccess()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private readonly System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.VbdEngineVaultInfrastructureAccess,
                "Engine accesses infrastructure",
                "Engine component '{0}' references infrastructure namespace '{1}'.",
                "VbdEngine",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            classNode.Identifier.GetLocation(),
            classNode.Identifier.Text,
            "System.Net.Http");

        var provider = new VbdEngineCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdEngineVaultInfrastructureAccess, updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForNondeterminism()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                public string ComputeId() => System.Guid.NewGuid().ToString();
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
                DiagnosticIds.VbdEngineVaultNondeterminism,
                "Engine contains nondeterministic operation",
                "Engine method '{0}' uses nondeterministic API '{1}'.",
                "VbdEngine",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            methodNode.Identifier.GetLocation(),
            methodNode.Identifier.Text,
            "NewGuid");

        var provider = new VbdEngineNondeterminismCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdEngineVaultNondeterminism, updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForStateViolation()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private int _count = 0;
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var fieldNode = root!
            .DescendantNodes()
            .OfType<FieldDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.VbdEngineVaultStateViolation,
                "Engine has mutable state",
                "Engine type '{0}' contains mutable field '{1}'.",
                "VbdEngine",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            fieldNode.Declaration.Variables[0].Identifier.GetLocation(),
            "RiskEngine",
            fieldNode.Declaration.Variables[0].Identifier.Text);

        var provider = new VbdEngineStateCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdEngineVaultStateViolation, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location));

        return workspace.AddDocument(project.Id, "RiskEngine.cs", SourceText.From(source));
    }

}