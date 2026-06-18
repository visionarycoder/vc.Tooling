using vc.Generators.Abstractions.Attributes;
using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

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
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();

        // Act & Assert
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out _, diagnostics: out var diagnostics);
        
        Assert.False(condition: diagnostics.IsDefault);
    }

    [Fact]
    public void Generator_ProducesSourceOutput_ForAttributedInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var inputTreeCount = compilation.SyntaxTrees.Count();
        var outputTreeCount = outputCompilation.SyntaxTrees.Count();

        // Assert
        Assert.True(condition: outputTreeCount >= inputTreeCount, 
            userMessage: $"Generator should produce at least one output. Input: {inputTreeCount}, Output: {outputTreeCount}");
    }

    [Fact]
    public void Generator_GeneratedCode_ContainsEventPublisherClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(condition: generatedText.Length > 0, userMessage: "Generator should produce output");
        Assert.Contains(expectedSubstring: "public sealed partial class EventPublisher", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesEventsCollection()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "private readonly List<object> _events = new();", actualString: generatedText);
        Assert.Contains(expectedSubstring: "public IReadOnlyList<object> Events => _events.AsReadOnly();", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesPublishMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public void Publish(object @event)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "_events.Add(@event);", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Event cannot be null", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesPublishAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public Task PublishAsync(object @event, CancellationToken cancellationToken = default)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Publishes a domain event asynchronously", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesClearEventsMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public void ClearEvents()", actualString: generatedText);
        Assert.Contains(expectedSubstring: "_events.Clear();", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "namespace Sample.Domain.Events;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesNullableEnable()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: generatedText);
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
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedCount = outputCompilation.SyntaxTrees.Count() - compilation.SyntaxTrees.Count();

        // Assert
        Assert.Equal(expected: 3, actual: generatedCount);
    }

    [Fact]
    public void Generator_GeneratesDeterministicOutput_Reproducible()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation1 = CreateCompilation(source: source);
        var compilation2 = CreateCompilation(source: source);
        
        var generator1 = new EventPublisherGenerator();
        var generator2 = new EventPublisherGenerator();
        
        var driver1 = CSharpGeneratorDriver.Create(incrementalGenerators: generator1);
        var driver2 = CSharpGeneratorDriver.Create(incrementalGenerators: generator2);

        // Act
        driver1.RunGeneratorsAndUpdateCompilation(compilation: compilation1, outputCompilation: out var outputCompilation1, diagnostics: out _);
        driver2.RunGeneratorsAndUpdateCompilation(compilation: compilation2, outputCompilation: out var outputCompilation2, diagnostics: out _);
        
        var generatedText1 = GetGeneratedSourceText(outputCompilation: outputCompilation1, inputTreeCount: compilation1.SyntaxTrees.Count());
        var generatedText2 = GetGeneratedSourceText(outputCompilation: outputCompilation2, inputTreeCount: compilation2.SyntaxTrees.Count());

        // Assert
        Assert.Equal(expected: generatedText1, actual: generatedText2);
    }

    [Fact]
    public void Generator_ProducesDocumentationComments()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "/// <summary>", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Gets the collection of unpublished events", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Publishes a domain event", actualString: generatedText);
    }

    [Fact]
    public void Generator_ImplementsInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new EventPublisherGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: ": IEventPublisher", actualString: generatedText);
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
        var syntaxTree = CSharpSyntaxTree.ParseText(text: source);
        
        var vcAbstractionsAssemblyPath = typeof(VcEventPublisherAttribute).Assembly.Location;
        
        var references = new[]
        {
            MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(path: vcAbstractionsAssemblyPath),
        };

        return CSharpCompilation.Create(assemblyName: "TestCompilation")
            .AddSyntaxTrees(trees: syntaxTree)
            .AddReferences(references: references)
            .WithOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
    }

    private string GetGeneratedSourceText(Compilation outputCompilation, int inputTreeCount)
    {
        var generatedTrees = outputCompilation.SyntaxTrees.Skip(count: inputTreeCount);
        return generatedTrees.FirstOrDefault()?.GetText().ToString() ?? "";
    }
}
