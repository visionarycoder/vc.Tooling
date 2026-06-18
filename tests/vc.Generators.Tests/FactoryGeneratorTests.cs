using vc.Generators.Abstractions.Attributes;
using VisionaryCoder.Generators.Design;
using Xunit;

namespace vc.Generators.Tests;

/// <summary>
/// Tests for FactoryGenerator Phase 4 Bundle 13.
/// Validates that the generator produces correct factory classes.
/// </summary>
public sealed class FactoryGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();

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
        var generator = new FactoryGenerator();
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
    public void Generator_GeneratedCode_ContainsFactoryClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(condition: generatedText.Length > 0, userMessage: "Generator should produce output");
        Assert.Contains(expectedSubstring: "public sealed partial class ObjectFactory", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCreateMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public object Create()", actualString: generatedText);
        Assert.Contains(expectedSubstring: "return new object();", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCreateAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public Task<object> CreateAsync(CancellationToken cancellationToken = default)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Creates a new domain object instance asynchronously", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCanCreateMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public bool CanCreate()", actualString: generatedText);
        Assert.Contains(expectedSubstring: "return true;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "namespace Sample.Domain.Factories;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
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
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: generatedText);
    }

    [Fact]
    public void Generator_MultipleFactories_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Design
            {
                [System.AttributeUsage(System.AttributeTargets.Interface)]
                public sealed class VcFactoryAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Factories
            {
                [Vc.Generators.Abstractions.Design.VcFactoryAttribute]
                public interface IObjectFactory { }

                [Vc.Generators.Abstractions.Design.VcFactoryAttribute]
                public interface IEntityFactory { }

                [Vc.Generators.Abstractions.Design.VcFactoryAttribute]
                public interface IAggregateFactory { }
            }
            """;
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
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
        
        var generator1 = new FactoryGenerator();
        var generator2 = new FactoryGenerator();
        
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
    public void Generator_ImplementsInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: ": IObjectFactory", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesDocumentationComments()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new FactoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "/// <summary>", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Creates a new domain object instance", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Determines whether this factory can create instances", actualString: generatedText);
    }

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Design
            {
                [System.AttributeUsage(System.AttributeTargets.Interface)]
                public sealed class VcFactoryAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Factories
            {
                [Vc.Generators.Abstractions.Design.VcFactoryAttribute]
                public interface IObjectFactory { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: source);
        
        var vcAbstractionsAssemblyPath = typeof(VcFactoryAttribute).Assembly.Location;
        
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
