Implement a Message Contract source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Distributed

Requirements:
- Attribute: [MessageContract] (define in Abstractions)
- Input:
  - Classes annotated with [MessageContract]
- Output:
  - Envelope<T> type (if not present)
  - Factory methods to create envelopes with metadata (timestamp, correlationId, causationId)
  - Optional serializer helpers (e.g., ToJson/FromJson) in Vc.Generated.Distributed
Use Incremental generator + shared utilities. Emit diagnostics for invalid message types. Return only the .cs file contents.
