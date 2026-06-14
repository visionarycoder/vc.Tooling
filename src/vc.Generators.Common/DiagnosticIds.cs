namespace VisionaryCoder.Tooling.Shared.Models
{
    public static class DiagnosticIds
    {
        public const string UnguardedNull        = "VC0001";
        public const string UncheckedNullReturn  = "VC0002";
        public const string AsyncVoid            = "VC1001";
        public const string SyncOverAsync        = "VC1002";
        public const string EmptyCatch           = "VC2001";
        public const string BroadCatch           = "VC2002";
        public const string HardcodedSecret      = "VC3001";
        public const string SqlInjectionRisk     = "VC3002";
        public const string MissingAuthorization = "VC3003";
        public const string UnusedParameter      = "VC4001";
        public const string MissingXmlDoc        = "VC4002";
        public const string NamingConvention     = "VC4003";
    }
}