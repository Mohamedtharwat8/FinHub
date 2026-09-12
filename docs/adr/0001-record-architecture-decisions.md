# 1. Record Architecture Decisions

Date: 2026-09-12
Status: Accepted

## Context
We need to capture architectural decisions for FinHub (Open Banking Platform) and PayCore (Payments & Wallet Engine) to maintain long-term technical intent, regulatory compliance traceability, and engineering standards.

## Decision
We will use Architecture Decision Records (ADRs) structured using the Nygard format stored under `docs/adr/`.

## Consequences
- Every major architectural choice (e.g. database isolation, messaging, consent management, idempotency) must be documented in an ADR.
- Code reviews must verify compliance with established ADRs.
