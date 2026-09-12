# FinTech Engineering Program: FinHub & PayCore Architecture & Implementation Plan

This implementation plan outlines the full architecture, technical domain models, engineering practices, and 18-sprint (24-week) Scrum roadmap for building two production-grade FinTech platforms aligned with Saudi Central Bank (SAMA) Open Banking and Electronic Payments standards:

1. **FinHub (Sector 1)**: Open Banking & Personal Financial Management (PFM) Platform built initially as a Modular Monolith using Clean Architecture and Domain-Driven Design (DDD).
2. **PayCore (Sector 2)**: High-Throughput Digital Wallet, Payment Gateway, Double-Entry Ledger, Fraud Engine, and Reconciliation Platform built as an Event-Driven Microservices ecosystem.

---

## Technical Stack & Architectural Guarantees

- **Backend**: .NET 10 (C# 14), ASP.NET Core Web API, Entity Framework Core 10, SQL Server 2022, Redis 7, Azure Service Bus, OpenTelemetry, Polly Resilience Pipelines, FluentValidation, SignalR.
- **Frontend**: Angular 22 using Standalone Components, Signal-based State Management (`signal()`, `computed()`), Feature-based folder structure, Typed Reactive Forms, HttpClient Interceptors.
- **Infrastructure**: Docker, Azure App Service, Azure Container Apps, Azure Key Vault (Managed Identities), Azure Front Door, GitHub Actions CI/CD.

---

## Sector 1: FinHub Domain & Technical Design

### SAMA Open Banking Consent Lifecycle
```text
Customer Requests Connection ──> Pending ──> OAuth2 + PKCE Redirect ──> Authorized ──> Token Exchange ──> ActiveSync
                                   │                                      │
                                   └──> Rejected                          ├──> Revoked (by User)
                                                                          └──> Expired (Window elapsed)
```

### Core Entities & Value Objects
- `Money`: Immutable Value Object with currency verification and decimal rounding invariants.
- `IBAN`: SAMA / ISO 13616 compliant string format validation.
- `Consent`: SAMA Open Banking scope and OAuth token manager aggregate.
- `BankAccount`: IBAN, Balance (`Money`), Bank Name, Account Type.
- `Budget`: Category limits, monthly budget allocations, spending alert triggers.

---

## Sector 2: PayCore Payment & Ledger Architecture

### Double-Entry Accounting Ledger Invariant
Every financial movement generates immutable ledger entries satisfying:

$$\sum \text{Debit} = \sum \text{Credit}$$

Direct balance mutations are strictly forbidden; balances are projections of posted ledger entries.

### Payment State Machine
`Created` $\rightarrow$ `Pending` $\rightarrow$ `Authorized` $\rightarrow$ `Processing` $\rightarrow$ `Captured` $\rightarrow$ `Settled` (or `Failed`, `Cancelled`, `Refunded`).

### Idempotency & Distributed Messaging
- **Idempotency-Key**: Redis cache deduplication for incoming HTTP requests.
- **Outbox Pattern**: Transactional outbox pattern with background worker publishing events to **Azure Service Bus**.
- **Fraud Engine**: `IFraudRule` pipeline calculating risk scores (0-100) based on velocity, amount, device signature, and geo-anomalies.

---

## 18-Sprint Scrum Delivery Roadmap (24 Weeks)

| Sprint | Phase | Epic / Focus Area | Key Deliverables & Engineering Outcomes |
| :--- | :--- | :--- | :--- |
| **Sprint 0** | Foundation | Solution Setup & Standards | Git repo, .NET 10 solution, Angular 22 workspace, Docker Compose, GitHub Actions CI pipeline, Serilog, ProblemDetails. |
| **Sprint 1** | Sector 1 | Identity & Authentication | Customer Registration, Login, JWT, Refresh Token Rotation, TOTP MFA foundation, Role-Based Access Control. |
| **Sprint 2** | Sector 1 | Customer Domain & DDD | Customer Aggregate Root, Value Objects (`Money`, `IBAN`), Domain Events, EF Core configurations, Audit Log pipeline. |
| **Sprint 3** | Sector 1 | SAMA Open Banking Integration | Mock Bank Provider Gateway, OAuth 2.0 PKCE consent flow, Token Exchange, Bank Connection manager. |
| **Sprint 4** | Sector 1 | Accounts & Transactions Engine | Account sync background service, Transaction ingestion, Categorization rules, Pagination, Filtering, CSV Export. |
| **Sprint 5** | Sector 1 | Budgeting & Alerts | Monthly Budgets, Category Limits, Savings Goals, SignalR real-time spending limit alerts. |
| **Sprint 6** | Sector 1 | FinHub Angular Dashboard | High-performance dashboard with Signal state, Charts (Spending, Cash flow), Financial Health score, End of Sector 1. |
| **Sprint 7** | Sector 2 | PayCore Wallet Foundation | Digital Wallet aggregate, Deposit, Withdrawal, P2P Transfer domain logic, Optimistic locking setup. |
| **Sprint 8** | Sector 2 | Double-Entry Ledger Core | Double-entry accounting module, Journal entries, Ledger accounts, Invariant validation (`Total Debit == Total Credit`). |
| **Sprint 9** | Sector 2 | Payment Processing & State Machine | Payment lifecycle state machine, Idempotency-Key engine with Redis, Concurrency control (`RowVersion`). |
| **Sprint 10** | Sector 2 | Event-Driven & Reliable Messaging | Azure Service Bus integration, Outbox & Inbox patterns, MassTransit event handlers, Microservices extraction. |
| **Sprint 11** | Sector 2 | Extensible Fraud Engine | `IFraudRule` pipeline, Velocity/Amount/Device rules, Risk scoring, Real-time Fraud Analyst dashboard in Angular. |
| **Sprint 12** | Sector 2 | Reconciliation & Settlement Engine | End-of-day settlement batch processing, External provider file parser, Mismatch detection dashboard. |
| **Sprint 13** | Hardening | Resilience & Fault Tolerance | Polly resilience pipelines (Circuit Breaker, Retries, Timeout), Dead-Letter Queue handlers, Health Checks. |
| **Sprint 14** | Hardening | Security Hardening & Audit | Threat modeling, OWASP Top 10 remediation, Azure Key Vault integration, Data-at-rest encryption, Security audit. |
| **Sprint 15** | Hardening | Performance Optimization | Load testing with k6, SQL index tuning, Redis caching strategy, Angular LCP/CLS optimization (LCP < 2.5s). |
| **Sprint 16** | Production | OpenTelemetry & Monitoring | Full-stack distributed tracing, Application Insights metrics, Correlation ID propagation, SLA alerts. |
| **Sprint 17** | Production | Azure Infrastructure & CI/CD | Bicep/Terraform provisioning (Azure App Service, Container Apps, Service Bus, Azure SQL), Production deployment. |
