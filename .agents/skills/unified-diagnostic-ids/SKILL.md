---
title: Unified Diagnostic IDs Skill
description: Project documentation for Unified Diagnostic IDs Skill.
status: active
updated: 2026-06-18
---
# Unified Diagnostic IDs Skill

## Overview

This skill documents the canonical approach to managing diagnostic IDs across the vc.Analyzers project. A unified diagnostic ID system provides a single source of truth for all analyzer diagnostics, eliminating magic strings and ensuring consistency across the codebase.

## Core Principles

### 1. Centralized Constant Definitions

All diagnostic IDs must be defined as `const string` in a centralized `DiagnosticIds.cs` file:

```csharp
namespace VisionaryCoder.Analyzers.Common;

public static class DiagnosticIds
{
    // Architecture Category (VCARCH)
    public const string ArchLayeringViolation = "VCARCH0001";
    public const string ArchCyclicDependency = "VCARCH0002";
    // ...
}
```

**Rationale**: Eliminates duplication, enables bulk ID changes, and serves as the authoritative reference.

### 2. Semantic Naming Convention

Constant names follow a three-part pattern: `{Category}{Rule}{Modifier}`

**Format:**
```
{Category}{Aspect}{Qualifier}
```

**Examples:**
- `ArchLayeringViolation` — Architecture/Layering/Violation
- `AsyncVoidMethod` — Async/Void/Method
- `ResilienceRetryPolicyMissing` — Resilience/RetryPolicy/Missing
- `DistributedEventSourcingMutableState` — Distributed/EventSourcing/MutableState

**Guidelines:**
- Use PascalCase for all constant names
- Lead with the primary concern (Architecture, Async, Resilience, etc.)
- Include the specific aspect being checked
- Include qualifiers (Missing, Excessive, Violation, etc.)
- **Avoid generic names** like `ValidationId` — be specific: `ApiValidationMissing`

### 3. ID Format

All diagnostic IDs follow this standard format:

```
VC{Category}{Number:0000}
```

Where:
- **VC** — Prefix (VisionaryCoder)
- **Category** — 3-6 letter category code (ARCH, API, ASYNC, NAMING, etc.)
- **Number** — Zero-padded 4-digit number (0001-9999)

**Examples:**
- `VCARCH0001` — Architecture, rule 1
- `VCAPI0003` — API, rule 3
- `VCPERF0001` — Performance, rule 1

### 4. Category Registry

Assign categories to logical concerns. Current categories:

| Code | Name | Rules | Purpose |
|------|------|-------|---------|
| VCARCH | Architecture | 0001-0006 | Layering, boundaries, project structure |
| VCAPI | API Design | 0001-0006 | HTTP/REST conventions, controller design |
| VCDTO | Data Transfer Objects | 0001-0002 | DTO purity and structure |
| VCDATA | Data Design | 0001-0002 | DTO/repository design coverage |
| VCCORE | Core | 0001-0003 | Core domain baseline checks |
| VCMAP | Mapping | 0001-0002 | Object mapping validation |
| VCASYNC | Async Correctness | 0001-0005 | Async/await patterns |
| VCDESIGN | Design Principles | 0001-0006 | Exception safety, immutability |
| VCVBDA | VBD Access Vault | 0001-0003 | Access layer boundary rules |
| VCVBDE | VBD Engine Vault | 0001-0003 | Engine/algorithm boundary rules |
| VCVBDM | VBD Manager Vault | 0001-0003 | Manager/orchestration boundary rules |
| VCDIST | Distributed Patterns | 0001-0009 | Event sourcing, messaging, repositories |
| VCPERF | Performance | 0001-0007 | Hot paths, allocations, optimizations |
| VCRES | Resilience | 0001-0012 | Circuit breakers, retries, timeouts |
| VCOBS | Observability | 0001-0003 | Telemetry, tracing, and metrics |
| VCSEC | Security | 0001-0009 | Secrets, injection, authorization |
| VCNULL | Null Safety | 0001-0002 | Null checks, nullable warnings |
| VCDOC | Documentation | 0001-0002 | XML comments, documentation |
| VCNAMING | Naming Conventions | 0001-0002 | Naming consistency |
| VC | Legacy/Stub | 0001-0006 | Transitional IDs pending full rule migration |

To add a new category, allocate a range (e.g., VCCUSTOM0001-0010) and document in DiagnosticIds.cs.

## Implementation Rules

### Rule VDI0001: Always Use DiagnosticIds Constants
✅ **DO:**
```csharp
using VisionaryCoder.Analyzers.Common;

public sealed class MyAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticIds.ArchLayeringViolation,
        // ...
    );
}
```

