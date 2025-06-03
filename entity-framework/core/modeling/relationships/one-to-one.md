---
title: One-to-one relationships - EF Core
description: How to configure one-to-one relationships between entity types when using Entity Framework Core
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships/one-to-one
---
# One-to-one relationships

One-to-one relationships are used when one entity is associated with at most one other entity. For example, a `Blog` has one `BlogHeader`, and that `BlogHeader` belongs to a single `Blog`.

This document is structured around lots of examples. The examples start with common cases, which also introduce concepts. Later examples cover less common kinds of configuration. A good approach here is to understand the first few examples and concepts, and then go to the later examples based on your specific needs. Based on this approach, we will start with simple "required" and "optional" one-to-one relationships.

> [!TIP]
> The code for all the examples below can be found in [OneToOne.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/Relationships/OneToOne.cs).

## Required one-to-one

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; }                                    // Required foreign key property
            public Blog Blog { get; set; } = null!;                            // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequired](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequired)]

A one-to-one relationship is made up from:

- One or more [primary or alternate key](xref:core/modeling/relationships/foreign-and-principal-keys#principal-keys) properties on the principal entity. For example, `Blog.Id`.
- One or more [foreign key](xref:core/modeling/relationships/foreign-and-principal-keys#foreign-keys) properties on the dependent entity. For example, `BlogHeader.BlogId`.
- Optionally, a [reference navigation](xref:core/modeling/relationships/navigations#reference-navigations) on the principal entity referencing the dependent entity. For example, `Blog.Header`.
- Optionally, a [reference navigation](xref:core/modeling/relationships/navigations#reference-navigations) on the dependent entity referencing the principal entity. For example, `BlogHeader.Blog`.

> [!TIP]
> It is not always obvious which side of a one-to-one relationship should be the principal, and which side should be the dependent. Some considerations are:
>
> - If the database tables for the two types already exist, then the table with the foreign key column(s) must map to the dependent type.
> - A type is usually the dependent type if it cannot logically exist without the other type. For example, it makes no sense to have a header for a blog that does not exist, so `BlogHeader` is naturally the dependent type.
> - If there is a natural parent/child relationship, then the child is usually the dependent type.

So, for the relationship in this example:

- The foreign key property `BlogHeader.BlogId` is not nullable. This makes the relationship "required" because every dependent (`BlogHeader`) _must be related to some principal_ (`Blog`), since its foreign key property must be set to some value.
- Both entities have navigations pointing to the related entity on the other side of the relationship.

> [!NOTE]
> A required relationship ensures that every dependent entity must be associated with some principal entity. However, a principal entity can _always_ exist without any dependent entity. That is, a required relationship does _not_ indicate that there will always be a dependent entity. There is no way in the EF model, and also no standard way in a relational database, to ensure that a principal is associated with a dependent. If this is needed, then it must be implemented in application (business) logic. See [_Required navigations_](xref:core/modeling/relationships/navigations#required-navigations) for more information.

> [!TIP]
> A relationship with two navigations--one from dependent to principal and an inverse from principal to dependent--is known as a bidirectional relationship.

This relationship is [discovered by convention](xref:core/modeling/relationships/conventions). That is:

- `Blog` is discovered as the principal in the relationship, and `BlogHeader` is discovered as the dependent.
- `BlogHeader.BlogId` is discovered as a foreign key of the dependent referencing the `Blog.Id` primary key of the principal. The relationship is discovered as required because `BlogHeader.BlogId` is not nullable.
- `Blog.BlogHeader` is discovered as a reference navigation.
- `BlogHeader.Blog` is discovered as a reference navigation.

> [!IMPORTANT]
> When using [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types), the navigation from the dependent to the principal must be nullable if the foreign key property is nullable. If the foreign key property is non-nullable, then the navigation may be nullable or not. In this case, `BlogHeader.BlogId` is non-nullable, and `BlogHeader.Blog` is also non-nullable. The `= null!;` construct is used to mark this as intentional for the C# compiler, since EF typically sets the `Blog` instance and it cannot be null for a fully loaded relationship. See [_Working with Nullable Reference Types_](xref:core/miscellaneous/nullable-reference-types) for more information.

For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipal)]

In the example above, configuration of the relationships starts the principal entity type (`Blog`). As with all relationships, it is exactly equivalent to start with dependent entity type (`BlogHeader`) instead. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromDependent](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromDependent)]

Neither of these options is better than the other; they both result in exactly the same configuration.

> [!TIP]
> It is never necessary to configure a relationship twice, once starting from the principal, and then again starting from the dependent. Also, attempting to configure the principal and dependent halves of a relationship separately generally does not work. Choose to configure each relationship from either one end or the other and then write the configuration code only once.

## Optional one-to-one

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }                                   // Optional foreign key property
            public Blog? Blog { get; set; }                                    // Optional reference navigation to principal
        }
