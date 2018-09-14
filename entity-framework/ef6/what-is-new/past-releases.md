---
title: "Past Releases of Entity Framework - EF6"
author: divega
ms.date: "10/23/2016"
ms.assetid: 1060bb99-765f-4f32-aaeb-d6635d3dbd3e
---
# Past Releases of Entity Framework

The first version of Entity Framework was released in 2008, as part of .NET Framework 3.5 SP1 and Visual Studio 2008 SP1.

Starting with the EF4.1 release it has shipped as the [EntityFramework NuGet Package](https://www.nuget.org/packages/EntityFramework/) - currently one of the most popular packages on NuGet.org.

Between versions 4.1 and 5.0, the EntityFramework NuGet package extended the EF libraries that shipped as part of .NET Framework.   

Starting with version 6, EF became an open source project and also moved completely out of band form the .NET Framework.
This means that when you add the EntityFramework version 6 NuGet package to an application, you are getting a complete copy of the EF library that does not depend on the EF bits that ship as part of .NET Framework.
This helped somewhat accelerate the pace of development and delivery of new features.

In June 2016, we released EF Core 1.0. EF Core is based on a new codebase and is designed as a more lightweight and extensible version of EF.
Currently EF Core is the main focus of development for the Entity Framework Team at Microsoft.
This means there are no new major features planned for EF6. However EF6 is still maintained as an open source project and a supported Microsoft product.

Here is the list of past releases, in reverse chronological order, with information on the new features that were introduced in each release.

## EF 6.1.3
The EF 6.1.3 runtime was released to NuGet in October of 2015.
This release contains only fixes to high-priority defects and regressions reported on the 6.1.2 release.
The fixes include:

- Query: Regression in EF 6.1.2: OUTER APPLY introduced and more complex queries for 1:1 relationships and “let” clause
- TPT problem with hiding base class property in inherited class
- DbMigration.Sql fails when the word ‘go’ is contained in the text
- Create compatibility flag for UnionAll and Intersect flattening support
- Query with multiple Includes does not work in 6.1.2 (working in 6.1.1)
- “You have an error in your SQL syntax” exception after upgrading from EF 6.1.1 to 6.1.2

## EF 6.1.2
The EF 6.1.2 runtime was released to NuGet in December of 2014.
This version is mostly about bug fixes. We also accepted a couple of noteworthy changes from members of the community:
- **Query cache parameters can be configured from the app/web.configuration file**
    ``` xml
    <entityFramework>
      <queryCache size='1000' cleaningIntervalInSeconds='-1'/>
    </entityFramework>
    ```
- **SqlFile and SqlResource methods on DbMigration** allow you to run a SQL script stored as a file or embedded resource.

## EF 6.1.1
The EF 6.1.1 runtime was released to NuGet in June of 2014.
This version contains fixes for issues that a number of people have encountered. Among others:
- Designer: Error opening EF5 edmx with decimal precision in EF6 designer
- Default instance detection logic for LocalDB doesn't work with SQL Server 2014

## EF 6.1.0
The EF 6.1.0 runtime was released to NuGet in March of 2014.
This minor update includes a significant number of new features:

- **Tooling consolidation** provides a consistent way to create a new EF model. This feature [extends the ADO.NET Entity Data Model wizard to support creating Code First models](~/ef6/modeling/code-first/workflows/existing-database.md), including reverse engineering from an existing database. These features were previously available in Beta quality in the EF Power Tools.
- **[Handling of transaction commit failures](~/ef6/fundamentals/connection-resiliency/commit-failures.md)** provides the CommitFailureHandler which makes use of the newly introduced ability to intercept transaction operations. The CommitFailureHandler allows automatic recovery from connection failures whilst committing a transaction.
- **[IndexAttribute](~/ef6/modeling/code-first/data-annotations.md)** allows indexes to be specified by placing an `[Index]` attribute on a property (or properties) in your Code First model. Code First will then create a corresponding index in the database.
- **The public mapping API** provides access to the information EF has on how properties and types are mapped to columns and tables in the database. In past releases this API was internal.
- **[Ability to configure interceptors via the App/Web.config file](~/ef6/fundamentals/configuring/config-file.md)** allows interceptors to be added without recompiling the application.
- **System.Data.Entity.Infrastructure.Interception.DatabaseLogger**is a new interceptor that makes it easy to log all database operations to a file. In combination with the previous feature, this allows you to easily [switch on logging of database operations for a deployed application](~/ef6/fundamentals/configuring/config-file.md), without the need to recompile.
- **Migrations model change detection** has been improved so that scaffolded migrations are more accurate; performance of the change detection process has also been enhanced.
- **Performance improvements** including reduced database operations during initialization, optimizations for null equality comparison in LINQ queries, faster view generation (model creation) in more scenarios, and more efficient materialization of tracked entities with multiple associations.

## EF 6.0.2
The EF 6.0.2 runtime was released to NuGet in December of 2013.
This patch release is limited to fixing issues that were introduced in the EF6 release (regressions in performance/behavior since EF5).

## EF 6.0.1
The EF 6.0.1 runtime was released to NuGet in October of 2013 simultaneously with EF 6.0.0, because the latter was embedded in a version of Visual Studio that had locked down a few months before.
This patch release is limited to fixing issues that were introduced in the EF6 release (regressions in performance/behavior since EF5).
The most notable changes were to fix some performance issues during warm-up for EF models.
This was important as warm-up performance was an area of focus in EF6 and these issues were negating some of the other performance gains made in EF6.

## EF 6.0
The EF 6.0.0 runtime was released to NuGet in October of 2013.
This is the first version in which a complete EF runtime is included in the [EntityFramework NuGet Package](https://www.nuget.org/packages/EntityFramework/) which does not depend on the EF bits that are part of the .NET Framework.
Moving the remaining parts of the runtime to the NuGet package required a number of breaking change for existing code.
See the section on [Upgrading to Entity Framework 6](upgrading-to-ef6.md) for more details on the manual steps required to upgrade.

This release includes numerous new features.
The following features work for models created with Code First or the EF Designer:

- **[Async Query and Save](~/ef6/fundamentals/async.md)** adds support for the task-based asynchronous patterns that were introduced in .NET 4.5.
- **[Connection Resiliency](~/ef6/fundamentals/connection-resiliency/retry-logic.md)** enables automatic recovery from transient connection failures.
- **[Code-Based Configuration](~/ef6/fundamentals/configuring/code-based.md)** gives you the option of performing configuration – that was traditionally performed in a config file – in code.
- **[Dependency Resolution](~/ef6/fundamentals/configuring/dependency-resolution.md)** introduces support for the Service Locator pattern and we've factored out some pieces of functionality that can be replaced with custom implementations.
- **[Interception/SQL logging](~/ef6/fundamentals/logging-and-interception.md)** provides low-level building blocks for interception of EF operations with simple SQL logging built on top.
- **Testability improvements** make it easier to create test doubles for DbContext and DbSet when [using a mocking framework](~/ef6/fundamentals/testing/mocking.md) or [writing your own test doubles](~/ef6/fundamentals/testing/writing-test-doubles.md).
- **[DbContext can now be created with a DbConnection that is already opened](~/ef6/fundamentals/connection-management.md)** which enables scenarios where it would be helpful if the connection could be open when creating the context (such as sharing a connection between components where you can not guarantee the state of the connection).
- **[Improved Transaction Support](~/ef6/saving/transactions.md)** provides support for a transaction external to the framework as well as improved ways of creating a transaction within the Framework.
- **Enums, Spatial and Better Performance on .NET 4.0** - By moving the core components that used to be in the .NET Framework into the EF NuGet package we are now able to offer enum support, spatial data types and the performance improvements from EF5 on .NET 4.0.
- **Improved performance of Enumerable.Contains in LINQ queries**.
- **Improved warm up time (view generation)**, especially for large models.
- **Pluggable Pluralization &amp; Singularization Service**.
- **Custom implementations of Equals or GetHashCode** on entity classes are now supported.
- **DbSet.AddRange/RemoveRange** provides an optimized way to add or remove multiple entities from a set.
- **DbChangeTracker.HasChanges** provides an easy and efficient way to see if there are any pending changes to be saved to the database.
- **SqlCeFunctions** provides a SQL Compact equivalent to the SqlFunctions.

The following features apply to Code First only:

- **[Custom Code First Conventions](~/ef6/modeling/code-first/conventions/custom.md)** allow write your own conventions to help avoid repetitive configuration. We provide a simple API for lightweight conventions as well as some more complex building blocks to allow you to author more complicated conventions.
- **[Code First Mapping to Insert/Update/Delete Stored Procedures](~/ef6/modeling/code-first/fluent/cud-stored-procedures.md)** is now supported.
- **[Idempotent migrations scripts](~/ef6/modeling/code-first/migrations/index.md)** allow you to generate a SQL script that can upgrade a database at any version up to the latest version.
- **[Configurable Migrations History Table](~/ef6/modeling/code-first/migrations/history-customization.md)** allows you to customize the definition of the migrations history table. This is particularly useful for database providers that require the appropriate data types etc. to be specified for the Migrations History table to work correctly.
- **Multiple Contexts per Database** removes the previous limitation of one Code First model per database when using Migrations or when Code First automatically created the database for you.
- **[DbModelBuilder.HasDefaultSchema](~/ef6/modeling/code-first/fluent/types-and-properties.md)** is a new Code First API that allows the default database schema for a Code First model to be configured in one place. Previously the Code First default schema was hard-coded to &quot;dbo&quot; and the only way to configure the schema to which a table belonged was via the ToTable API.
- **DbModelBuilder.Configurations.AddFromAssembly method** allows you to easily add all configuration classes defined in an assembly when you are using configuration classes with the Code First Fluent API.
- **[Custom Migrations Operations](http://romiller.com/2013/02/27/ef6-writing-your-own-code-first-migration-operations/)** enabled you to add additional operations to be used in your code-based migrations.
- **Default transaction isolation level is changed to READ_COMMITTED_SNAPSHOT** for databases created using Code First, allowing for more scalability and fewer deadlocks.
- **Entity and complex types can now be nestedinside classes**. |

## EF 5.0
The EF 5.0.0 runtime was released to NuGet in August of 2012.
This release introduces some new features including enum support, table-valued functions, spatial data types and various performance improvements.

The Entity Framework Designer in Visual Studio 2012 also introduces support for multiple-diagrams per model, coloring of shapes on the design surface and batch import of stored procedures.

Here is a list of content we put together specifically for the EF 5 release.

-   [EF 5 Release Post](https://blogs.msdn.com/b/adonet/archive/2012/08/15/ef5-released.aspx)
-   New Features in EF5
    -   [Enum Support in Code First](~/ef6/modeling/code-first/data-types/enums.md)
    -   [Enum Support in EF Designer](~/ef6/modeling/designer/data-types/enums.md)
    -   [Spatial Data Types in Code First](~/ef6/modeling/code-first/data-types/spatial.md)
    -   [Spatial Data Types in EF Designer](~/ef6/modeling/designer/data-types/spatial.md)
    -   [Provider Support for Spatial Types](~/ef6/fundamentals/providers/spatial-support.md)
    -   [Table-Valued Functions](~/ef6/modeling/designer/advanced/tvfs.md)
    -   [Multiple Diagrams per Model](~/ef6/modeling/designer/multiple-diagrams.md)
-   Setting up your model
    -   [Creating a Model](~/ef6/modeling/index.md)
    -   [Connections and Models](~/ef6/fundamentals/configuring/connection-strings.md)
    -   [Performance Considerations](~/ef6/fundamentals/performance/perf-whitepaper.md)
    -   [Working with Microsoft SQL Azure](~/ef6/fundamentals/connection-resiliency/retry-logic.md)
    -   [Configuration File Settings](~/ef6/fundamentals/configuring/config-file.md)
    -   [Glossary](~/ef6/resources/glossary.md)
    -   Code First
        -   [Code First to a new database (walkthrough and video)](~/ef6/modeling/code-first/workflows/new-database.md)
        -   [Code First to an existing database (walkthrough and video)](~/ef6/modeling/code-first/workflows/existing-database.md)
        -   [Conventions](~/ef6/modeling/code-first/conventions/built-in.md)
        -   [Data Annotations](~/ef6/modeling/code-first/data-annotations.md)
        -   [Fluent API - Configuring/Mapping Properties & Types](~/ef6/modeling/code-first/fluent/types-and-properties.md)
        -   [Fluent API - Configuring Relationships](~/ef6/modeling/code-first/fluent/relationships.md)
        -   [Fluent API with VB.NET](~/ef6/modeling/code-first/fluent/vb.md)
        -   [Code First Migrations](~/ef6/modeling/code-first/migrations/index.md)
        -   [Automatic Code First Migrations](~/ef6/modeling/code-first/migrations/automatic.md)
        -   [Migrate.exe](~/ef6/modeling/code-first/migrations/migrate-exe.md)
        -   [Defining DbSets](~/ef6/modeling/code-first/dbsets.md)
    -   EF Designer
        -   [Model First (walkthrough and video)](~/ef6/modeling/designer/workflows/model-first.md)
        -   [Database First (walkthrough and video)](~/ef6/modeling/designer/workflows/database-first.md)
        -   [Complex Types](~/ef6/modeling/designer/data-types/complex-types.md)
        -   [Associations/Relationships](~/ef6/modeling/designer/relationships.md)
        -   [TPT Inheritance Pattern](~/ef6/modeling/designer/inheritance/tpt.md)
        -   [TPH Inheritance Pattern](~/ef6/modeling/designer/inheritance/tph.md)
        -   [Query with Stored Procedures](~/ef6/modeling/designer/stored-procedures/query.md)
        -   [Stored Procedures with Multiple Result Sets](~/ef6/modeling/designer/advanced/multiple-result-sets.md)
        -   [Insert, Update & Delete with Stored Procedures](~/ef6/modeling/designer/stored-procedures/cud.md)
        -   [Map an Entity to Multiple Tables (Entity Splitting)](~/ef6/modeling/designer/entity-splitting.md)
        -   [Map Multiple Entities to One Table (Table Splitting)](~/ef6/modeling/designer/table-splitting.md)
        -   [Defining Queries](~/ef6/modeling/designer/advanced/defining-query.md)
        -   [Code Generation Templates](~/ef6/modeling/designer/codegen/index.md)
        -   [Reverting to ObjectContext](~/ef6/modeling/designer/codegen/legacy-objectcontext.md)
-   Using Your Model
    -   [Working with DbContext](~/ef6/fundamentals/working-with-dbcontext.md)
    -   [Querying/Finding Entities](~/ef6/querying/index.md)
    -   [Working with Relationships](~/ef6/fundamentals/relationships.md)
    -   [Loading Related Entities](~/ef6/querying/related-data.md)
    -   [Working with Local Data](~/ef6/querying/local-data.md)
    -   [N-Tier Applications](~/ef6/fundamentals/disconnected-entities/index.md)
    -   [Raw SQL Queries](~/ef6/querying/raw-sql.md)
    -   [Optimistic Concurrency Patterns](~/ef6/saving/concurrency.md)
    -   [Working with Proxies](~/ef6/fundamentals/proxies.md)
    -   [Automatic Detect Changes](~/ef6/saving/change-tracking/auto-detect-changes.md)
    -   [No-Tracking Queries](~/ef6/querying/no-tracking.md)
    -   [The Load Method](~/ef6/querying/load-method.md)
    -   [Add/Attach and Entity States](~/ef6/saving/change-tracking/entity-state.md)
    -   [Working with Property Values](~/ef6/saving/change-tracking/property-values.md)
    -   [Data Binding with WPF (Windows Presentation Foundation)](~/ef6/fundamentals/databinding/wpf.md)
    -   [Data Binding with WinForms (Windows Forms)](~/ef6/fundamentals/databinding/winforms.md)

## EF 4.3.1
The EF 4.3.1 runtime was released to NuGet in February 2012 shortly after EF 4.3.0.
This patch release included some bug fixes to the EF 4.3 release and introduced better LocalDB support for customers using EF 4.3 with Visual Studio 2012.

Here is a list of content we put together specifically for the EF 4.3.1 release, most of the content provided for EF 4.1 still applies to EF 4.3 as well.

-   [EF 4.3.1 Release Blog  Post](https://blogs.msdn.com/b/adonet/archive/2012/02/29/ef4-3-1-and-ef5-beta-1-available-on-nuget.aspx)

## EF 4.3
The EF 4.3.0 runtime was released to NuGet in February of 2012.
This release included the new Code First Migrations feature that allows a database created by Code First to be incrementally changed as your Code First model evolves.

Here is a list of content we put together specifically for the EF 4.3 release, most of the content provided for EF 4.1 still applies to EF 4.3 as well:
-   [EF 4.3 Release Post](https://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-released.aspx)
-   [EF 4.3 Code-Based Migrations Walkthrough](https://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-code-based-migrations-walkthrough.aspx)
-   [EF 4.3 Automatic Migrations Walkthrough](https://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-automatic-migrations-walkthrough.aspx)

## EF 4.2
The EF 4.2.0 runtime was released to NuGet in November of 2011.
This release includes bug fixes to the EF 4.1.1 release.
Because this release only included bug fixes it could have been the EF 4.1.2 patch release but we opted to move to 4.2 to allow us to move away from the date based patch version numbers we used in the 4.1.x releases and adopt the [Semantic Versionsing](https://semver.org) standard for semantic versioning.

Here is a list of content we put together specifically for the EF 4.2 release, the content provided for EF 4.1 still applies to EF 4.2 as well.

-   [EF 4.2 Release Post](https://blogs.msdn.com/b/adonet/archive/2011/11/01/ef-4-2-released.aspx)
-   [Code First Walkthrough](https://blogs.msdn.com/b/adonet/archive/2011/09/28/ef-4-2-code-first-walkthrough.aspx)
-   [Model & Database First Walkthrough](https://blogs.msdn.com/b/adonet/archive/2011/09/28/ef-4-2-model-amp-database-first-walkthrough.aspx)

## EF 4.1.1
The EF 4.1.10715 runtime was released to NuGet in July of 2011.
In addition to bug fixes this patch release introduced some components to make it easier for design time tooling to work with a Code First model.
These components are used by Code First Migrations (included in EF 4.3) and the EF Power Tools.

You’ll notice that the strange version number 4.1.10715 of the package.
We used to use date based patch versions before we decided to adopt [Semantic Versioning](https://semver.org).
Think of this version as EF 4.1 patch 1 (or EF 4.1.1).

Here is a list of content we put together for the 4.1.1 release:

-   [EF 4.1.1 Release Post](https://blogs.msdn.com/b/adonet/archive/2011/07/25/ef-4-1-update-1-released.aspx)

## EF 4.1
The EF 4.1.10331 runtime was the first to be published on NuGet, in April of 2011.
This release included the simplified DbContext API and the Code First workflow.

You will notice the strange version number, 4.1.10331, which should really have been 4.1. In addition there is a 4.1.10311 version which should have been 4.1.0-rc (the ‘rc’ stands for ‘release candidate’).
We used to use date based patch versions before we decided to adopt [Semantic Versioning](https://semver.org).

Here is a list of content we put together for the 4.1 release. Much of it still applies to later releases of Entity Framework:

-   [EF 4.1 Release Post](https://blogs.msdn.com/b/adonet/archive/2011/04/11/ef-4-1-released.aspx)
-   [Code First Walkthrough](https://blogs.msdn.com/b/adonet/archive/2011/03/15/ef-4-1-code-first-walkthrough.aspx)
-   [Model & Database First Walkthrough](https://blogs.msdn.com/b/adonet/archive/2011/03/15/ef-4-1-model-amp-database-first-walkthrough.aspx)
-   [SQL Azure Federations and the Entity Framework](https://blogs.msdn.com/b/adonet/archive/2012/01/10/sql-azure-federations-and-the-entity-framework.aspx)

## EF 4.0
This release was included in .NET Framework 4 and Visual Studio 2010, in April of 2010.
Important new features in this release included POCO support, foreign key mapping, lazy loading, testability improvements, customizable code generation and the Model First workflow.

Although it was the second release of Entity Framework, it was named EF 4 to align with the .NET Framework version that it shipped with.
After this release, we started making Entity Framework available on NuGet and adopted semantic versioning since we were no longer tied to the .NET Framework Version.

Note that some subsequent versions of .NET Framework have shipped with significant updates to the included EF bits.
In fact, many of the new features of EF 5.0 were implemented as improvements on these bits.
However, in order to rationalize the versioning story for EF, we continue to refer to the EF bits that are part of the .NET Framework as the EF 4.0 runtime, while all newer versions consist of the [EntityFramework NuGet Package](https://www.nuget.org/packages/EntityFramework/).         

## EF 3.5
The initial version of Entity Framework was included in .NET 3.5 Service Pack 1 and Visual Studio 2008 SP1, released in August of 2008.
This release provided basic O/RM support using the Database First workflow.
