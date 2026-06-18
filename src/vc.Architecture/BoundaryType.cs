namespace VisionaryCoder.Architecture;

/// <summary>
/// 
/// </summary>
public enum BoundaryType
{
    /// <summary>
    /// Represents an unknown or unspecified architectural boundary.
    /// This value is used as a default when the boundary type is not explicitly defined.
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Represents the architecture boundary.
    /// </summary>
    Architecture = 1,
    /// <summary>
    /// Represents the domain boundary.
    /// </summary>
    Domain = 2,
    /// <summary>
    /// Represents the runtime boundary.
    /// </summary>
    Runtime = 3,
    /// <summary>
    /// Represents the integration boundary.
    /// </summary>
    Integration = 4,
    /// <summary>
    /// Represents the utility boundary.
    /// </summary>
    Utility = 5,
    /// <summary>
    /// Represents the tooling boundary.
    /// </summary>  
    Tooling = 6,
}
