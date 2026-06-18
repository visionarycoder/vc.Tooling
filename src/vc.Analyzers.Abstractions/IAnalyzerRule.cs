namespace VisionaryCoder.Analyzers.Abstractions;

/// <summary>
/// Represents a rule that can be used by analyzers to define diagnostics and register analysis actions.
/// </summary>
public interface IAnalyzerRule
{
    /// <summary>
    /// Gets the <see cref="DiagnosticDescriptor"/> that defines the diagnostic rule.
    /// </summary>
    /// <remarks>
    /// The <see cref="DiagnosticDescriptor"/> provides metadata about the rule, including its ID, title, 
    /// message format, category, default severity, and whether it is enabled by default.
    /// </remarks>
    DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Registers the analysis actions for the rule.
    /// </summary>
    /// <param name="context">The analysis context used to register actions.</param>
    void Register(AnalysisContext context);

}