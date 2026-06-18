using VisionaryCoder.Generators.Distributed;
using Xunit;

namespace vc.Generators.Tests;

public sealed class OutboxGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Distributed; [System.AttributeUsage(System.AttributeTargets.Class)] public sealed class VcOutboxAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test", syntaxTrees: trees, references: refs, options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new OutboxGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        return (outComp, outComp.SyntaxTrees.Skip(count: 2).Select(selector: st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesOutboxEntry_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "OrderEventsOutboxEntry"));
    }

    [Fact]
    public void GeneratedOutput_ContainsOutboxRepository()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        var src = sources.First(predicate: s => s.Contains(value: "Outbox"));
        Assert.Contains(expectedSubstring: "IOrderEventsOutboxRepository", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_ContainsOutboxDispatcher()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        var src = sources.First(predicate: s => s.Contains(value: "Outbox"));
        Assert.Contains(expectedSubstring: "IOrderEventsOutboxDispatcher", actualString: src);
        Assert.Contains(expectedSubstring: "DispatchPendingAsync", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: "namespace Messaging; public class OrderEvents {}");
        Assert.Empty(collection: sources);
    }
}
