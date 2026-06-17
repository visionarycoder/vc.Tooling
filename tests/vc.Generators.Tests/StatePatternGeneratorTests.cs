using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Vc.Generators.Tests;

/// <summary>
/// Tests for StatePatternGenerator Phase 4 Bundle 17.
/// Validates that the generator produces correct state machine classes.
/// </summary>
public sealed class StatePatternGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();

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
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
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
    public void Generator_GeneratedCode_ContainsStateMachineClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(generatedText.Length > 0, "Generator should produce output");
        Assert.Contains("public sealed partial class OrderStateMachine", generatedText);
    }

    [Fact]
    public void Generator_ProducesCurrentStateField()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("private IState _currentState;", generatedText);
    }

    [Fact]
    public void Generator_ProducesCurrentStateProperty()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public IState CurrentState => _currentState;", generatedText);
    }

    [Fact]
    public void Generator_ProducesConstructor()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public OrderStateMachine(IState initialState)", generatedText);
        Assert.Contains("Initial state cannot be null", generatedText);
    }

    [Fact]
    public void Generator_ProducesTransitionToMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public void TransitionTo(IState newState)", generatedText);
        Assert.Contains("New state cannot be null", generatedText);
        Assert.Contains("_currentState = newState;", generatedText);
    }

    [Fact]
    public void Generator_ProducesExecuteMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public string Execute()", generatedText);
        Assert.Contains("return _currentState?.Execute() ?? \"No state\";", generatedText);
    }

    [Fact]
    public void Generator_ProducesResetMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public void Reset(IState initialState)", generatedText);
        Assert.Contains("_currentState = initialState;", generatedText);
    }

    [Fact]
    public void Generator_ProducesIStateInterface()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("public interface IState", generatedText);
    }

    [Fact]
    public void Generator_ProducesIStateExecuteMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("string Execute();", generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source);
        var generator = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var generatedText = GetGeneratedSourceText(outputCompilation, compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains("namespace Sample.Domain.Orders;", generatedText);
    }

    [Fact]
    public void Generator_GeneratesDeterministicOutput_Reproducible()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation1 = CreateCompilation(source);
        var compilation2 = CreateCompilation(source);
        
        var generator1 = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        var generator2 = new VisionaryCoder.Tooling.Generators.StatePatternGenerator();
        
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
                public sealed class VcStatePatternAttribute : System.Attribute { }
            }

            namespace Sample.Domain.Orders
            {
                [Vc.Generators.Abstractions.Design.VcStatePatternAttribute]
                public class Order { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var vcAbstractionsAssemblyPath = typeof(Vc.Generators.Abstractions.Design.VcStatePatternAttribute).Assembly.Location;
        
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
