---
title: Construct Analyzer Prompt
description: Project documentation for Construct Analyzer Prompt.
status: active
updated: 2026-06-18
---
# Construct Analyzer Prompt

Implement the full Roslyn analyzer for category `<CATEGORY>` in the VisionaryCoder.Tooling suite.

## Requirements

- Namespace: `Vc.Analyzers.<CATEGORY>` or the namespace implied by the target folder path when `<CATEGORY>` is nested
- Analyzer class: `<Concern>`Analyzer
- Analyzer must be a composition root only (no inline rule logic)
- Create or reuse a sibling `Rules` subfolder under the analyzer's current folder
- Move each diagnostic rule into a separate class file in that `Rules` subfolder
- Each rule class must expose a DiagnosticDescriptor and register its own callbacks
- Rule class and file names must match the semantic rule names used by the analyzer or descriptor, for example `MissingApiControllerRule` and `NonRestfulNameRule`
- Rule namespace must match the analyzer namespace plus `.Rules`
- Analyzer can compose one or more rule classes
- SupportedDiagnostics must be composed from all rule descriptors
- Use centralized IDs from VisionaryCoder.Tooling.Analyzers.Common.DiagnosticIds
- No magic string diagnostic IDs
- Use modern Roslyn APIs
- Return all created/updated .cs file contents needed for a complete implementation
