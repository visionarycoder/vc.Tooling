Implement a validation source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Design

Requirements:
- Input: classes with properties using [Required], [Email], etc. (System.ComponentModel.DataAnnotations)
- Output:
  - Validator class per type in Vc.Generated.Design
  - Validate(T instance) method returning a result object or throwing
  - Optional ValidationResult model
- Emit diagnostics for unsupported property types or invalid combinations.
Use Incremental generator + shared models/utilities. Return only the .cs file contents.
