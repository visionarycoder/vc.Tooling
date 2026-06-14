namespace Vc.Generators.Common.Diagnostics;

public static class VcGeneratorDiagnostics
{
    public static readonly DiagnosticDescriptor InvalidAttributeUsage =
        new(
            id: "VCG001",
            title: "Invalid attribute usage",
            messageFormat: "The attribute '{0}' is used incorrectly.",
            category: "Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic CreateError(string id, string message, Location? location = null)
    {
        return Diagnostic.Create(new DiagnosticDescriptor(
                id, "Generator Error", message, "Generators",
                DiagnosticSeverity.Error, true), location);
    }

    public static readonly DiagnosticDescriptor MissingAttribute =
        new("VCGEN001", "Missing attribute",
            "The required attribute '{0}' was not found.", "Generators",
            DiagnosticSeverity.Warning, true);

           public sealed class VcGeneratorException : Exception
{
    public VcGeneratorException(string message) : base(message) { }
} 
}
