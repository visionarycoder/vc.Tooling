# Changelog

All notable changes to VisionaryCoder.Tooling are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-06-17

### Added

#### Analyzers (42 Rules)

- **VCARCH (Architecture)**: Layering violations, cyclic dependencies, namespace boundaries, project references, VBD contract validation
- **VCAPI (API Design)**: Controller attribute validation, naming conventions, API versioning, response type validation, HTTP method correctness, input validation
- **VCDTO/VCDATA (Data Transfer Objects)**: DTO mutability detection, circular reference detection, repository contract validation
- **VCCORE (Core Patterns)**: Immutability requirement enforcement, value object detection, aggregate root patterns
- **VCASYNC (Async & Concurrency)**: Async void method detection, blocking call detection, naming suffix validation, fire-and-forget detection, async overhead in hot paths
- **VCDESIGN (Exception Safety & Design)**: Empty catch blocks, broad exception catches, swallowed exceptions, missing disposal patterns, missing logging
- **VCDIST (Distributed Systems)**: Event sourcing apply method validation, unused event detection, mutable state detection, repository async validation, queryable leakage detection, message contract validation, idempotency detection, saga compensation detection
- **VCPERF (Performance)**: LINQ in hot paths, boxing detection, allocation detection in hot paths, string concatenation in loops, lock contention, reflection in hot paths, regex recompilation
- **VCRES (Resilience)**: Circuit breaker missing, retry policy missing, timeout missing, retry configuration issues, timeout configuration issues, cancellation token missing, bulkhead missing, backpressure missing
- **VCSEC (Security)**: Hardcoded secrets, SQL injection vulnerabilities, missing authorization, unsafe deserialization, weak cryptography, XSS vulnerabilities, CSRF vulnerabilities, input validation missing, sensitive data in logs
- **VCOBS (Observability)**: Telemetry missing, tracing missing, metrics missing
- **VCDOC (Documentation)**: Missing XML documentation, incomplete XML documentation
- **VCNAMING (Naming Conventions)**: Convention violations, inconsistent naming
- **VCNULL (Null Safety)**: Missing null checks, nullable type warnings
- **VCVBDA/VCVBDE/VCVBDM (VBD Vaults)**: Access vault business logic leakage, access vault mapping missing, engine vault infrastructure access, manager vault unstable contracts

#### Code Generators (47 Generators)

**Resilience Generators**:
- RetryPolicyGenerator: Generates retry settings and configuration records with MaxAttempts, BaseDelay, exponential backoff
- TimeoutPolicyGenerator: Generates timeout settings and configuration records
- CircuitBreakerGenerator: Generates circuit breaker policy configurations

**Configuration Generators**:
- OptionsPatternGenerator: Generates IOptions<> registration and validator
- FeatureFlagGenerator: Generates feature flag configuration classes
- ConfigurationGenerator: Generates configuration binding helpers

**Domain Generators**:
- StrongIdGenerator: Generates strongly-typed GUID identifier structs with equality operators
- ValueObjectGenerator: Generates immutable value object bases
- AggregateRootGenerator: Generates aggregate root base classes
- DomainEventGenerator: Generates domain event types and handlers
- EventSourcingGenerator: Generates event sourcing infrastructure (event store contracts, snapshot handling)
- CommandHandlerGenerator: Generates CQRS command handler stubs
- QueryHandlerGenerator: Generates CQRS query handler stubs

**Data Generators**:
- DtoGenerator: Generates data transfer object records
- ValidatorGenerator: Generates entity validators with required field checking
- RepositoryGenerator: Generates repository interfaces and stubs
- MappingGenerator: Generates mapper classes for domain-to-DTO conversions

**Distributed Generators**:
- OutboxGenerator: Generates outbox entry records and repository interfaces
- EventPublisherGenerator: Generates strongly-typed event publisher helpers
- MessageContractGenerator: Generates serializable message contracts
- GraphQLTypeGenerator: Generates GraphQL type configurations

**API Generators**:
- HttpEndpointGenerator: Generates minimal API endpoint configurations
- ApiClientGenerator: Generates HTTP client implementations
- OpenApiGeneratorGenerates OpenAPI schema documentation

**Observability Generators**:
- LoggerMessageGenerator: Generates [LoggerMessage] source-generated logging methods
- TelemetryGenerator: Generates telemetry instrumentation hooks
- MetricsGenerator: Generates metrics collection helpers

**Security Generators**:
- JwtSecurityGenerator: Generates JWT token service with generation and validation
- PermissionsGenerator: Generates permission constant classes

