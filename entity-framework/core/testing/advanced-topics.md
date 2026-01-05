---
title: Advanced testing topics - EF Core
description: Advanced Entity Framework Core testing topics
author: AndriySvyryd
ms.date: 11/21/2025
uid: core/testing/advanced-topics
---
# Advanced testing topics

This page described some less common topics related to testing EF Core applications.

## EnableServiceProviderCaching

EF Core uses an internal service provider to manage services required for database operations, including query compilation, model building, and other core functionality. By default, EF Core caches these internal service providers to improve performance when multiple `DbContext` instances share the same configuration.

The <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.EnableServiceProviderCaching*> method controls whether EF Core caches the internal service provider:

```csharp
public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableServiceProviderCaching(false)
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
    }
}
```

**Default behavior**: Service provider caching is **enabled by default** (`true`). This means:

- Service providers are cached and reused across `DbContext` instances with the same configuration
- Better performance for applications that create many `DbContext` instances
- Lower memory overhead when multiple contexts share configurations

**When to disable caching**: Disabling service provider caching will greatly slow down `DbContext` creation and in the vast majority of cases the default behavior is recommended. If there are issues with incorrect internal services used, then they should be fixed in a different way. But if you are replacing services for testing purposes you could disable service provider caching (`false`) to ensure each test gets a fresh service provider.
