namespace Vc.Analyzers.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionSafetyAnalyzer : DiagnosticAnalyzer
{
    // IDs:
    // VCEX001 – Empty catch
    // VCEX002 – Swallowed exception
    // VCEX003 – Overly general catch
    // VCEX004 – Async void misuse

    // TODO: Full implementation exists in design notes; see Pass 2 section.
    // This stub exists to anchor the class and document intent.

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray<DiagnosticDescriptor>.Empty;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Wire up:
        // - CatchClause analysis for empty/swallowed/general catches.
        // - MethodDeclaration analysis for async void methods.
    }
}