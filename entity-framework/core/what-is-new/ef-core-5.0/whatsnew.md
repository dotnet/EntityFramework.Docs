---
title: What's New in EF Core 5.0
description: Overview of new features in EF Core 5.0
author: ajcvickers
ms.date: 09/10/2020
uid: core/what-is-new/ef-core-5.0/whatsnew
---

# What's New in EF Core 5.0

All features planned for EF Core 5.0 have now been completed. This page contains an overview of interesting changes introduced in each preview.

This page does not duplicate the [plan for EF Core 5.0](xref:core/what-is-new/ef-core-5.0/plan). The plan describes the overall themes for EF Core 5.0, including everything we are planning to include before shipping the final release.

## RC1

### Many-to-many

EF Core 5.0 supports many-to-many relationships without explicitly mapping the join table.

For example, consider these entity types:

```C#
public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Tag> Tags { get; set; }
}

public class Tag
{
    public int Id { get; set; }
    public string Text { get; set; }
    public ICollection<Post> Posts { get; set; }
}
```

Notice that `Post` contains a collection of `Tags`, and `Tag` contains a collection of `Posts`. EF Core 5.0 recognizes this as a many-to-many relationship by convention. This means no code is required in `OnModelCreating`:

```C#
public class BlogContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Blog> Blogs { get; set; }
}
```

When Migrations (or `EnsureCreated`) are used to create the database, EF Core will automatically create the join table. For example, on SQL Server for this model, EF Core generates:

```sql
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id])
);

CREATE TABLE [Tag] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NULL,
    CONSTRAINT [PK_Tag] PRIMARY KEY ([Id])
);

CREATE TABLE [PostTag] (
    [PostsId] int NOT NULL,
    [TagsId] int NOT NULL,
    CONSTRAINT [PK_PostTag] PRIMARY KEY ([PostsId], [TagsId]),
    CONSTRAINT [FK_PostTag_Posts_PostsId] FOREIGN KEY ([PostsId]) REFERENCES [Posts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PostTag_Tag_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [Tag] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PostTag_TagsId] ON [PostTag] ([TagsId]);
```

Creating and associating `Blog` and `Post` entities results in join table updates happening automatically. For example:

```C#
var beginnerTag = new Tag {Text = "Beginner"};
var advancedTag = new Tag {Text = "Advanced"};
var efCoreTag = new Tag {Text = "EF Core"};

context.AddRange(
    new Post {Name = "EF Core 101", Tags = new List<Tag> {beginnerTag, efCoreTag}},
    new Post {Name = "Writing an EF database provider", Tags = new List<Tag> {advancedTag, efCoreTag}},
    new Post {Name = "Savepoints in EF Core", Tags = new List<Tag> {beginnerTag, efCoreTag}});

context.SaveChanges();
```

After inserting the Posts and Tags, EF will then automatically create rows in the join table. For example, on SQL Server:

```sql
SET NOCOUNT ON;
INSERT INTO [PostTag] ([PostsId], [TagsId])
VALUES (@p6, @p7),
(@p8, @p9),
(@p10, @p11),
(@p12, @p13),
(@p14, @p15),
(@p16, @p17);
```

For queries, Include and other query operations work just like for any other relationship. For example:

```C#
foreach (var post in context.Posts.Include(e => e.Tags))
{
    Console.Write($"Post \"{post.Name}\" has tags");

    foreach (var tag in post.Tags)
    {
        Console.Write($" '{tag.Text}'");
    }
}
```

The SQL generated uses the join table automatically to bring back all related Tags:

```sql
SELECT [p].[Id], [p].[Name], [t0].[PostsId], [t0].[TagsId], [t0].[Id], [t0].[Text]
FROM [Posts] AS [p]
LEFT JOIN (
    SELECT [p0].[PostsId], [p0].[TagsId], [t].[Id], [t].[Text]
    FROM [PostTag] AS [p0]
    INNER JOIN [Tag] AS [t] ON [p0].[TagsId] = [t].[Id]
) AS [t0] ON [p].[Id] = [t0].[PostsId]
ORDER BY [p].[Id], [t0].[PostsId], [t0].[TagsId], [t0].[Id]
```

Unlike EF6, EF Core allows full customization of the join table. For example, the code below configures a many-to-many relationship that also has navigations to the join entity, and in which the join entity contains a payload property:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .Entity<Post>()
        .HasMany(p => p.Tags)
        .WithMany(p => p.Posts)
        .UsingEntity<PostTag>(
            j => j
                .HasOne(pt => pt.Tag)
                .WithMany()
                .HasForeignKey(pt => pt.TagId),
            j => j
                .HasOne(pt => pt.Post)
                .WithMany()
                .HasForeignKey(pt => pt.PostId),
            j =>
            {
                j.Property(pt => pt.PublicationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                j.HasKey(t => new { t.PostId, t.TagId });
            });
}
```

### Map entity types to queries

Entity types are commonly mapped to tables or views such that EF Core will pull back the contents of the table or view when querying for that type. EF Core 5.0 allows an entity type to mapped to a "defining query". (This was partially supported in previous versions, but is much improved and has different syntax in EF Core 5.0.)

For example, consider two tables; one with modern posts; the other with legacy posts. The modern posts table has some additional columns, but for the purpose of our application we want both modern and legacy posts tp be combined and mapped to an entity type with all necessary properties:

```c#
public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}
```

In EF Core 5.0, `ToSqlQuery` can be used to map this entity type to a query that pulls and combines rows from both tables:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>().ToSqlQuery(
        @"SELECT Id, Name, Category, BlogId FROM posts
          UNION ALL
          SELECT Id, Name, ""Legacy"", BlogId from legacy_posts");
}
```

Notice that the `legacy_posts` table does not have a `Category` column, so we instead synthesize a default value for all legacy posts.

This entity type can then be used in the normal way for LINQ queries. For example. the LINQ query:

```c#
var posts = context.Posts.Where(e => e.Blog.Name.Contains("Unicorn")).ToList();
```

Generates the following SQL on SQLite:

```sql
SELECT "p"."Id", "p"."BlogId", "p"."Category", "p"."Name"
FROM (
    SELECT Id, Name, Category, BlogId FROM posts
    UNION ALL
    SELECT Id, Name, "Legacy", BlogId from legacy_posts
) AS "p"
INNER JOIN "Blogs" AS "b" ON "p"."BlogId" = "b"."Id"
WHERE ('Unicorn' = '') OR (instr("b"."Name", 'Unicorn') > 0)
```

Notice that the query configured for the entity type is used as a starting for composing the full LINQ query.

### Event counters

