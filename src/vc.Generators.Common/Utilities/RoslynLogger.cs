namespace VisionaryCoder.Generators.Common.Utilities;

public static class RoslynLogger
{
    public static void Log(GeneratorExecutionContext context, string message)
    {
        context.ReportDiagnostic(diagnostic: Diagnostics.VcGeneratorDiagnostics.CreateError(id: "VCGENLOG", message: message));
    }
}