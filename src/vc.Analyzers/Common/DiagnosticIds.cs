namespace VisionaryCoder.Tooling.Analyzers.Common;

/// <summary>
/// Centralized diagnostic ID definitions for all vc.Analyzers.
/// Format: VC{Category}{Number:0000}
/// Example: VCARCH0001 (Architecture, rule 1)
/// </summary>
public static class DiagnosticIds
{
    // Architecture Category (VCARCH)
    public const string ArchLayeringViolation = "VCARCH0001";
    public const string ArchCyclicDependency = "VCARCH0002";
    public const string ArchNamespaceBoundaryViolation = "VCARCH0003";
    public const string ArchProjectReferenceViolation = "VCARCH0004";
    public const string ArchVbdBoundaryViolation = "VCARCH0005";
    public const string ArchDependencyInjectionViolation = "VCARCH0006";

    // API Category (VCAPI)
    public const string ApiDesignControllerMissing = "VCAPI0001";
    public const string ApiDesignNaming = "VCAPI0002";
    public const string ApiVersioningMissing = "VCAPI0003";
    public const string ApiResponseSerializationIssue = "VCAPI0004";
    public const string ApiHttpMethodIncorrect = "VCAPI0005";
    public const string ApiValidationMissing = "VCAPI0006";

    // Data Transfer Object Category (VCDTO)
    public const string DtoMutableState = "VCDTO0001";
    public const string DtoCircularReference = "VCDTO0002";

    // Data Design Category (VCDATA)
    public const string DataDesignDtoMissing = "VCDATA0001";
    public const string DataDesignRepositoryMissing = "VCDATA0002";

    // Mapping Category (VCMAP)
    public const string MappingUnmappedProperties = "VCMAP0001";
    public const string MappingConfigurationIssue = "VCMAP0002";

    // Async Category (VCASYNC)
    public const string AsyncVoidMethod = "VCASYNC0001";
    public const string AsyncBlockingCall = "VCASYNC0002";
    public const string AsyncMissingNameSuffix = "VCASYNC0003";
    public const string AsyncFireAndForget = "VCASYNC0004";
    public const string AsyncOverheadInHotPath = "VCASYNC0005";

    // Design Category (VCDESIGN)
    public const string DesignExceptionSafetyEmptyCatch = "VCDESIGN0001";
    public const string DesignExceptionSafetyBroadCatch = "VCDESIGN0002";
    public const string DesignExceptionSafetySwallowed = "VCDESIGN0003";
    public const string DesignImmutabilityViolation = "VCDESIGN0004";
    public const string DesignLoggingMissing = "VCDESIGN0005";
    public const string DesignDisposeMissing = "VCDESIGN0006";

    // VBD Access Vault Category (VCVBDA)
    public const string VbdAccessVaultBusinessLogicLeakage = "VCVBDA0001";
    public const string VbdAccessVaultSchemaMappingMissing = "VCVBDA0002";
    public const string VbdAccessVaultBoundaryViolation = "VCVBDA0003";

    // VBD Engine Vault Category (VCVBDE)
    public const string VbdEngineVaultInfrastructureAccess = "VCVBDE0001";
    public const string VbdEngineVaultNondeterminism = "VCVBDE0002";
    public const string VbdEngineVaultStateViolation = "VCVBDE0003";

    // VBD Manager Vault Category (VCVBDM)
    public const string VbdManagerVaultUnstableContract = "VCVBDM0001";
    public const string VbdManagerVaultFeatureSpecificLogic = "VCVBDM0002";
    public const string VbdManagerVaultMissingOrchestration = "VCVBDM0003";

