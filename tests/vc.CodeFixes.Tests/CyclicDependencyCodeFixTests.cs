using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Architecture;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

public sealed class CyclicDependencyCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForCyclicDependency()
    {
        var source = """
            namespace Sample.A
            {
                public sealed class AType
                {
                    private readonly Sample.B.BType _field = new Sample.B.BType();
                }
            }

            namespace Sample.B
            {
                public sealed class BType
                {
                    private readonly Sample.A.AType _field = new Sample.A.AType();
                }
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(node => node.Identifier.Text == "AType");

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.ArchCyclicDependency,
                "Cyclic namespace dependency detected",
                "Namespace '{0}' depends on '{1}', forming a cyclic dependency.",
                "Architecture",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            classNode.Identifier.GetLocation(),
            "Sample.A",
            "Sample.B");

        var provider = new CyclicDependencyCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.ArchCyclicDependency, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "CyclicDependency.cs", SourceText.From(source));
    }
}