---
title: Entity Framework Glossary - EF6
description: Entity Framework 6 Glossary
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/resources/glossary
---
# Entity Framework Glossary
## Code First
Creating an Entity Framework model using code. The model can target an existing database or a new database.

## Context
A class that represents a session with the database, allowing you to query and save data. A context derives from the DbContext or ObjectContext class.

## Convention (Code First)
A rule that Entity Framework uses to infer the shape of you model from your classes.

## Database First
Creating an Entity Framework model, using the EF Designer, that targets an existing database.

## Eager loading
A pattern of loading related data where a query for one type of entity also loads related entities as part of the query.

## EF Designer
A visual designer in Visual Studio that allows you to create an Entity Framework model using boxes and lines.

## Entity
A class or object that represents application data such as customers, products, and orders.

## Entity Data Model
A model that describes entities and the relationships between them. EF uses EDM to describe the conceptual model against which the developer programs. EDM builds on the Entity Relationship model introduced by Dr. Peter Chen. The EDM was originally developed with the primary goal of becoming the common data model across a suite of developer and server technologies from Microsoft. EDM is also used as part of the OData protocol.

## Explicit loading
A pattern of loading related data where related objects are loaded by calling an API.

## Fluent API
An API that can be used to configure a Code First model.

## Foreign key association
An association between entities where a property that represents the foreign key is included in the class of the dependent entity. For example, Product contains a CategoryId property.

## Identifying relationship
A relationship where the primary key of the principal entity is part of the primary key of the dependent entity. In this kind of relationship, the dependent entity cannot exist without the principal entity.

## Independent association
An association between entities where there is no property representing the foreign key in the class of the dependent entity. For example, a Product class contains a relationship to Category but no CategoryId property. Entity Framework tracks the state of the association independently of the state of the entities at the two association ends.

## Lazy loading
A pattern of loading related data where related objects are automatically loaded when a navigation property is accessed.

## Model First
Creating an Entity Framework model, using the EF Designer, that is then used to create a new database.

## Navigation property
A property of an entity that references another entity. For example, Product contains a Category navigation property and Category contains a Products navigation property.

## POCO
Acronym for Plain-Old CLR Object. A simple user class that has no dependencies with any framework. In the context of EF, an entity class that does not derive from EntityObject, implements any interfaces or carries any attributes defined in EF. Such entity classes that are decoupled from the persistence framework are also said to be "persistence ignorant".  

## Relationship inverse
The opposite end of a relationship, for example, product.Category and category.Product.

## Self-tracking entity
An entity built from a code generation template that helps with N-Tier development.

## Table-per-concrete type (TPC)
A method of mapping the inheritance where each non-abstract type in the hierarchy is mapped to separate table in the database.

## Table-per-hierarchy (TPH)
A method of mapping the inheritance where all types in the hierarchy are mapped to the same table in the database. A discriminator column(s) is used to identify what type each row is associated with.

## Table-per-type (TPT)
A method of mapping the inheritance where the common properties of all types in the hierarchy are mapped to the same table in the database, but properties unique to each type are mapped to a separate table.

## Type discovery
The process of identifying the types that should be part of an Entity Framework model.
