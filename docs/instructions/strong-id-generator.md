---
title: Strong Id Generator
description: Project documentation for Strong Id Generator.
status: active
updated: 2026-06-18
---
Implement the Strong ID source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Domain

Requirements:
- Attribute: [VcStrongId] from Vc.Tooling.Abstractions.Attributes
- Input: partial readonly struct declarations with [VcStrongId]
- Output: for each strong ID type, generate:
  - backing Guid Value property
  - ctor(Guid value)
  - ToString(), Equals, GetHashCode, ==, !=
  - IEquatable<T> implementation
- Use:
  - Incremental generator
  - TypeMetadata, AttributeInfo, SymbolExtensions, IndentedStringBuilder
  - VcGeneratorDiagnostics for invalid usage
- Namespace for generated code: Vc.Generated.Domain
Return only the .cs file contents.
