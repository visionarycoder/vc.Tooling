---
title: Graphql Type Generator
description: Project documentation for Graphql Type Generator.
status: active
updated: 2026-06-18
---
Implement a GraphQL Type source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Distributed (or Domain)

Requirements:
- Attribute: [GraphQLType] (define in Abstractions)
- Input:
  - Classes annotated with [GraphQLType]
- Output:
  - ObjectType<T> subclasses (for HotChocolate-style GraphQL)
  - Field definitions based on public properties
  - Optional registration helpers in Vc.Generated.Distributed
Use Incremental generator. Emit diagnostics for unsupported property types. Return only the .cs file contents.