-->
[!code-csharp[OneToOneOptional](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneOptional)]

This is the same as the previous example, except that the foreign key property and navigation to the principal are now nullable. This makes the relationship "optional" because a dependent (`BlogHeader`) can _not_ be related to _any_ principal (`Blog`) by setting its foreign key property and navigation to `null`.

> [!IMPORTANT]
> When using [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types), the navigation property from dependent to principal must be nullable if the foreign key property is nullable. In this case, `BlogHeader.BlogId` is nullable, so `BlogHeader.Blog` must be nullable too. See [_Working with Nullable Reference Types_](xref:core/miscellaneous/nullable-reference-types) for more information.

As before, this relationship is [discovered by convention](xref:core/modeling/relationships/conventions). For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
-->
[!code-csharp[OneToOneOptionalFromPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneOptionalFromPrincipal)]

## Required one-to-one with primary key to primary key relationship

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!;                            // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequiredPkToPk](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredPkToPk)]

Unlike with one-to-many relationships, the dependent end of a one-to-one relationship may use its primary key property or properties as the foreign key property or properties. This is often called a PK-to-PK relationship. This is only possible when the principal and dependent types have the same primary key types, and the resulting relationship is always required, since the primary key of the dependent cannot be nullable.

Any one-to-one relationship where the foreign key is not discovered by convention must be configured to indicate the principal and dependent ends of the relationship. This is typically done with a call to `HasForeignKey`. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>();
            }
-->
[!code-csharp[OneToOneRequiredPkToPkConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredPkToPkConfig)]

> [!TIP]
> `HasPrincipalKey` can also used for this purpose, but doing so is less common.

When no property is specified in the call to `HasForeignKey`, and the primary key is suitable, then it is used as the foreign key. For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.Id)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredPkToPkFromPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredPkToPkFromPrincipal)]

## Required one-to-one with shadow foreign key

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<BlogHeader> BlogHeaders { get; } = new List<BlogHeader>();  // Collection navigation containing dependents
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; }                               // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequiredShadow](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadow)]

In some cases, you may not want a foreign key property in your model, since foreign keys are a detail of how the relationship is represented in the database, which is not needed when using the relationship in a purely object-oriented manner. However, if entities are going to be serialized, for example to send over a wire, then the foreign key values can be a useful way to keep the relationship information intact when the entities are not in an object form. It is therefore often pragmatic to keep foreign key properties in the .NET type for this purpose. Foreign key properties can be private, which is often a good compromise to avoid exposing the foreign key while allowing its value to travel with the entity.

Following on from the previous example, this example removes the foreign key property from the dependent entity type. However, instead of using the primary key, EF is instead instructed to create a [shadow foreign key property](xref:core/modeling/shadow-properties) called `BlogId` of type `int`.

An important point to note here is that [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types) are being used, so the nullability of the navigation from dependent to principal is used to determine whether or not the foreign key property is nullable, and therefore whether the relationship is optional or required. If nullable reference types are not being used, then the shadow foreign key property will be nullable by default making the relationship optional by default. In this case, use `IsRequired` to force the shadow foreign key property to be non-nullable and make the relationship required.

This relationship again needs some configuration to indicate the principal and dependent ends:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId");
            }
-->
[!code-csharp[OneToOneRequiredShadowConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadowConfig)]

For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredShadowFromPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadowFromPrincipal)]

## Optional one-to-one with shadow foreign key

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; }                                    // Optional reference navigation to principal
        }
-->
[!code-csharp[OneToOneOptionalShadow](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneOptionalShadow)]

Like the previous example, the foreign key property has been removed from the dependent entity type. However, unlike the previous example, this time the foreign key property is created as nullable because [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types) are being used and the navigation on the dependent entity type is nullable. This makes the relationship optional.

When C# nullable reference types are not being used, then the foreign key property will, by default, be created as nullable. This means relationships with automatically created shadow properties are optional by default.

As before, this relationship needs some configuration to indicate the principal and dependent ends:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId");
            }
-->
[!code-csharp[OneToOneOptionalShadowConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneOptionalShadowConfig)]

