using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Api;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ApiDesignCodeFixProviderTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddApiControllerAttributeAndUsingDirective()
    {
        var source = """
            namespace SampleApp;

            public class WeatherController
            {
            }
            """;

        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        var document = workspace.AddDocument(projectId: project.Id, name: "WeatherController.cs", text: SourceText.From(text: source));
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().Single();

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.ApiDesignControllerMissing,
                title: "title",
                messageFormat: "message",
                category: "ApiDesign",
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(),
            messageArgs: classNode.Identifier.Text);

        var provider = new ApiDesignCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(
            document: document,
            diagnostic: diagnostic,
            registerCodeFix: (action, _) => codeAction = action,
            cancellationToken: CancellationToken.None);

        await provider.RegisterCodeFixesAsync(context: context);

        Assert.NotNull(@object: codeAction);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using Microsoft.AspNetCore.Mvc;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "[ApiController]", actualString: updatedText.ToString());
    }
}