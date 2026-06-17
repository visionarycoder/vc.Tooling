using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Performance;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

public sealed class PerformanceLinqCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageForLinqInHotPath()
    {
        var source = """
            namespace SampleApp;
            public sealed class DataProcessor
            {
                public int Count(System.Collections.Generic.List<int> items) => items.Count;
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(DiagnosticIds.PerfLinqInHotPath, "LINQ in hot path",
                "LINQ call '{0}' in performance-critical code.", "Performance",
                DiagnosticSeverity.Info, isEnabledByDefault: true),
            methodNode.Identifier.GetLocation(), "Count");

        var provider = new PerformanceLinqCodeFix();
        CodeAction? codeAction = null;
        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedText = await applyOperation.ChangedSolution.GetDocument(document.Id)!.GetTextAsync();

        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.PerfLinqInHotPath, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));
        return workspace.AddDocument(project.Id, "Sample.cs", SourceText.From(source));
    }
}
