---
title: Implementation Test Suite Prompt
description: Project documentation for Implementation Test Suite Prompt.
status: active
updated: 2026-06-18
---
Generate a complete test suite for <CATEGORY> in the VisionaryCoder.Tooling solution.

Requirements:
- Use Microsoft.CodeAnalysis.Testing
- Use xUnit
- Include happy-path and negative tests
- Include edge-case and boundary tests
- Include bad-input and malformed-input tests
- Include failure-path and recovery tests where applicable
- Include diagnostic tests
- Include code fix tests where applicable
- Cover cross-project integration where relevant:
	- src/vc.Architecture
	- src/vc.Ifx
	- src/vc.Runtime
	- src/vc.Tooling
	- src/vc.Utility
- Return only .cs files
