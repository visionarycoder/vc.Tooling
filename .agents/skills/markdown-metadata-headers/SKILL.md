---
title: Markdown Metadata Headers Skill
name: markdown-metadata-headers
description: Add and normalize metadata headers (YAML front matter) in Markdown files for consistency, tooling, and discoverability.
status: active
updated: 2026-06-18
---

# Markdown Metadata Headers Skill

Use this skill when a Markdown file needs a metadata header (YAML front matter) added, fixed, or standardized.

## Intent

Ensure Markdown files include valid, minimal, and consistent metadata headers so indexing, automation, and documentation tooling can parse them reliably.

## Core Rules

1. Place metadata at the very top of the file.
2. Use YAML front matter delimiters exactly as:
   - opening `---`
   - closing `---`
3. Do not place blank lines or text before the opening delimiter.
4. Preserve existing document content and heading structure after metadata.
5. Keep keys lowercase kebab-case unless repository convention says otherwise.
6. Prefer concise scalar values; quote only when YAML requires it.
7. Do not add speculative keys with unknown values.
8. Keep metadata deterministic and stable across repeated edits.

## Required Minimum Header

For generic docs, require at least:
- `title`
- `description`

Example:

```yaml
---
title: Dependency Injection Guidelines
description: Best practices for registration, lifetimes, and composition roots.
status: active
updated: 2024-06-18
---
```

## Optional Common Keys

Add only when known and useful:
- `slug`
- `tags`
- `category`
- `status` (for example: draft, active, deprecated)
- `owner`
- `last-reviewed`
- `audience`

## Key Quality Rules

1. `title` must match the document purpose (not necessarily identical to first H1).
2. `description` should be one sentence summarizing intent.
3. `tags` should be a short YAML list, not comma-separated text.
4. Dates should use ISO format (`YYYY-MM-DD`).
5. Avoid duplicate keys and mixed casing for the same concept.

## Migration / Normalization Workflow

1. Check whether front matter already exists.
2. If absent, insert a minimal valid header at line 1.
3. If present, normalize keys, casing, and obvious formatting issues.
4. Keep semantic values unchanged unless clearly incorrect.
5. Ensure exactly one front matter block per file.
6. Validate the Markdown still renders with the same body content.

## Anti-Patterns

- Adding front matter below the first heading.
- Using invalid YAML syntax.
- Adding placeholders like `TODO` as final metadata values.
- Duplicating the front matter block.
- Rewriting unrelated document body content while adding metadata.

## Skill-Specific Convention (for skills docs)

When adding metadata headers to `.agents/skills/**/SKILL.md`, use:
- `name`: skill identifier (kebab-case)
- `description`: one-line capability summary

Example:

```yaml
---
name: xml-documentation
description: Add consistent XML documentation comments for public and protected APIs.
---
```

## Completion Checklist

Before finishing:
1. Header is at top of file with valid delimiters.
2. Required keys are present for the target doc type.
3. YAML is syntactically valid.
4. No duplicate/contradictory keys.
5. Original Markdown content remains intact below the header.
