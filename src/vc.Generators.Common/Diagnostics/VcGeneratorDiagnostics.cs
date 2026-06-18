namespace VisionaryCoder.Generators.Common.Diagnostics;

public static class VcGeneratorDiagnostics
{
    
    public static readonly DiagnosticDescriptor InvalidAttributeUsage =
        new(
            id: "VCG001",
            title: "Invalid attribute usage",
            messageFormat: "The attribute '{0}' is used incorrectly",
            category: "Generators",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic CreateError(string id, string message, Location? location = null)
    {
        return Diagnostic.Create(descriptor: new DiagnosticDescriptor(
                id: id, title: "Generator Error", messageFormat: message, category: "Generators",
                defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true), location: location);
    }

    public static readonly DiagnosticDescriptor MissingAttribute =
        new(id: "VCGEN001", title: "Missing attribute",
            messageFormat: "The required attribute '{0}' was not found", 
            category: "Generators",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true);

           public sealed class VcGeneratorException(string message) : Exception(message: message); 
}
