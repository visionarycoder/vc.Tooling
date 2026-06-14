using System;
namespace VisionaryCoder.Tooling.Shared.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method |
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = false)]
    public sealed class SuppressAnalyzerAttribute : Attribute
    {
        public string[] DiagnosticIds { get; }
        public string Justification { get; }
        public SuppressAnalyzerAttribute(string justification, params string[] diagnosticIds)
        {
            Justification = justification ?? throw new ArgumentNullException(nameof(justification));
            DiagnosticIds = diagnosticIds ?? throw new ArgumentNullException(nameof(diagnosticIds));
        }
    }
}