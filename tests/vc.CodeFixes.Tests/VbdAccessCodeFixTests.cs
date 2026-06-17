using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Design.Vbd;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

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

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single();

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.VbdAccessVaultBusinessLogicLeakage,
                "Business logic leakage in access component",
                "Access component '{0}' appears to contain business logic method '{1}'.",
                "VbdAccess",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            methodNode.Identifier.GetLocation(),
            "OrderRepository",
            methodNode.Identifier.Text);

        var provider = new VbdAccessCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.VbdAccessVaultBusinessLogicLeakage, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "OrderRepository.cs", SourceText.From(source));
    }
}