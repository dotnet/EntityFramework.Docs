---
title: "Entity Framework Glossary | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: 3f05ffdd-49bc-499c-9732-4a368bf5d2d7
caps.latest.revision: 3
---
# Entity Framework Glossary
| Term | Definition |
| ---- | ---------- |
| Code First | Creating an Entity Framework model using code. The model can target and existing database or a new database. |
| Context | A class that represents a session with the database, allowing you to query and save data. A context derives from the DbContext or ObjectContext class. |
| Convention (Code First)	| A rule that Entity Framework uses to infer the shape of you model from your classes. |
| Database First | Creating an Entity Framework model, using the EF Designer, that targets an existing database. |
| Eager loading | A pattern of loading related data where a query for one type of entity also loads related entities as part of the query. |
| EF Designer | A visual designer in Visual Studio that allows you to create an Entity Framework model using boxes and lines. |
| Entity | A class or object that represents application data such as customers, products, and orders. |
| Entity Data Model | A model that describes entities and the relationships between them. |
| Explicit loading | A pattern of loading related data where related objects are loaded by calling an API. |
| Fluent API | An API that can be used to configure a Code First model. |
| Foreign key association | An association between entities where a property that represents the foreign key is included in the class of the dependent entity (i.e. the Product contains a CategoryId property). |
| Identifying relationship | A relationship where the primary key of the principal entity is part of the primary key of the dependent entity. In this kind of relationship, the dependent entity cannot exist without the principal entity. |
| Independent association | An association between entities where there is no property representing the foreign key in the class of the dependent entity (i.e. a Product class contains a relationship to Category but no CategoryId property). Entity Framework will use an independent object to track this relationship. |
| Lazy loading | A pattern of loading related data where related objects are automatically loaded when a navigation property is accessed. |
| Model First | 	Creating an Entity Framework model, using the EF Designer, that is then used to create a new database. |
| Navigation property | A property of an entity that references another entity (i.e. Product contains a Category navigation property and Category contains a Products navigation property). |
| Relationship inverse | The opposite end of a relationship, for example, product.Category and category.Product. |
| Self-tracking entity | An entity built from a code generation template that helps with N-Tier development. |
| Table-per-concrete type (TPC)	| A method of mapping the inheritance where each non-abstract type in the hierarchy is mapped to separate table in the database. |
| Table-per-hierarchy (TPH) | A method of mapping the inheritance where all types in the hierarchy are mapped to the same table in the database. A discriminator column(s) is used to identify what type each row is associated with. |
| Table-per-type (TPT) | A method of mapping the inheritance where the common properties of all types in the hierarchy are mapped to the same table in the database, but properties unique to each type are mapped to a separate table. |
| Type discovery | The process of identifying the types that should be part of an Entity Framework model. |
  