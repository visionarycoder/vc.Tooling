---
title: Command Handler Generator
description: Project documentation for Command Handler Generator.
status: active
updated: 2026-06-18
---
Implement a Command Handler source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Domain

Requirements:
- Attribute: [Command] (define in Abstractions)
- Input:
  - Records or classes annotated with [Command]
- Output:
  - ICommand<TCommand> and ICommandHandler<TCommand> interfaces (if not present)
  - Concrete handler stubs in Vc.Generated.Domain
  - Optional pipeline hooks (logging, telemetry, validation) as partial methods
Emit diagnostics if Command types are invalid (e.g., non-record, missing properties). Use Incremental generator. Return only the .cs file contents.
