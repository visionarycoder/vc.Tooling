---
title: Mapping Generator
description: Project documentation for Mapping Generator.
status: active
updated: 2026-06-18
---
Implement a mapping source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Dx

Requirements:
- Attribute: [MapTo(Type targetType)] (define in Abstractions)
- Input: classes annotated with [MapTo]
- Output:
  - Extension methods: ToTarget(this Source src)
  - Property-by-property mapping (same name + compatible type)
  - Emit diagnostics when required target properties are unmapped
Generated namespace: Vc.Generated.Dx. Use Incremental generator + IndentedStringBuilder. Return only the .cs file contents.