**Other Generators**:
- UnionGenerator: Generates discriminated union types
- ResultGenerator: Generates Result<T> types for functional error handling

#### Code Fixes (41 Providers)

- ResiliencePolicyCodeFix: Add [SuppressMessage] for resilience policy validation
- ResilienceRetryCodeFix: Suppress retry policy diagnostics with infrastructure layer justification
- ResilienceTimeoutCodeFix: Suppress timeout diagnostics
- ResilienceCircuitBreakerCodeFix: Suppress circuit breaker diagnostics
- ResilienceCancellationCodeFix: Suppress cancellation token diagnostics
- PerformanceLinqCodeFix: Suppress LINQ in hot path diagnostics
- PerformanceBoxingCodeFix: Suppress boxing diagnostics
- PerformanceAllocationCodeFix: Suppress allocation diagnostics
- [Plus 32 additional code fixes for all analyzer categories]

#### Runtime Components

- **BehaviorPipeline**: Chainable proxy behavior execution with ordering and composition
- **VbdMessageBus**: Pub-sub event messaging with strongly-typed envelopes
- **VbdMessageEnvelope<T>**: Message wrapper with correlation ID, causation ID, timestamp metadata
- **ToolingOperationInvoker**: Cross-cutting concern orchestration for logging, timing, error handling
- **DefaultPipelineFactory**: Factory for composing behavior pipelines
- **IProxyBehavior**: Contract for pipeline behaviors
- **Guard**: Static validation methods (NotNull, NotNullOrWhiteSpace)
- **StringExtensions**: Extension methods (OrEmpty, IsNullOrWhiteSpace)
- **IHttpAdapter/HttpAdapter**: HTTP client integration abstraction
- **IConfigAdapter/ConfigAdapter**: Configuration reading abstraction

#### Architecture Components

- **Boundary** & **VbdBoundary** attributes: Mark architectural boundaries (Unknown, Architecture, Domain, Runtime, Integration, Utility, Tooling)
- **Component** & **VbdComponent** attributes: Mark component roles (Unknown, Manager, Engine, Access, Adapter, Service, Utility)
- **VbdVolatility** enum: Volatility levels for VBD (PolicyOrchestration, Algorithm, InfrastructureIntegration)
- **BoundaryType** enum: Boundary type definitions
- **ComponentRole** enum: Component role definitions

#### Testing

- **382 Generator Tests**: Snapshot tests, compilation validation, error case handling
- **116 Analyzer Tests**: Happy path, diagnostic detection, edge cases, bad input syntax
- **47 CodeFix Tests**: Fix application validation, suppression attribute generation
- **66 Architecture/Runtime/Utility/Ifx Tests**: Component behavior, integration contracts, utility correctness

#### Sample Project

- **FullImplementation** sample demonstrating:
  - Architecture boundary and VBD pattern usage
  - Domain entities with Order model and OrderManager
  - Integration layer with OrderHttpGateway using adapters
  - Runtime event bus with pub-sub messaging
  - Four scenarios: Happy path, edge conditions, invalid input handling, integration failure
  - End-to-end execution with 100% test pass rate

#### Documentation

- Updated README.md with feature list, quick start guide, architecture overview, testing instructions
- CHANGELOG.md for release tracking
- Architecture Decision Records (ADRs) for design patterns
- Analyzer, CodeFix, and Generator roadmap documents
- Test coverage plan with coverage matrix
- Sample implementation documentation

### Quality Assurance

- ✅ 611 tests passing (100% pass rate)
- ✅ Zero build warnings in analyzer/generator code
- ✅ All diagnostic IDs centralized in DiagnosticIds.cs (no magic strings)
- ✅ All public APIs in generated output have XML documentation
- ✅ Deterministic generation verified (byte-stable across multiple builds)
- ✅ Sample builds and runs successfully on clean checkout

### Technical Specifications

- **Target Framework**: .NET 10.0
- **C# Language**: C# 14 with file-scoped namespaces, implicit usings, record types, init-only properties
- **Code Generation**: IIncrementalGenerator pattern with ForAttributeWithMetadataName discovery
- **Pattern**: Volatility-Based Decomposition (VBD) with Manager/Engine/Access vault separation
- **Testing**: Microsoft.CodeAnalysis.Testing framework with xUnit
- **Build**: Deterministic, reproducible, zero-warning compilation

## Notes

This is the initial release (v1.0.0) of VisionaryCoder.Tooling, representing the completion of the seven-phase implementation roadmap from concept through release hardening.
