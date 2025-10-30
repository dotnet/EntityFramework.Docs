---
title: Advanced table mapping - EF Core
description: How to configure table splitting and entity splitting using Entity Framework Core.
author: AndriySvyryd
ms.date: 10/10/2022
uid: core/modeling/table-splitting
---

# Advanced table mapping

EF Core offers a lot of flexibility when it comes to mapping entity types to tables in a database. This becomes even more useful when you need to use a database that wasn't created by EF.

The below techniques are described in terms of tables, but the same result can be achieved when mapping to views as well.

## Table splitting

EF Core allows to map two or more entities to a single row. This is called _table splitting_ or _table sharing_.

### Configuration

To use table splitting the entity types need to be mapped to the same table, have the primary keys mapped to the same columns and at least one relationship configured between the primary key of one entity type and another in the same table.

A common scenario for table splitting is using only a subset of the columns in the table for greater performance or encapsulation.

In this example `Order` represents a subset of `DetailedOrder`.

[!code-csharp[Order](../../../samples/core/Modeling/TableSplitting/Order.cs?name=Order)]

[!code-csharp[DetailedOrder](../../../samples/core/Modeling/TableSplitting/DetailedOrder.cs?name=DetailedOrder)]

In addition to the required configuration we call `Property(o => o.Status).HasColumnName("Status")` to map `DetailedOrder.Status` to the same column as `Order.Status`.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=TableSplitting)]

> [!TIP]
> See the [full sample project](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/TableSplitting) for more context.

### Usage

Saving and querying entities using table splitting is done in the same way as other entities:

[!code-csharp[Usage](../../../samples/core/Modeling/TableSplitting/Program.cs?name=Usage)]

### Optional dependent entity

If all of the columns used by a dependent entity are `NULL` in the database, then no instance for it will be created when queried. This allows modeling an optional dependent entity, where the relationship property on the principal would be null. Note that this would also happen if all of the dependent's properties are optional and set to `null`, which might not be expected.

However, the additional check can impact query performance. In addition, if the dependent entity type has dependents of its own, then determining whether an instance should be created becomes non-trivial. To avoid these issues the dependent entity type can be marked as required, see [Required one-to-one dependents](xref:core/modeling/relationships/navigations#required-navigations) for more information.

### Concurrency tokens

If any of the entity types sharing a table has a concurrency token then it must be included in all other entity types as well. This is necessary in order to avoid a stale concurrency token value when only one of the entities mapped to the same table is updated.

To avoid exposing the concurrency token to the consuming code, it's possible the create one as a [shadow property](xref:core/modeling/shadow-properties):

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=ConcurrencyToken&highlight=2)]

### Inheritance

It's recommended to read [the dedicated page on inheritance](xref:core/modeling/inheritance) before continuing with this section.

The dependent types using table splitting can have an inheritance hierarchy, but there are some limitations:

- The dependent entity type __cannot__ use TPC mapping as the derived types wouldn't be able to map to the same table.
- The dependent entity type __can__ use TPT mapping, but only the root entity type can use table splitting.
- If the principal entity type uses TPC, then only the entity types that don't have any descendants can use table splitting. Otherwise the dependent columns would need to be duplicated on the tables corresponding to the derived types, complicating all interactions.

## Entity splitting

EF Core allows to map an entity to rows in two or more tables. This is called _entity splitting_.

### Configuration

For example, consider a database with three tables that hold customer data:

- A `Customers` table for customer information
- A `PhoneNumbers` table for the customer's phone number
- An `Addresses` table for the customer's address

Here are definitions for these tables in SQL Server:

```sql
CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
    
CREATE TABLE [PhoneNumbers] (
    [CustomerId] int NOT NULL,
    [PhoneNumber] nvarchar(max) NULL,
    CONSTRAINT [PK_PhoneNumbers] PRIMARY KEY ([CustomerId]),
    CONSTRAINT [FK_PhoneNumbers_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Addresses] (
    [CustomerId] int NOT NULL,
    [Street] nvarchar(max) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [PostCode] nvarchar(max) NULL,
    [Country] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([CustomerId]),
    CONSTRAINT [FK_Addresses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);
```

Each of these tables would typically be mapped to their own entity type, with relationships between the types. However, if all three tables are always used together, then it can be more convenient to map them all to a single entity type. For example:

