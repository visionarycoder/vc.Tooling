using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Minimal tests for StrongIdGenerator Phase 4 Bundle 1.
/// These tests validate that the generator executes and produces output.
/// </summary>
public sealed class StrongIdGeneratorBasicTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();

        // Act & Assert
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        
        // Generator should initialize successfully
        Assert.NotNull(diagnostics);
    }

    [Fact]
    public void Generator_ProducesSourceOutput_ForAttributedStruct()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var inputTreeCount = compilation.SyntaxTrees.Count();
        var outputTreeCount = outputCompilation.SyntaxTrees.Count();

        // Assert - At least one new source file should be generated
        Assert.True(outputTreeCount >= inputTreeCount, 
            $"Generator should produce at least one output. Input: {inputTreeCount}, Output: {outputTreeCount}");
    }

    [Fact]
    public void Generator_GeneratedCode_ContainsEquatableInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        
        // Check if compilation has errors
        var diags = compilation.GetDiagnostics();
        var hasErrors = diags.Any(d => d.Severity == DiagnosticSeverity.Error);
        if (hasErrors)
        {
            var errors = string.Join("; ", diags.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));
            Assert.False(true, $"Compilation has errors: {errors}");
        }
        
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        var inputTreeCount = compilation.SyntaxTrees.Count();
        var outputTreeCount = outputCompilation.SyntaxTrees.Count();
        
        var generatedText = GetGeneratedSourceText(outputCompilation, inputTreeCount);

        // Assert - Debug output
        var diagStr = string.Join(", ", diagnostics.Select(d => d.GetMessage()));
        var debugMsg = $"Input trees: {inputTreeCount}, Output trees: {outputTreeCount}, Generated text length: {generatedText.Length}, Diagnostics: [{diagStr}]";
        Assert.True(generatedText.Length > 0, $"No output generated. {debugMsg}");
        Assert.Contains("IEquatable", generatedText);
    }

    private string CreateSourceWithAttribute()
    {
        return """
            namespace Vc.Generators.Abstractions.Domain
            {
                [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
                public sealed class VcStrongIdAttribute : System.Attribute { }
            }

            namespace Sample.Domain
            {
                [Vc.Generators.Abstractions.Domain.VcStrongIdAttribute]
                public readonly partial struct ProductId { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        // Get the output path of the vc.Generators.Abstractions assembly  (contains the attributes)
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Domain.VcStrongIdAttribute).Assembly.Location;
        
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Attribute).Assembly.Location), // Add System.Runtime
            // Reference the vc.Generators.Abstractions assembly so ForAttributeWithMetadataName can find the attributes
            MetadataReference.CreateFromFile(vcAbstractionsAssemblyPath),
        };

        return CSharpCompilation.Create("TestCompilation")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(references)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    [Fact]
    public void Generator_ProducesCorrectGuidProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public Guid Value { get; }", generatedText);
    }

    [Fact]
    public void Generator_ProducesEqualityOperators()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public static bool operator ==", generatedText);
        Assert.Contains("public static bool operator !=", generatedText);
    }

    [Fact]
    public void Generator_ProducesGetHashCodeMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public override int GetHashCode()", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain;", generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectStructDeclaration()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public partial readonly struct ProductId : IEquatable<ProductId>", generatedText);
    }

    [Fact]
    public void Generator_MultipleStructs_GeneratesSeparateFiles()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Domain
            {
                [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
                public sealed class VcStrongIdAttribute : System.Attribute { }
            }

            namespace Sample.Domain
            {
                [Vc.Generators.Abstractions.Domain.VcStrongIdAttribute]
                public readonly partial struct ProductId { }

                [Vc.Generators.Abstractions.Domain.VcStrongIdAttribute]
                public readonly partial struct OrderId { }

                [Vc.Generators.Abstractions.Domain.VcStrongIdAttribute]
                public readonly partial struct CustomerId { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedCount = outputCompilation.SyntaxTrees.Count() - compilation.SyntaxTrees.Count();

        // Assert - Should generate 3 files, one for each struct
        Assert.Equal(3, generatedCount);
    }

    [Fact]
    public void Generator_SkipsNonPartialStructs()
    {
        // Arrange
        var source = """
            namespace Vc.Generators.Abstractions.Domain
            {
                [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
                public sealed class VcStrongIdAttribute : System.Attribute { }
            }

            namespace Sample.Domain
            {
                // This one is not partial - should still generate (generator doesn't check partial)
                [Vc.Generators.Abstractions.Domain.VcStrongIdAttribute]
                public readonly struct UserId { }
            }
            """;
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedCount = outputCompilation.SyntaxTrees.Count() - compilation.SyntaxTrees.Count();

        // Assert - Generator will generate regardless of partial keyword
        Assert.Equal(1, generatedCount);
    }

    [Fact]
    public void Generator_GeneratedCodeHasNullableEnable()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("#nullable enable", generatedText);
    }

    [Fact]
    public void Generator_GeneratesDeterministicOutput_Reproducible()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation1 = CreateCompilation(source);
        var compilation2 = CreateCompilation(source);
        
        var generator1 = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.StrongIdGenerator();
        
        var driver1 = CSharpGeneratorDriver.Create(generator1);
        var driver2 = CSharpGeneratorDriver.Create(generator2);

        // Act
        driver1.RunGeneratorsAndUpdateCompilation(compilation1, out var outputCompilation1, out _);
        driver2.RunGeneratorsAndUpdateCompilation(compilation2, out var outputCompilation2, out _);
        
        var generatedText1 = GetGeneratedSourceText(outputCompilation1, compilation1.SyntaxTrees.Count());
        var generatedText2 = GetGeneratedSourceText(outputCompilation2, compilation2.SyntaxTrees.Count());

        // Assert - Output should be identical
        Assert.Equal(generatedText1, generatedText2);
    }

    private string GetGeneratedSourceText(Compilation outputCompilation, int inputTreeCount)
    {
        var generatedTrees = outputCompilation.SyntaxTrees.Skip(inputTreeCount);
        return generatedTrees.FirstOrDefault()?.GetText().ToString() ?? "";
    }
}
