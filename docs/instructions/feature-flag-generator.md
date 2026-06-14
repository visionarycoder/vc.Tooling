Implement a Feature Flag source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Resilience (or Design)

Requirements:
- Attribute: [FeatureFlag(string name)] (define in Abstractions)
- Input:
  - Partial classes annotated with [FeatureFlag]
- Output:
  - Strongly typed feature flag accessor class in Vc.Generated.Resilience
  - Methods like IsEnabled(IServiceProvider) or IsEnabled(IFeatureManager)
  - Optional extension methods for IServiceCollection registration
Use Incremental generator. Emit diagnostics for duplicate flag names. Return only the .cs file contents.
