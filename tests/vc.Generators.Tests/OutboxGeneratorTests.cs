using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class OutboxGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Distributed; [System.AttributeUsage(System.AttributeTargets.Class)] public sealed class VcOutboxAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test", trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new OutboxGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        return (outComp, outComp.SyntaxTrees.Skip(2).Select(st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesOutboxEntry_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("OrderEventsOutboxEntry"));
    }

    [Fact]
    public void GeneratedOutput_ContainsOutboxRepository()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        var src = sources.First(s => s.Contains("Outbox"));
        Assert.Contains("IOrderEventsOutboxRepository", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsOutboxDispatcher()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        var src = sources.First(s => s.Contains("Outbox"));
        Assert.Contains("IOrderEventsOutboxDispatcher", src);
        Assert.Contains("DispatchPendingAsync", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Distributed; namespace Messaging; [VcOutbox] public class OrderEvents {}");
        Assert.Contains(sources, s => s.Contains("auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("namespace Messaging; public class OrderEvents {}");
        Assert.Empty(sources);
    }
}
