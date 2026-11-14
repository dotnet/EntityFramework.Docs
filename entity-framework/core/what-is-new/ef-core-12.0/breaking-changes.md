---
title: Breaking changes in EF Core 12 (EF12) - EF Core
description: List of breaking changes introduced in Entity Framework Core 12 (EF12)
author: ajcvickers
ms.date: 11/14/2026
uid: core/what-is-new/ef-core-12.0/breaking-changes
---

# Breaking changes in EF Core 12 (EF12)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 11 to EF Core 12. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 11](xref:core/what-is-new/ef-core-11.0/breaking-changes)
- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

| **Breaking change**                                                                                              | **Impact** |
|:---------------------------------------------------------------------------------------------------------------- | -----------|
| [A stored `NULL` JSON collection is now materialized as `null` for nullable collection properties](#nullable-json-collections) | Low        |

## Low-impact changes

<a name="nullable-json-collections"></a>

### A stored `NULL` JSON collection is now materialized as `null` for nullable collection properties

[Tracking Issue #33452](https://github.com/dotnet/efcore/issues/33452)

#### Old behavior

EF didn't distinguish a `null` collection from an empty one for owned collections mapped to a JSON column. When a stored `NULL` was read into a property that has an initializer, the property kept the value from its initializer, so it was never `null`:

```csharp
public class Blog
{
    public int Id { get; set; }
    public List<Post>? Posts { get; set; } = [];
}
```

Changing such a collection between `null` and empty wasn't detected, so the change wasn't saved.

#### New behavior

An owned collection navigation mapped to JSON whose CLR property is annotated as nullable (here, `List<Post>?` with nullable reference types enabled) is now configured by convention as nullable. For these navigations:

- A stored `NULL` is materialized as `null`, even when the property has an initializer. A stored empty array `[]` is materialized as an empty collection.
- Changing the collection from `null` to a collection, or from a collection to `null`, is detected by change tracking, and `null` is written as JSON `NULL`.
- `CollectionEntry.IsModified` can be set. Setting it to `false` restores the original collection.

Collections that aren't annotated as nullable behave as before.

#### Why

A `null` collection and an empty one have different meanings in many models, and applications couldn't store or read that difference. See [#33452](https://github.com/dotnet/efcore/issues/33452).

#### Mitigations

- Check for `null` where code reads a nullable-annotated JSON collection that previously was never `null`. The property type already says that it can be `null`, so the compiler warns about unchecked uses.
- If you don't want a `null` collection to be distinguished from an empty one, remove the nullable annotation from the property, or opt the navigation out:

```csharp
modelBuilder.Entity<Blog>()
    .Navigation(b => b.Posts)
    .Metadata.SetIsNullable(false);
```

- If you use compiled models, regenerate them to pick up the nullable configuration. Existing compiled models keep working but treat the collections as non-nullable.
