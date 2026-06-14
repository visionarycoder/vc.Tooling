Implement a Builder Pattern source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Design

Requirements:
- Attribute: [GenerateBuilder] (define in Abstractions)
- Input:
  - Classes annotated with [GenerateBuilder]
- Output:
  - <TypeName>Builder class in Vc.Generated.Design
  - Fluent WithX(...) methods for each settable property
  - Build() method that constructs the target type
  - Optional validation hook before Build()
Use Incremental generator + IndentedStringBuilder. Emit diagnostics for unsupported patterns (e.g., required ctor params not handled). Return only the .cs file contents.
