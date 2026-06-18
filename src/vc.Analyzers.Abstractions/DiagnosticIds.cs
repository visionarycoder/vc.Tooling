namespace VisionaryCoder.Analyzers.Abstractions;

/// <summary>
/// Centralized diagnostic ID definitions for all vc.Analyzers.
/// Format: VC{Category}{Number:0000}
/// Example: VCARCH0001 (Architecture, rule 1)
/// </summary>
public static class DiagnosticIds
{
    // Architecture Category (VCARCH)
    /// <summary>Layering violation diagnostic ID.</summary>
    public const string ArchLayeringViolation = "VCARCH0001";
    /// <summary>Cyclic dependency diagnostic ID.</summary>
    public const string ArchCyclicDependency = "VCARCH0002";
    /// <summary>Namespace boundary violation diagnostic ID.</summary>
    public const string ArchNamespaceBoundaryViolation = "VCARCH0003";
    /// <summary>Project reference violation diagnostic ID.</summary>
    public const string ArchProjectReferenceViolation = "VCARCH0004";
    /// <summary>VBD boundary violation diagnostic ID.</summary>
    public const string ArchVbdBoundaryViolation = "VCARCH0005";
    /// <summary>Dependency injection violation diagnostic ID.</summary>
    public const string ArchDependencyInjectionViolation = "VCARCH0006";

    // API Category (VCAPI)
    /// <summary>Missing API controller diagnostic ID.</summary>
    public const string ApiDesignControllerMissing = "VCAPI0001";
    /// <summary>API naming convention diagnostic ID.</summary>
    public const string ApiDesignNaming = "VCAPI0002";
    /// <summary>Missing API versioning diagnostic ID.</summary>
    public const string ApiVersioningMissing = "VCAPI0003";
    /// <summary>API response serialization issue diagnostic ID.</summary>
    public const string ApiResponseSerializationIssue = "VCAPI0004";
    /// <summary>Incorrect API HTTP method diagnostic ID.</summary>
    public const string ApiHttpMethodIncorrect = "VCAPI0005";
    /// <summary>Missing API validation diagnostic ID.</summary>
    public const string ApiValidationMissing = "VCAPI0006";

    // Data Transfer Object Category (VCDTO)
    /// <summary>DTO mutable state diagnostic ID.</summary>
    public const string DtoMutableState = "VCDTO0001";
    /// <summary>DTO circular reference diagnostic ID.</summary>
    public const string DtoCircularReference = "VCDTO0002";

    // Data Design Category (VCDATA)
    /// <summary>Missing DTO design diagnostic ID.</summary>
    public const string DataDesignDtoMissing = "VCDATA0001";
    /// <summary>Missing repository design diagnostic ID.</summary>
    public const string DataDesignRepositoryMissing = "VCDATA0002";

    // Core Category (VCCORE)
    /// <summary>Missing immutability rule diagnostic ID.</summary>
    public const string CoreImmutableMissing = "VCCORE0001";
    /// <summary>Missing value object rule diagnostic ID.</summary>
    public const string CoreValueObjectMissing = "VCCORE0002";
    /// <summary>Missing aggregate root rule diagnostic ID.</summary>
    public const string CoreAggregateRootMissing = "VCCORE0003";

    // Mapping Category (VCMAP)
    /// <summary>Unmapped properties diagnostic ID.</summary>
    public const string MappingUnmappedProperties = "VCMAP0001";
    /// <summary>Mapping configuration issue diagnostic ID.</summary>
    public const string MappingConfigurationIssue = "VCMAP0002";

    // Async Category (VCASYNC)
    /// <summary>Async void method diagnostic ID.</summary>
    public const string AsyncVoidMethod = "VCASYNC0001";
    /// <summary>Blocking call in async code diagnostic ID.</summary>
    public const string AsyncBlockingCall = "VCASYNC0002";
    /// <summary>Missing async suffix diagnostic ID.</summary>
    public const string AsyncMissingNameSuffix = "VCASYNC0003";
    /// <summary>Fire-and-forget usage diagnostic ID.</summary>
    public const string AsyncFireAndForget = "VCASYNC0004";
    /// <summary>Async overhead in hot path diagnostic ID.</summary>
    public const string AsyncOverheadInHotPath = "VCASYNC0005";

    // Design Category (VCDESIGN)
    /// <summary>Empty catch block diagnostic ID.</summary>
    public const string DesignExceptionSafetyEmptyCatch = "VCDESIGN0001";
    /// <summary>Broad catch usage diagnostic ID.</summary>
    public const string DesignExceptionSafetyBroadCatch = "VCDESIGN0002";
    /// <summary>Swallowed exception diagnostic ID.</summary>
    public const string DesignExceptionSafetySwallowed = "VCDESIGN0003";
    /// <summary>Immutability violation diagnostic ID.</summary>
    public const string DesignImmutabilityViolation = "VCDESIGN0004";
    /// <summary>Missing logging diagnostic ID.</summary>
    public const string DesignLoggingMissing = "VCDESIGN0005";
    /// <summary>Missing dispose pattern diagnostic ID.</summary>
    public const string DesignDisposeMissing = "VCDESIGN0006";

