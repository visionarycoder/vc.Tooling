using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Security;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class SqlInjectionAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainSqlInjectionRule()
    {
        var analyzer = new SqlInjectionAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.SecuritySqlInjection);
    }

    [Fact]
    public async Task CommandTextWithStringConcatenation_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Repository
            {
                public string CommandText { get; set; } = "";

                public void GetUser(string userId)
                {
                    CommandText = "SELECT * FROM Users WHERE Id = " + userId;
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Contains(collection: diagnostics, filter: d => d.Id == DiagnosticIds.SecuritySqlInjection);
    }

    [Fact]
    public async Task CommandTextWithLiteral_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Repository
            {
                public void GetUser()
                {
                    string CommandText = "SELECT * FROM Users WHERE Id = @Id";
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.DoesNotContain(collection: diagnostics, filter: d => d.Id == DiagnosticIds.SecuritySqlInjection);
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
        return await compilation.WithAnalyzers(analyzers: [new SqlInjectionAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
