Implement a Permissions / RBAC source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Security

Requirements:
- Attribute: [Permission(string name)] (define in Abstractions)
- Input:
  - Classes or partial types annotated with [Permission]
- Output:
  - Static Permissions class with string constants
  - Policy registration helpers in Vc.Generated.Security
  - Optional IAuthorizationHandler stubs
Use Incremental generator + IndentedStringBuilder. Emit diagnostics for duplicate permission names. Return only the .cs file contents.
