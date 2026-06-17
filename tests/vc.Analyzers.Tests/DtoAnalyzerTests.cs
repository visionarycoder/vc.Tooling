using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class DtoAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainDtoMutableStateRule()
    {
        var analyzer = new DtoAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.DtoMutableState);
    }

    [Fact]
    public async Task DtoWithMethod_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class UserDto
            {
                public string Name { get; set; } = "";
                public string GetFullName() => Name;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.DtoMutableState);
    }

    [Fact]
    public async Task DtoWithReadOnlyProperties_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class UserDto
            {
                public UserDto(string name, int age) { Name = name; Age = age; }
                public string Name { get; }
                public int Age { get; }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.DtoMutableState);
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
        return await compilation.WithAnalyzers([new DtoAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