[.NET event counters](https://devblogs.microsoft.com/dotnet/introducing-diagnostics-improvements-in-net-core-3-0/) are a way to efficiently expose performance metrics from an application. EF Core 5.0 includes event counters under the `Microsoft.EntityFrameworkCore` category. For example:

```
dotnet counters monitor Microsoft.EntityFrameworkCore -p 49496
```

This tells dotnet counters to start collecting EF Core events for process 49496. This generates output like this in the console:

```
[Microsoft.EntityFrameworkCore]
    Active DbContexts                                               1
    Execution Strategy Operation Failures (Count / 1 sec)           0
    Execution Strategy Operation Failures (Total)                   0
    Optimistic Concurrency Failures (Count / 1 sec)                 0
    Optimistic Concurrency Failures (Total)                         0
    Queries (Count / 1 sec)                                     1,755
    Queries (Total)                                            98,402
    Query Cache Hit Rate (%)                                      100
    SaveChanges (Count / 1 sec)                                     0
    SaveChanges (Total)                                             1
```

### Property bags

EF Core 5.0 allows the same CLR type to be mapped to multiple different entity types. Such types are known as shared-type entity types. This feature combined with indexer properties (included in preview 1) allows property bags to be used as entity type.

For example, the DbContext below configures the BCL type `Dictionary<string, object>` as a shared-type entity type for both products and categories.

```c#
public class ProductsContext : DbContext
{
    public DbSet<Dictionary<string, object>> Products => Set<Dictionary<string, object>>("Product");
    public DbSet<Dictionary<string, object>> Categories => Set<Dictionary<string, object>>("Category");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Category", b =>
        {
            b.IndexerProperty<string>("Description");
            b.IndexerProperty<int>("Id");
            b.IndexerProperty<string>("Name").IsRequired();
        });

        modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Product", b =>
        {
            b.IndexerProperty<int>("Id");
            b.IndexerProperty<string>("Name").IsRequired();
            b.IndexerProperty<string>("Description");
            b.IndexerProperty<decimal>("Price");
            b.IndexerProperty<int?>("CategoryId");

            b.HasOne("Category", null).WithMany();
        });
    }
}
```

Dictionary objects ("property bags") can now be added to the context as entity instances and saved. For example:

```c#
var beverages = new Dictionary<string, object>
{
    ["Name"] = "Beverages",
    ["Description"] = "Stuff to sip on"
};

context.Categories.Add(beverages);

context.SaveChanges();
```

These entities can then be queried and updated in the normal way:

```c#
var foods = context.Categories.Single(e => e["Name"] == "Foods");
var marmite = context.Products.Single(e => e["Name"] == "Marmite");

marmite["CategoryId"] = foods["Id"];
marmite["Description"] = "Yummy when spread _thinly_ on buttered Toast!";

context.SaveChanges();
```

### SaveChanges interception and events

EF Core 5.0 introduces both .NET events and an EF Core interceptor triggered when SaveChanges is called.

The events are simple to use; for example:

```c#
context.SavingChanges += (sender, args) =>
{
    Console.WriteLine($"Saving changes for {((DbContext)sender).Database.GetConnectionString()}");
};

context.SavedChanges += (sender, args) =>
{
    Console.WriteLine($"Saved {args.EntitiesSavedCount} changes for {((DbContext)sender).Database.GetConnectionString()}");
};
```

Notice that:
* The event sender is the `DbContext` instance
* The args for the `SavedChanges` event contains the number of entities saved to the database

The interceptor is defined by `ISaveChangesInterceptor`, but it is often convienient to inherit from `SaveChangesInterceptor` to avoid implementing every method. For example:

```c#
public class MySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Console.WriteLine($"Saving changes for {eventData.Context.Database.GetConnectionString()}");

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Console.WriteLine($"Saving changes asynchronously for {eventData.Context.Database.GetConnectionString()}");

        return new ValueTask<InterceptionResult<int>>(result);
    }
}
```

Notice that:
* The interceptor has both sync and async methods. This can be useful if you need to perform async I/O, such as writing to an audit server.
* The interceptor allows SaveChanges to be skipped using the `InterceptionResult` mechanism common to all interceptors.

The downside of interceptors is that they must be registered on the DbContext when it is being constructed. For example:

```c#
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(new MySaveChangesInterceptor())
            .UseSqlite("Data Source = test.db");
```

In contrast, the events can be registered on the DbContext instance at any time.

### Exclude tables from migrations

It is sometimes useful to have a single entity type mapped in multiple DbContexts. This is especially true when using [bounded contexts](https://www.martinfowler.com/bliki/BoundedContext.html), for which it is common to have a different DbContext type for each bounded context.

For example, a `User` type may be needed by both an authorization context and a reporting context. If a change is made to the `User` type, then migrations for both DbContexts will attempt to update the database. To prevent this, the model for one of the contexts can be configured to exclude the table from its migrations.

In the code below, the `AuthorizationContext` will generate migrations for changes to the `Users` table, but the `ReportingContext` will not, preventing the migrations from clashing.

```C#
public class AuthorizationContext : DbContext
{
    public DbSet<User> Users { get; set; }
}

public class ReportingContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users", t => t.ExcludeFromMigrations());
    }
}
```

### Required 1:1 dependents

In EF Core 3.1, the dependent end of a one-to-one relationship was always considered optional. This was most apparent when using owned entities. For example, consider the following model and configuration:

```c#
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Address HomeAddress { get; set; }
    public Address WorkAddress { get; set; }
}

public class Address
{
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string Postcode { get; set; }
}
```

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>(b =>
    {
        b.OwnsOne(e => e.HomeAddress,
            b =>
            {
                b.Property(e => e.Line1).IsRequired();
                b.Property(e => e.City).IsRequired();
                b.Property(e => e.Region).IsRequired();
                b.Property(e => e.Postcode).IsRequired();
            });

        b.OwnsOne(e => e.WorkAddress);
    });
}
```

This results in Migrations creating the following table for SQLite:

```sql
CREATE TABLE "People" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_People" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "HomeAddress_Line1" TEXT NULL,
    "HomeAddress_Line2" TEXT NULL,
    "HomeAddress_City" TEXT NULL,
    "HomeAddress_Region" TEXT NULL,
    "HomeAddress_Country" TEXT NULL,
    "HomeAddress_Postcode" TEXT NULL,
    "WorkAddress_Line1" TEXT NULL,
    "WorkAddress_Line2" TEXT NULL,
    "WorkAddress_City" TEXT NULL,
    "WorkAddress_Region" TEXT NULL,
    "WorkAddress_Country" TEXT NULL,
    "WorkAddress_Postcode" TEXT NULL
);
```

Notice that all the columns are nullable, even though some of the `HomeAddress` properties have been configured as required. Also, when querying for a `Person`, if all the columns for either the home or work address are null, then EF Core will leave the `HomeAddress` and/or `WorkAddress` properties as null, rather than setting an empty instance of `Address`.

In EF Core 5.0, the `HomeAddress` navigation can now be configured as as a required dependent. For example:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>(b =>
    {
        b.OwnsOne(e => e.HomeAddress,
            b =>
            {
                b.Property(e => e.Line1).IsRequired();
                b.Property(e => e.City).IsRequired();
                b.Property(e => e.Region).IsRequired();
                b.Property(e => e.Postcode).IsRequired();
            });
        b.Navigation(e => e.HomeAddress).IsRequired();

        b.OwnsOne(e => e.WorkAddress);
    });
}
```

The table created by Migrations will now included non-nullable columns for the required properties of the required dependent:

```sql
CREATE TABLE "People" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_People" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "HomeAddress_Line1" TEXT NOT NULL,
    "HomeAddress_Line2" TEXT NULL,
    "HomeAddress_City" TEXT NOT NULL,
    "HomeAddress_Region" TEXT NOT NULL,
    "HomeAddress_Country" TEXT NULL,
    "HomeAddress_Postcode" TEXT NOT NULL,
    "WorkAddress_Line1" TEXT NULL,
    "WorkAddress_Line2" TEXT NULL,
    "WorkAddress_City" TEXT NULL,
    "WorkAddress_Region" TEXT NULL,
    "WorkAddress_Country" TEXT NULL,
    "WorkAddress_Postcode" TEXT NULL
);
```

In addition, EF Core will now throw an exception if an attempt is made to save an owner which has a null required dependent. In this example, EF Core will throw when attempting to save a `Person` with a null `HomeAddress`.

