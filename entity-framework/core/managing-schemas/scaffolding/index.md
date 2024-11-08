---
title: Reverse Engineering - EF Core
description: Reverse engineering a model from an existing database using Entity Framework Core
author: SamMonoRT
ms.date: 03/27/2023
uid: core/managing-schemas/scaffolding
---
# Scaffolding (Reverse Engineering)

Reverse engineering is the process of scaffolding entity type classes and a [DbContext](/dotnet/api/microsoft.entityframeworkcore.dbcontext) class based on a database schema. It can be performed using the `Scaffold-DbContext` command of the EF Core Package Manager Console (PMC) tools or the `dotnet ef dbcontext scaffold` command of the .NET Command-line Interface (CLI) tools.

> [!NOTE]
> The scaffolding of a `DbContext` and entity types documented here is distinct from the [scaffolding of controllers in ASP.NET Core](/aspnet/mvc/overview/getting-started/introduction/adding-a-controller) using Visual Studio, which is not documented here.

> [!TIP]
> If you use Visual Studio, try out the [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools/) community extension. These tools provide a graphical tool which builds on top of the EF Core command line tools and offers additional workflow and customization options.

## Prerequisites

- Before scaffolding, you'll need to install either the [PMC tools](xref:core/cli/powershell), which work on Visual Studio only, or the [.NET CLI tools](xref:core/cli/dotnet), which across all platforms supported by .NET.
- Install the NuGet package for `Microsoft.EntityFrameworkCore.Design` in the project you are scaffolding to.
- Install the NuGet package for the [database provider](xref:core/providers/index) that targets the database schema you want to scaffold from.

## Required arguments

Both the PMC and the .NET CLI commands have two required arguments: the connection string to the database, and the EF Core database provider to use.

### Connection string

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

The first argument to the command is a connection string to the database. The tools use this connection string to read the database schema.

How the connection string is quoted and escaped depends on the shell that is used to run the command. Refer to the shell's documentation. For example, PowerShell requires escaping `$`, but not `\`.

The following example scaffolds entity types and a `DbContext` from the `Chinook` database located on the machine's SQL Server LocalDB instance, making use of the `Microsoft.EntityFrameworkCore.SqlServer` database provider.

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

#### [Visual Studio PMC](#tab/vs)

```powershell
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer
```

***

