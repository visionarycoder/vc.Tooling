using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Core;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class MissingXmlDocAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainDocumentationRule()
    {
        var analyzer = new MissingXmlDocAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
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

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
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

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.DocumentationMissingXmlDoc);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create("AnalyzerTests", [tree], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return await compilation.WithAnalyzers([new MissingXmlDocAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