For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired(false);
            }
-->
[!code-csharp[OneToOneOptionalShadowFromPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneOptionalShadowFromPrincipal)]

## One-to-one without navigation to principal

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; }                                    // Required foreign key property
        }
-->
[!code-csharp[OneToOneRequiredNoNavigationToPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredNoNavigationToPrincipal)]

For this example, the foreign key property has been re-introduced, but the navigation on the dependent has been removed.

> [!TIP]
> A relationship with only one navigation--one from dependent to principal or one from principal to dependent, but not both--is known as a unidirectional relationship.

This relationship is [discovered by convention](xref:core/modeling/relationships/conventions), since the foreign key is discovered, thereby indicating the dependent side. For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipalNoNavigationToPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipalNoNavigationToPrincipal)]

Notice that the call to `WithOne` has no arguments. This is the way to tell EF that there is no navigation from `BlogHeader` to `Blog`.

If configuration starts from the entity with no navigation, then the type of the entity on the other end of the relationship must be explicitly specified using the generic `HasOne<>()` call. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromDependentNoNavigationToPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromDependentNoNavigationToPrincipal)]

## One-to-one without navigation to principal and with shadow foreign key

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
        }
-->
[!code-csharp[OneToOneRequiredShadowNoNavigationToPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadowNoNavigationToPrincipal)]

This example combines two of the previous examples by removing both the foreign key property and the navigation on the dependent.

As before, this relationship needs some configuration to indicate the principal and dependent ends:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredShadowNoNavigationToPrincipalConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadowNoNavigationToPrincipalConfig)]

A more complete configuration can be used to explicitly configure the navigation and foreign key name, with an appropriate call to `IsRequired()` or `IsRequired(false)` as needed. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.BlogHeaders)
                    .WithOne()
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredShadowFromPrincipalNoNavigationToPrincipal](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredShadowFromPrincipalNoNavigationToPrincipal)]

## One-to-one without navigation to dependent

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; }           // Required foreign key property
            public Blog Blog { get; set; } = null!;   // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequiredNoNavigationToDependents](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredNoNavigationToDependents)]

The previous two examples had navigations from the principal to dependents, but no navigation from the dependent to principal. For the next couple of examples, the navigation on the dependent is re-introduced, while the navigation on the principal is removed instead.

By convention, EF will treat this as a one-to-many relationship. Some minimal configuration is needed to make it one-to-one:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne();
            }
-->
[!code-csharp[OneToOneRequiredNoNavigationToDependentsConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredNoNavigationToDependentsConfig)]

Notice again that `WithOne()` is called with no arguments to indicate that there is no navigation in this direction.

For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromDependentNoNavigationToDependents](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromDependentNoNavigationToDependents)]

If configuration starts from the entity with no navigation, then the type of the entity on the other end of the relationship must be explicitly specified using the generic `HasOne<>()` call. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipalNoNavigationToDependents](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipalNoNavigationToDependents)]

## One-to-one with no navigations

Occasionally, it can be useful to configure a relationship with no navigations. Such a relationship can only be manipulated by changing the foreign key value directly.

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
        }
-->
[!code-csharp[OneToOneRequiredNoNavigations](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredNoNavigations)]

This relationship is not discovered by convention, since there are no navigations indicating that the two types are related. It can be configured explicitly in `OnModelCreating`. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<BlogHeader>()
                    .WithOne();
            }
-->
[!code-csharp[OneToOneRequiredNoNavigationsConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredNoNavigationsConfig)]

With this configuration, the `BlogHeader.BlogId` property is still detected as the foreign key by convention, and the relationship is "required" because the foreign key property is not nullable. The relationship can be made "optional" by making the foreign key property nullable.

A more complete explicit configuration of this relationship is::

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipalNoNavigations](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipalNoNavigations)]

## One-to-one with alternate key

In all the examples so far, the foreign key property on the dependent is constrained to the primary key property on the principal. The foreign key can instead be constrained to a different property, which then becomes an alternate key for the principal entity type. For example:

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; }             // Alternate key as target of the BlogHeader.BlogId foreign key
            public BlogHeader? Header { get; set; }          // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; }                  // Required foreign key property
            public Blog Blog { get; set; } = null!;          // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequiredWithAlternateKey](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredWithAlternateKey)]

This relationship is not discovered by convention, since EF will always, by convention, create a relationship to the primary key. It can be configured explicitly in `OnModelCreating` using a call to `HasPrincipalKey`. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId);
            }
