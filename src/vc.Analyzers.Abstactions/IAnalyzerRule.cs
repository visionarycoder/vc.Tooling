namespace vc.Analyzers.Abstactions;

/// <summary>
/// Represents a rule that can be analyzed by a diagnostic analyzer.
/// </summary>
public interface IAnalyzerRule
{
    /// <summary>
    /// Gets the descriptor for the diagnostic rule.
    /// </summary>
    DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Registers the rule with the analysis context.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    void Register(AnalysisContext context);

}