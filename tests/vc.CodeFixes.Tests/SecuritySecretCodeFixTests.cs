using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Security;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

public sealed class SecuritySecretCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForHardcodedSecret()
    {
        var source = """
            namespace Sample.Security
            {
                public sealed class ApiConfig
                {
                    private const string ApiKey = "sk-1234567890abcdef";
                }
            }
            """;

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var fieldNode = root!
            .DescendantNodes()
            .OfType<FieldDeclarationSyntax>()
            .First(node => node.Declaration.Variables.Any(v => v.Identifier.Text == "ApiKey"));

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.SecurityHardcodedSecret,
                "Hardcoded secret detected",
                "Field contains hardcoded secret value.",
                "Security",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            fieldNode.GetLocation());

        var provider = new SecuritySecretCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.SecurityHardcodedSecret, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "SecuritySecret.cs", SourceText.From(source));
    }
}
