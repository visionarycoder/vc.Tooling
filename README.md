---
title: vc.Tooling
description: Project documentation for vc.Tooling.
status: active
updated: 2026-06-18
---
# vc.Tooling

A comprehensive .NET 10 + C# 14 analysis, code generation, and runtime composition framework for production-grade applications.

## Overview

VisionaryCoder.Tooling is an integrated suite of Roslyn-based analyzers, code generators, code fix providers, and runtime components designed to enforce architectural patterns, validate design contracts, and automate boilerplate code generation.

## Features

### 📊 Analyzers (42 Rules)

- **Architecture & Design** (VCARCH): Layering violations, cyclic dependencies, namespace boundaries, project references, VBD boundary contracts
- **API Design** (VCAPI): Controller contracts, naming, versioning, response types, HTTP methods, validation
- **Data & Transfer Objects** (VCDTO, VCDATA): DTO mutability, repository contracts, mapping validation
- **Core Patterns** (VCCORE): Immutability, value objects, aggregate roots
- **Async & Concurrency** (VCASYNC): Void methods, blocking calls, naming conventions, fire-and-forget, overhead
- **Exception Safety** (VCDESIGN): Empty catch, broad catch, swallowed exceptions, disposal, logging
- **Distributed Systems** (VCDIST): Event sourcing, repository contracts, idempotency, saga compensation
- **Performance** (VCPERF): LINQ in hot paths, boxing, allocations, string work, reflection, lock contention
- **Resilience** (VCRES): Circuit breakers, retry policies, timeout, cancellation, bulkhead, backpressure
- **Security** (VCSEC): Hardcoded secrets, SQL injection, authorization, deserialization, weak crypto, XSS, CSRF
- **Observability** (VCOBS): Telemetry, tracing, metrics
- **Naming & Documentation** (VCNAMING, VCDOC): Convention violations, missing XML docs
- **VBD (Volatility-Based Decomposition)** (VCVBDA, VCVBDE, VCVBDM): Manager/Engine/Access vault boundary contracts
- **Null Safety** (VCNULL): Missing checks, nullable warnings

### 🔧 Code Generators (47 Generators)

Auto-generates deterministic source code for:

- **Resilience**: Retry policies, timeout configurations, circuit breaker settings
- **Configuration**: Options pattern registration, feature flags, environment binding
- **Domain**: Strong IDs, value objects, aggregate roots, domain events, event sourcing, command/query handlers
- **Distributed**: Outbox pattern, event publishing, message contracts, saga orchestration
- **Data**: DTOs, repositories, mapping configurations, validators
- **API**: HTTP endpoints, API clients, GraphQL types, OpenAPI documentation
- **Observability**: Logger message sources, telemetry hooks, metrics collection
- **Security**: JWT token services, authorization helpers, permission validators

### 🛠️ Code Fixes (41 Providers)

Automated fixes for all analyzer rules:

- Add resilience attributes ([SuppressMessage] for policy validation)
- Add async/await patterns and CancellationToken parameters
- Add missing validation and null-safety checks
- Add API controller attributes and response types
- Add exception handling and logging
- Add DI registration and configuration
- Add documentation and naming corrections

### 🏗️ Runtime Components

- **BehaviorPipeline**: Chainable proxy behavior execution
- **VbdMessageBus**: Pub-sub event messaging with typed envelopes
- **ToolingOperationInvoker**: Cross-cutting concern orchestration (logging, timing, error handling)
- **Guard & StringExtensions**: Utility validation and string helpers
- **IHttpAdapter & IConfigAdapter**: Integration boundaries for HTTP and configuration

## Quick Start

### Installation

Clone the repository and add a reference to the analyzer package:

```bash
dotnet add package VisionaryCoder.Tooling.Analyzers --version 1.0.0
```

### Basic Usage

#### 1. Mark architectural boundaries

```csharp
[assembly: Boundary(BoundaryType.Tooling)]

namespace MyApp.Domain;

[Boundary(BoundaryType.Domain)]
[VbdBoundary(BoundaryType.Domain)]
public class Order
{
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
}
```

#### 2. Auto-generate retry policies

