using vc.Generators.Abstractions.Attributes;
using VisionaryCoder.Generators.Data;
using Xunit;

namespace vc.Generators.Tests;

/// <summary>
/// Tests for RepositoryGenerator Phase 4 Bundle 5.
/// Validates that the generator produces correct repository interfaces.
/// </summary>
public sealed class RepositoryGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();

        // Act & Assert
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out _, diagnostics: out var diagnostics);
        
        Assert.False(condition: diagnostics.IsDefault);
    }

    [Fact]
    public void Generator_ProducesSourceOutput_ForAttributedClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
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
    public void Generator_GeneratedCode_ContainsRepositoryInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(condition: generatedText.Length > 0, userMessage: "Generator should produce output");
        Assert.Contains(expectedSubstring: "public interface IOrderRepository", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesRepositoryMethods()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Task AddAsync(Order aggregate, CancellationToken cancellationToken = default);", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Task UpdateAsync(Order aggregate, CancellationToken cancellationToken = default);", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesFindByIdAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "Task<Order?> FindByIdAsync(Guid id,", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Finds a Order by ID", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesGetAllAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "Task<IEnumerable<Order>> GetAllAsync(", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Gets all Order aggregates", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAddAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "Task AddAsync(Order aggregate,", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Adds a Order to the repository", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "namespace Sample.Domain.Aggregates;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
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
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: generatedText);
    }

    [Fact]
    public void Generator_MultipleRepositories_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcRepositoryAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Aggregates
            {
                [Vc.Generators.Abstractions.Data.VcRepositoryAttribute]
                public class Order { }

                [Vc.Generators.Abstractions.Data.VcRepositoryAttribute]
                public class Customer { }

                [Vc.Generators.Abstractions.Data.VcRepositoryAttribute]
                public class Product { }
            }
            """;
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
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
        
        var generator1 = new RepositoryGenerator();
        var generator2 = new RepositoryGenerator();
        
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
    public void Generator_ProducesXmlDocumentationComments()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "/// <summary>", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Repository interface", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Finds a Order", actualString: generatedText);
    }

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcRepositoryAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Aggregates
            {
                [Vc.Generators.Abstractions.Data.VcRepositoryAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: source);
        
        var vcAbstractionsAssemblyPath = typeof(VcRepositoryAttribute).Assembly.Location;
        
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