❌ **DON'T:**
```csharp
public static readonly DiagnosticDescriptor Rule = new(
    id: "VCARCH0001",  // Magic string!
    // ...
);
```

### Rule VDI0002: Organize DiagnosticIds by Category
Group constants in DiagnosticIds.cs by category with section comments:

```csharp
public static class DiagnosticIds
{
    // Architecture Category (VCARCH)
    public const string ArchLayeringViolation = "VCARCH0001";
    public const string ArchCyclicDependency = "VCARCH0002";
    
    // API Category (VCAPI)
    public const string ApiDesignControllerMissing = "VCAPI0001";
    public const string ApiDesignNaming = "VCAPI0002";
}
```

### Rule VDI0003: Import the Common Namespace
Every analyzer should reference `VisionaryCoder.Analyzers.Common` (directly or via global usings):

```csharp
using VisionaryCoder.Analyzers.Common;

namespace VisionaryCoder.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LayeringAnalyzer : DiagnosticAnalyzer { }
```

This is often satisfied by GlobalUsings.cs, but explicit imports are safe.

### Rule VDI0004: Multi-Rule Analyzers Must Use Rule Composition
When an analyzer has more than one rule, each rule must be implemented in a dedicated class under a rules subfolder.

- Analyzer class composes rule classes and registers them.
- Rule classes own descriptors, IDs, and analysis callbacks.
- Keep analyzer classes orchestration-only.

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionSafetyAnalyzer : DiagnosticAnalyzer
{
  private static readonly ImmutableArray<IAnalyzerRule> _rules =
    ImmutableArray.Create<IAnalyzerRule>(
      new EmptyCatchRule(),
      new GeneralCatchRule(),
      new SwallowedExceptionRule(),
      new AsyncVoidRule());

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    _rules.Select(r => r.Descriptor).ToImmutableArray();

  public override void Initialize(AnalysisContext context)
  {
    foreach (var rule in _rules)
    {
      rule.Register(context);
    }
  }
}
```

### Rule VDI0005: Document ID Semantics
Include inline XML comments in DiagnosticIds.cs for non-obvious IDs:

```csharp
/// <summary>
/// DTO contains mutable fields or properties. DTOs should be immutable or use init-only members.
/// </summary>
public const string DtoMutableState = "VCDTO0001";
```

## Project Configuration

### Roslyn Analyzer Project Setup

Analyzer projects (e.g., `vc.Analyzers.csproj`) must be configured correctly:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="$(MSBuildThisFileDirectory)../../GlobalUsings.cs" Link="GlobalUsings.cs" />
  </ItemGroup>
</Project>
```

**Key requirements:**
- Target `netstandard2.0` (not `net10.0`)
- Use Central Package Management for version control
- Include GlobalUsings.cs to provide implicit Roslyn imports

### GlobalUsings.cs

Ensures all Roslyn types are available without explicit imports:

```csharp
global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;
```

## Workflow

When implementing a new analyzer:

1. **Identify the category** (Architecture, API, Async, etc.)
2. **Allocate ID number** (increment from existing category max)
3. **Create semantic constant name** in DiagnosticIds.cs
4. **Add constant to appropriate section** with XML comment
5. **Reference VisionaryCoder.Analyzers.Common** in analyzer (directly or via global usings)
6. **Reference constant** in DiagnosticDescriptor: `id: DiagnosticIds.ArchLayeringViolation`
7. **Update DIAGNOSTIC_IDS_REFERENCE.md** if adding a new category

## Benefits

- **Eliminates magic strings** across 25+ analyzers
- **Single source of truth** for ID definitions
- **Bulk ID changes** possible via one file edit
- **Self-documenting code** — constant names describe rules
- **IDE support** — IntelliSense shows all available IDs
- **Consistency** — uniform format across all diagnostics
- **Extensibility** — easy to add new categories and rules

## Related Documents

- `src/vc.Analyzers/Common/DiagnosticIds.cs` — Complete constant definitions
- `src/vc.Analyzers/Common/DIAGNOSTIC_IDS_REFERENCE.md` — User-facing reference
- `docs/instructions/unified-diagnostic-ids.md` — Implementation guide
- `.agents/skills/analyzer-rule-composition/SKILL.md` — Rule-per-class analyzer structure
- `docs/instructions/analyzer-rule-composition.md` — Rule composition implementation steps
- ADR-0001 — Volatility-Based Decomposition (related boundary concepts)
- ADR-0002 — Dependency Injection (related structural concepts)
