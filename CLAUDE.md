# BestMed Platform - Claude Guidance

This repository contains a .NET 10 microservices solution built on .NET Aspire with a gateway-first architecture and an Angular frontend.

## Tech Stack
- .NET 10 / ASP.NET Core
- .NET Aspire orchestration and service defaults
- YARP reverse proxy for the API gateway
- EF Core with Azure SQL Database
- Azure Service Bus for asynchronous cross-service integration events in non-development environments
- Event sourcing as a solution-level architectural pattern for state changes that need to be replayable or auditable
- OpenTelemetry for tracing, metrics, and logs
- Serilog for structured logging
- Angular frontend running separately from the backend solution

## Solution Design
- `BestMed.Platform.AppHost` is the orchestration entry point.
- `BestMed.Gateway` is the only externally exposed backend service.
- Domain services are isolated by responsibility: auth, users, roles, prescribers, warehouses, pharmacies, facilities.
- `BestMed.Platform.ServiceDefaults` contains shared cross-cutting configuration and should be used consistently by every service.
- `BestMed.Common` is for pure shared .NET code only.
- `BestMed.Data` is for shared EF Core conventions and base types.

## Long-Term Standards
- Prefer small, service-specific changes over cross-cutting edits.
- Keep business logic inside the owning service.
- Share only true cross-cutting primitives in `BestMed.Common` or `BestMed.Data`.
- Do not introduce ASP.NET Core or EF Core dependencies into `BestMed.Common`.
- Keep service contracts explicit and version-friendly.
- Preserve nullable reference types, implicit usings, and .NET 10 target framework consistency.
- Follow the existing async-first pattern and use cancellation tokens where appropriate.
- Use structured logging instead of ad hoc string logging.
- Prefer configuration-driven behavior over hardcoded values.

## Shared Logic Rules
- Put reusable DTOs, helpers, constants, and simple result models in `BestMed.Common` only when they are framework-agnostic.
- Put EF Core abstractions, entity interfaces, and database conventions in `BestMed.Data`.
- Put infrastructure defaults such as auth, rate limiting, output caching, health checks, resilience, and telemetry in `BestMed.Platform.ServiceDefaults`.
- Avoid duplicating validation, paging, or guard logic across services when a shared abstraction already exists.
- If a change affects multiple services, prefer extracting a minimal shared contract instead of copying implementation.

## Service Bus Communication
- Use Azure Service Bus for asynchronous, cross-service communication, especially when a workflow does not need an immediate request/response.
- Keep command-style HTTP calls within a service boundary and use events to notify other services about completed state changes.
- Publish integration events through the shared `BestMed.Common.Messaging` abstractions and keep event contracts in `BestMed.Common`.
- Use the established topic naming convention derived from event type names, and keep subscription names service-specific.
- Ensure every event publisher or subscriber is registered through `BestMed.Platform.ServiceDefaults` so provisioning and startup behavior stay consistent.
- Treat Service Bus consumers as idempotent and resilient to duplicate deliveries, retries, and delayed processing.
- In development, respect the existing connection-string-based setup; in production, rely on Aspire-managed provisioning and managed identity patterns.

## Event Sourcing Guidance
- Event sourcing is an architectural direction for the solution and should be used where a domain benefits from a durable history of state changes.
- Use event sourcing for aggregates that need replayability, audit history, or reconstruction from ordered domain events.
- Keep the event stream as the source of truth for the aggregate state, and derive read models or projections separately.
- Store event contracts in shared, framework-agnostic types, but keep aggregate logic and replay rules inside the owning service.
- Do not confuse integration events with event-sourced domain events: integration events are for communication between services, while domain events represent changes within a service boundary.
- If event sourcing is introduced for a service, document the aggregate boundaries, event versioning strategy, replay rules, and projection update flow alongside that service.
- Favor additive, version-tolerant event evolution so old streams remain readable.
- If a service does not yet have an event store or replay pipeline, do not pretend it does; implement the pattern explicitly before treating it as a runtime dependency.

## .NET Aspire Guidance
- Register every backend service with AppHost.
- Use service discovery for internal service-to-service HTTP calls.
- Keep internal communication HTTPS-only unless the existing pattern explicitly allows otherwise.
- Add health checks for every service exposed through Aspire.
- Use `WaitFor` to express startup dependencies.
- Treat the gateway as the boundary for external traffic.
- Keep local development and production hosting concerns separated when necessary.
- When adding new distributed resources, make the AppHost the source of truth for orchestration.

## Code Style and Review Expectations
- Match the existing naming and folder structure before adding new patterns.
- Prefer minimal, readable changes that fit the current architecture.
- Do not add new libraries unless there is a clear platform-level need.
- Keep endpoint, middleware, and configuration changes consistent across services.
- If logic is repeated in more than one place, evaluate whether it belongs in a shared project.

## Working Rules
- Check the current solution structure before editing shared code.
- Update documentation only when behavior or architecture changes materially.
- Validate changes with build or tests before finishing.
- Keep the repository friendly for long-term maintenance and onboarding.
- Keep the README.md file up to date with every change to the solution structure or development process.
