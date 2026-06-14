Implement a DTO source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Data

Requirements:
- Attribute: [GenerateDto] (define in Vc.Tooling.Abstractions if missing)
- Input: classes annotated with [GenerateDto]
- Output:
  - Immutable DTO record in Vc.Generated.Data
  - Properties mirrored from source (public get; init;)
  - Mapping extension: ToDto(this T source)
- Use Incremental generator + shared models/utilities.
Return only the .cs file contents.
