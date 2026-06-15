## Unreleased

### Feat

- **auth**: enforce login on all pages + dual home view
- add /timeline page + fix DB transient failures
- administration timeline + FEUR dedicated page
- merge backend into Blazor project + UI polish
- implement Blazor phases 3-8 — full platform implementation
- **home**: match React prototype design exactly
- initial Blazor scaffold with Entra ID auth and U-ERRE design

### Fix

- **timeline**: replace N+1 queries with single batch query
- **db**: resolve DbContext concurrency errors
- **db**: graceful fallback when DB is unreachable (dev machine limitation)
- **razor**: remove @ prefix from else/else-if blocks
- **auth**: remove api scope from initial login request
- **pages**: handle backend connection errors gracefully
- **auth**: add EnableTokenAcquisitionToCallDownstreamApi and wire client secret

### Refactor

- **auth**: rename AzureAd config section to EntraId

### Perf

- eliminate 20-30s OIDC instance discovery latency
- disable Blazor prerendering — 33s → 0.16s TTFB
