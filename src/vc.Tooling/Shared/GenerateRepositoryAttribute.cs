using System;
namespace VisionaryCoder.Tooling.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GenerateRepositoryAttribute : Attribute
    {
        public string? InterfaceName { get; set; }
        public bool AsyncOnly { get; set; } = true;
        public bool SoftDelete { get; set; } = false;
        public bool Paginated { get; set; } = true;
        public GenerateRepositoryAttribute() { }
    }
}