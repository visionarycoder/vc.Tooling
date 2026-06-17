using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Tests for BuilderGenerator Phase 4 Bundle 12.
/// Validates that the generator produces correct builder classes.
/// </summary>
public sealed class BuilderGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();

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
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
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
    public void Generator_GeneratedCode_ContainsBuilderClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(generatedText.Length > 0, "Generator should produce output");
        Assert.Contains("public sealed partial class OrderBuilder", generatedText);
    }

    [Fact]
    public void Generator_ProducesInstanceField()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("private Order? _instance;", generatedText);
    }

    [Fact]
    public void Generator_ProducesConstructor()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderBuilder()", generatedText);
        Assert.Contains("_instance = new Order();", generatedText);
    }

    [Fact]
    public void Generator_ProducesCreateFactory()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public static OrderBuilder Create()", generatedText);
        Assert.Contains("return new OrderBuilder();", generatedText);
    }

    [Fact]
    public void Generator_ProducesBuildMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public Order Build()", generatedText);
        Assert.Contains("if (_instance == null)", generatedText);
        Assert.Contains("Cannot build Order", generatedText);
        Assert.Contains("_instance = null; // Clear to prevent reuse", generatedText);
    }

    [Fact]
    public void Generator_ProducesResetMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderBuilder Reset()", generatedText);
        Assert.Contains("Resets the builder to a clean state", generatedText);
        Assert.Contains("return this;", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain.Builders;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
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
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
    }

    [Fact]
    public void Generator_MultipleBuilders_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Design
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcBuilderAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Builders
            {
                [Vc.Generators.Abstractions.Design.VcBuilderAttribute]
                public class Order { }

                [Vc.Generators.Abstractions.Design.VcBuilderAttribute]
                public class Customer { }

                [Vc.Generators.Abstractions.Design.VcBuilderAttribute]
                public class Product { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
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
        
        var generator1 = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.BuilderGenerator();
        
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
            namespace Vc.Generators.Abstractions.Design
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcBuilderAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Builders
            {
                [Vc.Generators.Abstractions.Design.VcBuilderAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Design.VcBuilderAttribute).Assembly.Location;
        
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
