# 5. Outbox Pattern & Azure Service Bus for Reliable Messaging

Date: 2026-09-12
Status: Accepted

## Context
When a payment transaction state updates in the database, downstream consumers (Ledger, Fraud Engine, Notifications) must receive domain events reliably without dual-write inconsistencies.

## Decision
Implement the **Transactional Outbox Pattern**:
1. State changes and outbox messages are saved within the same database transaction.
2. An asynchronous background processor polls outbox records and publishes them to **Azure Service Bus**.
3. Downstream services implement idempotent consumer logic (Inbox Pattern) via message deduplication headers.

## Consequences
- Guarantees at-least-once message delivery.
- Eliminates dual-write failures where database commits succeed but event publication fails.
