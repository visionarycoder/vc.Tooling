using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class DtoAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainDtoMutableStateRule()
    {
        var analyzer = new DtoAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.DtoMutableState);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Contains(collection: diagnostics, filter: d => d.Id == DiagnosticIds.DtoMutableState);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.DoesNotContain(collection: diagnostics, filter: d => d.Id == DiagnosticIds.DtoMutableState);
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
        return await compilation.WithAnalyzers(analyzers: [new DtoAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