    // Distributed/Event Sourcing Category (VCDIST)
    public const string DistributedEventSourcingMissingApply = "VCDIST0001";
    public const string DistributedEventSourcingUnusedEvent = "VCDIST0002";
    public const string DistributedEventSourcingMutableState = "VCDIST0003";
    public const string DistributedRepositoryAsyncMissing = "VCDIST0004";
    public const string DistributedRepositoryQueryableLeakage = "VCDIST0005";
    public const string DistributedRepositoryContractViolation = "VCDIST0006";
    public const string DistributedMessageContractIssue = "VCDIST0007";
    public const string DistributedIdempotencyMissing = "VCDIST0008";
    public const string DistributedSagaCompensationMissing = "VCDIST0009";

    // Performance Category (VCPERF)
    public const string PerfLinqInHotPath = "VCPERF0001";
    public const string PerfAllocationInHotPath = "VCPERF0002";
    public const string PerfRegexRecompilation = "VCPERF0003";
    public const string PerfStringConcatenationInLoop = "VCPERF0004";
    public const string PerfLockContention = "VCPERF0005";
    public const string PerfReflectionInHotPath = "VCPERF0006";
    public const string PerfBoxing = "VCPERF0007";

    // Resilience Category (VCRES)
    public const string ResilienceCircuitBreakerMissing = "VCRES0001";
    public const string ResilienceCircuitBreakerUnused = "VCRES0002";
    public const string ResilienceRetryPolicyMissing = "VCRES0003";
    public const string ResilienceRetryPolicyExcessive = "VCRES0004";
    public const string ResilienceRetryConfigurationIssue = "VCRES0005";
    public const string ResilienceTimeoutMissing = "VCRES0006";
    public const string ResilienceTimeoutExcessive = "VCRES0007";
    public const string ResilienceTimeoutConfigurationIssue = "VCRES0008";
    public const string ResiliencePolicyMissing = "VCRES0009";
    public const string ResilienceCancellationTokenMissing = "VCRES0010";
    public const string ResilienceBulkheadMissing = "VCRES0011";
    public const string ResilienceBackpressureMissing = "VCRES0012";

    // Observability Category (VCOBS)
    public const string ObservabilityTelemetryMissing = "VCOBS0001";
    public const string ObservabilityTracingMissing = "VCOBS0002";
    public const string ObservabilityMetricsMissing = "VCOBS0003";

    // Security Category (VCSEC)
    public const string SecurityHardcodedSecret = "VCSEC0001";
    public const string SecuritySqlInjection = "VCSEC0002";
    public const string SecurityAuthorizationMissing = "VCSEC0003";
    public const string SecurityUnsafeDeserialization = "VCSEC0004";
    public const string SecurityCryptographyWeak = "VCSEC0005";
    public const string SecurityXssVulnerability = "VCSEC0006";
    public const string SecurityCsrfVulnerability = "VCSEC0007";
    public const string SecurityInputValidationMissing = "VCSEC0008";
    public const string SecuritySensitiveDataInLogs = "VCSEC0009";

    // Null Safety Category (VCNULL)
    public const string NullSafetyMissingCheck = "VCNULL0001";
    public const string NullSafetyNullableWarning = "VCNULL0002";

    // Documentation Category (VCDOC)
    public const string DocumentationMissingXmlDoc = "VCDOC0001";
    public const string DocumentationIncompleteXmlDoc = "VCDOC0002";

    // Naming Convention Category (VCNAMING)
    public const string NamingConventionViolation = "VCNAMING0001";
    public const string NamingInconsistency = "VCNAMING0002";

    /// <summary>
    /// Gets the category prefix from a diagnostic ID (e.g., "ARCH" from "VCARCH0001").
    /// </summary>
    public static string GetCategory(string diagnosticId)
    {
        // Format: VC{Category}{Number}
        // Example: VCARCH0001 -> ARCH
        if (diagnosticId.StartsWith("VC") && diagnosticId.Length >= 6)
        {
            return diagnosticId.Substring(2, diagnosticId.Length - 6);
        }
        return "Unknown";
    }

