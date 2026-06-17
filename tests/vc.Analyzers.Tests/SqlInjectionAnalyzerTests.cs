using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Security;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class SqlInjectionAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainSqlInjectionRule()
    {
        var analyzer = new SqlInjectionAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.SecuritySqlInjection);
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

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.SecuritySqlInjection);
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

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.SecuritySqlInjection);
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
        return await compilation.WithAnalyzers([new SqlInjectionAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
