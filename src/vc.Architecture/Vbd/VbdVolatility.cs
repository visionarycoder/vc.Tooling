namespace VisionaryCoder.Architecture.Vbd;

/// <summary>
/// Represents the dominant source of change for a VBD component.
/// </summary>
public enum VbdVolatility
{
    /// <summary>
    /// Volatility has not been classified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Changes are primarily driven by policies and orchestration flows.
    /// </summary>
    PolicyOrchestration = 1,

    /// <summary>
    /// Changes are primarily driven by algorithmic behavior.
    /// </summary>
    Algorithm = 2,

    /// <summary>
    /// Changes are primarily driven by infrastructure integrations.
    /// </summary>
    InfrastructureIntegration = 3,
}
