---
title: Global Query Filters - EF Core
author: anpete
ms.author: anpete
ms.date: 11/03/2017
ms.technology: entity-framework-core
uid: core/querying/filters
---
# Global Query Filters

Global query filters are LINQ query predicates (a boolean expression typically passed to the LINQ *Where* query operator) applied to Entity Types in the metadata model (usually in *OnModelCreating*). Such filters are automatically applied to any LINQ queries involving those Entity Types, including Entity Types referenced indirectly, such as through the use of Include or direct navigation property references. Some common applications of this feature are:

* **Soft delete** - An Entity Type defines an *IsDeleted* property.
* **Multi-tenancy** - An Entity Type defines a *TenantId* property.

## Example

The following example shows how to use Global Query Filters to implement soft-delete and multi-tenancy query behaviors in a simple blogging model.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFrameworkCore/tree/dev/samples/QueryFilters) on GitHub.

First, define the entities:

[!code-csharp[Main](../../../efcore-dev/samples/QueryFilters/Program.cs#Entities)]

Note the declaration of a __tenantId_ field on the _Blog_ entity. This will be used to associate each Blog instance with a specific tenant. Also defined is an _IsDeleted_ property on the _Post_ entity type. This is used this to keep track of whether a _Post_ instance has been "soft-deleted". I.e. The instance is marked as deleted withouth physically removing the underlying data.

Next, configure the query filters in _OnModelCreating_ using the ```HasQueryFilter``` API.

[!code-csharp[Main](../../../efcore-dev/samples/QueryFilters/Program.cs#Configuration)]

The predicate expressions passed to the _HasQueryFilter_ calls will now automatically be applied to any LINQ queries for those types.

> [!TIP]
> Note the use of a DbContext instance level field: ```_tenantId``` used to set the current tenant. Model-level filters will use the value from the correct context instance. I.e. The instance that is executing the query.

## Disabling Filters

Filters may be disabled for individual LINQ queries by using the ```IgnoreQueryFilters()``` operator.

[!code-csharp[Main](../../../efcore-dev/samples/QueryFilters/Program.cs#IgnoreFilters)]

## Limitations

Global query filters have the following limitations:

* Filters cannot contain references to navigation properties.
* Filters can only be defined for the root Entity Type of an inheritance hierarchy.
