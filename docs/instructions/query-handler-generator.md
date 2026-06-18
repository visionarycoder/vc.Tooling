---
title: Query Handler Generator
description: Project documentation for Query Handler Generator.
status: active
updated: 2026-06-18
---
Implement a Query Handler source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Domain

Requirements:
- Attribute: [Query] (define in Abstractions)
- Input:
  - Records or classes annotated with [Query]
- Output:
  - IQuery<TQuery, TResult> and IQueryHandler<TQuery, TResult> (if not present)
  - Concrete handler stubs in Vc.Generated.Domain
  - Optional caching hooks as partial methods
Use Incremental generator + shared utilities. Emit diagnostics for invalid usage. Return only the .cs file contents.