-->
[!code-csharp[OneToOneRequiredWithAlternateKeyConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredWithAlternateKeyConfig)]

`HasPrincipalKey` can be combined with other calls to explicitly configure the navigations, foreign key properties, and required/optional nature. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipalWithAlternateKey](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipalWithAlternateKey)]

## One-to-one with composite foreign key

In all the examples so far, the primary or alternate key property of the principal consisted of a single property. Primary or alternate keys can also be formed form more than one property--these are known as ["composite keys"](xref:core/modeling/keys). When the principal of a relationship has a composite key, then the foreign key of the dependent must also be a composite key with the same number of properties. For example:

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; }              // Composite key part 1
            public int Id2 { get; set; }              // Composite key part 2
            public BlogHeader? Header { get; set; }   // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId1 { get; set; }          // Required foreign key property part 1
            public int BlogId2 { get; set; }          // Required foreign key property part 2
            public Blog Blog { get; set; } = null!;   // Required reference navigation to principal
        }
-->
[!code-csharp[OneToOneRequiredWithCompositeKey](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredWithCompositeKey)]

This relationship is discovered by convention. However, it will only be discovered if the composite key has been configured explicitly, since composite keys are not discovered automatically. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
-->
[!code-csharp[OneToOneRequiredWithCompositeKeyConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredWithCompositeKeyConfig)]

> [!IMPORTANT]
> A composite foreign key value is considered to be `null` if any of its property values are null. A composite foreign key with one property null and another non-null will not be considered a match for a primary or alternate key with the same values. Both will be considered `null`.

Both `HasForeignKey` and `HasPrincipalKey` can be used to explicitly specify keys with multiple properties. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    nestedBuilder =>
                    {
                        nestedBuilder.HasKey(e => new { e.Id1, e.Id2 });

                        nestedBuilder.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                            .HasForeignKey<BlogHeader>(e => new { e.BlogId1, e.BlogId2 })
                            .IsRequired();
                    });
            }
-->
[!code-csharp[OneToOneRequiredFromPrincipalWithCompositeKey](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=OneToOneRequiredFromPrincipalWithCompositeKey)]

> [!TIP]
> In the code above, the calls to `HasKey` and `HasOne` have been grouped together into a nested builder. Nested builders remove the need to call `Entity<>()` multiple times for the same entity type, but are functionally equivalent to calling `Entity<>()` multiple times.

## Required one-to-one without cascade delete

<!--
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; }                            // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; }                                    // Required foreign key property
            public Blog Blog { get; set; } = null!;                            // Required reference navigation to principal
        }
-->
[!code-csharp[RequiredWithoutCascadeDelete](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=RequiredWithoutCascadeDelete)]

By convention, required relationships are configured to [cascade delete](xref:core/saving/cascade-delete). This is because the dependent cannot exist in the database once the principal has been deleted. The database can be configured to generate an error, typically crashing the application, instead of automatically deleting dependent rows that can no longer exist. This requires some configuration:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .OnDelete(DeleteBehavior.Restrict);
            }
-->
[!code-csharp[RequiredWithoutCascadeDeleteConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=RequiredWithoutCascadeDeleteConfig)]

## Self-referencing one-to-one

In all the previous examples, the principal entity type was different from the dependent entity type. This does not have to be the case. For example, in the types below, each `Person` is optionally related to another `Person`.

<!--
        public class Person
        {
            public int Id { get; set; }

            public int? HusbandId { get; set; }      // Optional foreign key property
            public Person? Husband { get; set; }     // Optional reference navigation to principal
            public Person? Wife { get; set; }        // Reference navigation to dependent
        }
-->
[!code-csharp[SelfReferencing](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=SelfReferencing)]

This relationship is [discovered by convention](xref:core/modeling/relationships/conventions). For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasOne(e => e.Husband)
                    .WithOne(e => e.Wife)
                    .HasForeignKey<Person>(e => e.HusbandId)
                    .IsRequired(false);
            }
-->
[!code-csharp[SelfReferencingConfig](../../../../samples/core/Modeling/Relationships/OneToOne.cs?name=SelfReferencingConfig)]

> [!NOTE]
> For one-to-one self referencing relationships, since the principal and dependent entity types are the same, specifying which type contains the foreign key does not clarify the dependent end. In this case, the navigation specified in `HasOne` points from dependent to principal, and the navigation specified in `WithOne` points from principal to dependent.
