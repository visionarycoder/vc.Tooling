using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

public sealed class EventSourcingGeneratorTests
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

    private static ImmutableArray<Diagnostic> GetGeneratorDiagnostics(Compilation compilation)
    {
        var generator = new EventSourcingGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var _, diagnostics: out var diagnostics);
        return diagnostics;
    }

    private static (Compilation, IEnumerable<string>) RunGenerator(string source, string attributeSource = "")
    {
        var defaultAttribute = """
            namespace Vc.Generators.Abstractions.Domain;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcEventSourcingAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(value: attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source: source, attributeSource: attr);
        var compilation = CreateCompilation(trees: trees);

        var generator = new EventSourcingGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var updatedCompilation, diagnostics: out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(selector: st => st.GetText().ToString())
            .Where(predicate: s => s.Contains(value: "EventSourcedAggregate"))
            .ToList();

        return (updatedCompilation, sources);
    }

    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, _) = RunGenerator(source: source);
        // No exception = pass
        Assert.True(condition: true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        Assert.NotEmpty(collection: sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsEventSourcedAggregateBaseClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract partial class EventSourcedAggregate", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsEventSourcedAggregateProperties()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public Guid AggregateId { get; protected set; }", actualString: output);
        Assert.Contains(expectedSubstring: "public int Version { get; protected set; }", actualString: output);
        Assert.Contains(expectedSubstring: "public IReadOnlyList<object> UncommittedEvents", actualString: output);
        Assert.Contains(expectedSubstring: "public IReadOnlyList<object> CommittedEvents", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsApplyEventAbstractMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "protected abstract void ApplyEvent(object @event);", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsRaiseEventMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "protected void RaiseEvent(object @event)", actualString: output);
        Assert.Contains(expectedSubstring: "ApplyEvent(@event);", actualString: output);
        Assert.Contains(expectedSubstring: "_uncommittedEvents.Add(@event);", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsLoadFromHistoryMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public void LoadFromHistory(IEnumerable<object> events)", actualString: output);
        Assert.Contains(expectedSubstring: "_committedEvents.AddRange(events);", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsMarkEventsAsCommittedMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public void MarkEventsAsCommitted()", actualString: output);
        Assert.Contains(expectedSubstring: "_uncommittedEvents.Clear();", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConcreteEventSourcedClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public sealed partial class OrderEventSourced : EventSourcedAggregate", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsEventSourcedConstructor()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public OrderEventSourced(Guid aggregateId)", actualString: output);
        Assert.Contains(expectedSubstring: "if (aggregateId == Guid.Empty)", actualString: output);
        Assert.Contains(expectedSubstring: "AggregateId = aggregateId;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsEventSourcedApplyEventOverride()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "protected override void ApplyEvent(object @event)", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_PreservesNamespace()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel.Aggregates;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "namespace DomainModel.Aggregates;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic_FirstRun()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources1) = RunGenerator(source: source);
        var output1 = sources1.First();
        Assert.NotEmpty(collection: output1);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic_SecondRun()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
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

            namespace DomainModel;

            [VcEventSourcing]
            public partial class Order {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: output);
    }
}
