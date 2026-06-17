using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Core;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class NamingConventionAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainNamingRule()
    {
        var analyzer = new NamingConventionAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.NamingConventionViolation);
    }

    [Fact]
    public async Task PrivateField_NotStartingWithUnderscore_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                private int count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.NamingConventionViolation);
    }

    [Fact]
    public async Task PrivateField_StartingWithUnderscore_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                private int _count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.NamingConventionViolation);
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
        return await compilation.WithAnalyzers([new NamingConventionAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
