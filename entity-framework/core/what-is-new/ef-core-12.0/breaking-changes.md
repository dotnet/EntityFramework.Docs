---
title: Breaking changes in EF Core 12 (EF12) - EF Core
description: List of breaking changes introduced in Entity Framework Core 12 (EF12)
author: ajcvickers
ms.date: 11/10/2026
uid: core/what-is-new/ef-core-12.0/breaking-changes
---

# Breaking changes in EF Core 12 (EF12)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 11 to EF Core 12. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 11](xref:core/what-is-new/ef-core-11.0/breaking-changes)
- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

| **Breaking change**                                                                                                      | **Impact** |
|:------------------------------------------------------------------------------------------------------------------------ | -----------|
| [Proxies now throw for navigations whose accessors they cannot override](#proxy-navigation-visibility)                   | Low        |

## Low-impact changes

<a name="proxy-navigation-visibility"></a>

### Proxies now throw for navigations whose accessors they cannot override

[Tracking Issue #32352](https://github.com/dotnet/efcore/issues/32352)

#### Old behavior

Lazy-loading proxies (`UseLazyLoadingProxies`) require navigation getters to be `virtual`, and change-tracking proxies (`UseChangeTrackingProxies`) require navigation setters to be `virtual`. EF Core checked only that the accessor was `virtual`.

The proxy types are generated in a separate assembly (`DynamicProxyGenAssembly2`), so they can only override accessors that are visible to that assembly. An accessor that is `internal virtual` or `private protected virtual` passed model validation but was never overridden. No error was reported, and:

- With lazy-loading proxies, the navigation was never loaded and was always `null` (or empty).
- With change-tracking proxies, changes to the navigation were not detected and were not saved.

For example, with lazy-loading proxies enabled, `Profile` below was `null` after querying a `Pool`, even when a related `Profile` existed:

```csharp
public class Pool
{
    public int Id { get; set; }
    internal virtual Profile? Profile { get; set; }
}
```

#### New behavior

Starting with EF Core 12, model validation throws an <xref:System.InvalidOperationException> when a proxy cannot override the accessor it needs for a navigation:

- For lazy-loading proxies, the navigation getter must be visible to the proxy assembly.
- For change-tracking proxies, the navigation setter must be visible to the proxy assembly. This includes collection navigations, so `public virtual ICollection<Post> Posts { get; internal set; }` now throws.

An accessor is visible if it is `public`, `protected`, or `protected internal`, or if the assembly that declares it has `[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]`. A `private` accessor is never visible.

The exception is not thrown:

- For a navigation on which lazy-loading is disabled with `EnableLazyLoading(false)`.
- For lazy-loading proxies when `IgnoreNonVirtualNavigations` is specified. The navigation is then not proxied and is not lazy-loaded, as before.
- For lazy-loading proxies when only the setter is not visible. Only the getter needs to be overridden.

#### Why

Navigations that silently did not load, or silently did not save, were very hard to diagnose. Throwing during model validation reports the problem immediately and names the property.

#### Mitigations

Use one of the following:

- Make the accessor `public`, `protected` or `protected internal`.
- Add `[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]` to the assembly that declares the entity type. If the assembly is strong-named, the attribute must also specify the public key of the proxy assembly; see the Castle DynamicProxy documentation.
- If the navigation was never meant to be lazy-loaded, disable lazy-loading for it, or call `IgnoreNonVirtualNavigations`:

```csharp
optionsBuilder.UseLazyLoadingProxies(b => b.IgnoreNonVirtualNavigations());
```

```csharp
modelBuilder.Entity<Pool>().Navigation(e => e.Profile).EnableLazyLoading(false);
```

Change-tracking proxies have no equivalent opt-out, so for these the accessor must be made visible.
