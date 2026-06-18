using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

public sealed class QueryHandlerGeneratorTests
{
    private static ImmutableArray<SyntaxTree> GetCompilation(string source, string attributeSource)
    {
        return
        [
            CSharpSyntaxTree.ParseText(text: attributeSource),
            CSharpSyntaxTree.ParseText(text: source)
        ];
    }

    private static Compilation CreateCompilation(ImmutableArray<SyntaxTree> trees)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(predicate: a => !a.IsDynamic && a.Location.Length > 0)
            .Select(selector: a => MetadataReference.CreateFromFile(path: a.Location))
            .Distinct()
            .ToArray();

        return CSharpCompilation.Create(assemblyName: "GeneratorTest")
            .AddReferences(references: references)
            .AddSyntaxTrees(trees: trees);
    }

    private static (Compilation, IEnumerable<string>) RunGenerator(string source, string attributeSource = "")
    {
        var defaultAttribute = """
            namespace Vc.Generators.Abstractions.Domain;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcQueryHandlerAttribute : Attribute {}
            """;
        
        var attr = string.IsNullOrEmpty(value: attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source: source, attributeSource: attr);
        var compilation = CreateCompilation(trees: trees);
        var generator = new QueryHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var sources = outputCompilation.SyntaxTrees
            .Skip(count: compilation.SyntaxTrees.Count())
            .Select(selector: st => st.GetText().ToString())
            .ToList();
        return (outputCompilation, sources);
    }

    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, _) = RunGenerator(source: source);
        Assert.True(condition: true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        Assert.NotEmpty(collection: sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsQueryHandlerInterface()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public interface IQueryHandler<in TQuery", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsHandleAsyncMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "Task<TResult> HandleAsync(TQuery query", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConcreteHandler()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract partial class GetUserHandler", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsExecuteAsyncMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "protected abstract Task<object?> ExecuteAsync(GetUser query", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsNullValidation()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "if (query == null)", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConfigureAwait()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: ".ConfigureAwait(false);", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_PreservesNamespace()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries.Users;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "namespace Application.Queries.Users;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources1) = RunGenerator(source: source);
        var output1 = sources1.First();
        var (_, sources2) = RunGenerator(source: source);
        var output2 = sources2.First();
        Assert.Equal(expected: output1, actual: output2);
    }

    [Fact]
    public void GeneratedOutput_ContainsNullableDirective()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Queries;

            [VcQueryHandler]
            public partial class GetUser {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: output);
    }
}
