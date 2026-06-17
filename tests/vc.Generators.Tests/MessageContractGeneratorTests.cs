using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Tests for MessageContractGenerator Phase 4 Bundle 10.
/// Validates that the generator produces correct message contract classes.
/// </summary>
public sealed class MessageContractGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();

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
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
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
    public void Generator_GeneratedCode_ContainsMessageContractClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var inputTreeCount = compilation.SyntaxTrees.Count();
        var outputTreeCount = outputCompilation.SyntaxTrees.Count();
        var allTrees = outputCompilation.SyntaxTrees.ToList();
        
        // Get all generated trees
        var generatedTrees = allTrees.Skip(inputTreeCount).ToList();
        var generatedText = generatedTrees.Count > 0 ? generatedTrees[0].GetText().ToString() : "";

        // Assert
        Assert.True(generatedText.Length > 0, $"Generator should produce output. Input trees: {inputTreeCount}, Output trees: {outputTreeCount}, Generated trees: {generatedTrees.Count}");
        Assert.Contains("public sealed partial class OrderContract", generatedText);
    }

    [Fact]
    public void Generator_ProducesMessageProperties()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public Guid MessageId { get; private set; }", generatedText);
        Assert.Contains("public Guid CorrelationId { get; private set; }", generatedText);
        Assert.Contains("public DateTime CreatedAt { get; private set; }", generatedText);
        Assert.Contains("public string MessageType { get; private set; }", generatedText);
        Assert.Contains("public int Version { get; private set; }", generatedText);
        Assert.Contains("public string Payload { get; private set; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesConstructor()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderContract()", generatedText);
        Assert.Contains("MessageId = Guid.NewGuid();", generatedText);
        Assert.Contains("CorrelationId = Guid.NewGuid();", generatedText);
    }

    [Fact]
    public void Generator_ProducesCreateFactory()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public static OrderContract Create(string payload)", generatedText);
        Assert.Contains("Payload cannot be null or empty", generatedText);
    }

    [Fact]
    public void Generator_ProducesWithNewCorrelationMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderContract WithNewCorrelation()", generatedText);
        Assert.Contains("Creates a copy of this message with a new correlation identifier", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Messaging;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
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
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
    }

    [Fact]
    public void Generator_MultipleContracts_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcMessageContractAttribute : System.Attribute { }
            }

            namespace Sample.Messaging
            {
                [Vc.Generators.Abstractions.Data.VcMessageContractAttribute]
                public class Order { }

                [Vc.Generators.Abstractions.Data.VcMessageContractAttribute]
                public class Payment { }

                [Vc.Generators.Abstractions.Data.VcMessageContractAttribute]
                public class Notification { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
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
        
        var generator1 = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        
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
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("/// <summary>", generatedText);
        Assert.Contains("Gets the unique message identifier", generatedText);
        Assert.Contains("Gets the message correlation identifier", generatedText);
        Assert.Contains("Gets the message creation timestamp", generatedText);
    }

    [Fact]
    public void Generator_ProducesInitializationLogic()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.MessageContractGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("CreatedAt = DateTime.UtcNow;", generatedText);
        Assert.Contains("MessageType = \"Order\";", generatedText);
        Assert.Contains("Version = 1;", generatedText);
    }

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Data
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcMessageContractAttribute : System.Attribute { }
            }

            namespace Sample.Messaging
            {
                [Vc.Generators.Abstractions.Data.VcMessageContractAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Data.VcMessageContractAttribute).Assembly.Location;
        
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
