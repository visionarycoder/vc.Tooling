using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed partial class ExceptionSafetyAnalyzerTests
{
    [Fact]
    public async Task ExceptionSafetyAnalyzer_ShouldReportDiagnostic_WhenCatchBlockIsEmpty()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public void Handle()
                    {
                        try
                        {
                            var x = 1 / int.Parse("0");
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetyEmptyCatch);
    }

    [Fact]
    public async Task ExceptionSafetyAnalyzer_ShouldNotReportDiagnostic_WhenCatchBlockHasCode()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public void Handle()
                    {
                        try
                        {
                            var x = 1 / int.Parse("0");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetyEmptyCatch);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [tree],
            references:
            [
                MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new ExceptionSafetyAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
