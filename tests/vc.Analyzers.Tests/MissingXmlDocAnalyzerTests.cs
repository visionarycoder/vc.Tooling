using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Core;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class MissingXmlDocAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainDocumentationRule()
    {
        var analyzer = new MissingXmlDocAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
    }

    [Fact]
    public async Task PublicMethod_WithoutXmlDoc_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                public string GetData() => "data";
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Contains(collection: diagnostics, filter: d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
    }

    [Fact]
    public async Task PublicMethod_WithXmlDoc_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            /// <summary>Service class.</summary>
            public class Service
            {
                /// <summary>Gets data.</summary>
                public string GetData() => "data";
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.DoesNotContain(collection: diagnostics, filter: d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var references = new[]
        {
            MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create(assemblyName: "AnalyzerTests", syntaxTrees: [tree], references: references,
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        return await compilation.WithAnalyzers(analyzers: [new MissingXmlDocAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