Finally, EF Core will still create an instance of a required dependent even when all the columns for the required dependent have null values.

### Options for migration generation

EF Core 5.0 introduces greater control over generation of migrations for different purposes. This includes the ability to:

* Know if the migration is being generated for a script or for immediate execution
* Know if an idempotent script is being generated
* Know if the script should exclude transaction statements (See _Migrations scripts with transactions_ below.)

This behavior is specified by an the `MigrationsSqlGenerationOptions` enum, which can now be passed to `IMigrator.GenerateScript`.

Also included in this work is better generation of idempotent scripts with calls to `EXEC` on SQL Server when needed. This work also enables similar improvements to the scripts generated by other database providers, including PostgreSQL.

### Migrations scripts with transactions

SQL scripts generated from migrations now contain statements to begin and commit transactions as appropriate for the migration. For example, the migration script below was generated from two migrations. Notice that each migration is now applied inside a transaction.

```sql
BEGIN TRANSACTION;
GO

CREATE TABLE [Groups] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Members] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [GroupId] int NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Members_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Members_GroupId] ON [Members] ([GroupId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200910194835_One', N'6.0.0-alpha.1.20460.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Groups].[Name]', N'GroupName', N'COLUMN';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200910195234_Two', N'6.0.0-alpha.1.20460.2');
GO

COMMIT;
```

As mentioned in the previous section, this use of transactions can be disabled if transactions need to be handled differently.

### See pending migrations

