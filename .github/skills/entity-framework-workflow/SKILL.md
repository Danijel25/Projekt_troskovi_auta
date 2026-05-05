---
name: entity-framework-workflow
description: "Use when working with Entity Framework Core: creating LINQ queries, changing DbContext or entity classes, generating migrations, updating schema, and seeding database data."
argument-hint: "EF Core queries, migrations, model changes, or seeding"
---

# Entity Framework Workflow

## When to Use
- Writing or refining EF Core LINQ queries.
- Changing entity classes, relationships, or the DbContext.
- Creating, reviewing, or applying migrations.
- Adding or updating seed data.
- Checking that a model change is reflected in the database schema and repositories.

## Workflow
1. Inspect the current model, DbContext, repository, and migration state before changing anything.
2. Identify the smallest EF Core surface that should change: entity, configuration, query, migration, or seed data.
3. If the request is about data retrieval, prefer a LINQ query that stays translatable by EF Core and avoids unnecessary client-side evaluation.
4. If the request changes entities or DbContext mapping, update the model and configuration first, then generate or adjust the migration.
5. If the schema changes, verify the migration includes the intended add, alter, rename, or delete operations before applying it.
6. If seeding changes are needed, keep seed data deterministic and aligned with the current model shape.
7. Confirm any repository or service code that depends on the changed model still compiles and uses the updated members consistently.

## Query Guidance
- Prefer projection with `Select` when the caller only needs part of the entity.
- Include navigation properties only when they are actually required.
- Use `AsNoTracking()` for read-only queries when change tracking is unnecessary.
- Keep filtering, sorting, and paging inside the query so EF Core can translate it to SQL.
- Avoid loading more data than needed just to filter in memory later.

## Migration Guidance
- Generate a migration after the model change is complete, not before.
- Review the migration file and snapshot for unintended table or column changes.
- Be careful with rename scenarios, since EF may otherwise generate drop-and-create operations.
- If a change affects existing data, add explicit data-migration steps instead of assuming defaults will be safe.

## Seeding Guidance
- Seed only stable reference data or bootstrap records that belong in source control.
- Keep seeded keys and values consistent so repeated runs do not create duplicates.
- Update seed data together with related enum, relationship, or required-field changes.
- Verify the seed shape matches the current entity constraints and nullable rules.

## Completion Checks
- The query compiles and is translatable by EF Core.
- The entity and DbContext changes match the intended schema.
- The migration reflects the actual model delta.
- Seed data still satisfies constraints and relationships.
- Any dependent repository, controller, or service code still uses the updated model correctly.