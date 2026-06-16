using Microsoft.CodeAnalysis;

namespace Vc.Generators.Common.Utilities;

public static class SourceEmitter
{
    public static void Emit(IncrementalGeneratorPostInitializationContext context, string fileName, string source)
    {
        context.AddSource(fileName, source);
    }

    public static void Emit(IncrementalGeneratorInitializationContext context, string fileName, string source)
    {
        context.RegisterPostInitializationOutput(i => i.AddSource(fileName, source));
    }
}

public static class FileNameHelper
{
    public static string Normalize(string name)
    {
        return name.Replace("<", "").Replace(">", "").Replace(":", "_");
    }
}

public static class TypeNameHelper
{
    public static string Sanitize(string name)
    {
        return name.Replace(".", "_").Replace("+", "_");
    }
}

public static class RoslynLogger
{
    public static void Log(GeneratorExecutionContext context, string message)
    {
        context.ReportDiagnostic(Diagnostics.VcGeneratorDiagnostics.CreateError("VCGENLOG", message));
    }
}

public static class GeneratorExecutionContextExtensions
{
    public static void AddGeneratedFile(this GeneratorExecutionContext context, string name, string source)
    {
        context.AddSource(name, source);
    }
}