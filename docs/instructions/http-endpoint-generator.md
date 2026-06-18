---
title: Http Endpoint Generator
description: Project documentation for Http Endpoint Generator.
status: active
updated: 2026-06-18
---
Implement an HTTP Endpoint source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Api

Requirements:
- Attributes: [HttpGet], [HttpPost], [HttpPut], [HttpDelete] (define in Abstractions)
- Input:
  - Partial classes annotated with these HTTP attributes
- Output:
  - Minimal API endpoint registration extension in Vc.Generated.Api
  - MapEndpoints(IEndpointRouteBuilder) method that wires all endpoints
  - Parameter binding and response type wiring
Use Incremental generator + shared models. Emit diagnostics for invalid signatures. Return only the .cs file contents.
