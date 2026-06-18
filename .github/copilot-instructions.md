---
title: Copilot Instructions
description: Project documentation for Copilot Instructions.
status: active
updated: 2026-06-18
---
You are Copilot assisting with the VisionaryCoder.Tooling solution.



Your job is to generate production‑grade C# code that fits into an existing

.NET 10 / C# 14 Roslyn-based architecture.



Follow these rules:



============================================================

SOLUTION STRUCTURE

============================================================

The solution contains:

\- Source Generators (12 categories)

\- Analyzers (7 categories)

\- CodeFix Providers (7 categories)

\- Shared Libraries (5 libraries)

\- Test Projects (26 total)



All code must fit into this namespace pattern:

\- Vc.Generators.<Category>

\- Vc.Analyzers.<Category>

\- Vc.CodeFixes.<Category>

\- Vc.Tooling.<Library>

\- Vc.Generated.<Category>



============================================================

CODING RULES

============================================================

1\. Use .NET 10 and C# 14 syntax.

2\. Use Incremental Generators for all generators.

3\. Use DiagnosticAnalyzer for analyzers.

4\. Use CodeFixProvider for code fixes.

5\. Use SyntaxFactory for all edits.

6\. Use the shared libraries:

&#x20;  - Vc.Generators.Common

&#x20;  - Vc.Tooling.Core

&#x20;  - Vc.Tooling.Abstractions

&#x20;  - Vc.Tooling.Metadata

&#x20;  - Vc.Tooling.Diagnostics

7\. All output must be deterministic and production‑ready.



============================================================

GENERATOR REQUIREMENTS

============================================================

For any generator:

\- Scan the compilation for attributed types.

\- Extract metadata using: AttributeInfo, TypeMetadata, SymbolExtensions.

\- Build output using IndentedStringBuilder.

\- Emit diagnostics using VcGeneratorDiagnostics.

\- Generate code under namespace Vc.Generated.<Category>.



============================================================

ANALYZER REQUIREMENTS

============================================================

For any analyzer:

\- Use DiagnosticDescriptor with category from VcDiagnosticCategories.

\- Rule ID format: VC<CATEGORY ABBREV>001.

\- Support concurrent execution.

\- Ignore generated code.



============================================================

CODEFIX REQUIREMENTS

============================================================

For any code fix:

\- Fix diagnostic ID: VC<CATEGORY ABBREV>001.

\- Provide a single CodeAction.

\- Use SyntaxFactory for deterministic edits.

\- Use BatchFixer for FixAllProvider.



============================================================

TEST REQUIREMENTS

============================================================

For any test suite:

\- Use Microsoft.CodeAnalysis.Testing.

\- Use xUnit.

\- Include positive, negative, diagnostic, and codefix tests.



============================================================

OUTPUT FORMAT

============================================================

When generating code:

\- Return ONLY the .cs file contents.

\- No explanations, no markdown, no commentary.

\- Produce compilable C#.



============================================================

BEGIN

============================================================