This feature was contributed from the community by [@Psypher9](https://github.com/Psypher9). Many thanks for the contribution!

The `dotnet ef migrations list` command now shows which migrations have not yet been applied to the database. For example:

```
ajcvickers@avickers420u:~/AllTogetherNow/Daily$ dotnet ef migrations list
Build started...
Build succeeded.
20200910201647_One
20200910201708_Two
20200910202050_Three (Pending)
ajcvickers@avickers420u:~/AllTogetherNow/Daily$
```

In addition, there is now a `Get-Migration` command for the Package Manager Console with the same functionality.

### ModelBuilder API for value comparers

EF Core properties for custom mutable types [require a value comparer](xref:core/modeling/value-comparers) for property changes to be detected correctly. This can now be specified as part of configuring the value conversion for the type. For example:

```c#
modelBuilder
    .Entity<EntityType>()
    .Property(e => e.MyProperty)
    .HasConversion(
        v => JsonSerializer.Serialize(v, null),
        v => JsonSerializer.Deserialize<List<int>>(v, null),
        new ValueComparer<List<int>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
```

### EntityEntry TryGetValue methods

This feature was contributed from the community by [@m4ss1m0g](https://github.com/m4ss1m0g). Many thanks for the contribution!

A `TryGetValue` method has been added to `EntityEntry.CurrentValues` and `EntityEntry.OriginalValues`. This allows the value of a property to be requested without first checking if the property is mapped in the EF model. For example:

```c#
if (entry.CurrentValues.TryGetValue(propertyName, out var value))
{
    Console.WriteLine(value);
}
```

### Default max batch size for SQL Server

Starting with EF Core 5.0, the default maximum batch size for SaveChanges on SQL Server is now 42. As is well known, this is also the answer to the Ultimate Question of Life, the Universe, and Everything. However, this is probably a coincidence, since the value was obtained through [analysis of batching performance](https://github.com/dotnet/efcore/issues/9270). We do not believe that we have discovered a form of the Ultimate Question, although it does seem somewhat plausible that the Earth was created to understand why SQL Server works the way it does.

### Default environment to Development

The EF Core command line tools now automatically configure the `ASPNETCORE_ENVIRONMENT` _and_ `DOTNET_ENVIRONMENT` environment variables to "Development". This brings the experience when using the generic host in line with the experience for ASP.NET Core during development. See [#19903](https://github.com/dotnet/efcore/issues/19903).

### Better migrations column ordering

The columns for unmapped base classes are now ordered after other columns for mapped entity types. Note this only impacts newly created tables. The column order for existing tables remains unchanged. See [#11314](https://github.com/dotnet/efcore/issues/11314).

### Query improvements

EF Core 5.0 RC1 contains some additional query translation improvements:

* Translation of `is` on Cosmos--see [#16391](https://github.com/dotnet/efcore/issues/16391)
* User-mapped functions can now be annotated to control null propagation--see [#19609](https://github.com/dotnet/efcore/issues/19609)
* Support for translation of GroupBy with conditional aggregates--see [#11711](https://github.com/dotnet/efcore/issues/11711)
* Translation of Distinct operator over group element before aggregate--see [#17376](https://github.com/dotnet/efcore/issues/17376)

### Model building for fields

Finally for RC1, EF Core now allows use of the lambda methods in the ModelBuilder for fields as well as properties. For example, if you are averse to properties for some reason and decide to use public fields, then these fields can now be mapped using the lambda builders:

```c#
public class Post
{
    public int Id;
    public string Name;
    public string Category;
    public int BlogId;
    public Blog Blog;
}

public class Blog
{
    public int Id;
    public string Name;
    public ICollection<Post> Posts;
}
```

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>(b =>
    {
        b.Property(e => e.Id);
        b.Property(e => e.Name);
    });

    modelBuilder.Entity<Post>(b =>
    {
        b.Property(e => e.Id);
        b.Property(e => e.Name);
        b.Property(e => e.Category);
        b.Property(e => e.BlogId);
        b.HasOne(e => e.Blog).WithMany(e => e.Posts);
    });
}
```

While this is now possible, we are certainly not recommending that you do this. Also, note that this does not add any additional field mapping capabilities to EF Core, it only allows the lambda methods to be used instead of always requiring the string methods. This is seldom useful since fields are rarely public.

## Preview 8

### Table-per-type (TPT) mapping

By default, EF Core maps an inheritance hierarchy of .NET types to a single database table. This is known as table-per-hierarchy (TPH) mapping. EF Core 5.0 also allows mapping each .NET type in an inheritance hierarchy to a different database table; known as table-per-type (TPT) mapping.

For example, consider this model with a mapped hierarchy:

```c#
public class Animal
{
    public int Id { get; set; }
    public string Species { get; set; }
}

public class Pet : Animal
{
    public string Name { get; set; }
}

public class Cat : Pet
{
    public string EducationLevel { get; set; }
}

public class Dog : Pet
{
    public string FavoriteToy { get; set; }
}
```

By default, EF Core will map this to a single table:

```sql
CREATE TABLE [Animals] (
    [Id] int NOT NULL IDENTITY,
    [Species] nvarchar(max) NULL,
    [Discriminator] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NULL,
    [EdcuationLevel] nvarchar(max) NULL,
    [FavoriteToy] nvarchar(max) NULL,
    CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])
);
```

However, mapping each entity type to a different table will instead result in one table per type:

```sql
CREATE TABLE [Animals] (
    [Id] int NOT NULL IDENTITY,
    [Species] nvarchar(max) NULL,
    CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])
);

CREATE TABLE [Pets] (
    [Id] int NOT NULL,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Pets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Pets_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Cats] (
    [Id] int NOT NULL,
    [EdcuationLevel] nvarchar(max) NULL,
    CONSTRAINT [PK_Cats] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cats_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Cats_Pets_Id] FOREIGN KEY ([Id]) REFERENCES [Pets] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Dogs] (
    [Id] int NOT NULL,
    [FavoriteToy] nvarchar(max) NULL,
    CONSTRAINT [PK_Dogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Dogs_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Dogs_Pets_Id] FOREIGN KEY ([Id]) REFERENCES [Pets] ([Id]) ON DELETE NO ACTION
);
```

Note that creation of the foreign key constraints shown above were added after branching the code for preview 8.

Entity types can be mapped to different tables using mapping attributes:

```c#
[Table("Animals")]
public class Animal
{
    public int Id { get; set; }
    public string Species { get; set; }
}

[Table("Pets")]
public class Pet : Animal
{
    public string Name { get; set; }
}

[Table("Cats")]
public class Cat : Pet
{
    public string EdcuationLevel { get; set; }
}

[Table("Dogs")]
public class Dog : Pet
{
    public string FavoriteToy { get; set; }
}
```

Or using `ModelBuilder` configuration:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Animal>().ToTable("Animals");
    modelBuilder.Entity<Pet>().ToTable("Pets");
    modelBuilder.Entity<Cat>().ToTable("Cats");
    modelBuilder.Entity<Dog>().ToTable("Dogs");
}
```

Documentation is tracked by issue [#1979](https://github.com/dotnet/EntityFramework.Docs/issues/1979).

### Migrations: Rebuild SQLite tables

Compared to other database, SQLite is relatively limited in its schema manipulation capabilities. For example, dropping a column from an existing table requires that the entire table be dropped and re-created. EF Core 5.0 Migrations now supports automatic rebuilding the table for schema changes that require it.

For example, imagine we have a `Unicorns` table created for a `Unicorn` entity type:

```c#
public class Unicorn
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
```

```sql
CREATE TABLE "Unicorns" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Unicorns" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "Age" INTEGER NOT NULL
);
```

We then learn that storing the age of a unicorn is considered very rude, so let's remove that property, add a new migration, and update the database. This update will fail when using EF Core 3.1 because the column cannot be dropped. In EF Core 5.0, Migrations will instead rebuild the table:

```sql
CREATE TABLE "ef_temp_Unicorns" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Unicorns" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL
);

INSERT INTO "ef_temp_Unicorns" ("Id", "Name")
SELECT "Id", "Name"
FROM Unicorns;

PRAGMA foreign_keys = 0;

DROP TABLE "Unicorns";

ALTER TABLE "ef_temp_Unicorns" RENAME TO "Unicorns";

PRAGMA foreign_keys = 1;
```

Notice that:
* A temporary table is created with the desired schema for the new table
* Data is copied from the current table into the temporary table
* Foreign key enforcement is switched off
* The current table is dropped
* The temporary table is renamed to be the new table

Documentation is tracked by issue [#2583](https://github.com/dotnet/EntityFramework.Docs/issues/2583).

### Table-valued functions

This feature was contributed from the community by [@pmiddleton](https://github.com/pmiddleton). Many thanks for the contribution!

EF Core 5.0 includes first-class support for mapping .NET methods to table-valued functions (TVFs). These functions can then be used in LINQ queries where additional composition on the results of the function will also be translated to SQL.

For example, consider this TVF defined in a SQL Server database:

```sql
CREATE FUNCTION GetReports(@employeeId int)
RETURNS @reports TABLE
(
	Name nvarchar(50) NOT NULL,
	IsDeveloper bit NOT NULL
)
AS
BEGIN
	WITH cteEmployees AS
	(
		SELECT Id, Name, ManagerId, IsDeveloper
		FROM Employees
		WHERE Id = @employeeId
		UNION ALL
		SELECT e.Id, e.Name, e.ManagerId, e.IsDeveloper
		FROM Employees e
		INNER JOIN cteEmployees cteEmp ON cteEmp.Id = e.ManagerId
	)
	INSERT INTO @reports
	SELECT Name, IsDeveloper
	FROM cteEmployees
	WHERE Id != @employeeId

	RETURN
END
```

The EF Core model requires two entity types to use this TVF:
* An `Employee` type that maps to the Employees table in the normal way
* A `Report` type that matches the shape returned by the TVF

```c#
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDeveloper { get; set; }

    public int? ManagerId { get; set; }
    public virtual Employee Manager { get; set; }
}
```

```c#
public class Report
{
    public string Name { get; set; }
    public bool IsDeveloper { get; set; }
}
```

These types must be included in the EF Core model:

```c#
modelBuilder.Entity<Employee>();
modelBuilder.Entity(typeof(Report)).HasNoKey();
```

Notice that `Report` has no primary key and so must be configured as such.

Finally, a .NET method must be mapped to the TVF in the database. This method can be defined on the DbContext using the new `FromExpression` method:

```c#
public IQueryable<Report> GetReports(int managerId)
    => FromExpression(() => GetReports(managerId));
```

This method uses a parameter and return type that match the TVF defined above. The method is then added to the EF Core model in OnModelCreating:

```c#
modelBuilder.HasDbFunction(() => GetReports(default));
```

(Using a lambda here is an easy way to pass the `MethodInfo` to EF Core. The arguments passed to the method are ignored.)

We can now write queries that call `GetReports` and compose over the results. For example:

```c#
from e in context.Employees
from rc in context.GetReports(e.Id)
where rc.IsDeveloper == true
select new
{
  ManagerName = e.Name,
  EmployeeName = rc.Name,
})
```

On SQL Server, this translates to:

```sql
SELECT [e].[Name] AS [ManagerName], [g].[Name] AS [EmployeeName]
FROM [Employees] AS [e]
CROSS APPLY [dbo].[GetReports]([e].[Id]) AS [g]
WHERE [g].[IsDeveloper] = CAST(1 AS bit)
```

Notice that the SQL is rooted in the `Employees` table, calls `GetReports`, and then adds an additional WHERE clause on the results of the function.

### Flexible query/update mapping

EF Core 5.0 allows mapping the same entity type to different database objects. These objects may be tables, views, or functions.

For example, an entity type can be mapped to both a database view and a database table:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .Entity<Blog>()
        .ToTable("Blogs")
        .ToView("BlogsView");
}
```

By default, EF Core will then query from the view and send updates to the table. For example, executing the following code:

```c#
var blog = context.Set<Blog>().Single(e => e.Name == "One Unicorn");

blog.Name = "1unicorn2";

context.SaveChanges();
```

Results in a query against the view, and then an update to the table:

```sql
SELECT TOP(2) [b].[Id], [b].[Name], [b].[Url]
FROM [BlogsView] AS [b]
WHERE [b].[Name] = N'One Unicorn'

SET NOCOUNT ON;
UPDATE [Blogs] SET [Name] = @p0
WHERE [Id] = @p1;
SELECT @@ROWCOUNT;
```

### Context-wide split-query configuration

Split queries (see below) can now be configured as the default for any query executed by the DbContext. This configuration is only available for relational providers, and so must be specified as part of the `UseProvider` configuration. For example:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer(
            Your.SqlServerConnectionString,
            b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
```

Documentation is tracked by issue [#2407](https://github.com/dotnet/EntityFramework.Docs/issues/2407).

### PhysicalAddress mapping

This feature was contributed from the community by [@ralmsdeveloper](https://github.com/ralmsdeveloper). Many thanks for the contribution!

The standard .NET [PhysicalAddress class](/dotnet/api/system.net.networkinformation.physicaladdress) is now automatically mapped to a string column for databases that do not already have native support. For more information, see the examples for `IPAddress` below.

## Preview 7

### DbContextFactory

EF Core 5.0 introduces `AddDbContextFactory` and `AddPooledDbContextFactory` to register a factory for creating DbContext instances in the application's dependency injection (D.I.) container. For example:

```csharp
services.AddDbContextFactory<SomeDbContext>(b =>
    b.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test"));
```

Application services such as ASP.NET Core controllers can then depend on `IDbContextFactory<TContext>` in the service constructor. For example:

```csharp
public class MyController
{
    private readonly IDbContextFactory<SomeDbContext> _contextFactory;

    public MyController(IDbContextFactory<SomeDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
```

DbContext instances can then be created and used as needed. For example:

```csharp
public void DoSomeThing()
{
    using (var context = _contextFactory.CreateDbContext())
    {
        // ...
    }
}
```

Note that the DbContext instances created in this way are _not_ managed by the application's service provider and therefore must be disposed by the application. This decoupling is very useful for Blazor applications, where using `IDbContextFactory` is recommended, but may also be useful in other scenarios.

DbContext instances can be pooled by calling `AddPooledDbContextFactory`. This pooling works the same way as for `AddDbContextPool`, and also has the same limitations.

Documentation is tracked by issue [#2523](https://github.com/dotnet/EntityFramework.Docs/issues/2523).

### Reset DbContext state

EF Core 5.0 introduces `ChangeTracker.Clear()` which clears the DbContext of all tracked entities. This should usually not be needed when using the best practice of creating a new, short-lived context instance for each unit-of-work. However, if there is a need to reset the state of a DbContext instance, then using the new `Clear()` method is more performant and robust than mass-detaching all entities.

Documentation is tracked by issue [#2524](https://github.com/dotnet/EntityFramework.Docs/issues/2524).

### New pattern for store-generated defaults

EF Core allows an explicit value to be set for a column that may also have default value constraint. EF Core uses the CLR default of type property type as a sentinel for this; if the value is not the CLR default, then it is inserted, otherwise the database default is used.

This creates problems for types where the CLR default is not a good sentinel--most notably, `bool` properties. EF Core 5.0 now allows the backing field to be nullable for cases like this. For example:

```csharp
public class Blog
{
    private bool? _isValid;

    public bool IsValid
    {
        get => _isValid ?? false;
        set => _isValid = value;
    }
}
```

Note that the backing field is nullable, but the publicly exposed property is not. This allows the sentinel value to be `null` without impacting the public surface of the entity type. In this case, if the `IsValid` is never set, then the database default will be used since the backing field remains null. If either `true` or `false` are set, then this value is saved explicitly to the database.

Documentation is tracked by issue [#2525](https://github.com/dotnet/EntityFramework.Docs/issues/2525).

### Cosmos partition keys

EF Core allows the Cosmos partition key is included in the EF model. For example:

```csharp
modelBuilder.Entity<Customer>().HasPartitionKey(b => b.AlternateKey)
```

Starting with preview 7, the partition key is included in the entity type's PK and is used to improved performance in some queries.

Documentation is tracked by issue [#2471](https://github.com/dotnet/EntityFramework.Docs/issues/2471).

### Cosmos configuration

EF Core 5.0 improves configuration of Cosmos and Cosmos connections.

Previously, EF Core required the end-point and key to be specified explicitly when connecting to a Cosmos database. EF Core 5.0 allows use of a connection string instead. In addition, EF Core 5.0 allows the WebProxy instance to be explicitly set. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseCosmos("my-cosmos-connection-string", "MyDb",
            cosmosOptionsBuilder =>
            {
                cosmosOptionsBuilder.WebProxy(myProxyInstance);
            });
```

Many other timeout values, limits, etc. can now also be configured. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseCosmos("my-cosmos-connection-string", "MyDb",
            cosmosOptionsBuilder =>
            {
                cosmosOptionsBuilder.LimitToEndpoint();
                cosmosOptionsBuilder.RequestTimeout(requestTimeout);
                cosmosOptionsBuilder.OpenTcpConnectionTimeout(timeout);
                cosmosOptionsBuilder.IdleTcpConnectionTimeout(timeout);
                cosmosOptionsBuilder.GatewayModeMaxConnectionLimit(connectionLimit);
                cosmosOptionsBuilder.MaxTcpConnectionsPerEndpoint(connectionLimit);
                cosmosOptionsBuilder.MaxRequestsPerTcpConnection(requestLimit);
            });
```

Finally, the default connection mode is now `ConnectionMode.Gateway`, which is generally more compatible.

Documentation is tracked by issue [#2471](https://github.com/dotnet/EntityFramework.Docs/issues/2471).

### Scaffold-DbContext now singularizes

Previously when scaffolding a DbContext from an existing database, EF Core will create entity type names that match the table names in the database. For example, tables `People` and `Addresses` resulted in entity types named `People` and `Addresses`.

In previous releases, this behavior was configurable through registration of a pluralization service. Now in EF Core 5.0, the [Humanizer](https://www.nuget.org/packages/Humanizer.Core/) package is used as a default pluralization service. This means tables `People` and `Addresses` will now be reverse engineered to entity types named `Person` and `Address`.

### Savepoints

EF Core now supports [savepoints](/sql/t-sql/language-elements/save-transaction-transact-sql#remarks) for greater control over transactions that execute multiple operations.

Savepoints can be manually created, released, and rolled back. For example:

```csharp
context.Database.CreateSavepoint("MySavePoint");
```

In addition, EF Core will now roll back to the last savepoint when executing `SaveChanges` fails. This allows SaveChanges to be re-tried without re-trying the entire transaction.

Documentation is tracked by issue [#2429](https://github.com/dotnet/EntityFramework.Docs/issues/2429).

## Preview 6

### Split queries for related collections

Starting with EF Core 3.0, EF Core always generates a single SQL query for each LINQ query. This ensures consistency of the data returned within the constraints of the transaction mode in use. However, this can become very slow when the query uses `Include` or a projection to bring back multiple related collections.

EF Core 5.0 now allows a single LINQ query including related collections to be split into multiple SQL queries. This can significantly improve performance, but can result in inconsistency in the results returned if the data changes between the two queries. Serializable or snapshot transactions can be used to mitigate this and achieve consistency with split queries, but that may bring other performance costs and behavioral difference.

#### Split queries with Include

For example, consider a query that pulls in two levels of related collections using `Include`:

```CSharp
var artists = context.Artists
    .Include(e => e.Albums).ThenInclude(e => e.Tags)
    .ToList();
```

By default, EF Core will generate the following SQL when using the SQLite provider:

```sql
SELECT "a"."Id", "a"."Name", "t0"."Id", "t0"."ArtistId", "t0"."Title", "t0"."Id0", "t0"."AlbumId", "t0"."Name"
FROM "Artists" AS "a"
LEFT JOIN (
    SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "t"."Id" AS "Id0", "t"."AlbumId", "t"."Name"
    FROM "Album" AS "a0"
    LEFT JOIN "Tag" AS "t" ON "a0"."Id" = "t"."AlbumId"
) AS "t0" ON "a"."Id" = "t0"."ArtistId"
ORDER BY "a"."Id", "t0"."Id", "t0"."Id0"
```

The new `AsSplitQuery` API can be used to change this behavior. For example:

```CSharp
var artists = context.Artists
    .AsSplitQuery()
    .Include(e => e.Albums).ThenInclude(e => e.Tags)
    .ToList();
```

AsSplitQuery is available for all relational database providers and can be used anywhere in the query, just like AsNoTracking. EF Core will now generate the following three SQL queries:

```sql
SELECT "a"."Id", "a"."Name"
FROM "Artists" AS "a"
ORDER BY "a"."Id"

SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "a"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
ORDER BY "a"."Id", "a0"."Id"

SELECT "t"."Id", "t"."AlbumId", "t"."Name", "a"."Id", "a0"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
INNER JOIN "Tag" AS "t" ON "a0"."Id" = "t"."AlbumId"
ORDER BY "a"."Id", "a0"."Id"
```

All operations on the query root are supported. This includes OrderBy/Skip/Take, Join operations, FirstOrDefault and similar single result selecting operations.

Note that filtered Includes with OrderBy/Skip/Take are not supported in preview 6, but are available in the daily builds and will be included in preview 7.

#### Split queries with collection projections

`AsSplitQuery` can also be used when collections are loaded in projections. For example:

```CSharp
context.Artists
    .AsSplitQuery()
    .Select(e => new
    {
        Artist = e,
        Albums = e.Albums,
    }).ToList();
```

This LINQ query generates the following two SQL queries when using the SQLite provider:

```sql
SELECT "a"."Id", "a"."Name"
FROM "Artists" AS "a"
ORDER BY "a"."Id"

SELECT "a0"."Id", "a0"."ArtistId", "a0"."Title", "a"."Id"
FROM "Artists" AS "a"
INNER JOIN "Album" AS "a0" ON "a"."Id" = "a0"."ArtistId"
ORDER BY "a"."Id"
```

Note that only materialization of the collection is supported. Any composition after `e.Albums` in above case won't result in a split query. Improvements in this area are tracked by [#21234](https://github.com/dotnet/efcore/issues/21234).

### IndexAttribute

The new IndexAttribute can be placed on an entity type to specify an index for a single column. For example:

```CSharp
[Index(nameof(FullName), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string FullName { get; set; }
}
```

For SQL Server, Migrations will then generate the following SQL:

```sql
CREATE UNIQUE INDEX [IX_Users_FullName]
    ON [Users] ([FullName])
    WHERE [FullName] IS NOT NULL;
```

IndexAttribute can also be used to specify an index spanning multiple columns. For example:

```CSharp
[Index(nameof(FirstName), nameof(LastName), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string FirstName { get; set; }

    [MaxLength(64)]
    public string LastName { get; set; }
}
```

For SQL Server, this results in:

```sql
CREATE UNIQUE INDEX [IX_Users_FirstName_LastName]
    ON [Users] ([FirstName], [LastName])
    WHERE [FirstName] IS NOT NULL AND [LastName] IS NOT NULL;
```

Documentation is tracked by issue [#2407](https://github.com/dotnet/EntityFramework.Docs/issues/2407).

### Improved query translation exceptions

We are continuing to improve the exception messages generated when query translation fails. For example, this query uses the unmapped property `IsSigned`:

```CSharp
var artists = context.Artists.Where(e => e.IsSigned).ToList();
```

EF Core will throw the following exception indicating that translation failed because `IsSigned` is not mapped:

> Unhandled exception. System.InvalidOperationException: The LINQ expression 'DbSet<Artist>()
>    .Where(a => a.IsSigned)' could not be translated. Additional information: Translation of member 'IsSigned' on entity type 'Artist' failed. Possibly the specified member is not mapped. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.

Similarly, better exception messages are now generated when attempting to translate string comparisons with culture-dependent semantics. For example, this query attempts to use `StringComparison.CurrentCulture`:

```CSharp
var artists = context.Artists
    .Where(e => e.Name.Equals("The Unicorns", StringComparison.CurrentCulture))
    .ToList();
```

EF Core will now throw the following exception:

> Unhandled exception. System.InvalidOperationException: The LINQ expression 'DbSet<Artist>()
>      .Where(a => a.Name.Equals(
>          value: "The Unicorns",
>          comparisonType: CurrentCulture))' could not be translated. Additional information: Translation of 'string.Equals' method which takes 'StringComparison' argument is not supported. See https://go.microsoft.com/fwlink/?linkid=2129535 for more information. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.

### Specify transaction ID

This feature was contributed from the community by [@Marusyk](https://github.com/Marusyk). Many thanks for the contribution!

EF Core exposes a transaction ID for correlation of transactions across calls. This ID typically set by EF Core when a transaction is started. If the application starts the transaction instead, then this feature allows the application to explicitly set the transaction ID so it is correlated correctly everywhere it is used. For example:

```CSharp
using (context.Database.UseTransaction(myTransaction, myId))
{
   ...
}
```

### IPAddress mapping

This feature was contributed from the community by [@ralmsdeveloper](https://github.com/ralmsdeveloper). Many thanks for the contribution!

The standard .NET [IPAddress class](/dotnet/api/system.net.ipaddress) is now automatically mapped to a string column for databases that do not already have native support. For example, consider mapping this entity type:

```CSharp
public class Host
{
    public int Id { get; set; }
    public IPAddress Address { get; set; }
}
```

On SQL Server, this will result in Migrations creating the following table:

```sql
CREATE TABLE [Host] (
    [Id] int NOT NULL,
    [Address] nvarchar(45) NULL,
    CONSTRAINT [PK_Host] PRIMARY KEY ([Id]));
```

Entities can then be added in the normal way:

```CSharp
context.AddRange(
    new Host { Address = IPAddress.Parse("127.0.0.1")},
    new Host { Address = IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:0001")});
```

And the resulting SQL will insert the normalized IPv4 or IPv6 address:

```sql
Executed DbCommand (14ms) [Parameters=[@p0='1', @p1='127.0.0.1' (Size = 45), @p2='2', @p3='::1' (Size = 45)], CommandType='Text', CommandTimeout='30']
      SET NOCOUNT ON;
      INSERT INTO [Host] ([Id], [Address])
      VALUES (@p0, @p1), (@p2, @p3);
```

### Exclude OnConfiguring when scaffolding

When a DbContext is scaffolded from an existing database, EF Core by default creates an OnConfiguring overload with a connection string so that the context is immediately usable. However, this is not useful if you already have a partial class with OnConfiguring, or if you are configuring the context some other way.

To address this, the scaffolding commands can now be instructed to omit generation of OnConfiguring. For example:

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer --no-onconfiguring
```

Or in the Package Manager Console:

```
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer -NoOnConfiguring
```

Note that we recommend using [a named connection string and secure storage like User Secrets](xref:core/managing-schemas/scaffolding#configuration-and-user-secrets).

### Translations for FirstOrDefault on strings

This feature was contributed from the community by [@dvoreckyaa](https://github.com/dvoreckyaa). Many thanks for the contribution!

FirstOrDefault and similar operators for characters in strings are now translated. For example, this LINQ query:

```CSharp
context.Customers.Where(c => c.ContactName.FirstOrDefault() == 'A').ToList();
```

Will be translated to the following SQL when using SQL Server:

```sql
SELECT [c].[Id], [c].[ContactName]
FROM [Customer] AS [c]
WHERE SUBSTRING([c].[ContactName], 1, 1) = N'A'
```

### Simplify case blocks

EF Core now generates better queries with CASE blocks. For example, this LINQ query:

```CSharp
context.Weapons
    .OrderBy(w => w.Name.CompareTo("Marcus' Lancer") == 0)
    .ThenBy(w => w.Id)
```

Was on SQL Server formally translated to:

```sql
SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
ORDER BY CASE
    WHEN (CASE
        WHEN [w].[Name] = N'Marcus'' Lancer' THEN 0
        WHEN [w].[Name] > N'Marcus'' Lancer' THEN 1
        WHEN [w].[Name] < N'Marcus'' Lancer' THEN -1
    END = 0) AND CASE
        WHEN [w].[Name] = N'Marcus'' Lancer' THEN 0
        WHEN [w].[Name] > N'Marcus'' Lancer' THEN 1
        WHEN [w].[Name] < N'Marcus'' Lancer' THEN -1
    END IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [w].[Id]");
```

But is now translated to:

```sql
SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
ORDER BY CASE
    WHEN ([w].[Name] = N'Marcus'' Lancer') AND [w].[Name] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [w].[Id]");
```

## Preview 5

### Database collations

The default collation for a database can now be specified in the EF model. This will flow through to generated migrations to set the collation when the database is created. For example:

```CSharp
modelBuilder.UseCollation("German_PhoneBook_CI_AS");
```

Migrations then generates the following to create the database on SQL Server:

```sql
CREATE DATABASE [Test]
COLLATE German_PhoneBook_CI_AS;
```

The collation to use for specific database columns can also be specified. For example:

```CSharp
 modelBuilder
     .Entity<User>()
     .Property(e => e.Name)
     .UseCollation("German_PhoneBook_CI_AS");
```

For those not using migrations, collations are now reverse-engineered from the database when scaffolding a DbContext.

Finally, the `EF.Functions.Collate()` allows for ad-hoc queries using different collations. For example:

```CSharp
context.Users.Single(e => EF.Functions.Collate(e.Name, "French_CI_AS") == "Jean-Michel Jarre");
```

This will generate the following query for SQL Server:

```sql
SELECT TOP(2) [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE [u].[Name] COLLATE French_CI_AS = N'Jean-Michel Jarre'
```

Note that ad-hoc collations should be used with care as they can negatively impact database performance.

Documentation is tracked by issue [#2273](https://github.com/dotnet/EntityFramework.Docs/issues/2273).

### Flow arguments into IDesignTimeDbContextFactory

Arguments are now flowed from the command line into the `CreateDbContext` method of [IDesignTimeDbContextFactory](/dotnet/api/microsoft.entityframeworkcore.design.idesigntimedbcontextfactory-1). For example, to indicate this is a dev build, a custom argument (e.g. `dev`) can be passed on the command line:

```
dotnet ef migrations add two --verbose --dev
```

This argument will then flow into the factory, where it can be used to control how the context is created and initialized. For example:

```CSharp
public class MyDbContextFactory : IDesignTimeDbContextFactory<SomeDbContext>
{
    public SomeDbContext CreateDbContext(string[] args)
        => new SomeDbContext(args.Contains("--dev"));
}
```

Documentation is tracked by issue [#2419](https://github.com/dotnet/EntityFramework.Docs/issues/2419).

### No-tracking queries with identity resolution

No-tracking queries can now be configured to perform identity resolution. For example, the following query will create a new Blog instance for each Post, even if each Blog has the same primary key.

```CSharp
context.Posts.AsNoTracking().Include(e => e.Blog).ToList();
```

However, at the expense of usually being slightly slower and always using more memory, this query can be changed to ensure only a single Blog instance is created:

```CSharp
context.Posts.AsNoTracking().PerformIdentityResolution().Include(e => e.Blog).ToList();
```

Note that this is only useful for no-tracking queries since all tracking queries already exhibit this behavior. Also, following API review, the `PerformIdentityResolution` syntax will be changed. See [#19877](https://github.com/dotnet/efcore/issues/19877#issuecomment-637371073).

Documentation is tracked by issue [#1895](https://github.com/dotnet/EntityFramework.Docs/issues/1895).

### Stored (persisted) computed columns

Most databases allow computed column values to be stored after computation. While this takes up disk space, the computed column is calculated only once on update, instead of each time its value is retrieved. This also allows the column to be indexed for some databases.

EF Core 5.0 allows computed columns to be configured as stored. For example:

```CSharp
modelBuilder
    .Entity<User>()
    .Property(e => e.SomethingComputed)
    .HasComputedColumnSql("my sql", stored: true);
```

### SQLite computed columns

EF Core now supports computed columns in SQLite databases.

## Preview 4

### Configure database precision/scale in model

Precision and scale for a property can now be specified using the model builder. For example:

```CSharp
modelBuilder
    .Entity<Blog>()
    .Property(b => b.Numeric)
    .HasPrecision(16, 4);
```

Precision and scale can still be set via the full database type, such as "decimal(16,4)".

Documentation is tracked by issue [#527](https://github.com/dotnet/EntityFramework.Docs/issues/527).

### Specify SQL Server index fill factor

The fill factor can now be specified when creating an index on SQL Server. For example:

```CSharp
modelBuilder
    .Entity<Customer>()
    .HasIndex(e => e.Name)
    .HasFillFactor(90);
```

## Preview 3

### Filtered Include

The Include method now supports filtering of the entities included. For example:

```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.Where(p => p.Title.Contains("Cheese")))
    .ToList();
```

This query will return blogs together with each associated post, but only when the post title contains "Cheese".

Skip and Take can also be used to reduce the number of included entities. For example:

```CSharp
var blogs = context.Blogs
    .Include(e => e.Posts.OrderByDescending(post => post.Title).Take(5)))
    .ToList();
```
This query will return blogs with at most five posts included on each blog.

See the [Include documentation](xref:core/querying/related-data#filtered-include) for full details.

### New ModelBuilder API for navigation properties

Navigation properties are primarily configured when [defining relationships](xref:core/modeling/relationships). However, the new `Navigation` method can be used in the cases where navigation properties need additional configuration. For example, to set a backing field for the navigation when the field would not be found by convention:

```CSharp
modelBuilder.Entity<Blog>().Navigation(e => e.Posts).HasField("_myposts");
```

Note that the `Navigation` API does not replace relationship configuration. Instead it allows additional configuration of navigation properties in already discovered or defined relationships.

See the [Configuring Navigation Properties documentation](xref:core/modeling/relationships#configuring-navigation-properties).

### New command-line parameters for namespaces and connection strings

Migrations and scaffolding now allow namespaces to be specified on the command line. For example, to reverse engineer a database putting the context and model classes in different namespaces:

```dotnetcli
dotnet ef dbcontext scaffold "connection string" Microsoft.EntityFrameworkCore.SqlServer --context-namespace "My.Context" --namespace "My.Model"
```

See the [Migrations](xref:core/managing-schemas/migrations/index#namespaces) and [Reverse Engineering](xref:core/managing-schemas/scaffolding#directories-and-namespaces) documentation for full details.

---
Also, a connection string can now be passed to the `database-update` command:

```dotnetcli
dotnet ef database update --connection "connection string"
```

See the [Tools documentation](xref:core/miscellaneous/cli/dotnet#dotnet-ef-database-update) for full details.

Equivalent parameters have also been added to the PowerShell commands used in the VS Package Manager Console.

### EnableDetailedErrors has returned

For performance reasons, EF doesn't do additional null-checks when reading values from the database. This can result in exceptions that are hard to root-cause when an unexpected null is encountered.

Using `EnableDetailedErrors` will add extra null checking to queries such that, for a small performance overhead, these errors are easier to trace back to a root cause.

For example:

```CSharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging() // Often also useful with EnableDetailedErrors
        .UseSqlServer(Your.SqlServerConnectionString);
```

Documentation is tracked by issue [#955](https://github.com/dotnet/EntityFramework.Docs/issues/955).

### Cosmos partition keys

The partition key to use for a given query can now be specified in the query. For example:

```CSharp
await context.Set<Customer>()
             .WithPartitionKey(myPartitionKey)
             .FirstAsync();
```

Documentation is tracked by issue [#2199](https://github.com/dotnet/EntityFramework.Docs/issues/2199).

### Support for the SQL Server DATALENGTH function

This can be accessed using the new `EF.Functions.DataLength` method. For example:

```CSharp
var count = context.Orders.Count(c => 100 < EF.Functions.DataLength(c.OrderDate));
```

## Preview 2

### Use a C# attribute to specify a property backing field

A C# attribute can now be used to specify the backing field for a property. This attribute allows EF Core to still write to and read from the backing field as would normally happen, even when the backing field cannot be found automatically. For example:

```CSharp
public class Blog
{
    private string _mainTitle;

    public int Id { get; set; }

    [BackingField(nameof(_mainTitle))]
    public string Title
    {
        get => _mainTitle;
        set => _mainTitle = value;
    }
}
```

Documentation is tracked by issue [#2230](https://github.com/dotnet/EntityFramework.Docs/issues/2230).

### Complete discriminator mapping

EF Core uses a discriminator column for [TPH mapping of an inheritance hierarchy](xref:core/modeling/inheritance). Some performance enhancements are possible so long as EF Core knows all possible values for the discriminator. EF Core 5.0 now implements these enhancements.

For example, previous versions of EF Core would always generate this SQL for a query returning all types in a hierarchy:

```sql
SELECT [a].[Id], [a].[Discriminator], [a].[Name]
FROM [Animal] AS [a]
WHERE [a].[Discriminator] IN (N'Animal', N'Cat', N'Dog', N'Human')
```

EF Core 5.0 will now generate the following when a complete discriminator mapping is configured:

```sql
SELECT [a].[Id], [a].[Discriminator], [a].[Name]
FROM [Animal] AS [a]
```

It will be the default behavior starting with preview 3.

### Performance improvements in Microsoft.Data.Sqlite

We have made two performance improvements for SQLIte:

* Retrieving binary and string data with GetBytes, GetChars, and GetTextReader is now more efficient by making use of SqliteBlob and streams.
* Initialization of SqliteConnection is now lazy.

These improvements are in the ADO.NET Microsoft.Data.Sqlite provider and hence also improve performance outside of EF Core.

## Preview 1

### Simple logging

This feature adds functionality similar to `Database.Log` in EF6. That is, it provides a simple way to get logs from EF Core without the need to configure any kind of external logging framework.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2085](https://github.com/dotnet/EntityFramework.Docs/issues/2085).

### Simple way to get generated SQL

EF Core 5.0 introduces the `ToQueryString` extension method, which will return the SQL that EF Core will generate when executing a LINQ query.

Preliminary documentation is included in the [EF weekly status for January 9, 2020](https://github.com/dotnet/efcore/issues/19549#issuecomment-572823246).

Additional documentation is tracked by issue [#1331](https://github.com/dotnet/EntityFramework.Docs/issues/1331).

### Use a C# attribute to indicate that an entity has no key

An entity type can now be configured as having no key using the new `KeylessAttribute`. For example:

```CSharp
[Keyless]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public int Zip { get; set; }
}
```

Documentation is tracked by issue [#2186](https://github.com/dotnet/EntityFramework.Docs/issues/2186).

### Connection or connection string can be changed on initialized DbContext

It is now easier to create a DbContext instance without any connection or connection string. Also, the connection or connection string can now be mutated on the context instance. This feature allows the same context instance to dynamically connect to different databases.

Documentation is tracked by issue [#2075](https://github.com/dotnet/EntityFramework.Docs/issues/2075).

### Change-tracking proxies

EF Core can now generate runtime proxies that automatically implement [INotifyPropertyChanging](/dotnet/api/system.componentmodel.inotifypropertychanging) and [INotifyPropertyChanged](/dotnet/api/system.componentmodel.inotifypropertychanged). These then report value changes on entity properties directly to EF Core, avoiding the need to scan for changes. However, proxies come with their own set of limitations, so they are not for everyone.

Documentation is tracked by issue [#2076](https://github.com/dotnet/EntityFramework.Docs/issues/2076).

### Enhanced debug views

Debug views are an easy way to look at the internals of EF Core when debugging issues. A debug view for the Model was implemented some time ago. For EF Core 5.0, we have made the model view easier to read and added a new debug view for tracked entities in the state manager.

Preliminary documentation is included in the [EF weekly status for December 12, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-565196206).

Additional documentation is tracked by issue [#2086](https://github.com/dotnet/EntityFramework.Docs/issues/2086).

### Improved handling of database null semantics

Relational databases typically treat NULL as an unknown value and therefore not equal to any other NULL. While C# treats null as a defined value, which compares equal to any other null. EF Core by default translates queries so that they use C# null semantics. EF Core 5.0 greatly improves the efficiency of these translations.

Documentation is tracked by issue [#1612](https://github.com/dotnet/EntityFramework.Docs/issues/1612).

### Indexer properties

EF Core 5.0 supports mapping of C# indexer properties. These properties allow entities to act as property bags where columns are mapped to named properties in the bag.

Documentation is tracked by issue [#2018](https://github.com/dotnet/EntityFramework.Docs/issues/2018).

### Generation of check constraints for enum mappings

EF Core 5.0 Migrations can now generate CHECK constraints for enum property mappings. For example:

```SQL
MyEnumColumn VARCHAR(10) NOT NULL CHECK (MyEnumColumn IN ('Useful', 'Useless', 'Unknown'))
```

Documentation is tracked by issue [#2082](https://github.com/dotnet/EntityFramework.Docs/issues/2082).

### IsRelational

A new `IsRelational` method has been added in addition to the existing `IsSqlServer`, `IsSqlite`, and `IsInMemory`. This method can be used to test if the DbContext is using any relational database provider. For example:

```CSharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    if (Database.IsRelational())
    {
        // Do relational-specific model configuration.
    }
}
```

Documentation is tracked by issue [#2185](https://github.com/dotnet/EntityFramework.Docs/issues/2185).

### Cosmos optimistic concurrency with ETags

The Azure Cosmos DB database provider now supports optimistic concurrency using ETags. Use the model builder in OnModelCreating to configure an ETag:

```CSharp
builder.Entity<Customer>().Property(c => c.ETag).IsEtagConcurrency();
```

SaveChanges will then throw an `DbUpdateConcurrencyException` on a concurrency conflict, which [can be handled](xref:core/saving/concurrency) to implement retries, etc.

Documentation is tracked by issue [#2099](https://github.com/dotnet/EntityFramework.Docs/issues/2099).

### Query translations for more DateTime constructs

Queries containing new DateTime construction are now translated.

In addition, the following SQL Server functions are now mapped:

* DateDiffWeek
* DateFromParts

For example:

```CSharp
var count = context.Orders.Count(c => date > EF.Functions.DateFromParts(DateTime.Now.Year, 12, 25));

```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translations for more byte array constructs

Queries using Contains, Length, SequenceEqual, etc. on byte[] properties are now translated to SQL.

Preliminary documentation is included in the [EF weekly status for December 5, 2019](https://github.com/dotnet/efcore/issues/15403#issuecomment-562332863).

Additional documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for Reverse

Queries using `Reverse` are now translated. For example:

```CSharp
context.Employees.OrderBy(e => e.EmployeeID).Reverse()
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for bitwise operators

Queries using bitwise operators are now translated in more cases For example:

```CSharp
context.Orders.Where(o => ~o.OrderID == negatedId)
```

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).

### Query translation for strings on Cosmos

Queries that use the string methods Contains, StartsWith, and EndsWith are now translated when using the Azure Cosmos DB provider.

Documentation is tracked by issue [#2079](https://github.com/dotnet/EntityFramework.Docs/issues/2079).
