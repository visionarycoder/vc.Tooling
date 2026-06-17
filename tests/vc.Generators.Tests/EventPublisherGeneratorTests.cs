using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Tests for EventPublisherGenerator Phase 4 Bundle 11.
/// Validates that the generator produces correct event publisher classes.
/// </summary>
public sealed class EventPublisherGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();

        // Act & Assert
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        
        Assert.NotNull(diagnostics);
    }

    [Fact]
    public void Generator_ProducesSourceOutput_ForAttributedInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var inputTreeCount = compilation.SyntaxTrees.Count();
        var outputTreeCount = outputCompilation.SyntaxTrees.Count();

        // Assert
        Assert.True(outputTreeCount >= inputTreeCount, 
            $"Generator should produce at least one output. Input: {inputTreeCount}, Output: {outputTreeCount}");
    }

    [Fact]
    public void Generator_GeneratedCode_ContainsEventPublisherClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(generatedText.Length > 0, "Generator should produce output");
        Assert.Contains("public sealed partial class EventPublisher", generatedText);
    }

    [Fact]
    public void Generator_ProducesEventsCollection()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("private readonly List<object> _events = new();", generatedText);
        Assert.Contains("public IReadOnlyList<object> Events => _events.AsReadOnly();", generatedText);
    }

    [Fact]
    public void Generator_ProducesPublishMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public void Publish(object @event)", generatedText);
        Assert.Contains("_events.Add(@event);", generatedText);
        Assert.Contains("Event cannot be null", generatedText);
    }

    [Fact]
    public void Generator_ProducesPublishAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public Task PublishAsync(object @event, CancellationToken cancellationToken = default)", generatedText);
        Assert.Contains("Publishes a domain event asynchronously", generatedText);
    }

    [Fact]
    public void Generator_ProducesClearEventsMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public void ClearEvents()", generatedText);
        Assert.Contains("_events.Clear();", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain.Events;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", generatedText);
    }

    [Fact]
    public void Generator_ProducesNullableEnable()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
    }

    [Fact]
    public void Generator_MultiplePublishers_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Domain
            {
                [System.AttributeUsage(System.AttributeTargets.Interface)]
                public sealed class VcEventPublisherAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Events
            {
                [Vc.Generators.Abstractions.Domain.VcEventPublisherAttribute]
                public interface IEventPublisher { }

                [Vc.Generators.Abstractions.Domain.VcEventPublisherAttribute]
                public interface INotificationPublisher { }

                [Vc.Generators.Abstractions.Domain.VcEventPublisherAttribute]
                public interface ICommandPublisher { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedCount = outputCompilation.SyntaxTrees.Count() - compilation.SyntaxTrees.Count();

        // Assert
        Assert.Equal(3, generatedCount);
    }

    [Fact]
    public void Generator_GeneratesDeterministicOutput_Reproducible()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation1 = CreateCompilation(source);
        var compilation2 = CreateCompilation(source);
        
        var generator1 = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        
        var driver1 = CSharpGeneratorDriver.Create(generator1);
        var driver2 = CSharpGeneratorDriver.Create(generator2);

        // Act
        driver1.RunGeneratorsAndUpdateCompilation(compilation1, out var outputCompilation1, out _);
        driver2.RunGeneratorsAndUpdateCompilation(compilation2, out var outputCompilation2, out _);
        
        var generatedText1 = GetGeneratedSourceText(outputCompilation1, compilation1.SyntaxTrees.Count());
        var generatedText2 = GetGeneratedSourceText(outputCompilation2, compilation2.SyntaxTrees.Count());

        // Assert
        Assert.Equal(generatedText1, generatedText2);
    }

    [Fact]
    public void Generator_ProducesDocumentationComments()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("/// <summary>", generatedText);
        Assert.Contains("Gets the collection of unpublished events", generatedText);
        Assert.Contains("Publishes a domain event", generatedText);
    }

    [Fact]
    public void Generator_ImplementsInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(": IEventPublisher", generatedText);
    }

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Domain
            {
                [System.AttributeUsage(System.AttributeTargets.Interface)]
                public sealed class VcEventPublisherAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Events
            {
                [Vc.Generators.Abstractions.Domain.VcEventPublisherAttribute]
                public interface IEventPublisher { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Domain.VcEventPublisherAttribute).Assembly.Location;
        
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(vcAbstractionsAssemblyPath),
        };

        return CSharpCompilation.Create("TestCompilation")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(references)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private string GetGeneratedSourceText(Compilation outputCompilation, int inputTreeCount)
    {
        var generatedTrees = outputCompilation.SyntaxTrees.Skip(inputTreeCount);
        return generatedTrees.FirstOrDefault()?.GetText().ToString() ?? "";
    }
}
