Implement a configuration source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Resilience

Requirements:
- Attribute: [ConfigSection(string name)] (define in Abstractions)
- Input: POCO config classes annotated with [ConfigSection]
- Output:
  - Extension method: Add<ConfigType>Configuration(this IServiceCollection, IConfiguration)
  - Binds section, validates, registers IOptions<ConfigType>
Generated namespace: Vc.Generated.Resilience. Use Incremental generator. Return only the .cs file contents.
