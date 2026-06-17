using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Architecture;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class ProjectReferenceAnalyzerTests
{
    [Fact]
    public async Task ProjectReferenceAnalyzer_ShouldReportDiagnostic_WhenApiReferencesInfrastructure()
    {
        var referencedAssembly = CreateAssemblyReference(
            "Sample.Infrastructure",
            "namespace Sample.Infrastructure { public sealed class InfraType { } }");

        var source = """
            namespace Sample.Api
            {
                public sealed class Controller
                {
                    private readonly Sample.Infrastructure.InfraType _infra = new Sample.Infrastructure.InfraType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source, "Sample.Api", referencedAssembly);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchProjectReferenceViolation);
    }

    [Fact]
    public async Task ProjectReferenceAnalyzer_ShouldNotReportDiagnostic_WhenApiReferencesApplication()
    {
        var referencedAssembly = CreateAssemblyReference(
            "Sample.Application",
            "namespace Sample.Application { public sealed class AppType { } }");

        var source = """
            namespace Sample.Api
            {
                public sealed class Controller
                {
                    private readonly Sample.Application.AppType _app = new Sample.Application.AppType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source, "Sample.Api", referencedAssembly);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchProjectReferenceViolation);
    }

    private static MetadataReference CreateAssemblyReference(string assemblyName, string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [tree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        Assert.True(emitResult.Success);

        stream.Position = 0;
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string source,
        string assemblyName,
        MetadataReference referencedAssembly)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [tree],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                referencedAssembly
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers([new ProjectReferenceAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}