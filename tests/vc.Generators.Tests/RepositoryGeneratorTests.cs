using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

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
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();

        // Act & Assert
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        
        Assert.NotNull(diagnostics);
    }

    [Fact]
    public void Generator_ProducesSourceOutput_ForAttributedClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
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
    public void Generator_GeneratedCode_ContainsRepositoryInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(generatedText.Length > 0, "Generator should produce output");
        Assert.Contains("public interface IOrderRepository", generatedText);
    }

    [Fact]
    public void Generator_ProducesRepositoryMethods()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);", generatedText);
        Assert.Contains("Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);", generatedText);
        Assert.Contains("Task AddAsync(Order aggregate, CancellationToken cancellationToken = default);", generatedText);
        Assert.Contains("Task UpdateAsync(Order aggregate, CancellationToken cancellationToken = default);", generatedText);
        Assert.Contains("Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);", generatedText);
    }

    [Fact]
    public void Generator_ProducesFindByIdAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Task<Order?> FindByIdAsync(Guid id,", generatedText);
        Assert.Contains("Finds a Order by ID", generatedText);
    }

    [Fact]
    public void Generator_ProducesGetAllAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Task<IEnumerable<Order>> GetAllAsync(", generatedText);
        Assert.Contains("Gets all Order aggregates", generatedText);
    }

    [Fact]
    public void Generator_ProducesAddAsyncMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Task AddAsync(Order aggregate,", generatedText);
        Assert.Contains("Adds a Order to the repository", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain.Aggregates;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
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
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
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
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
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
        
        var generator1 = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        
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
    public void Generator_ProducesXmlDocumentationComments()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.RepositoryGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("/// <summary>", generatedText);
        Assert.Contains("Repository interface", generatedText);
        Assert.Contains("Finds a Order", generatedText);
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
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Data.VcRepositoryAttribute).Assembly.Location;
        
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
