---
title: Event Generator
description: Project documentation for Event Generator.
status: active
updated: 2026-06-18
---
Implement a domain event source generator in VisionaryCoder.Tooling.

Project:
- Vc.Generators.Domain

Requirements:
- Attribute: [DomainEvent] (define in Abstractions)
- Input: classes annotated with [DomainEvent]
- Output:
  - Event metadata class per event in Vc.Generated.Domain
  - Envelope type (EventEnvelope<TEvent>)
  - Factory methods to create envelopes with timestamp, correlationId, etc.
Use Incremental generator + shared models/utilities. Return only the .cs file contents.
