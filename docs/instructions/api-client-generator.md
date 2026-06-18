---
title: Api Client Generator
description: Project documentation for Api Client Generator.
status: active
updated: 2026-06-18
---
Implement an API client source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Api

Requirements:
- Attribute: [ApiClient(string name)] (define in Abstractions)
- Input: interfaces annotated with [ApiClient], methods representing endpoints
- Output:
  - HttpClient-based implementation class in Vc.Generated.Api
  - Methods that:
    - build request URLs
    - serialize body (if any)
    - deserialize JSON responses
  - Optional retry + logging hooks (leave as virtual/partial methods)
Use Incremental generator + shared utilities. Return only the .cs file contents.
