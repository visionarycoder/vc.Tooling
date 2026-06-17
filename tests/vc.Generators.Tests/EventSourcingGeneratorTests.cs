using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class EventSourcingGeneratorTests
{
    private static ImmutableArray<SyntaxTree> GetCompilation(string source, string attributeSource)
    {
        return ImmutableArray.Create(
            CSharpSyntaxTree.ParseText(attributeSource),
            CSharpSyntaxTree.ParseText(source)
        );
    }

    private static Compilation CreateCompilation(ImmutableArray<SyntaxTree> trees)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.Location.Length > 0)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Distinct()
            .ToArray();

        return CSharpCompilation.Create("GeneratorTest")
            .AddReferences(references)
            .AddSyntaxTrees(trees);
    }

    private static ImmutableArray<Diagnostic> GetGeneratorDiagnostics(Compilation compilation)
    {
        var generator = new EventSourcingGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var diagnostics);
        return diagnostics;
    }

    private static (Compilation, IEnumerable<string>) RunGenerator(string source, string attributeSource = "")
    {
        var defaultAttribute = """
            namespace Vc.Generators.Abstractions.Domain;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcEventSourcingAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source, attr);
        var compilation = CreateCompilation(trees);

        var generator = new EventSourcingGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(st => st.GetText().ToString())
            .Where(s => s.Contains("EventSourcedAggregate"))
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

        var (_, _) = RunGenerator(source);
        // No exception = pass
        Assert.True(true);
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

        var (_, sources) = RunGenerator(source);
        Assert.NotEmpty(sources);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract partial class EventSourcedAggregate", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public Guid AggregateId { get; protected set; }", output);
        Assert.Contains("public int Version { get; protected set; }", output);
        Assert.Contains("public IReadOnlyList<object> UncommittedEvents", output);
        Assert.Contains("public IReadOnlyList<object> CommittedEvents", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("protected abstract void ApplyEvent(object @event);", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("protected void RaiseEvent(object @event)", output);
        Assert.Contains("ApplyEvent(@event);", output);
        Assert.Contains("_uncommittedEvents.Add(@event);", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public void LoadFromHistory(IEnumerable<object> events)", output);
        Assert.Contains("_committedEvents.AddRange(events);", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public void MarkEventsAsCommitted()", output);
        Assert.Contains("_uncommittedEvents.Clear();", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class OrderEventSourced : EventSourcedAggregate", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public OrderEventSourced(Guid aggregateId)", output);
        Assert.Contains("if (aggregateId == Guid.Empty)", output);
        Assert.Contains("AggregateId = aggregateId;", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("protected override void ApplyEvent(object @event)", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("namespace DomainModel.Aggregates;", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", output);
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

        var (_, sources1) = RunGenerator(source);
        var output1 = sources1.First();
        Assert.NotEmpty(output1);
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

        var (_, sources1) = RunGenerator(source);
        var output1 = sources1.First();
        var (_, sources2) = RunGenerator(source);
        var output2 = sources2.First();
        Assert.Equal(output1, output2);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("#nullable enable", output);
    }
}
