---
title: Mapping attributes (aka Data Annotations) for relationships - EF Core
description: Using mapping attributes (also know as Data Annotations) to configure Entity Framework Core relationships
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships/mapping-attributes
---
# Mapping attributes (aka data annotations) for relationships

Mapping attributes are used to modify or override the configuration discovered by [model building conventions](xref:core/modeling/relationships/conventions). The configuration performed by mapping attributes can itself be overridden by [the model building API used in `OnModelCreating`](xref:core/modeling/index).

> [!IMPORTANT]
> This document only covers mapping attributes in the context of relationship configuration. Other uses of mapping attributes are covered in the relevant sections of the wider [modeling documentation](xref:core/modeling/index).

> [!TIP]
> The code below can be found in [MappingAttributes.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/Relationships/MappingAttributes.cs).

## Where to get mapping attributes

Many mapping attributes come from the [System.ComponentModel.DataAnnotations](/dotnet/api/system.componentmodel.dataannotations) and [System.ComponentModel.DataAnnotations.Schema](/dotnet/api/system.componentmodel.dataannotations.schema) namespaces. The attributes in these namespaces are included as part of the base framework in all supported versions of .NET, and so do not require the installation of any additional NuGet packages. These mapping attributes are commonly called "data annotations" and are used by a variety of frameworks, including EF Core, EF6, ASP.NET Core MVC, and so on. They are also used for validation.

The use of data annotations across many technologies and for both mapping and validation has led to differences in semantics across technologies. All new mapping attributes designed for EF Core are now specific to EF Core, thereby keeping their semantics and use simple and clear. These attributes are contained in the [Microsoft.EntityFrameworkCore.Abstractions](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Abstractions/) NuGet package. This package is included as a dependency whenever the main [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/) package, or one of the associated database provider packages, is used. However, the Abstractions package is a lightweight package that can be referenced directly by application code without bringing in all of EF Core and its dependencies.

## RequiredAttribute

<xref:System.ComponentModel.DataAnnotations.RequiredAttribute> is applied to a property to indicate that the property cannot be `null`. In the context of relationships, `[Required]` is usually used on a foreign key property. Doing so makes the foreign key not nullable, thereby making the relationship required. For example, with the following types, the `Post.BlogId` property is made non-nullable, and the relationship becomes required.

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [Required]
            public string BlogId { get; set; }

            public Blog Blog { get; init; }
        }
-->
[!code-csharp[RequiredOnForeignKey](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=RequiredOnForeignKey)]

> [!NOTE]
> When using [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types), the `BlogId` property in this example is already non-nullable, which means the `[Required]` attribute will have no affect.

`[Required]` placed on the dependent navigation has the same effect. That is, making the foreign key non-nullable, and thereby making the relationship required. For example:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            public string BlogId { get; set; }

            [Required]
            public Blog Blog { get; init; }
        }
-->
[!code-csharp[RequiredOnDependentNavigation](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=RequiredOnDependentNavigation)]

If `[Required]` is found on the dependent navigation and the foreign key property is in shadow state, then shadow property is made non-nullable, thereby making the relationship required. For example:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [Required]
            public Blog Blog { get; init; }
        }
-->
[!code-csharp[RequiredOnDependentNavigationShadowFk](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=RequiredOnDependentNavigationShadowFk)]

> [!NOTE]
> Using `[Required]` on the principal navigation side of a relationship has no effect.

## ForeignKeyAttribute

<xref:System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute> is used to connect a foreign key property with its navigations. `[ForeignKey]` can be placed on the foreign key property with the name of the dependent navigation. For example:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [ForeignKey(nameof(Blog))]
            public string BlogKey { get; set; }

            public Blog Blog { get; init; }
        }

-->
[!code-csharp[ForeignKeyOnProperty](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=ForeignKeyOnProperty)]

Or, `[ForeignKey]` can be placed on either the dependent or principal navigation with the name of the property to use as the foreign key. For example:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            public string BlogKey { get; set; }

            [ForeignKey(nameof(BlogKey))]
            public Blog Blog { get; init; }
        }

-->
[!code-csharp[ForeignKeyOnDependentNavigation](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=ForeignKeyOnDependentNavigation)]

When `[ForeignKey]` is placed on a navigation and the name provided does not match any property name, then a [shadow property](xref:core/modeling/shadow-properties) with that name will be created to act as the foreign key. For example:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [ForeignKey("BlogKey")]
            public Blog Blog { get; init; }
        }
-->
[!code-csharp[ForeignKeyOnDependentNavigationShadowFk](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=ForeignKeyOnDependentNavigationShadowFk)]

## InversePropertyAttribute

<xref:System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute> is used to connect a navigation with its inverse. For example, in the following entity types, there are two relationships between `Blog` and `Post`. Without any configuration, [EF conventions](xref:core/modeling/relationships/conventions) cannot determine which navigations between the two types should be paired. Adding `[InverseProperty]` to one of the paired navigations resolves this ambiguity and allows EF to build the model.

<!--
        public class Blog
        {
            public int Id { get; set; }

            [InverseProperty("Blog")]
            public List<Post> Posts { get; } = new();

            public int FeaturedPostId { get; set; }
            public Post FeaturedPost { get; set; }
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            public Blog Blog { get; init; }
        }
-->
[!code-csharp[InverseOnPrincipalNavigation](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=InverseOnPrincipalNavigation)]

> [!IMPORTANT]
> `[InverseProperty]` is only needed when there is more than one relationship between the same types. With a single relationship, the two navigations are paired automatically.

## DeleteBehaviorAttribute

[By convention](xref:core/modeling/relationships/conventions), EF uses the `ClientSetNull` <xref:Microsoft.EntityFrameworkCore.DeleteBehavior> for optional relationships, and the `Cascade` behavior for required relationships. This can be changed by placing the <xref:Microsoft.EntityFrameworkCore.DeleteBehaviorAttribute> on one of the navigations of the relationship. For example:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            [DeleteBehavior(DeleteBehavior.Restrict)]
            public Blog Blog { get; init; }
        }
-->
[!code-csharp[DeleteBehaviorOnDependentNavigation](../../../../samples/core/Modeling/Relationships/MappingAttributes.cs?name=DeleteBehaviorOnDependentNavigation)]

See [_Cascade delete_](xref:core/saving/cascade-delete) for more information on cascading behaviors.
