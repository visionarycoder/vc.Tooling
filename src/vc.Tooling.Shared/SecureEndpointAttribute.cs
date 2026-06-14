using System;
namespace VisionaryCoder.Tooling.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SecureEndpointAttribute : Attribute
    {
        public string? Policy { get; set; }
        public string? Roles { get; set; }
        public string Scheme { get; set; } = "Bearer";
        public bool RateLimit { get; set; } = true;
        public int MaxRequests { get; set; } = 60;
        public int WindowSeconds { get; set; } = 60;
        public bool RequireApiKey { get; set; } = false;
        public SecureEndpointAttribute() { }
        public SecureEndpointAttribute(string policy) => Policy = policy;
    }
}