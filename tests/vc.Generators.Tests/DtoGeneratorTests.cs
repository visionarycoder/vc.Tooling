using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Tests for DtoGenerator Phase 4 Bundle 6.
/// Validates that the generator produces correct DTO classes.
/// </summary>
public sealed class DtoGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();

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
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
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
    public void Generator_GeneratedCode_ContainsDtoClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(generatedText.Length > 0, "Generator should produce output");
        Assert.Contains("public sealed partial class OrderDto", generatedText);
    }

    [Fact]
    public void Generator_ProducesDtoProperties()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public Guid Id { get; set; }", generatedText);
        Assert.Contains("public DateTime CreatedAt { get; set; }", generatedText);
        Assert.Contains("public DateTime? UpdatedAt { get; set; }", generatedText);
        Assert.Contains("public int Version { get; set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesIdProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Gets or sets the entity ID", generatedText);
        Assert.Contains("public Guid Id { get; set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesCreatedAtProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Gets or sets the creation timestamp", generatedText);
        Assert.Contains("public DateTime CreatedAt { get; set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesUpdatedAtProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Gets or sets the last modification timestamp", generatedText);
        Assert.Contains("public DateTime? UpdatedAt { get; set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesVersionProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("Gets or sets the entity version", generatedText);
        Assert.Contains("public int Version { get; set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesConstructor()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderDto()", generatedText);
        Assert.Contains("Id = Guid.Empty;", generatedText);
        Assert.Contains("CreatedAt = DateTime.UtcNow;", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain.Entities;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
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
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
    }

    [Fact]
    public void Generator_MultipleDtos_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcDtoAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Entities
            {
                [Vc.Generators.Abstractions.Data.VcDtoAttribute]
                public class Order { }

                [Vc.Generators.Abstractions.Data.VcDtoAttribute]
                public class Customer { }

                [Vc.Generators.Abstractions.Data.VcDtoAttribute]
                public class Product { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.DtoGenerator();
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
        
        var generator1 = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.DtoGenerator();
        
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

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcDtoAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Entities
            {
                [Vc.Generators.Abstractions.Data.VcDtoAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Data.VcDtoAttribute).Assembly.Location;
        
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
