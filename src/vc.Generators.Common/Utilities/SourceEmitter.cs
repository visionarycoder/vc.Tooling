namespace VisionaryCoder.Generators.Common.Utilities;

public static class SourceEmitter
{
    public static void Emit(IncrementalGeneratorPostInitializationContext context, string fileName, string source)
    {
        context.AddSource(hintName: fileName, source: source);
    }

    public static void Emit(IncrementalGeneratorInitializationContext context, string fileName, string source)
    {
        context.RegisterPostInitializationOutput(callback: i => i.AddSource(hintName: fileName, source: source));
    }
}