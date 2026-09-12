# FinHub & PayCore — Enterprise FinTech Platforms

[![Build & Test CI](https://github.com/mohamedth8/FinHub/actions/workflows/ci.yml/badge.svg)](https://github.com/mohamedth8/FinHub/actions/workflows/ci.yml)
[![Security Scan](https://github.com/mohamedth8/FinHub/actions/workflows/security-scan.yml/badge.svg)](https://github.com/mohamedth8/FinHub/actions/workflows/security-scan.yml)
[![.NET 10 LTS](https://img.shields.io/badge/.NET-10.0%20LTS-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular 22](https://img.shields.io/badge/Angular-22.0-DD0031?logo=angular)](https://angular.dev/)
[![Azure Ready](https://img.shields.io/badge/Azure-Cloud%20Native-0089D6?logo=microsoftazure)](https://azure.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

An enterprise-grade FinTech program engineering two production platforms designed to comply with the **Saudi Central Bank (SAMA)** Open Banking Framework and high-throughput electronic payment processing standards (aligned with Saudi Arabia's 85% electronic retail payment milestone).

---

## 📄 Implementation Plan & Master Reference

For the comprehensive 24-week engineering breakdown, domain models, and architecture design documents, view the [Implementation Plan](docs/architecture/implementation_plan.md) (or local file: [implementation_plan.md](file:///C:/Users/Asus/.gemini/antigravity/brain/c8921a28-d545-4410-beed-82a82a5bf5dc/implementation_plan.md)).

---

## 🏛 Architecture Overview

### Sector 1: FinHub — Open Banking Platform (Modular Monolith)
Built as a **Modular Monolith** using Clean Architecture and Domain-Driven Design (DDD) to balance high cohesion with zero premature microservice complexity.

```text
                     Angular 22 SPA (Signals Architecture)
                                      │
                                      ▼
                        ASP.NET Core Web API Gateway
                                      │
           ┌──────────────────────────┼──────────────────────────┐
           ▼                          ▼                          ▼
    Identity Module            Banking Module            Budgeting Module
           │                          │                          │
           └──────────────────────────┼──────────────────────────┘
                                      │
                                      ▼
                           SAMA Open Banking Gateway
                                      │
                                      ▼
                             External Bank APIs
```

### Sector 2: PayCore — Digital Wallet & Payments (Event-Driven Microservices)
Extracted microservices architecture powered by **Azure Service Bus**, **Double-Entry Accounting**, and an **Idempotent Payment Engine**.

```text
                          API Gateway (YARP / ASP.NET Core)
                                         │
        ┌────────────────────────────────┼────────────────────────────────┐
        ▼                                ▼                                ▼
  Wallet Service                  Payment Service                  Fraud Engine
        │                                │                                │
        └────────────────────────────────┼────────────────────────────────┘
                                         │
                                         ▼
                             Azure Service Bus (Outbox)
                                         │
                      ┌──────────────────┴──────────────────┐
                      ▼                                     ▼
                Ledger Service                   Reconciliation Service
```

---

## 🛠 Tech Stack & Core Libraries

- **Backend Framework**: .NET 10 (C# 14), ASP.NET Core Web API
- **Persistence & Caching**: Entity Framework Core 10, SQL Server 2022, Redis 7
- **Frontend Framework**: Angular 22 (Standalone Components, Signals, Typed Reactive Forms)
- **Messaging & Async**: Azure Service Bus, MassTransit, BackgroundService
- **Resilience & Security**: Polly Pipelines, FluentValidation, OpenID Connect (OIDC), OAuth 2.0 PKCE, TOTP MFA, Azure Key Vault Managed Identities
- **Observability**: OpenTelemetry, Serilog Structured Logging, Application Insights, Correlation ID propagation
- **Infrastructure & Testing**: Docker, Docker Compose, xUnit, FluentAssertions, Testcontainers, Playwright

---

## ⚖️ Financial Invariants & Domain Rules

### 1. Double-Entry Accounting Ledger (PayCore)
PayCore prohibits raw balance mutations (`wallet.Balance -= amount`). Financial movements generate immutable balanced journal entries satisfying the fundamental accounting equation:

$$\sum \text{Debit} = \sum \text{Credit}$$

```csharp
public bool IsBalanced()
{
    var totalDebit = Postings.Where(p => p.Direction == LedgerDirection.Debit).Sum(p => p.Amount.Amount);
    var totalCredit = Postings.Where(p => p.Direction == LedgerDirection.Credit).Sum(p => p.Amount.Amount);
    return totalDebit == totalCredit;
}
```

### 2. Idempotent Payment Processing
HTTP requests containing the `Idempotency-Key` header (UUIDv4) are cached in Redis. Retried requests bypass transaction re-execution and return original payload responses, preventing duplicate charges.

### 3. SAMA Open Banking Consent Lifecycle
Explicit consent management enforcing statuses: `Pending` $\rightarrow$ `Authorized` $\rightarrow$ `ActiveSync` $\mid$ `Revoked` $\mid$ `Expired` $\mid$ `Rejected`.

---

## 🗺 18-Sprint Scrum Roadmap (24 Weeks)

| Sprint | Phase | Focus Area | Deliverables |
| :--- | :--- | :--- | :--- |
| **Sprint 0** | Foundation | Solution & CI/CD | .NET 10 Solution, Angular 22 Setup, GitHub Actions, Docker, ADRs. |
| **Sprint 1** | Sector 1 | Identity & Auth | Registration, Login, OIDC / JWT, Refresh Tokens, TOTP MFA foundation. |
| **Sprint 2** | Sector 1 | Customer DDD Core | Customer Aggregate Root, `Money` & `IBAN` Value Objects, Audit Logging. |
| **Sprint 3** | Sector 1 | SAMA Open Banking | Mock Bank Provider Gateway, OAuth2 PKCE Consent Flow, Token Exchange. |
| **Sprint 4** | Sector 1 | Accounts & Ingestion | Account Sync Service, Transaction Ingestion, Filtering, CSV Export. |
| **Sprint 5** | Sector 1 | Budgeting & Alerts | Monthly Budgets, Category Limits, SignalR real-time spending alerts. |
| **Sprint 6** | Sector 1 | FinHub Dashboard | Angular 22 Dashboard, Spending Charts, Cash Flow analytics. |
| **Sprint 7** | Sector 2 | Wallet Platform | Digital Wallet aggregate, Deposit, Withdrawal, P2P Transfers. |
| **Sprint 8** | Sector 2 | Double-Entry Ledger | Immutable Journal Entries, Ledger Accounts, Invariant Enforcement. |
| **Sprint 9** | Sector 2 | Payment Engine | Payment State Machine, Redis Idempotency Engine, Optimistic Locking. |
| **Sprint 10** | Sector 2 | Event Messaging | Azure Service Bus, Transactional Outbox & Inbox Patterns. |
| **Sprint 11** | Sector 2 | Fraud Engine | `IFraudRule` pipeline, Velocity & Device rules, Risk Scoring Dashboard. |
| **Sprint 12** | Sector 2 | Settlement & Reco | End-of-day batch settlement, Provider file reconciliation, Mismatch detector. |
| **Sprint 13** | Hardening | Resilience | Polly Circuit Breakers, Retries, Timeouts, Dead-Letter Queues. |
| **Sprint 14** | Hardening | Security | Threat Modeling, Key Vault migration, Data-at-Rest AES-256 encryption. |
| **Sprint 15** | Hardening | Performance | Load testing (k6), SQL Index tuning, Angular LCP/CLS optimization. |
| **Sprint 16** | Production | Observability | OpenTelemetry distributed tracing, Application Insights metrics. |
| **Sprint 17** | Production | Cloud Deployment | Azure App Service, Container Apps, Service Bus, Bicep / Terraform. |

---

## 📜 Architecture Decision Records (ADRs)

Maintained under [`docs/adr/`](docs/adr/):

- [ADR 0001](docs/adr/0001-record-architecture-decisions.md) — Record Architecture Decisions
- [ADR 0002](docs/adr/0002-use-dotnet10-modular-monolith-for-finhub.md) — .NET 10 Modular Monolith for FinHub
- [ADR 0003](docs/adr/0003-use-angular22-signals-and-standalone-components.md) — Angular 22 Signals & Standalone Components
- [ADR 0004](docs/adr/0004-double-entry-accounting-for-paycore-ledger.md) — Double-Entry Accounting Invariant for PayCore Ledger
- [ADR 0005](docs/adr/0005-outbox-pattern-azure-service-bus-for-events.md) — Transactional Outbox Pattern & Azure Service Bus

---

## 🚀 Quickstart & Local Setup

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 24+](https://nodejs.org/) & npm 11+
- [Docker Desktop](https://www.docker.com/)

### 1. Clone & Setup Infrastructure
```bash
git clone https://github.com/mohamedth8/FinHub.git
cd FinHub

# Start SQL Server 2022, Redis, and Azurite containers
docker-compose up -d
```

### 2. Build & Test .NET 10 Solution
```bash
dotnet restore FinTech.slnx
dotnet build FinTech.slnx
dotnet test FinTech.slnx --verbosity normal
```

### 3. Run Angular 22 Frontend
```bash
cd src/Web
npm install
npm start
```
Navigate to `http://localhost:4200/`.

---

## 👤 Author & Maintainer

**Mohamed Tharwat** ([@Mohamedtharwat8](https://github.com/Mohamedtharwat8))  
*Senior Full-Stack & FinTech Software Engineer*
