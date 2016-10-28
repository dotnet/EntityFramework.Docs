---
uid: modeling/index
---
# Creating a Model

> [!WARNING]
> This documentation is for EF Core. For EF6.x and earlier release see [http://msdn.com/data/ef](http://msdn.com/data/ef).

Entity Framework uses a set of conventions to build a model based on the shape of your entity classes. You can specify additional configuration to supplement and/or override what was discovered by convention.

This article covers configuration that can be applied to a model targeting any data store and that which can be applied when targeting any relational database. Providers may also enable configuration that is specific to a particular data store. For documentation on provider specific configuration see the [Database Providers](../providers/index.md) section.

In this section you can find information about conventions and configuration for the following:

- [Including & Excluding Types](included-types.md)
- [Including & Excluding Properties](included-properties.md)
- [Keys (primary)](keys.md)
- [Generated Properties](generated-properties.md)
- [Required/optional properties](required-optional.md)
- [Maximum Length](max-length.md)
- [Concurrency Tokens](concurrency.md)
- [Shadow Properties](shadow-properties.md)
- [Relationships](relationships.md)
- [Indexes](indexes.md)
- [Alternate Keys](alternate-keys.md)
- [Inheritance](inheritance.md)
- [Backing Fields](backing-field.md)
- [Relational Database Modeling](relational/index.md)
    - [Table Mapping](relational/tables.md)
    - [Column Mapping](relational/columns.md)
    - [Data Types](relational/data-types.md)
    - [Primary Keys](relational/primary-keys.md)
    - [Default Schema](relational/default-schema.md)
    - [Computed Columns](relational/computed-columns.md)
    - [Sequences](relational/sequences.md)
    - [Default Values](relational/default-values.md)
    - [Indexes](relational/indexes.md)
    - [Foreign Key Constraints](relational/fk-constraints.md)
    - [Alternate Keys (Unique Constraints)](relational/unique-constraints.md)
    - [Inheritance (Relational Database)](relational/inheritance.md)

> [!TIP]
> You can view this article’s [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples) on GitHub.

## Methods of configuration

### Fluent API

You can override the `OnModelCreating` method in your derived context and use the `ModelBuilder API` to configure your model. This is the most powerful method of configuration and allows configuration to be specified without modifying your entity classes. Fluent API configuration has the highest precedence and will override conventions and data annotations.

<!-- [!code-csharp[Main](samples/Modeling/FluentAPI/Samples/Required.cs?range=5-15&highlight=5-10)] -->

````csharp
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.Url)
                .IsRequired();
        }
    }
````

### Data Annotations

You can also apply attributes (known as Data Annotations) to your classes and properties. Data annotations will override conventions, but will be overwritten by Fluent API configuration.

<!-- [!code-csharp[Main](samples/Modeling/DataAnnotations/Samples/Required.cs?range=11-16&highlight=4)] -->

````csharp
    public class Blog
    {
        public int BlogId { get; set; }
        [Required]
        public string Url { get; set; }
    }
````

