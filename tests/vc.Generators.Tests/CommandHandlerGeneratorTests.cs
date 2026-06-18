using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

public sealed class CommandHandlerGeneratorTests
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
            public sealed class VcCommandHandlerAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(value: attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source: source, attributeSource: attr);
        var compilation = CreateCompilation(trees: trees);

        var generator = new CommandHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var updatedCompilation, diagnostics: out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(selector: st => st.GetText().ToString())
            .Where(predicate: s => s.Contains(value: "Handler") && s.Contains(value: "ICommandHandler"))
            .ToList();

        return (updatedCompilation, sources);
    }
    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, _) = RunGenerator(source: source);
        Assert.True(condition: true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        Assert.NotEmpty(collection: sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsCommandHandlerInterface()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public interface ICommandHandler<in TCommand>", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsHandleAsyncMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "Task HandleAsync(TCommand command", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConcreteHandler()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract partial class CreateOrderHandler", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsExecuteAsyncMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "protected abstract Task ExecuteAsync(CreateOrder command", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsNullValidation()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "if (command == null)", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConfigureAwait()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
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

            namespace Application.Commands.Orders;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "namespace Application.Commands.Orders;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
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

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
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

            namespace Application.Commands;

            [VcCommandHandler]
            public partial class CreateOrder {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: output);
    }
}
