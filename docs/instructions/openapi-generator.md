---
title: Openapi Generator
description: Project documentation for Openapi Generator.
status: active
updated: 2026-06-18
---
Implement an OpenAPI-based source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Distributed

Requirements:
- Attribute: [OpenApiClient(string specPath, string clientName)] (define in Abstractions)
- Input: marker interfaces annotated with [OpenApiClient]
- Output:
  - DTOs, client classes, and optional controllers based on the OpenAPI spec
  - Place generated code under Vc.Generated.Distributed
You may assume the spec is accessible as an AdditionalFile. Use Incremental generator + shared utilities. Return only the .cs file contents.