    // VBD Access Vault Category (VCVBDA)
    /// <summary>VBD access business-logic leakage diagnostic ID.</summary>
    public const string VbdAccessVaultBusinessLogicLeakage = "VCVBDA0001";
    /// <summary>VBD access schema-mapping missing diagnostic ID.</summary>
    public const string VbdAccessVaultSchemaMappingMissing = "VCVBDA0002";
    /// <summary>VBD access boundary violation diagnostic ID.</summary>
    public const string VbdAccessVaultBoundaryViolation = "VCVBDA0003";

    // VBD Engine Vault Category (VCVBDE)
    /// <summary>VBD engine infrastructure access diagnostic ID.</summary>
    public const string VbdEngineVaultInfrastructureAccess = "VCVBDE0001";
    /// <summary>VBD engine nondeterminism diagnostic ID.</summary>
    public const string VbdEngineVaultNondeterminism = "VCVBDE0002";
    /// <summary>VBD engine state violation diagnostic ID.</summary>
    public const string VbdEngineVaultStateViolation = "VCVBDE0003";

    // VBD Manager Vault Category (VCVBDM)
    /// <summary>VBD manager unstable contract diagnostic ID.</summary>
    public const string VbdManagerVaultUnstableContract = "VCVBDM0001";
    /// <summary>VBD manager feature-specific logic diagnostic ID.</summary>
    public const string VbdManagerVaultFeatureSpecificLogic = "VCVBDM0002";
    /// <summary>VBD manager missing orchestration diagnostic ID.</summary>
    public const string VbdManagerVaultMissingOrchestration = "VCVBDM0003";

    // Distributed/Event Sourcing Category (VCDIST)
    /// <summary>Missing event-sourcing apply method diagnostic ID.</summary>
    public const string DistributedEventSourcingMissingApply = "VCDIST0001";
    /// <summary>Unused event diagnostic ID.</summary>
    public const string DistributedEventSourcingUnusedEvent = "VCDIST0002";
    /// <summary>Mutable event-sourcing state diagnostic ID.</summary>
    public const string DistributedEventSourcingMutableState = "VCDIST0003";
    /// <summary>Missing async repository method diagnostic ID.</summary>
    public const string DistributedRepositoryAsyncMissing = "VCDIST0004";
    /// <summary>IQueryable leakage diagnostic ID.</summary>
    public const string DistributedRepositoryQueryableLeakage = "VCDIST0005";
    /// <summary>Repository contract violation diagnostic ID.</summary>
    public const string DistributedRepositoryContractViolation = "VCDIST0006";
    /// <summary>Distributed message contract issue diagnostic ID.</summary>
    public const string DistributedMessageContractIssue = "VCDIST0007";
    /// <summary>Missing idempotency diagnostic ID.</summary>
    public const string DistributedIdempotencyMissing = "VCDIST0008";
    /// <summary>Missing saga compensation diagnostic ID.</summary>
    public const string DistributedSagaCompensationMissing = "VCDIST0009";

    // Performance Category (VCPERF)
    /// <summary>LINQ in hot path diagnostic ID.</summary>
    public const string PerfLinqInHotPath = "VCPERF0001";
    /// <summary>Allocation in hot path diagnostic ID.</summary>
    public const string PerfAllocationInHotPath = "VCPERF0002";
    /// <summary>Regex recompilation diagnostic ID.</summary>
    public const string PerfRegexRecompilation = "VCPERF0003";
    /// <summary>String concatenation in loop diagnostic ID.</summary>
    public const string PerfStringConcatenationInLoop = "VCPERF0004";
    /// <summary>Lock contention diagnostic ID.</summary>
    public const string PerfLockContention = "VCPERF0005";
    /// <summary>Reflection in hot path diagnostic ID.</summary>
    public const string PerfReflectionInHotPath = "VCPERF0006";
    /// <summary>Boxing operation diagnostic ID.</summary>
    public const string PerfBoxing = "VCPERF0007";