<!--
    public class Customer
    {
        public Customer(string name, string street, string city, string? postCode, string country)
        {
            Name = name;
            Street = street;
            City = city;
            PostCode = postCode;
            Country = country;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string? PostCode { get; set; }
        public string Country { get; set; }
    }
-->
[!code-csharp[CombinedCustomer](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=CombinedCustomer)]

This is achieved in EF7 by calling `SplitToTable` for each split in the entity type. For example, the following code splits the `Customer` entity type to the `Customers`, `PhoneNumbers`, and `Addresses` tables shown above:

<!--
            modelBuilder.Entity<Customer>(
                entityBuilder =>
                {
                    entityBuilder
                        .ToTable("Customers")
                        .SplitToTable(
                            "PhoneNumbers",
                            tableBuilder =>
                            {
                                tableBuilder.Property(customer => customer.Id).HasColumnName("CustomerId");
                                tableBuilder.Property(customer => customer.PhoneNumber);
                            })
                        .SplitToTable(
                            "Addresses",
                            tableBuilder =>
                            {
                                tableBuilder.Property(customer => customer.Id).HasColumnName("CustomerId");
                                tableBuilder.Property(customer => customer.Street);
                                tableBuilder.Property(customer => customer.City);
                                tableBuilder.Property(customer => customer.PostCode);
                                tableBuilder.Property(customer => customer.Country);
                            });
                });
-->
[!code-csharp[EntitySplitting](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=EntitySplitting)]

Notice also that, if necessary, different column names can be specified for each of the tables. To configure the column name for the main table see [Table-specific facet configuration](#table-specific-facet-configuration).

### Configuring the linking foreign key

The FK linking the mapped tables is targeting the same properties on which it is declared. Normally it wouldn't be created in the database, as it would be redundant. But there's an exception for when the entity type is mapped to more than one table. To change its facets you can use the [relationship configuration Fluent API](xref:core/modeling/relationships/foreign-and-principal-keys):

<!--
            modelBuilder.Entity<Customer>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Customer>(a => a.Id)
                .OnDelete(DeleteBehavior.Restrict);
-->
[!code-csharp[LinkingForeignKey](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=LinkingForeignKey)]

### Limitations

- Entity splitting can't be used for entity types in hierarchies.
- For any row in the main table there must be a row in each of the split tables (the fragments are not optional).

## Table-specific facet configuration

Some mapping patterns result in the same CLR property being mapped to a column in each of multiple different tables. EF7 allows these columns to have different names. For example, consider a simple inheritance hierarchy:

<!--
    public class Animal
    {
        public int Id { get; set; }
        public string Breed { get; set; } = null!;
    }

    public class Cat : Animal
    {
        public string? EducationalLevel { get; set; }
    }

    public class Dog : Animal
    {
        public string? FavoriteToy { get; set; }
    }
-->
[!code-csharp[Animals](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=Animals)]

With the TPT [inheritance mapping strategy](xref:core/modeling/inheritance), these types will be mapped to three tables. However, the primary key column in each table may have a different name. For example:

```sql
CREATE TABLE [Animals] (
    [Id] int NOT NULL IDENTITY,
    [Breed] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])
);

CREATE TABLE [Cats] (
    [CatId] int NOT NULL,
    [EducationalLevel] nvarchar(max) NULL,
    CONSTRAINT [PK_Cats] PRIMARY KEY ([CatId]),
    CONSTRAINT [FK_Cats_Animals_CatId] FOREIGN KEY ([CatId]) REFERENCES [Animals] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Dogs] (
    [DogId] int NOT NULL,
    [FavoriteToy] nvarchar(max) NULL,
    CONSTRAINT [PK_Dogs] PRIMARY KEY ([DogId]),
    CONSTRAINT [FK_Dogs_Animals_DogId] FOREIGN KEY ([DogId]) REFERENCES [Animals] ([Id]) ON DELETE CASCADE
);
```

EF7 allows this mapping to be configured using a nested table builder:

<!--
            modelBuilder.Entity<Animal>().ToTable("Animals");

            modelBuilder.Entity<Cat>()
                .ToTable(
                    "Cats",
                    tableBuilder => tableBuilder.Property(cat => cat.Id).HasColumnName("CatId"));

            modelBuilder.Entity<Dog>()
                .ToTable(
                    "Dogs",
                    tableBuilder => tableBuilder.Property(dog => dog.Id).HasColumnName("DogId"));
-->
[!code-csharp[AnimalsTpt](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=AnimalsTpt)]

With the TPC inheritance mapping, the `Breed` property can also be mapped to different column names in different tables. For example, consider the following TPC tables:

```sql
CREATE TABLE [Cats] (
    [CatId] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [CatBreed] nvarchar(max) NOT NULL,
    [EducationalLevel] nvarchar(max) NULL,
    CONSTRAINT [PK_Cats] PRIMARY KEY ([CatId])
);

CREATE TABLE [Dogs] (
    [DogId] int NOT NULL DEFAULT (NEXT VALUE FOR [AnimalSequence]),
    [DogBreed] nvarchar(max) NOT NULL,
    [FavoriteToy] nvarchar(max) NULL,
    CONSTRAINT [PK_Dogs] PRIMARY KEY ([DogId])
);
```

EF7 supports this table mapping:

<!--
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();

            modelBuilder.Entity<Cat>()
                .ToTable(
                    "Cats",
                    builder =>
                    {
                        builder.Property(cat => cat.Id).HasColumnName("CatId");
                        builder.Property(cat => cat.Breed).HasColumnName("CatBreed");
                    });

            modelBuilder.Entity<Dog>()
                .ToTable(
                    "Dogs",
                    builder =>
                    {
                        builder.Property(dog => dog.Id).HasColumnName("DogId");
                        builder.Property(dog => dog.Breed).HasColumnName("DogBreed");
                    });
-->
[!code-csharp[AnimalsTpc](../../../samples/core/Miscellaneous/NewInEFCore7/ModelBuildingSample.cs?name=AnimalsTpc)]
