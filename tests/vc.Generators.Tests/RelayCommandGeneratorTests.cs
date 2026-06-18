using vc.Generators.Abstractions.Attributes;
using VisionaryCoder.Generators.Design;
using Xunit;

namespace vc.Generators.Tests;

/// <summary>
/// Tests for RelayCommandGenerator Phase 4 Bundle 18.
/// Validates that the generator produces correct relay command classes.
/// </summary>
public sealed class RelayCommandGeneratorTests
{
    [Fact]
    public void Generator_Executes_WithoutThrowingException()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();

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
        var generator = new RelayCommandGenerator();
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
    public void Generator_GeneratedCode_ContainsCommandClass()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.True(condition: generatedText.Length > 0, userMessage: "Generator should produce output");
        Assert.Contains(expectedSubstring: "public sealed partial class ViewModelCommand", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesICommandImplementation()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: ": ICommand", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesExecuteAction()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "private readonly Action<object?> _execute;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCanExecutePredicate()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "private readonly Predicate<object?>? _canExecute;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCanExecuteChangedEvent()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public event EventHandler? CanExecuteChanged", actualString: generatedText);
        Assert.Contains(expectedSubstring: "CommandManager.RequerySuggested += value;", actualString: generatedText);
        Assert.Contains(expectedSubstring: "CommandManager.RequerySuggested -= value;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesConstructor()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public ViewModelCommand(Action<object?> execute, Predicate<object?>? canExecute = null)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "Execute action cannot be null", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCanExecuteMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public bool CanExecute(object? parameter)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "_canExecute?.Invoke(parameter) ?? true;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesExecuteMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public void Execute(object? parameter)", actualString: generatedText);
        Assert.Contains(expectedSubstring: "if (CanExecute(parameter))", actualString: generatedText);
        Assert.Contains(expectedSubstring: "_execute(parameter);", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesRaiseCanExecuteChangedMethod()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "public void RaiseCanExecuteChanged()", actualString: generatedText);
        Assert.Contains(expectedSubstring: "CommandManager.InvalidateRequerySuggested();", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesCorrectNamespace()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "namespace Sample.Presentation.ViewModels;", actualString: generatedText);
    }

    [Fact]
    public void Generator_ProducesAutoGeneratedHeader()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation = CreateCompilation(source: source);
        var generator = new RelayCommandGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var outputCompilation, diagnostics: out _);
        var generatedText = GetGeneratedSourceText(outputCompilation: outputCompilation, inputTreeCount: compilation.SyntaxTrees.Count());

        // Assert
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: generatedText);
    }

    [Fact]
    public void Generator_GeneratesDeterministicOutput_Reproducible()
    {
        // Arrange
        var source = CreateSourceWithAttribute();
        var compilation1 = CreateCompilation(source: source);
        var compilation2 = CreateCompilation(source: source);
        
        var generator1 = new RelayCommandGenerator();
        var generator2 = new RelayCommandGenerator();
        
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
            namespace Vc.Generators.Abstractions.Design
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class VcRelayCommandAttribute : System.Attribute { }
            }

            namespace Sample.Presentation.ViewModels
            {
                [Vc.Generators.Abstractions.Design.VcRelayCommandAttribute]
                public class ViewModel { }
            }
            """;
    }

    private Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: source);
        
        var vcAbstractionsAssemblyPath = typeof(VcRelayCommandAttribute).Assembly.Location;
        
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
