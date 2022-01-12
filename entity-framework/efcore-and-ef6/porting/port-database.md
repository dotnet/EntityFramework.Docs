---
title: Port from EF6 to EF Core - Database as Source of Truth
description: How to port from EF6 to EF Core when you generate your model from the database.
author: jeremylikness
ms.alias: jeliknes
ms.date: 12/09/2021
uid: efcore-and-ef6/porting/port-database
---

# Port from EF6 to EF Core - Database as Source of Truth

If you're using the database as the source of truth, the upgrade will mostly involve addressing any changes to the shape of entities generated. The steps to migrate include:

1. Pick a point-in-time to model the database.
1. Ensure your EF6 projects are up to date and in-sync with the database.
1. Create the EF Core project.
1. Use the [scaffolding tools](xref:core/managing-schemas/scaffolding) to reverse-engineer your database to code.
1. Validate that the EF Core generated classes are compatible with your code.
1. For exceptions, either modify the generated classes and update the [model configuration](xref:core/modeling/index) or adapt your code to the model.

Note that although EF Core currently scaffolds everything needed to successfully generate a copy of the database, much of the code is not needed for the database-first approach. A fix for this is tracked in [Issue #10890](https://github.com/dotnet/efcore/issues/10890). Things that you can safely ignore as not needed include: sequences, constraint names, non-unique indexes and index filters.

## Handling schema changes

When your database is the source of truth, EF Core pulls schema information from the database rather than pushing it via migrations. The typical workflow is to re-run the reverse-engineering step whenever the database schema changes. A comprehensive test suite is valuable for this approach because you can automate the scaffolding process and verify the changes by running your tests.

## Tips for handling model differences

For various reasons, you may want your C# domain model to be shaped differently from the one generated in reverse engineering. In many cases, this means manually updating the code that is auto-generated after every schema change. One way to prevent extra effort when the code is regenerated is to use partial classes for your DbContext and related entities. Then you can keep code related to business logic and properties not tracked in the database in a separate set of class files that won't get overwritten.

 If your model is significantly different from the generated one, but doesn't change frequently, one option to consider is use of the [repository pattern](/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) as an adapter. The repository can consume the EF Core generated classes and publish the custom classes you use. This may reduce the impact of changes by isolating them to the repository code, rather than having to perform an application-wide refactoring each time the schema changes.

You may want to consider an alternate workflow and follow steps similar to the [hybrid approach](xref:efcore-and-ef6/porting/port-hybrid). Instead of generating a new set of classes each time, you indicate specific tables to only generate new classes. You keep existing classes "as is" and directly add or remove properties that changed. You then update the model configuration to address any changes in how the database maps to your existing classes.

## Customize the code generation

EF Core 6 currently does not support customization of the generated code. There are third party solutions like [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools/wiki) that are available. For a list of featured community tools and extensions, see: [EF Core Tools and Extensions](xref:core/extensions/index).

Finally, review the [detailed list of differences between EF6 and EF Core](xref:efcore-and-ef6/porting/port-detailed-cases) to address any remaining issues with porting.
