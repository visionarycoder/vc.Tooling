using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Design.Vbd;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class VbdAccessCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForBusinessLogicLeakage()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public int ComputeRiskScore() => 42;
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
                id: DiagnosticIds.VbdAccessVaultBusinessLogicLeakage,
                title: "Business logic leakage in access component",
                messageFormat: "Access component '{0}' appears to contain business logic method '{1}'.",
                category: "VbdAccess",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(), messageArgs: ["OrderRepository", methodNode.Identifier.Text]);

        var provider = new VbdAccessCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdAccessVaultBusinessLogicLeakage, actualString: updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForBoundaryViolation()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                private readonly OrderManager _manager = new();
            }

            public sealed class OrderManager
            {
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "OrderRepository");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.VbdAccessVaultBoundaryViolation,
                title: "Access boundary violation",
                messageFormat: "Access component '{0}' depends on manager/engine type '{1}'.",
                category: "VbdAccess",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(), messageArgs: [classNode.Identifier.Text, "OrderManager"]);

        var provider = new VbdAccessBoundaryCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdAccessVaultBoundaryViolation, actualString: updatedText.ToString());
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForSchemaMappingMissing()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public OrderEntity GetEntity() => new();
            }

            public sealed class OrderEntity
            {
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "OrderRepository");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.VbdAccessVaultSchemaMappingMissing,
                title: "Schema mapping missing",
                messageFormat: "Access component '{0}' returns persistence models but exposes no map/transform method.",
                category: "VbdAccess",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(),
            messageArgs: classNode.Identifier.Text);

        var provider = new VbdAccessSchemaCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.VbdAccessVaultSchemaMappingMissing, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "OrderRepository.cs", text: SourceText.From(text: source));
    }
}