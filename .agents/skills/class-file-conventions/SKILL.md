---
name: class-file-conventions
description: Enforce class-to-file naming parity and one top-level class per file, ignoring nested classes.
---

# Class/File Conventions

Use this skill as the canonical rule set for class/file structure consistency.

## Intent

Keep source layout predictable by enforcing a 1:1 mapping between top-level class names and file names.

## Scope

1. Applies to top-level class declarations only.
2. Ignores nested classes completely.
3. Does not apply to interfaces, enums, delegates, records, or structs unless explicitly extended by another skill.

## Rules

1. Filename Match Rule
- A top-level class name must exactly match the filename (without extension).
- Example: `OrderProcessor` must be in `OrderProcessor.cs`.

2. One Top-Level Class Rule
- A file may contain only one top-level class declaration.
- Additional top-level classes in the same file are violations.

3. Nested Class Exclusion Rule
- Nested classes are ignored by both rules above.
- Nested classes do not count toward one-class-per-file enforcement.

## Analyzer/CodeFix/Generator Alignment Contract

All tooling must use the same rule IDs and severities.

| Rule ID | Default Severity | Summary |
|---|---|---|
| CFC0001 | Warning | Top-level class name does not match filename |
| CFC0002 | Warning | More than one top-level class in file |

## Required Tooling Behavior

1. Analyzer
- Report CFC0001 on the top-level class declaration when class name and filename differ.
- Report CFC0002 on each extra top-level class beyond the first.
- Do not report nested classes.

2. Code Fix
- For CFC0001, offer:
- Rename class to match filename.
- Rename file to match class name.
- For CFC0002, offer:
- Move each additional top-level class to its own file named after the class.
- Preserve namespace, usings, trivia, and access modifiers.

3. Generator
- Emit one top-level class per generated file.
- Always name generated files after the top-level class.
- Never split nested classes into separate files automatically.

## Edge-Case Guidance

1. Partial Classes
- Partial class declarations may exist across multiple files.
- Each file containing a partial top-level class should still follow filename match where possible.
- If multiple partials for the same class exist, do not raise CFC0002 solely due to partial usage.

2. Global Namespace and File-Scoped Namespace
- Rules apply the same regardless of namespace style.

3. Designer/Generated Files
- Allow opt-out via analyzer suppression or generated-code detection policy.

## Decision Checklist

Before accepting a file layout, confirm:

1. Is there exactly one top-level class in the file?
2. Does the top-level class name match the filename?
3. Are nested classes ignored as intended?
4. Are partial class scenarios handled without false positives?
