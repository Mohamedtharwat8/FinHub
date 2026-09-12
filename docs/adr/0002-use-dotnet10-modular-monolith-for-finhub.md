# 2. Use .NET 10 Modular Monolith for FinHub

Date: 2026-09-12
Status: Accepted

## Context
FinHub is an Open Banking and Personal Financial Management platform. Building a 15-microservice architecture from day 1 introduces premature operational complexity, distributed transaction overhead, and deployment friction.

## Decision
Build FinHub as a **Modular Monolith** using Clean Architecture and Domain-Driven Design (DDD) principles in .NET 10 (C# 14).
Modules (`Identity`, `Banking`, `Transactions`, `Budgeting`, `Notifications`) will communicate via in-process C# interfaces and MediatR/in-memory domain events.

## Consequences
- High cohesion within modules and strict coupling control.
- Clear module boundaries allow extracting microservices seamlessly if scaling demands require it later.
- Single deployment unit initially for rapid iteration and simple operational maintenance.
