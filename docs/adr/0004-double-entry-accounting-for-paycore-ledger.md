# 4. Double-Entry Accounting Invariant for PayCore Ledger

Date: 2026-09-12
Status: Accepted

## Context
Payment engines cannot rely on primitive balance mutations (`balance -= amount`). Financial systems require non-repudiable audit trails and mathematical verification of funds movement.

## Decision
PayCore Ledger will enforce strict **Double-Entry Accounting**:
Every financial movement creates an immutable `JournalEntry` containing balanced `LedgerPosting` records.
The aggregate root enforces the domain invariant:

$$\sum \text{Debit} = \sum \text{Credit}$$

Direct balance updates are strictly prohibited. Balances are derived projections of posted ledger entries.

## Consequences
- Guaranteed mathematical consistency across wallet transfers and merchant settlements.
- Complete financial traceability for regulatory compliance (SAMA audit requirements).
