---
title: Telemetry Generator
description: Project documentation for Telemetry Generator.
status: active
updated: 2026-06-18
---
Implement a telemetry source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Observability

Requirements:
- Attribute: [Trace] (define in Abstractions)
- Input: methods annotated with [Trace]
- Output:
  - Partial method wrappers that:
    - start Activity
    - log start/stop
    - capture exceptions
  - Generated into Vc.Generated.Observability
Use Incremental generator + IndentedStringBuilder. Return only the .cs file contents.