    // Resilience Category (VCRES)
    /// <summary>Missing circuit breaker diagnostic ID.</summary>
    public const string ResilienceCircuitBreakerMissing = "VCRES0001";
    /// <summary>Unused circuit breaker diagnostic ID.</summary>
    public const string ResilienceCircuitBreakerUnused = "VCRES0002";
    /// <summary>Missing retry policy diagnostic ID.</summary>
    public const string ResilienceRetryPolicyMissing = "VCRES0003";
    /// <summary>Excessive retry policy diagnostic ID.</summary>
    public const string ResilienceRetryPolicyExcessive = "VCRES0004";
    /// <summary>Retry configuration issue diagnostic ID.</summary>
    public const string ResilienceRetryConfigurationIssue = "VCRES0005";
    /// <summary>Missing timeout policy diagnostic ID.</summary>
    public const string ResilienceTimeoutMissing = "VCRES0006";
    /// <summary>Excessive timeout diagnostic ID.</summary>
    public const string ResilienceTimeoutExcessive = "VCRES0007";
    /// <summary>Timeout configuration issue diagnostic ID.</summary>
    public const string ResilienceTimeoutConfigurationIssue = "VCRES0008";
    /// <summary>Missing resilience policy diagnostic ID.</summary>
    public const string ResiliencePolicyMissing = "VCRES0009";
    /// <summary>Missing cancellation token diagnostic ID.</summary>
    public const string ResilienceCancellationTokenMissing = "VCRES0010";
    /// <summary>Missing bulkhead policy diagnostic ID.</summary>
    public const string ResilienceBulkheadMissing = "VCRES0011";
    /// <summary>Missing backpressure policy diagnostic ID.</summary>
    public const string ResilienceBackpressureMissing = "VCRES0012";

    // Observability Category (VCOBS)
    /// <summary>Missing telemetry diagnostic ID.</summary>
    public const string ObservabilityTelemetryMissing = "VCOBS0001";
    /// <summary>Missing tracing diagnostic ID.</summary>
    public const string ObservabilityTracingMissing = "VCOBS0002";
    /// <summary>Missing metrics diagnostic ID.</summary>
    public const string ObservabilityMetricsMissing = "VCOBS0003";

    // Security Category (VCSEC)
    /// <summary>Hardcoded secret diagnostic ID.</summary>
    public const string SecurityHardcodedSecret = "VCSEC0001";
    /// <summary>SQL injection diagnostic ID.</summary>
    public const string SecuritySqlInjection = "VCSEC0002";
    /// <summary>Missing authorization diagnostic ID.</summary>
    public const string SecurityAuthorizationMissing = "VCSEC0003";
    /// <summary>Unsafe deserialization diagnostic ID.</summary>
    public const string SecurityUnsafeDeserialization = "VCSEC0004";
    /// <summary>Weak cryptography diagnostic ID.</summary>
    public const string SecurityCryptographyWeak = "VCSEC0005";
    /// <summary>XSS vulnerability diagnostic ID.</summary>
    public const string SecurityXssVulnerability = "VCSEC0006";
    /// <summary>CSRF vulnerability diagnostic ID.</summary>
    public const string SecurityCsrfVulnerability = "VCSEC0007";
    /// <summary>Missing input validation diagnostic ID.</summary>
    public const string SecurityInputValidationMissing = "VCSEC0008";
    /// <summary>Sensitive data in logs diagnostic ID.</summary>
    public const string SecuritySensitiveDataInLogs = "VCSEC0009";

    // Null Safety Category (VCNULL)
    /// <summary>Missing null check diagnostic ID.</summary>
    public const string NullSafetyMissingCheck = "VCNULL0001";
    /// <summary>Nullable warning diagnostic ID.</summary>
    public const string NullSafetyNullableWarning = "VCNULL0002";

    // Documentation Category (VCDOC)
    /// <summary>Missing XML documentation diagnostic ID.</summary>
    public const string DocumentationMissingXmlDoc = "VCDOC0001";
    /// <summary>Incomplete XML documentation diagnostic ID.</summary>
    public const string DocumentationIncompleteXmlDoc = "VCDOC0002";

    // Naming Convention Category (VCNAMING)
    /// <summary>Naming convention violation diagnostic ID.</summary>
    public const string NamingConventionViolation = "VCNAMING0001";
    /// <summary>Naming inconsistency diagnostic ID.</summary>
    public const string NamingInconsistency = "VCNAMING0002";

    // Legacy/Stub Category (VC)
    // These are stub analyzer implementations pending full implementation
    /// <summary>Legacy configuration diagnostic ID.</summary>
    public const string LegacyConfiguration = "VC0001";
    /// <summary>Legacy missing configuration diagnostic ID.</summary>
    public const string LegacyMissingConfiguration = "VC0002";
    /// <summary>Legacy dependency injection diagnostic ID.</summary>
    public const string LegacyDependencyInjection = "VC0003";
    /// <summary>Legacy serialization diagnostic ID.</summary>
    public const string LegacySerialization = "VC0004";
    /// <summary>Legacy reflection diagnostic ID.</summary>
    public const string LegacyReflection = "VC0005";
    /// <summary>Legacy interop diagnostic ID.</summary>
    public const string LegacyInterop = "VC0006";

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
