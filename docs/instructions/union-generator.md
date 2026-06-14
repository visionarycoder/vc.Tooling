Implement a Union (discriminated union) source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Dx

Requirements:
- Attribute: [Union] (define in Abstractions)
- Input:
  - Partial classes annotated with [Union]
  - Nested types or attributes defining cases
- Output:
  - Case factory methods (Success, Failure, etc.)
  - Match/Map/Bind methods
  - IsX properties for each case
Generated namespace: Vc.Generated.Dx. Use Incremental generator + shared models. Return only the .cs file contents.
