---
title: Relationships - EF Core
description: How to configure relationships between entity types when using Entity Framework Core
author: ajcvickers
ms.date: 02/25/2023
uid: core/modeling/relationships
---
# Relationships

A relationship defines how two entities relate to each other. In the EF Core model, a relationship is made up from one or more foreign key properties, together with optional navigation properties ("navigations") that connect the entity types.

## Definition of terms

There are a number of terms used to describe relationships. It is not necessary to understand all these terms up-front. Refer back here as needed when reading the relationships documentation pages.

- **Dependent entity:** This is the entity that contains the foreign key property or properties. A dependent is sometimes called a "child".

- **Principal entity:** This is the entity that contains the primary/alternate key property or properties. A principal is sometimes called the "parent".

- **Principal key:** The property or properties whose values uniquely identify the principal entity. The principal key may be the primary key or an alternate key.

- **Foreign key:** The property or properties of the dependent entity type that are used to store the key values that match the principal key values of the related principal entity.

- **Navigation:** A property on the entity on one side of the relationship that references the related entity or entities at the other end of the relationship.

  - **Collection navigation:** A navigation that contains references to many related entities. Used to reference the "many" side(s) of one-to-many and many-to-many relationships.

  - **Reference navigation:** A navigation that holds a reference to a single related entity. Used to reference the "one" side(s) of one-to-one and one-to-many relationships.

  - **Inverse navigation:** When discussing a particular navigation, this term refers to the navigation on the other end of the relationship.

- **Self-referencing relationship:** A relationship in which the dependent and the principal entity types are the same.

- **Required relationship** A relationship represented by a non-nullable foreign key. A dependent entity in a required relationship cannot exist without a principal entity to which it refers.

- **Optional relationship** A relationship represented by a nullable foreign key. A dependent entity in an optional relationship can exist without referring to any principal entity.

- **Bidirectional relationship** A relationship that has navigations on both sides of the relationship.

- **Unidirectional relationship** A relationship that has a navigation on one side of the relationship, but no navigation on the other side.

## Cardinality

EF supports three basic forms of relationship between entities:

- In [one-to-many](xref:core/modeling/relationships/one-to-many) relationships, a single entity is associated with any number of other entities. For example, a `Blog` can have many associated `Posts`, but each `Post` is associated with only one `Blog`.
- In [one-to-one](xref:core/modeling/relationships/one-to-one) relationships, a single entity is associated with another single entity. For example, a `Blog` has one `BlogHeader`, and that `BlogHeader` belongs to a single `Blog`.
- In [many-to-many](xref:core/modeling/relationships/many-to-many) relationships, any number of entities are associated with any number of other entities. For example, a `Post` can have many associated `Tags`, and each `Tag` can in turn be associated with many `Posts`.

See the linked documentation pages for more details and examples of each of these different kinds of relationships.

### Optional and required relationships

For one-to-many and one-to-one relationships, the relationship can be either "optional" or "required". In required relationships, for the dependent(s) (child/children) to exist, the principal (parent) of the relationship _must_ exist. In optional relationships, the dependent(s) (child/children) may exist without any principal (parent). This makes optional relationships equivalent to "0..1 to 1" or "0..1 to many", although they are not referred to as such in the EF Core APIs or documentation.

> [!NOTE]
> EF does not, in general, support required dependents, where the principal entity cannot exist without its dependents. See [_Required navigations_](xref:core/modeling/relationships/navigations#required-navigations) for more information.  

## Foreign keys and navigations

At the most basic level, EF relationships are defined by foreign keys on a dependent entity type that reference primary or alternate keys on a principal entity type. Examples of how foreign keys are used can be found in the documentation for [one-to-many](xref:core/modeling/relationships/one-to-many), [one-to-one](xref:core/modeling/relationships/one-to-one), and [many-to-many](xref:core/modeling/relationships/many-to-many) relationships. [_Foreign principal keys in relationships_](xref:core/modeling/relationships/foreign-and-principal-keys) covers more specific information about how foreign keys map to the database.

Navigations are layered over a foreign key to provide an object-oriented view of the relationship. Again, there are many examples of navigations in the documentation for the different relationship types. See also [_Relationship navigations_](xref:core/modeling/relationships/navigations) for more information specific to navigations.

## Relationship configuration

EF models are built using a combination of three mechanisms: conventions, mapping attributes, and the model builder API. Model building always starts with [conventions](xref:core/modeling/relationships/conventions), which discover entity types, their properties, and the relationships between the types. The behavior of these conventions can be modified or overridden using [mapping attributes](xref:core/modeling/relationships/mapping-attributes), or using the [model building API](xref:core/modeling/index) in `OnModelCreating`.

The model-building API is the final source of truth for the EF model--it will always take precedence over configuration discovered by convention or specified by mapping attributes. It is also the only mechanism with full fidelity to configure every aspect of the EF model. Many examples of using the model building API are shown in the documentation for [one-to-many](xref:core/modeling/relationships/one-to-many), [one-to-one](xref:core/modeling/relationships/one-to-one), and [many-to-many](xref:core/modeling/relationships/many-to-many) relationships, and when discussing [foreign and principal keys](xref:core/modeling/relationships/foreign-and-principal-keys).

## Cascade deletes and deleting orphans

Cascading deletes and automatic deletion of orphans are configured by convention for required relationships. Examples showing how to change the cascading behavior are included in the documentation for [one-to-many](xref:core/modeling/relationships/one-to-many), [one-to-one](xref:core/modeling/relationships/one-to-one), and [many-to-many](xref:core/modeling/relationships/many-to-many) relationships.

See [_Cascade Delete_](xref:core/saving/cascade-delete) for more information on how cascading behaviors work in `SaveChanges` and `SaveChangesAsync`.

## Owned entity types

Aggregates of entity types cane be defined using a special type of "owning" relationship that implies a stronger connection between the two types than the "normal" relationships discussed here. Many of the concepts described here for normal relationships are carried over to owned relationships. However, owned relationships also have their own special behaviors, which are covered in the [owned entity types](xref:core/modeling/owned-entities) documentation.

## Relationships across context boundaries

Relationships are defined in the EF model between entity types included in that model. Some relationships may need to reference an entity type in the model of a different context--for example, when using the [BoundedContext pattern](https://www.martinfowler.com/bliki/BoundedContext.html). In these situation, the foreign key column(s) should be mapped to normal properties, and these properties can then be manipulated manually to handle changes to the relationship.

## Using relationships

Relationships defined in the model can be used in various ways. For example:

- Relationships can be used to [query related data](xref:core/querying/related-data) in any of three ways:
  - [Eagerly](xref:core/querying/related-data/eager) as part of a LINQ query, using `Include`.
  - [Lazily](xref:core/querying/related-data/lazy) using lazy-loading proxies, or lazy-loading without proxies.
  - [Explicitly](xref:core/querying/related-data/explicit) using the `Load` or `LoadAsync` methods.
- Relationships can be used in [data seeding](xref:core/modeling/data-seeding).
- Relationships can be used to [track graphs of entities](xref:core/change-tracking/index). Relationships are then used by the change tracker to:
  - [Detect changes in relationships and perform fixup](xref:core/change-tracking/relationship-changes)
  - [Send foreign key updates to the database](xref:core/saving/related-data) with `SaveChanges` or `SaveChangesAsync`
