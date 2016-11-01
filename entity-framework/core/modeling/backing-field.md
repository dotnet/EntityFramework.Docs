---
title: Backing Fields
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: a628795e-64df-4f24-a5e8-76bc261e7ed8
ms.prod: entity-framework
uid: core/modeling/backing-field
---
# Backing Fields

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

When a backing field is configured, EF will write directly to that field when materializing entity instances from the database (rather than using the property setter). This is useful when there is no property setter, or the setter contains logic that should not be executed when setting initial property values for existing entities being loaded from the database.

> [!WARNING]
> The `ChangeTracker` has not yet been enabled to use backing fields when it needs to set the value of a property. This is only an issue for foreign key properties and generated properties - as the change tracker needs to propagate values into these properties. For these properties, a property setter must still be exposed.[Issue #4461](https://github.com/aspnet/EntityFramework/issues/4461) is tracking enabling the `ChangeTracker` to write to backing fields for properties with no setter.

## Conventions

**By convention, the following fields will be discovered as backing fields for a given property (listed in precedence order):**

* <propertyName> differing only by case

* _<propertyName>

* m_<propertyName>

<!-- [!code-csharp[Main](samples/core/Modeling/Conventions/Samples/BackingField.cs?highlight=3,7,8,9,10,11)] -->
````csharp
public class Blog
{
    private string _url;

    public int BlogId { get; set; }

    public string Url
    {
        get { return _url; }
        set { _url = value; }
    }
}
````

## Data Annotations

Backing fields cannot be configured with data annotations.

## Fluent API

There is no top level API for configuring backing fields, but you can use the Fluent API to set annotations that are used to store backing field information.

<!-- [!code-csharp[Main](samples/core/Modeling/FluentAPI/Samples/BackingField.cs?highlight=7,8,9,15,18)] -->
````csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Url)
            .HasAnnotation("BackingField", "_blogUrl");
    }
}

public class Blog
{
    private string _blogUrl;

    public int BlogId { get; set; }
    public string Url => _blogUrl;
}
````
