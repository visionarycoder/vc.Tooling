using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Design;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ExceptionSafetySwallowedCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForSwallowedException()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public string SafeParse(string input)
                    {
                        try
                        {
                            return int.Parse(input).ToString();
                        }
                        catch
                        {
                            return "failed";
                        }
                    }
                }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "SafeParse");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.DesignExceptionSafetySwallowed,
                title: "Swallowed exception",
                messageFormat: "Exception in method '{0}' is swallowed without rethrow or logging.",
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(),
            messageArgs: "SafeParse");

        var provider = new ExceptionSafetySwallowedCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.DesignExceptionSafetySwallowed, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "ExceptionSafetySwallowed.cs", text: SourceText.From(text: source));
    }
}
