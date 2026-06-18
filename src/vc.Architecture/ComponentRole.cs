namespace VisionaryCoder.Architecture;

/// <summary>
/// Defines the architectural role assigned to a component.
/// </summary>
public enum ComponentRole
{
    /// <summary>
    /// Role is not specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Coordinates policies and workflow orchestration.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Encapsulates deterministic business algorithms.
    /// </summary>
    Engine = 2,

    /// <summary>
    /// Integrates with infrastructure and external systems.
    /// </summary>
    Access = 3,

    /// <summary>
    /// Adapts external contracts to internal abstractions.
    /// </summary>
    Adapter = 4,

    /// <summary>
    /// Exposes reusable application services.
    /// </summary>
    Service = 5,

    /// <summary>
    /// Provides shared helper functionality.
    /// </summary>
    Utility = 6,
}
