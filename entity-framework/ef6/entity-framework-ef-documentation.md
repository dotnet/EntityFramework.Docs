---
title: "Entity Framework (EF) Documentation - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 66ce9113-81d2-480f-8c16-d00ec405b2f7
caps.latest.revision: 3
---
# Entity Framework (EF) Documentation
This page provides an index of videos, tutorials, and advanced documentation to help you make the most of Entity Framework. The following sections are included on this page.

## Get Started

[Which workflow should I use?](../ef6/entity-framework-development-workflows-video.md)  
Entity Framework allows you to create a model by writing code or using boxes and lines in the EF Designer. Both of these approaches can be used to target an existing database or create a new database. This short video explains the differences and how to find the one that is right for you.

### I just want to write code...

[I am creating a new database](../ef6/entity-framework-code-first-to-a-new-database.md)  
Use **Code First** to define your model in code and then generate a database.

[I need to access an existing database](../ef6/entity-framework-code-first-to-an-existing-database.md)  
Use **Code First** to create a code based model that maps to an existing database.

### I want to use a designer...

[I am creating a new database](../ef6/entity-framework-model-first.md)  
Use **Model First** to define your model using boxes and lines and then generate a database.

[I need to access an existing database](../ef6/entity-framework-database-first.md)  
Use **Database First** to create a boxes and lines model that maps to an existing database.


## Entity Framework Versions

This page provides information on the latest version of Entity Framework, although much of it also applies to past versions. Check out the [Version History](../ef6/entity-framework-version-history.md) for a complete list of versions and the features they introduced. The [Past Versions](../ef6/past-versions-of-entity-framework.md) page contains a snapshot of the documentation relating to each previous version of Entity Framework.

## API Documentation