```csharp
[VcRetryPolicy]
public class PaymentGateway
{
    // Generator creates: PaymentGatewayRetrySettings and PaymentGatewayRetryConfiguration
}
```

#### 3. Auto-generate validators

```csharp
[VcValidator]
public class OrderRequest
{
    public string OrderId { get; init; }
    public string CustomerId { get; init; }
    
    // Generator creates: OrderRequestValidator with built-in validation logic
}
```

#### 4. Use the runtime pipeline

```csharp
var pipeline = DefaultPipelineFactory.Create();
var order = await pipeline.InvokeAsync<Order>("PlaceOrder", request: orderDto, 
    async () => await orderService.CreateAsync(orderDto));
```

#### 5. Use the message bus

```csharp
var bus = new VbdMessageBus();
bus.Subscribe<OrderPlaced>("order.created", env => 
{
    Console.WriteLine($"Order {env.Payload.OrderId} created");
    return Task.CompletedTask;
});

await bus.PublishAsync("order.created", new OrderPlaced { OrderId = orderId });
```

## Architecture

### Five Core Projects

- **src/vc.Architecture**: Boundary contracts, VBD enums, composition attributes
- **src/vc.Runtime**: Pipeline execution, message bus, operation invoker
- **src/vc.Ifx**: Integration adapters (HTTP, configuration)
- **src/vc.Utility**: Guard clauses, string utilities, helper extensions
- **src/vc.Tooling**: Composition root, shared abstractions, diagnostics

### Generator Categories

Organized into logical folders matching analyzer categories:

- **Resilience/**: Retry, timeout, circuit breaker configs
- **Domain/**: Strong IDs, value objects, event sourcing
- **Distributed/**: Outbox, event publishing, saga
- **Data/**: DTOs, repositories, validators
- **Api/**: Endpoints, clients, OpenAPI
- **Observability/**: Logging, telemetry, metrics

## Testing

All 611 tests pass:

- **382 Generator tests**: Snapshot, compilation, error cases
- **116 Analyzer tests**: Happy path, edge cases, bad input
- **47 CodeFix tests**: Fix application, compilation validation
- **42 Architecture/Runtime/Utility/Ifx tests**: Contract validation, behavior verification

Run tests:

```bash
dotnet test
```

Run specific test category:

```bash
dotnet test tests/vc.Generators.Tests
dotnet test tests/vc.Analyzers.Tests
dotnet test tests/vc.CodeFixes.Tests
```

## Sample Project

See **samples/FullImplementation/** for a complete end-to-end example demonstrating:

- Architecture boundaries and VBD patterns
- Domain model with validators
- Integration layer with HTTP/config adapters
- Runtime pipeline orchestration
- Event publishing and handling
- Error handling and resilience

Run the sample:

```bash
dotnet run --project samples/FullImplementation
```

## Roadmap

### Phase 1 ✅ - Baseline & Contracts
- Solution structure, shared contracts, naming conventions

### Phase 2 ✅ - Shared Platform  
- Runtime, Ifx, Utility, Architecture components

### Phase 3 ✅ - Analyzers & CodeFixes
- 42 analyzer rules with 41 matching code fixes

### Phase 4 ✅ - Generators
- 47 deterministic source generators with full test coverage

### Phase 5 ✅ - Full Sample
- End-to-end sample with all architecture patterns

### Phase 6 ✅ - Quality Gates
- Comprehensive test matrix (happy path, edge cases, failures)
- Deterministic generation verification
- 100% test pass rate

### Phase 7 ✅ - Release Hardening
- Remove magic string diagnostics
- Add XML documentation to all public APIs
- Package for NuGet
- Release tag

## Contributing

Contributions are welcome. Please ensure:

- All tests pass: `dotnet test`
- Code follows established patterns from src/vc.Architecture
- New analyzers have matching code fixes
- New generators emit deterministic output
- Generated code includes XML documentation

## License

MIT License - See LICENSE file for details

## References

- [Architecture & Design Decisions](docs/adr/)
- [Analyzer Roadmap](docs/analyzers-plan.md)
- [CodeFix Roadmap](docs/codefixes-plan.md)
- [Generator Roadmap](docs/generators-plan.md)
- [Test Coverage Plan](docs/test-coverage-plan.md)