using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.Analyzers.Common;

public interface IAnalyzerRule
{
    DiagnosticDescriptor Descriptor { get; }

    void Register(AnalysisContext context);
}