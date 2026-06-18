---
title: Generator Prompts
description: Project documentation for Generator Prompts.
status: active
updated: 2026-06-18
---

# Generator Prompts

Implement the full source generator for the VisionaryCoder.Tooling project in the category `<CATEGORY>`.
Use the existing project structure and naming conventions:

- Namespace: `Vc.Generators.<CATEGORY>`
- Generator class: Vc`<CATEGORY>`Generator
- Use Incremental Generators only
- Use Vc.Generators.Common for:
  - IndentedStringBuilder
  - SymbolExtensions
  - AttributeInfo
  - TypeMetadata
  - Diagnostics

The generator must:

1. Scan the compilation for attributed types.
2. Extract attribute metadata into TypeMetadata.
3. Generate deterministic, formatted C# output.
4. Emit diagnostics using VcGeneratorDiagnostics where appropriate.
5. Produce output under namespace Vc.Generated.`<CATEGORY>`.

Return only the .cs file contents.
