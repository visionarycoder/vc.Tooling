namespace VisionaryCoder.Tooling.Shared
{
    [AttributeUsage(
        validOn: AttributeTargets.Class | AttributeTargets.Method |
                 AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = false)]
    public sealed class SuppressAnalyzerAttribute(string justification, params string[] diagnosticIds) : Attribute
    {
        public string[] DiagnosticIds { get; } = diagnosticIds ?? throw new ArgumentNullException(paramName: nameof(diagnosticIds));
        public string Justification { get; } = justification ?? throw new ArgumentNullException(paramName: nameof(justification));
    }
}