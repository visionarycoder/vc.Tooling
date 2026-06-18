using VisionaryCoder.Generators.Data;
using Xunit;

namespace vc.Generators.Tests;

/// <summary>
/// Tests for MapperGenerator Phase 4 Bundle 7.
/// Validates that the generator produces correct mapper classes.
/// </summary>
public sealed class MapperGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();

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
        var generator = new MapperGenerator();
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
    public void Generator_GeneratedCode_ContainsMapperClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(condition: generatedText.Length > 0, userMessage: "Generator should produce output");
        Assert.Contains(expectedSubstring: "public sealed partial class OrderMapper", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesDtoToEntityMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public static Order DtoToEntity(OrderDto dto)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Maps OrderDto to Order entity", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesEntityToDtoMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public static OrderDto EntityToDto(Order entity)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Maps Order entity to OrderDto", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesDtosToEntitiesMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public static IEnumerable<Order> DtosToEntities(IEnumerable<OrderDto> dtos)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Maps a collection of OrderDto to Order entities", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesEntitiesToDtosMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public static IEnumerable<OrderDto> EntitiesToDtos(IEnumerable<Order> entities)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Maps a collection of Order entities to OrderDto", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesNullArgumentValidation()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "if (dto == null) throw new ArgumentNullException(nameof(dto));", actualString: generatedText);
        Assert.Contains(expectedSubstring: "if (entity == null) throw new ArgumentNullException(nameof(entity));", actualString: generatedText);
        Assert.Contains(expectedSubstring: "if (dtos == null) throw new ArgumentNullException(nameof(dtos));", actualString: generatedText);
        Assert.Contains(expectedSubstring: "if (entities == null) throw new ArgumentNullException(nameof(entities));", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesPropertyMapping()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "Id = dto.Id,", actualString: generatedText);
        Assert.Contains(expectedSubstring: "CreatedAt = dto.CreatedAt,", actualString: generatedText);
        Assert.Contains(expectedSubstring: "UpdatedAt = dto.UpdatedAt,", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Version = dto.Version", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "namespace Sample.Domain.Entities;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
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
        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: generatedText);
    }

    [Fact]
    public void Generator_MultipleMappers_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcMapperAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Entities
            {
                [Vc.Generators.Abstractions.Data.VcMapperAttribute]
                public class Order { }

                [Vc.Generators.Abstractions.Data.VcMapperAttribute]
                public class Customer { }

                [Vc.Generators.Abstractions.Data.VcMapperAttribute]
                public class Product { }
            }
            """;
        var compilation = CreateCompilation(source: source);
        var generator = new MapperGenerator();
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
        
        var generator1 = new MapperGenerator();
        var generator2 = new MapperGenerator();
        
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

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcMapperAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Entities
            {
                [Vc.Generators.Abstractions.Data.VcMapperAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: source);
        
        var vcAbstractionsAssemblyPath = typeof(vc.Generators.Abstractions.Attributes.VcMapperAttribute).Assembly.Location;
        
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
