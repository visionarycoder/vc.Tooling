using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Architecture;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class ProjectReferenceAnalyzerTests
{
    [Fact]
    public async Task ProjectReferenceAnalyzer_ShouldReportDiagnostic_WhenApiReferencesInfrastructure()
    {
        var referencedAssembly = CreateAssemblyReference(
            assemblyName: "Sample.Infrastructure",
            source: "namespace Sample.Infrastructure { public sealed class InfraType { } }");

        var source = """
            namespace Sample.Api
            {
                public sealed class Controller
                {
                    private readonly Sample.Infrastructure.InfraType _infra = new Sample.Infrastructure.InfraType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source, assemblyName: "Sample.Api", referencedAssembly: referencedAssembly);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchProjectReferenceViolation);
    }

    [Fact]
    public async Task ProjectReferenceAnalyzer_ShouldNotReportDiagnostic_WhenApiReferencesApplication()
    {
        var referencedAssembly = CreateAssemblyReference(
            assemblyName: "Sample.Application",
            source: "namespace Sample.Application { public sealed class AppType { } }");

        var source = """
            namespace Sample.Api
            {
                public sealed class Controller
                {
                    private readonly Sample.Application.AppType _app = new Sample.Application.AppType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source, assemblyName: "Sample.Api", referencedAssembly: referencedAssembly);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchProjectReferenceViolation);
    }

    private static MetadataReference CreateAssemblyReference(string assemblyName, string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: [tree],
            references: [MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location)],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(peStream: stream);
        Assert.True(condition: emitResult.Success);

        stream.Position = 0;
        return MetadataReference.CreateFromImage(peImage: stream.ToArray());
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string source,
        string assemblyName,
        MetadataReference referencedAssembly)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: [tree],
            references:
            [
                MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                referencedAssembly
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new ProjectReferenceAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}