    /// <summary>
    /// Gets all diagnostic IDs organized by category.
    /// </summary>
    public static IEnumerable<string> GetAllDiagnosticIds()
    {
        yield return ArchLayeringViolation;
        yield return ArchCyclicDependency;
        yield return ArchNamespaceBoundaryViolation;
        yield return ArchProjectReferenceViolation;
        yield return ArchVbdBoundaryViolation;
        yield return ArchDependencyInjectionViolation;

        yield return ApiDesignControllerMissing;
        yield return ApiDesignNaming;
        yield return ApiVersioningMissing;
        yield return ApiResponseSerializationIssue;
        yield return ApiHttpMethodIncorrect;
        yield return ApiValidationMissing;

        yield return DtoMutableState;
        yield return DtoCircularReference;

        yield return MappingUnmappedProperties;
        yield return MappingConfigurationIssue;

        yield return AsyncVoidMethod;
        yield return AsyncBlockingCall;
        yield return AsyncMissingNameSuffix;
        yield return AsyncFireAndForget;
        yield return AsyncOverheadInHotPath;

        yield return DesignExceptionSafetyEmptyCatch;
        yield return DesignExceptionSafetyBroadCatch;
        yield return DesignExceptionSafetySwallowed;
        yield return DesignImmutabilityViolation;
        yield return DesignLoggingMissing;
        yield return DesignDisposeMissing;

        yield return VbdAccessVaultBusinessLogicLeakage;
        yield return VbdAccessVaultSchemaMappingMissing;
        yield return VbdAccessVaultBoundaryViolation;

        yield return VbdEngineVaultInfrastructureAccess;
        yield return VbdEngineVaultNondeterminism;
        yield return VbdEngineVaultStateViolation;

        yield return VbdManagerVaultUnstableContract;
        yield return VbdManagerVaultFeatureSpecificLogic;
        yield return VbdManagerVaultMissingOrchestration;

        yield return DistributedEventSourcingMissingApply;
        yield return DistributedEventSourcingUnusedEvent;
        yield return DistributedEventSourcingMutableState;
        yield return DistributedRepositoryAsyncMissing;
        yield return DistributedRepositoryQueryableLeakage;
        yield return DistributedRepositoryContractViolation;
        yield return DistributedMessageContractIssue;
        yield return DistributedIdempotencyMissing;
        yield return DistributedSagaCompensationMissing;

        yield return PerfLinqInHotPath;
        yield return PerfAllocationInHotPath;
        yield return PerfRegexRecompilation;
        yield return PerfStringConcatenationInLoop;
        yield return PerfLockContention;
        yield return PerfReflectionInHotPath;
        yield return PerfBoxing;

        yield return ResilienceCircuitBreakerMissing;
        yield return ResilienceCircuitBreakerUnused;
        yield return ResilienceRetryPolicyMissing;
        yield return ResilienceRetryPolicyExcessive;
        yield return ResilienceRetryConfigurationIssue;
        yield return ResilienceTimeoutMissing;
        yield return ResilienceTimeoutExcessive;
        yield return ResilienceTimeoutConfigurationIssue;
        yield return ResiliencePolicyMissing;
        yield return ResilienceCancellationTokenMissing;
        yield return ResilienceBulkheadMissing;
        yield return ResilienceBackpressureMissing;

        yield return SecurityHardcodedSecret;
        yield return SecuritySqlInjection;
        yield return SecurityAuthorizationMissing;
        yield return SecurityUnsafeDeserialization;
        yield return SecurityCryptographyWeak;
        yield return SecurityXssVulnerability;
        yield return SecurityCsrfVulnerability;
        yield return SecurityInputValidationMissing;
        yield return SecuritySensitiveDataInLogs;

        yield return NullSafetyMissingCheck;
        yield return NullSafetyNullableWarning;

        yield return DocumentationMissingXmlDoc;
        yield return DocumentationIncompleteXmlDoc;

        yield return NamingConventionViolation;
        yield return NamingInconsistency;

    }
}
