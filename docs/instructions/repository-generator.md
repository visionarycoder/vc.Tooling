Implement a repository source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Data

Requirements:
- Attribute: [Repository] (define in Abstractions)
- Input: aggregate root classes annotated with [Repository]
- Output:
  - IRepository<T> interface (if not already present)
  - Concrete repository class per aggregate in Vc.Generated.Data
  - CRUD methods with simple in-memory or placeholder implementation
Emit diagnostics for invalid usage (non-class, missing key, etc.). Use Incremental generator. Return only the .cs file contents.
