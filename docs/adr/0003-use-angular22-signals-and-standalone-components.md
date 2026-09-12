# 3. Use Angular 22 Signals and Standalone Component Architecture

Date: 2026-09-12
Status: Accepted

## Context
The frontend requires responsive financial dashboards, real-time transaction updates, and clean state isolation across financial features.

## Decision
Adopt Angular 22 using:
1. **Standalone Components**: Eliminating NgModules in favor of clean component imports.
2. **Signals & Computed Primitives**: `signal()`, `computed()`, and `linkedSignal()` for fine-grained reactivity and minimal change detection overhead.
3. **Feature-Based Folder Structure**: Structuring code by feature (`features/accounts`, `features/transactions`, `features/consent`) containing pages, components, and data access layers.

## Consequences
- Enhanced LCP and CLS performance due to direct signal reactivity.
- Modular code division facilitating independent feature development.
