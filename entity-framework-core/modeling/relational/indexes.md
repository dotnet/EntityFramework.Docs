---
uid: modeling/relational/indexes
---
# Indexes

> [!WARNING]
> This documentation is for EF Core. For EF6.x and earlier release see [http://msdn.com/data/ef](http://msdn.com/data/ef).

> [!NOTE]
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

An index in a relational database maps to the same concept as an index in the core of Entity Framework.

## Conventions

By convention, indexes are named `IX_<type name>_<property name>`. For composite indexes `<property name>` becomes an underscore separated list of property names.

## Data Annotations

Indexes can not be configured using Data Annotations.

## Fluent API

You can use the Fluent API to configure the name of an index.

<!-- [!code-csharp[Main](samples/relational/Modeling/FluentAPI/Samples/Relational/IndexName.cs?highlight=9)] -->
````csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasIndex(b => b.Url)
            .HasName("Index_Url");
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
````
