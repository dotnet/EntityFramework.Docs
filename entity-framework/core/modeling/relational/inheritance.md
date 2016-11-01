---
title: Inheritance (Relational Database)
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 9a7c5488-aaf4-4b40-b1ff-f435ff30f6ec
ms.prod: entity-framework
uid: core/modeling/relational/inheritance
---
# Inheritance (Relational Database)

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

> [!NOTE]
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

Inheritance in the EF model is used to control how inheritance in the entity classes is represented in the database.

## Conventions

By convention, inheritance will be mapped using the table-per-hierarchy (TPH) pattern. TPH uses a single table to store the data for all types in the hierarchy. A discriminator column is used to identify which type each row represents.

EF will only setup inheritance if two or more inherited types are explicitly included in the model (see [Inheritance](../inheritance.md) for more details).

Below is an example showing a simple inheritance scenario and the data stored in a relational database table using the TPH pattern. The *Discriminator* column identifies which type of *Blog* is stored in each row.

<!-- [!code-csharp[Main](samples/core/relational/Modeling/Conventions/Samples/InheritanceDbSets.cs)] -->
````csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<RssBlog> RssBlogs { get; set; }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}
````

![image](_static/inheritance-tph-data.png)

## Data Annotations

You cannot use Data Annotations to configure inheritance.

## Fluent API

You can use the Fluent API to configure the name and type of the discriminator column and the values that are used to identify each type in the hierarchy.

<!-- [!code-csharp[Main](samples/core/relational/Modeling/FluentAPI/Samples/InheritanceTPHDiscriminator.cs?highlight=7,8,9,10)] -->
````csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasDiscriminator<string>("blog_type")
            .HasValue<Blog>("blog_base")
            .HasValue<RssBlog>("blog_rss");
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}
````
