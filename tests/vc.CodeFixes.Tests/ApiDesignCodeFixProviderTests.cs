using System;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Workspaces;
using Vc.CodeFixes.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

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
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        var document = workspace.AddDocument(project.Id, "WeatherController.cs", SourceText.From(source));
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.ApiDesignControllerMissing,
                "title",
                "message",
                "ApiDesign",
                DiagnosticSeverity.Info,
                true),
            classNode.Identifier.GetLocation(),
            classNode.Identifier.Text);

        var provider = new ApiDesignCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => codeAction = action,
            CancellationToken.None);

        await provider.RegisterCodeFixesAsync(context);

        Assert.NotNull(codeAction);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using Microsoft.AspNetCore.Mvc;", updatedText.ToString());
        Assert.Contains("[ApiController]", updatedText.ToString());
    }
}