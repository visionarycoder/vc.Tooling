namespace VisionaryCoder.Tooling.Shared
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GenerateValidatorAttribute : Attribute
    {
        public bool UseDataAnnotations { get; set; } = true;
        public bool CascadeChildren { get; set; } = false;
        public string ClassSuffix { get; set; } = "Validator";
        public GenerateValidatorAttribute() { }
    }
}