using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Design.Vbd;
using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Vbd;
using Xunit;

namespace vc.CodeFixes.Tests;

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

        var document = await CreateDocumentAsync(source: source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document: document);
        var diagnostic = Assert.Single(collection: diagnostics.Where(predicate: d => d.Id == DiagnosticIds.VbdManagerVaultUnstableContract));

        var provider = new VbdManagerCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdManagerVaultUnstableContract, actualString: updatedText.ToString());
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

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.VbdManagerVaultFeatureSpecificLogic,
                title: "Manager contains feature-specific logic",
                messageFormat: "Manager method '{0}' appears feature-specific; extract to policy/strategy.",
                category: "VbdManager",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(),
            messageArgs: methodNode.Identifier.Text);

        var provider = new VbdManagerFeatureCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdManagerVaultFeatureSpecificLogic, actualString: updatedText.ToString());
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

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.VbdManagerVaultMissingOrchestration,
                title: "Manager missing orchestration",
                messageFormat: "Manager type '{0}' has no orchestration-style method calls.",
                category: "VbdManager",
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(),
            messageArgs: classNode.Identifier.Text);

        var provider = new VbdManagerOrchestrationCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdManagerVaultMissingOrchestration, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "OrderManager.cs", text: SourceText.From(text: source));
    }

    private static async Task<IReadOnlyList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Document document)
    {
        var compilation = await document.Project.GetCompilationAsync();
        return await compilation!.WithAnalyzers(analyzers: [new VbdManagerAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}