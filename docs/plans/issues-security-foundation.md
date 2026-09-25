# Security foundation issue record

Status: implemented on `release` through small, independently revertible commits.

## Scope

This record covers `SEC-001..003`, `PRIV-001..003`, `SAFE-001..002`,
`AUTHZ-001`, `ADMIN-001`, `AUTH-001..002`, `VALID-001`, `ERR-002` and
`EXT-001`. Work is delivered as small green commits directly on `release`.

## Invariants

- Entra authenticates; the server authorizes an active local user and the exact
  resource on every protected operation.
- A leader of group A cannot read or mutate protected data from group B.
- Anonymous responses do not expose email addresses, private events or drafts.
- User, event, membership and RSVP states are allow-listed on the server.
- Destructive actions require explicit intent and report their actual outcome.
- Unexpected exception details never reach the browser or REST response.
- Tests use synthetic data and disposable PostgreSQL; they never read developer
  settings or contact Entra, Supabase or Resend.

## Verification strategy

1. The .NET test project uses xUnit and a PostgreSQL Testcontainer pinned by
   digest; service and HTTP-facing contracts are tested with synthetic data.
2. Reproduce each issue with a negative test before or alongside the fix.
3. Cover student, leader A, leader B, admin, inactive user, missing resource,
   manipulated ID and repeated transition where applicable.
4. Run the affected tests, the complete test project and a Release build for each
   coherent commit.
5. Finish with an isolated HTTP smoke run against synthetic data.

The repeatable local verification is:

```text
dotnet test tests/StudentGroupsHub.Tests/StudentGroupsHub.Tests.csproj
dotnet build blazor/StudentGroupsHub.csproj --configuration Release
```

The test database is disposable and does not use developer settings or
institutional credentials.

## Delivery and rollback

Each commit references its issue ID and contains one coherent behavior plus its
tests. A failing change can therefore be reverted independently with
`git revert <commit>`; no data migration in this plan may be irreversible.

## Residual external checks

Real Entra sign-in and tenant-wide session revocation require a configured test
tenant and remain a deployment smoke check. Local authorization and inactive-user
enforcement are fully testable without institutional credentials.
