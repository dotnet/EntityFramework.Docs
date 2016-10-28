---
uid: modeling/included-properties
---
# Including & Excluding Properties

> [!WARNING]
> This documentation is for EF Core. For EF6.x and earlier release see [http://msdn.com/data/ef](http://msdn.com/data/ef).

Including a property in the model means that EF has metadata about that property and will attempt to read and write values from/to the database.

## Conventions

By convention, public properties with a getter and a setter will be included in the model.

## Data Annotations

You can use Data Annotations to exclude a property from the model.

<!-- [!code-csharp[Main](samples/Modeling/DataAnnotations/Samples/IgnoreProperty.cs?highlight=6)] -->
````csharp
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    [NotMapped]
    public DateTime LoadedFromDatabase { get; set; }
}
````

## Fluent API

You can use the Fluent API to exclude a property from the model.

<!-- [!code-csharp[Main](samples/Modeling/FluentAPI/Samples/IgnoreProperty.cs?highlight=7,8)] -->
````csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Ignore(b => b.LoadedFromDatabase);
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public DateTime LoadedFromDatabase { get; set; }
}
````
