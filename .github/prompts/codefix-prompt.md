---
title: Construct CodeFix Prompt
description: Project documentation for Construct CodeFix Prompt.
status: active
updated: 2026-06-18
---
# Construct CodeFix Prompt

Implement the full CodeFixProvider for category `<CATEGORY>` in the solution.

## Requirements

- Namespace: `Vc.CodeFixes.<CATEGORY>` or the namespace implied by the target folder path when `<CATEGORY>` is nested
- Provider class: `<Concern>CodeFixProvider`
- Provider must be a composition root only
- Create or reuse a sibling `Fixes` subfolder under the provider's current folder
- Move each actionable analyzer rule into a separate fix class file in that `Fixes` subfolder
- Each fix class must register a deterministic code action for its matching diagnostic ID
- Fix class and file names must match the semantic rule names used by the analyzer or diagnostic, for example `MissingApiControllerFix` and `NonRestfulNameFix`
- Fix namespace must match the provider namespace plus `.Fixes`
- Provider can compose one or more fix classes
- FixableDiagnosticIds must be composed from all fix classes
- Use centralized IDs from `VisionaryCoder.Tooling.Analyzers.Common.DiagnosticIds`
- No magic string diagnostic IDs
- Prefer `SyntaxFactory` or narrowly scoped node replacement for edits
- Preserve trivia and formatting where practical
- Keep fixes deterministic and conservative
- Return all created/updated .cs file contents needed for a complete implementation