> [!TIP]
> You can [use Configuration to store and retrieve the connection string](xref:core/miscellaneous/connection-strings#aspnet-core)

#### Connection strings in the scaffolded code

By default, the scaffolder will include the connection string in the scaffolded code, but with a warning. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    => optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=AllTogetherNow");
```

This is done so that the generated code does not crash when first used, which would be a very poor learning experience. However, as the warning says, connection strings should not exist in production code. See [_DbContext Lifetime, Configuration, and Initialization_](xref:core/dbcontext-configuration/index) for the various ways that connection strings can be managed.

> [!TIP]
> The `-NoOnConfiguring` (Visual Studio PMC) or `--no-onconfiguring` (.NET CLI) option can be passed to suppress creation of the `OnConfiguring` method containing the connection string.

### Provider name

The second argument is the provider name. The provider name is typically the same as the provider's NuGet package name. For example, for SQL Server or Azure SQL, use `Microsoft.EntityFrameworkCore.SqlServer`.

## Command line options

The scaffolding process can be controlled by various command line options.

### Specifying tables and views

By default, all tables and views in the database schema are scaffolded into entity types. You can limit which tables and views are scaffolded by specifying schemas and tables.

The `-Schemas` (Visual Studio PMC) or `--schema` (.NET CLI) argument specifies the schemas of tables and views for which entity types will be generated. If this argument is omitted, then all schemas are included. If this option is used, then all tables and views in the schemas will be included in the model, even if they are not explicitly included using `-Tables` or `--table`.

The `-Tables` (Visual Studio PMC) or `--table` (.NET CLI) argument specified the tables and views for which entity types will be generated. Tables or views in a specific schema can be included using the 'schema.table' or 'schema.view' format. If this option is omitted, then all tables and views are included.                                              |

For example, to scaffold only the `Artists` and `Albums` tables:

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold ... --table Artist --table Album
```

#### [Visual Studio PMC](#tab/vs)

```powershell
Scaffold-DbContext ... -Tables Artists, Albums
```

***

To scaffold all tables and views from the `Customer` and `Contractor` schemas:

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold ... --schema Customer --schema Contractor
```

#### [Visual Studio PMC](#tab/vs)

```powershell
Scaffold-DbContext ... -Schemas Customer, Contractor
```

***

For example, to scaffold the `Purchases` table from the `Customer` schema, and the `Accounts` and `Contracts` tables from the `Contractor` schema:

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold ... --table Customer.Purchases --table Contractor.Accounts --table Contractor.Contracts
```

#### [Visual Studio PMC](#tab/vs)

```powershell
Scaffold-DbContext ... -Tables Customer.Purchases, Contractor.Accounts, Contractor.Contracts
```

***

### Preserving database names

Table and column names are fixed up to better match the .NET naming conventions for types and properties by default. Specifying `-UseDatabaseNames` (Visual Studio PMC) or `--use-database-names` (.NET CLI) will disable this behavior preserving the original database names as much as possible. Invalid .NET identifiers will still be fixed and synthesized names like navigation properties will still conform to .NET naming conventions.

For example, consider the following tables:

```sql
CREATE TABLE [BLOGS] (
    [ID] int NOT NULL IDENTITY,
    [Blog_Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([ID]));

CREATE TABLE [posts] (
    [id] int NOT NULL IDENTITY,
    [postTitle] nvarchar(max) NOT NULL,
    [post content] nvarchar(max) NOT NULL,
    [1 PublishedON] datetime2 NOT NULL,
    [2 DeletedON] datetime2 NULL,
    [BlogID] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Posts_Blogs_BlogId] FOREIGN KEY ([BlogID]) REFERENCES [Blogs] ([ID]) ON DELETE CASCADE);
```

By default, the following entity types will be scaffolded from these tables:

```csharp
public partial class Blog
{
    public int Id { get; set; }
    public string BlogName { get; set; } = null!;
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}

public partial class Post
{
    public int Id { get; set; }
    public string PostTitle { get; set; } = null!;
    public string PostContent { get; set; } = null!;
    public DateTime _1PublishedOn { get; set; }
    public DateTime? _2DeletedOn { get; set; }
    public int BlogId { get; set; }
    public virtual Blog Blog { get; set; } = null!;
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
```

However, using `-UseDatabaseNames` or `--use-database-names` results in the following entity types:

```csharp
public partial class BLOG
{
    public int ID { get; set; }
    public string Blog_Name { get; set; } = null!;
    public virtual ICollection<post> posts { get; set; } = new List<post>();
}

public partial class post
{
    public int id { get; set; }
    public string postTitle { get; set; } = null!;
    public string post_content { get; set; } = null!;
    public DateTime _1_PublishedON { get; set; }
    public DateTime? _2_DeletedON { get; set; }
    public int BlogID { get; set; }
    public virtual BLOG Blog { get; set; } = null!;
}
```

### Use mapping attributes (aka Data Annotations)

Entity types are configured using the [`ModelBuilder` API in `OnModelCreating`](xref:core/modeling/index#use-fluent-api-to-configure-a-model) by default. Specify `-DataAnnotations` (PMC) or `--data-annotations` (.NET Core CLI) to instead use [mapping attributes](xref:core/modeling/index#use-data-annotations-to-configure-a-model) when possible.

For example, using the Fluent API will scaffold this:

```csharp
entity.Property(e => e.Title)
    .IsRequired()
    .HasMaxLength(160);
```

While using Data Annotations will scaffold this:

```csharp
[Required]
[StringLength(160)]
public string Title { get; set; }
```

> [!TIP]
> Some aspects of the model cannot be configured using mapping attributes. The scaffolder will still use the model building API to handle these cases.

### DbContext name

The scaffolded `DbContext` class name will be the name of the database suffixed with _Context_ by default. To specify a different one, use `-Context` in PMC and `--context` in the .NET Core CLI.

### Target directories and namespaces

The entity classes and a DbContext class are scaffolded into the project's root directory and use the project's default namespace.

#### [.NET CLI](#tab/dotnet-core-cli)

You can specify the directory where classes are scaffolded using `--output-dir`, and `--context-dir` can be used to scaffold the DbContext class into a separate directory from the entity type classes:

```dotnetcli
dotnet ef dbcontext scaffold ... --context-dir Data --output-dir Models
```

By default, the namespace will be the root namespace plus the names of any subdirectories under the project's root directory. However, you can override the namespace for all output classes by using `--namespace`. You can also override the namespace for just the DbContext class using `--context-namespace`:

```dotnetcli
dotnet ef dbcontext scaffold ... --namespace Your.Namespace --context-namespace Your.DbContext.Namespace
```

#### [Visual Studio PMC](#tab/vs)

You can specify the directory where classes are scaffolded using `-OutputDir`, and `-ContextDir` can be used to scaffold the DbContext class into a separate directory from the entity type classes:

```powershell
Scaffold-DbContext ... -ContextDir Data -OutputDir Models
```

By default, the namespace will be the root namespace plus the names of any subdirectories under the project's root directory. However, you can override the namespace for all output classes by using `-Namespace`. You can also override the namespace for just the DbContext class using `-ContextNamespace`.

```powershell
Scaffold-DbContext ... -Namespace Your.Namespace -ContextNamespace Your.DbContext.Namespace
```

***

## The scaffolded code

The result of scaffolding from an existing database is:

- A file containing a class that inherits from `DbContext`
- A file for each entity type

> [!TIP]
> Starting in EF7, you can also use T4 text templates to customize the generated code. See [_Custom Reverse Engineering Templates_](xref:core/managing-schemas/scaffolding/templates) for more details.

### C# Nullable reference types

The scaffolder can create EF model and entity types that use [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types) (NRTs). NRT usage is scaffolded automatically when NRT support is enabled in the C# project into which the code is being scaffolded.

For example, the following `Tags` table contains both nullable non-nullable string columns:

```sql
CREATE TABLE [Tags] (
  [Id] int NOT NULL IDENTITY,
  [Name] nvarchar(max) NOT NULL,
  [Description] nvarchar(max) NULL,
  CONSTRAINT [PK_Tags] PRIMARY KEY ([Id]));
```

This results in corresponding nullable and non-nullable string properties in the generated class:

<!--
    public partial class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
-->
[!code-csharp[Tag](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=Tag)]

Similarly, the following `Posts` tables contains a required relationship to the `Blogs` table:

```sql
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Contents] nvarchar(max) NOT NULL,
    [PostedOn] datetime2 NOT NULL,
    [UpdatedOn] datetime2 NULL,
    [BlogId] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Posts_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([Id]));
```

This results in the scaffolding of non-nullable (required) relationship between blogs:

<!--
    public partial class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }
-->
[!code-csharp[Blog](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=Blog)]

And posts:

<!--
public partial class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Contents { get; set; } = null!;
    public DateTime PostedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public int BlogId { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
-->
[!code-csharp[Post](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=Post)]

### Many-to-many relationships

The scaffolding process detects simple join tables and automatically generates a [many-to-many mapping](xref:core/modeling/relationships#many-to-many) for them. For example, consider tables for `Posts` and `Tags`, and a join table `PostTag` connecting them:

```sql
CREATE TABLE [Tags] (
  [Id] int NOT NULL IDENTITY,
  [Name] nvarchar(max) NOT NULL,
  [Description] nvarchar(max) NULL,
  CONSTRAINT [PK_Tags] PRIMARY KEY ([Id]));

CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Contents] nvarchar(max) NOT NULL,
    [PostedOn] datetime2 NOT NULL,
    [UpdatedOn] datetime2 NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]));

CREATE TABLE [PostTag] (
    [PostsId] int NOT NULL,
    [TagsId] int NOT NULL,
    CONSTRAINT [PK_PostTag] PRIMARY KEY ([PostsId], [TagsId]),
    CONSTRAINT [FK_PostTag_Posts_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PostTag_Tags_PostsId] FOREIGN KEY ([PostsId]) REFERENCES [Posts] ([Id]) ON DELETE CASCADE);
```

When scaffolded, this results in a class for Post:

<!--
public partial class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Contents { get; set; } = null!;
    public DateTime PostedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public int BlogId { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
-->
[!code-csharp[Post](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=Post)]

And a class for Tag:

<!--
    public partial class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
-->
[!code-csharp[Tag](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=Tag)]

But no class for the `PostTag` table. Instead, configuration for a many-to-many relationship is scaffolded:

<!--
entity.HasMany(d => d.Tags)
    .WithMany(p => p.Posts)
    .UsingEntity<Dictionary<string, object>>(
        "PostTag",
        l => l.HasOne<Tag>().WithMany().HasForeignKey("PostsId"),
        r => r.HasOne<Post>().WithMany().HasForeignKey("TagsId"),
        j =>
            {
                j.HasKey("PostsId", "TagsId");
                j.ToTable("PostTag");
                j.HasIndex(new[] { "TagsId" }, "IX_PostTag_TagsId");
            });
-->
[!code-csharp[ManyToMany](../../../../samples/core/Miscellaneous/NewInEFCore6/ScaffoldingSample.cs?name=ManyToMany)]

### Other programming languages

The EF Core packages published by Microsoft scaffold C# code. However, the underlying scaffolding system supports a plugin model for scaffolding to other languages. This plugin model is used by various community-run projects, for example:

- [EntityFrameworkCore.VisualBasic](https://github.com/efcore/EFCore.VisualBasic) provides support for Visual Basic
- [EFCore.FSharp](https://github.com/efcore/EFCore.FSharp) provides support for F#

### Customizing the code

Starting with EF7, one of the best ways to customize the generated code is by [customizing the T4 templates used to generate it](xref:core/managing-schemas/scaffolding/templates).

The code can also be changed after it is generated, but the best way to do this depends on whether you intend to re-run the scaffolding process when the database model changes.

#### Scaffold once only

With this approach, the scaffolded code provides a starting point for code-based mapping going forward. Any changes to the generated code can be made as desired--it becomes normal code just like any other code in your project.

Keeping the database and the EF model in sync can be done in one of two ways:

- Switch to using [EF Core database migrations](xref:core/managing-schemas/migrations/index), and use the entity types and EF model configuration as the source of truth, using migrations to drive the schema.
- Manually update the entity types and EF configuration when the database changes. For example, if a new column is added to a table, then add a property for the column to the mapped entity type, and add any necessary configuration using mapping attributes and/or code in `OnModelCreating`. This is relatively easy, with the only real challenge being a process to make sure that database changes are recorded or detected in some way so that the developer(s) responsible for the code can react.

#### Repeated scaffolding

An alternate approach to scaffolding once is to re-scaffold every time the database changes. This will _overwrite_ any previously scaffolded code, meaning any changes made to entity types or EF configuration in that code will be lost.

> [TIP]
> By default, the EF commands will not overwrite any existing code to protect against accidental code loss. The `-Force` (Visual Studio PMC) or `--force` (.NET CLI) argument can be used to force overwriting of existing files.

Since the scaffolded code will be overwritten, it is best not to modify it directly, but instead rely on [partial classes and methods](/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods), and the mechanisms in EF Core that allow configuration to be overridden. Specifically:

- Both the `DbContext` class and the entity classes are generated as partial. This allows introducing additional members and code in a separate file which will not be overridden when scaffolding is run.
- The `DbContext` class contains a partial method called `OnModelCreatingPartial`. An implementation of this method can be added to the partial class for the `DbContext`. It will then be called after `OnModelCreating` is called.
- Model configuration made using the `ModelBuilder` APIs overrides any configuration done by conventions or mapping attributes, as well earlier configuration done on the model builder. This means that code in `OnModelCreatingPartial` can be used to override the configuration generated by the scaffolding process, without the need to remove that configuration.

Finally, remember that starting with EF7, the [T4 templates used to generate code can be customized](xref:core/managing-schemas/scaffolding/templates). This is often a more effective approach than scaffolding with the defaults and then modifying with partial classes and/or methods.

## How it works

Reverse engineering starts by reading the database schema. It reads information about tables, columns, constraints, and indexes.

Next, it uses the schema information to create an EF Core model. Tables are used to create entity types; columns are used to create properties; and foreign keys are used to create relationships.

Finally, the model is used to generate code. The corresponding entity type classes, Fluent API, and data annotations are scaffolded in order to re-create the same model from your app.

## Limitations

- Not everything about a model can be represented using a database schema. For example, information about [**inheritance hierarchies**](xref:core/modeling/inheritance), [**owned types**](xref:core/modeling/owned-entities), and [**table splitting**](xref:core/modeling/table-splitting) are not present in the database schema. Because of this, these constructs will never be scaffolded.
- In addition, **some column types** may not be supported by the EF Core provider. These columns won't be included in the model.
- You can define [**concurrency tokens**](xref:core/saving/concurrency) in an EF Core model to prevent two users from updating the same entity at the same time. Some databases have a special type to represent this type of column (for example, rowversion in SQL Server) in which case we can reverse engineer this information; however, other concurrency tokens will not be scaffolded.
