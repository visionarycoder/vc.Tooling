namespace VisionaryCoder.Generators.Common.Utilities;

public static class GeneratorExecutionContextExtensions
{
    public static void AddGeneratedFile(this GeneratorExecutionContext context, string name, string source)
    {
        context.AddSource(hintName: name, source: source);
    }
}