This page provides a series of articles, walkthroughs and other detailed content to help you achieve specific tasks. For detailed API documentation for the types included in Entity Framework, see the [Entity Framework API Documentation](https://msdn.microsoft.com/library/dn223258.aspx).

## Entity Framework 6.x

See the [Get Entity Framework](../ef6/get-entity-framework.md) page for information on how to install EF6. See the [Version History](../ef6/entity-framework-version-history.md) page for a list of new features in these releases.

These pages will help you move existing applications to EF6.

- [Upgrading to EF6](../ef6/upgrading-to-entity-framework-6.md)  
    A checklist of the things you need to do to upgrade an existing application to EF6.
- [Database Provider with EF6 Support](../ef6/entity-framework-providers-for-ef6.md)  
    Find out which database providers currently support EF6.

## Learn More About Setting Up Your Model

[Which Workflow Should I Use](../ef6/entity-framework-development-workflows-video.md)  
Find out about the EF Designer and Code First and which one is best for you.

[Connections and Models](../ef6/entity-framework-connections-and-models.md)  
Learn how EF works out which database to connect to and how to calculate the model.

[Connection Resiliency / Retry Logic (EF6 onwards)](../ef6/entity-framework-connection-resiliency-and-retry-logic-ef6-onwards.md)  
Automatically retry any commands that fail due to connection breaks.

[Handling of Transaction Commit Failures](../ef6/entity-framework-handling-of-transaction-commit-failures-ef6-1-onwards.md)  
Automatically recover from network errors when committing a transaction.

[Performance Considerations](../ef6/performance-considerations-for-ef-4-5-and-6.md)  
Find out how to get the best performance out of Entity Framework.

[Improving Startup Performance with NGen (EF6 Onwards)](../ef6/entity-framework-improving-startup-performance-with-ngen-ef6-onwards.md)  
Improve application warm up time by pre-generating native images for the EF assemblies.

[Pre-Generated Mapping Views](../ef6/entity-framework-pre-generated-mapping-views.md)  
Improve application warm-up time with pre-generated mapping views.

[Working with Microsoft SQL Azure](../ef6/entity-framework-windows-sql-azure.md)  
There are a number of characteristics of SQL Azure that need to be taken into account when using EF.

[Entity Framework Power Tools](../ef6/entity-framework-power-tools.md)  
The EF Power Tools provide a preview of features that are being considered for the main EF tooling.

[Code-Based Configuration (EF6 onwards)](../ef6/entity-framework-code-based-configuration-ef6-onwards.md)  
Code-based configuration is achieved by creating a subclass of DbConfiguration.

[Configuration File Settings](../ef6/entity-framework-config-file-settings.md)  
EF allows a number of settings to be configured from your applications configuration file.

[Dependency Resolution (EF6 Onwards)](../ef6/entity-framework-idbdependencyresolver-services-ef6-onwards.md)  
Lower level building blocks that enable the [Code-Based Configuration](../ef6/entity-framework-code-based-configuration-ef6-onwards.md) feature.

[Glossary](../ef6/entity-framework-glossary.md)  
Definition of terms that are commonly used when talking about Entity Framework.

### Creating a Model with Code First

These topics are specific to models created using EF Code First.

[Code First to a New Database](../ef6/entity-framework-code-first-to-a-new-database.md)  
Use Code First to define your model in code and then generate a database.

[Code First to an Existing Database](../ef6/entity-framework-code-first-to-an-existing-database.md)  
Use Code First to create a code based model that maps to an existing database.

[Customizing Code First to an Existing Database](../ef6/customizing-code-first-to-an-existing-database.md)  
Customize the code that gets scaffolded by the Code First to Existing Database wizard.

[Conventions](../ef6/entity-framework-code-first-conventions.md)  
Find out about the conventions Code First uses to build your model.

[Custom Code First Conventions (EF6 onwards)](../ef6/entity-framework-custom-code-first-conventions-ef6-onwards.md)  
Write your own conventions to help avoid repetitive configuration. For more advanced scenarios you can use Model-Based Conventions.

[Data Annotations](../ef6/entity-framework-code-first-data-annotations.md)  
Data Annotations provide a simple way to configure your model by applying attributes to your classes.

[Fluent API - Configuring/Mapping Properties & Types](../ef6/entity-framework-fluent-api-configuring-and-mapping-properties-and-types.md)  
Learn how to configure properties/types and the columns/tables they map to using the Fluent API.

[Fluent API - Configuring Relationships](../ef6/entity-framework-fluent-api-relationships.md)  
Find out how to configure relationships and the foreign key constraints they map to using the Fluent API.

[Fluent API with VB.NET](../ef6/entity-framework-fluent-api-with-vb-net.md)  
This walkthrough shows how to use the fluent API in VB.NET projects.

[Code First Insert/Update/Delete Stored Procedures (EF6 onwards)](../ef6/entity-framework-code-first-insert-update-and-delete-stored-procedures.md)  
Configure EF to use stored procedures to update data.

[Enum Support (EF5 Onwards)](../ef6/entity-framework-enum-support-code-first-ef5-onwards.md)  
The domain classes that make up your Code First model can contain enum properties.

[Spatial Data Types (EF5 Onwards)](../ef6/entity-framework-spatial-code-first-ef5-onwards.md)  
The DbGeography and DbGeometry types can be used in your model. Also see the information about provider support for spatial types.

[Code First Migrations](../ef6/entity-framework-code-first-migrations.md)  
Learn more about upgrading, downgrading and creating SQL scripts with Code First Migrations.

[Code First Migrations in Team Environments](../ef6/entity-framework-code-first-migrations-in-team-environments.md)  
Learn how to successfully use migrations when working in a team of developers.

[Code First Migrations with an Existing Database](../ef6/entity-framework-code-first-migrations-with-an-existing-database.md)  
Use Migrations with an existing database, one that wasn't created by Entity Framework.

[Automatic Code First Migrations](../ef6/entity-framework-automatic-code-first-migrations.md)  
Automatic migrations allow you to upgrade your database without code-based migrations in your project.

[Customizing the Migrations History Table (Code First only)](../ef6/entity-framework-customizing-the-migrations-history-table-ef6-onwards.md)  
Customize the definition of the __MigrationHistory table.

[Custom Migrations Operations (EF6 onwards)](http://romiller.com/2013/02/27/ef6-writing-your-own-code-first-migration-operations/)  
Create additional operations to be used in your code-based migrations.

[Migrate.exe](../ef6/entity-framework-migrate-exe.md)  
Use migrate.exe to apply migrations to a database from a command line.

[Defining DbSets](../ef6/entity-framework-defining-dbsets.md)  
Discover the various options for defining DbSets on your derived context.

### Creating a Model with the EF Designer

These topics are specific to models created using the EF Designer.

[EF Designer to a New Database (Model First)](../ef6/entity-framework-model-first.md)  
Use Model First to define your model using boxes and lines and then generate a database.

[EF Designer to an Existing Database (Database First)](../ef6/entity-framework-database-first.md)  
Use Database First to create a boxes and lines model that maps to an existing database.

[Complex Types](../ef6/entity-framework-complex-types-ef-designer.md)  
Find out how to group properties on your entities into complex types.

[Associations/Relationships](../ef6/entity-framework-relationships-ef-designer.md)  
Learn how to configure relationships in your model.

[Enum Support (EF5 Onwards)](../ef6/entity-framework-enum-support-ef-designer-ef5-onwards.md)  
Using the EF Designer you can now add enum properties to your entities.

[Spatial Data Types (EF5 Onwards)](../ef6/entity-framework-spatial-ef-designer-ef5-onwards.md)  
The DbGeography and DbGeometry types can be used in your model. Also see the information about provider support for spatial types.

[TPT Inheritance Pattern](../ef6/entity-framework-designer-tpt-inheritance.md)  
Learn how to implement the Table-per-Type (TPT) inheritance pattern in your model.

[TPH Inheritance Pattern](../ef6/entity-framework-designer-tph-inheritance.md)  
Learn how to implement the Table-per-Hierarchy (TPH) inheritance pattern in your model.

[Query with Stored Procedures](../ef6/entity-framework-designer-query-sprocs.md)  
Use stored procedures to load data from the database.

[Stored Procedures with Multiple Result Sets](../ef6/entity-framework-sprocs-with-multiple-result-sets.md)  
Use stored procedures with multiple result sets to load data from the database.

[Insert, Update & Delete with Stored Procedures](../ef6/entity-framework-designer-cud-sprocs.md)  
Use stored procedures to insert, update and delete data.

[Map an Entity to Multiple Tables (Entity Splitting)](../ef6/entity-framework-designer-entity-splitting.md)  
Learn how to map the properties of an entity to columns in multiple tables.

[Map Multiple Entities to One Table (Table Splitting)](../ef6/entity-framework-designer-table-splitting.md)  
Learn how to map the columns of a table to properties in multiple entities.

[Table-Valued Functions (EF5 onwards)](../ef6/entity-framework-table-valued-functions-tvfs-ef5-onwards.md)  
Table-valued functions (TVFs) in your database can be used with models created using the EF Designer.

[Multiple Diagrams per Model (EF5 onwards)](../ef6/entity-framework-multiple-diagrams-per-model-ef5-onwards.md)  
The EF Designer allows you to have several diagrams that visualize subsections of your overall model.

[Defining Queries](../ef6/entity-framework-defining-query-ef-designer.md)  
A defining query is like a view that is defined in your model, rather than the database.

[Code Generation Templates](../ef6/entity-framework-designer-code-generation-templates.md)  
Find out how to customize the code that is generated from your model.

[Reverting to ObjectContext](../ef6/reverting-to-objectcontext-in-entity-framework-designer.md)  
New models created in VS2012 generate code that uses DbContext, but you can revert to ObjectContext.

[EDMX Files](../ef6/entity-framework-edmx.md)  
Covers properties of EDMX files and specification of the xml format (including [CSDL](../ef6/entity-framework-csdl-specification.md), [SSDL](../ef6/entity-framework-ssdl-specification.md) & [MSL](../ef6/entity-framework-msl-specification.md)).

[Keyboard Shortcuts](../ef6/entity-framework-tools-keyboard-shortcuts.md)  
A helpful list of keyboard shortcuts in Entity Framework Tools for Visual Studio.

## Learn More About Using Your Model

[Working with DbContext](../ef6/entity-framework-working-with-dbcontext.md)  
Guidance on how to manage instances of your context class.

[Querying/Finding Entities](../ef6/entity-framework-querying-and-finding-entities.md)  
Learn how to retrieve data from the database using LINQ and the Find method.

[Async Query & Save (EF6 onwards)](../ef6/entity-framework-async-query-and-save-ef6-onwards.md)  
Asynchronous query and save using the async and await keywords.

[Logging and Intercepting Database Operations (EF6 onwards)](../ef6/entity-framework-logging-and-intercepting-database-operations-ef6-onwards.md)  
Anytime EF sends a command to the database this command can be intercepted by application code.

[Working with Relationships](../ef6/entity-framework-relationships-and-navigation-properties.md)  
Find out how to access and manipulate data using relationships.

[Loading Related Entities](../ef6/entity-framework-loading-related-entities.md)  
EF supports the eager, lazy and explicit loading patterns for loading related data.

[Working with Local Data](../ef6/entity-framework-local-data.md)  
Access your in-memory entity instances and the additional information EF is tracking about them.

[N-Tier Applications](../ef6/entity-framework-n-tier.md)  
Learn how to use Entity Framework to build N-Tier applications, including [Self-Tracking Entities](../ef6/entity-framework-self-tracking-entities.md).

[Raw SQL Queries](../ef6/entity-framework-raw-sql-queries.md)  
Find out how to load data from a raw SQL query directly against the database.

[Validation](../ef6/entity-framework-validation.md)  
Discover how Entity Framework can provide server side validation of entities.

[Optimistic Concurrency Patterns](../ef6/entity-framework-optimistic-concurrency-patterns.md)  
Learn about the various strategies for dealing with concurrency exceptions in EF.

[Working with Proxies](../ef6/entity-framework-working-with-proxies.md)  
Proxies derive from your entities and override virtual properties to enable features such as lazy loading.

[Automatic Detect Changes](../ef6/entity-framework-automatic-detect-changes.md)  
Find out what detect changes is and when you may want to disable automatic detect changes.

[No-Tracking Queries](../ef6/entity-framework-no-tracking-queries.md)  
No-tracking allows you to query for entities without having the results be tracked by the context.

[The Load Method](../ef6/entity-framework-the-load-method.md)  
Load entities from the database into the context without immediately doing anything with those entities.

[Add/Attach and Entity States](../ef6/entity-framework-add-and-attach-and-entity-states.md)  
Get familiar with adding and attaching entities and setting entity states in disconnected/N-Tier scenarios.

[Working with Property Values](../ef6/entity-framework-working-with-property-values.md)  
Find out how to access the current, original and database values for your entity instances.

[Testing with a Mocking Framework (EF6 onwards)](../ef6/entity-framework-testing-with-a-mocking-framework-ef6-onwards.md)  
Create test doubles using a mocking framework (such as Moq).

[Testing with Your Own Test Doubles (EF6 onwards)](../ef6/entity-framework-testing-with-your-own-test-doubles-ef6-onwards.md)  
Write your own in-memory implementation of your context and DbSets.

[Working with Transactions (EF6 onwards)](../ef6/entity-framework-working-with-transactions-ef6-onwards.md)  
Learn how to control the use of trasactions with Entity Framework.

[Connection Management (EF6 onwards)](../ef6/entity-framework-connection-management.md)  
Learn how to control connection open/close and work with existing database connections.

## Using EF With Other Technologies

[ASP.NET MVC](http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application)  
EF provides the M (Model) in MVC.

[ASP.NET Web API](http://blogs.msdn.com/b/jasonz/archive/2012/07/23/my-favorite-features-entity-framework-code-first-and-asp-net-web-api.aspx)  
Use EF and ASP.NET Web API to build HTTP services that reach a broad range of clients.

[ASP.NET Web Forms](http://www.asp.net/web-forms/tutorials/aspnet-45/getting-started-with-aspnet-45-web-forms/create_the_data_access_layer)  
Find out how to perform data access in your Web Forms application using EF.

[Data Binding with WPF (Windows Presentation Foundation)](../ef6/entity-framework-databinding-with-wpf.md)  
Learn how to create a master/detail window with WPF data binding using EF for data access.

[Data Binding with WinForms (Windows Forms)](../ef6/entity-framework-databinding-with-winforms.md)  
Learn how to create a master/detail window with WinForms data binding using EF for data access.
