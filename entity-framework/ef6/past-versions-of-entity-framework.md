---
title: "Past Versions of Entity Framework | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: 1060bb99-765f-4f32-aaeb-d6635d3dbd3e
caps.latest.revision: 4
---
# Past Versions of Entity Framework
We highly recommend that you use the latest version of Entity Framework to ensure you get the latest features and bug fixes. Check out the [Getting Started](../ef6/entity-framework-ef-documentation.md) section for documentation that applies to the latest version.

We realize that you may need to use a previous version so this page serves as an archive of documentation for past releases. We’ll move outdated content here as new versions of Entity Framework are released.

Some of the content we built for previous versions is still relevant to the latest release, so you may find some links are shown on both this page and the [Getting Started](../ef6/entity-framework-ef-documentation.md) page.

## EF 5

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) to install Entity Framework 5.

```
Install-Package EntityFramework -Version 5.0.0
```

Here is a list of content we put together specifically for the EF 5 release.

-   [EF 5 Release Post](http://blogs.msdn.com/b/adonet/archive/2012/08/15/ef5-released.aspx)
-   Getting Started
    -   [Which workflow should I use?](../ef6/entity-framework-development-workflows-video.md)
    -   [Code First to a new database (walkthrough and video)](../ef6/entity-framework-code-first-to-a-new-database.md)
    -   [Code First to an existing database (walkthrough and video)](../ef6/entity-framework-code-first-to-an-existing-database.md)
    -   [Model First (walkthrough and video)](../ef6/entity-framework-model-first.md)
    -   [Database First (walkthrough and video)](../ef6/entity-framework-database-first.md)
-   New Features in EF5
    -   [Enum Support in Code First](../ef6/entity-framework-enum-support-code-first-ef5-onwards.md)
    -   [Enum Support in EF Designer](../ef6/entity-framework-enum-support-ef-designer-ef5-onwards.md)
    -   [Spatial Data Types in Code First](../ef6/entity-framework-spatial-code-first-ef5-onwards.md)
    -   [Spatial Data Types in EF Designer](../ef6/entity-framework-spatial-ef-designer-ef5-onwards.md)
    -   [Provider Support for Spatial Types](../ef6/entity-framework-provider-support-for-spatial-types.md)
    -   [Table-Valued Functions](../ef6/entity-framework-table-valued-functions-tvfs-ef5-onwards.md)
    -   [Multiple Diagrams per Model](../ef6/entity-framework-multiple-diagrams-per-model-ef5-onwards.md)
-   Setting up your model
    -   [Which Workflow Should I Use](../ef6/entity-framework-development-workflows-video.md)
    -   [Connections and Models](../ef6/entity-framework-connections-and-models.md)
    -   [Performance Considerations](../ef6/performance-considerations-for-ef-4-5-and-6.md)
    -   [Working with Microsoft SQL Azure](../ef6/entity-framework-windows-sql-azure.md)
    -   [Entity Framework Power Tools](../ef6/entity-framework-power-tools.md)
    -   [Configuration File Settings](../ef6/entity-framework-config-file-settings.md)
    -   [Glossary](../ef6/entity-framework-glossary.md)
    -   Code First
        -   [Code First to a new database (walkthrough and video)](../ef6/entity-framework-code-first-to-a-new-database.md)
        -   [Code First to an existing database (walkthrough and video)](../ef6/entity-framework-code-first-to-an-existing-database.md)
        -   [Conventions](../ef6/entity-framework-code-first-conventions.md)
        -   [Data Annotations](../ef6/entity-framework-code-first-data-annotations.md)
        -   [Fluent API - Configuring/Mapping Properties & Types](../ef6/entity-framework-fluent-api-configuring-and-mapping-properties-and-types.md)
        -   [Fluent API - Configuring Relationships](../ef6/entity-framework-fluent-api-relationships.md)
        -   [Fluent API with VB.NET](../ef6/entity-framework-fluent-api-with-vb-net.md)
        -   [Code First Migrations](../ef6/entity-framework-code-first-migrations.md)
        -   [Automatic Code First Migrations](../ef6/entity-framework-automatic-code-first-migrations.md)
        -   [Migrate.exe](../ef6/entity-framework-migrate-exe.md)
        -   [Defining DbSets](../ef6/entity-framework-defining-dbsets.md)
    -   EF Designer
        -   [Model First (walkthrough and video)](../ef6/entity-framework-model-first.md)
        -   [Database First (walkthrough and video)](../ef6/entity-framework-database-first.md)
        -   [Complex Types](../ef6/entity-framework-complex-types-ef-designer.md)
        -   [Associations/Relationships](../ef6/entity-framework-relationships-ef-designer.md)
        -   [TPT Inheritance Pattern](../ef6/entity-framework-designer-tpt-inheritance.md)
        -   [TPH Inheritance Pattern](../ef6/entity-framework-designer-tph-inheritance.md)
        -   [Query with Stored Procedures](../ef6/entity-framework-designer-query-sprocs.md)
        -   [Stored Procedures with Multiple Result Sets](../ef6/entity-framework-sprocs-with-multiple-result-sets.md)
        -   [Insert, Update & Delete with Stored Procedures](../ef6/entity-framework-designer-cud-sprocs.md)
        -   [Map an Entity to Multiple Tables (Entity Splitting)](../ef6/entity-framework-designer-entity-splitting.md)
        -   [Map Multiple Entities to One Table (Table Splitting)](../ef6/entity-framework-designer-table-splitting.md)
        -   [Defining Queries](../ef6/entity-framework-defining-query-ef-designer.md)
        -   [Code Generation Templates](../ef6/entity-framework-designer-code-generation-templates.md)
        -   [Reverting to ObjectContext](../ef6/reverting-to-objectcontext-in-entity-framework-designer.md)
        -   [EDMX Files](../ef6/entity-framework-edmx.md)
-   Using Your Model
    -   [Working with DbContext](../ef6/entity-framework-working-with-dbcontext.md)
    -   [Querying/Finding Entities](../ef6/entity-framework-querying-and-finding-entities.md)
    -   [Working with Relationships](../ef6/entity-framework-relationships-and-navigation-properties.md)
    -   [Loading Related Entities](../ef6/entity-framework-loading-related-entities.md)
    -   [Working with Local Data](../ef6/entity-framework-local-data.md)
    -   [N-Tier Applications](../ef6/entity-framework-n-tier.md)
    -   [Raw SQL Queries](../ef6/entity-framework-raw-sql-queries.md)
    -   [Optimistic Concurrency Patterns](../ef6/entity-framework-optimistic-concurrency-patterns.md)
    -   [Working with Proxies](../ef6/entity-framework-working-with-proxies.md)
    -   [Automatic Detect Changes](../ef6/entity-framework-automatic-detect-changes.md)
    -   [No-Tracking Queries](../ef6/entity-framework-no-tracking-queries.md)
    -   [The Load Method](../ef6/entity-framework-the-load-method.md)
    -   [Add/Attach and Entity States](../ef6/entity-framework-add-and-attach-and-entity-states.md)
    -   [Working with Property Values](../ef6/entity-framework-working-with-property-values.md)
    -   [API Documentation](https://msdn.microsoft.com/library/hh289362)
-   Using EF With Other Technologies
    -   [ASP.NET MVC](http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application)
    -   [ASP.NET Web API](http://blogs.msdn.com/b/jasonz/archive/2012/07/23/my-favorite-features-entity-framework-code-first-and-asp-net-web-api.aspx)
    -   [ASP.NET Web Forms](http://www.asp.net/web-forms/tutorials/aspnet-45/getting-started-with-aspnet-45-web-forms/create_the_data_access_layer)
    -   [Data Binding with WPF (Windows Presentation Foundation)](../ef6/data-binding-with-wpf-and-the-entity-framework.md)
    -   [Data Binding with WinForms (Windows Forms)](../ef6/entity-framework-databinding-with-winforms.md)

## EF 4.3

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) to install Entity Framework 4.3.

```
Install-Package EntityFramework -Version 4.3.1
```

Here is a list of content we put together specifically for the EF 4.3 release, most of the content provided for EF 4.1 still applies to EF 4.3 as well.

-   [EF 4.3.1 Release Post](http://blogs.msdn.com/b/adonet/archive/2012/02/29/ef4-3-1-and-ef5-beta-1-available-on-nuget.aspx)
-   [EF 4.3 Release Post](http://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-released.aspx)
-   [EF 4.3 Code-Based Migrations Walkthrough](http://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-code-based-migrations-walkthrough.aspx)
-   [EF 4.3 Automatic Migrations Walkthrough](http://blogs.msdn.com/b/adonet/archive/2012/02/09/ef-4-3-automatic-migrations-walkthrough.aspx)
-   Entity Framework Interview on VS Toolbox ([Part 1](http://channel9.msdn.com/shows/visual-studio-toolbox/visual-studio-toolbox-entity-framework-part-1) | [Part 2](http://channel9.msdn.com/shows/visual-studio-toolbox/visual-studio-toolbox-entity-framework-part-2))

## EF 4.2

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) to install Entity Framework 4.2.

```
Install-Package EntityFramework -Version 4.2
```

Here is a list of content we put together specifically for the EF 4.2 release, the content provided for EF 4.1 still applies to EF 4.2 as well.

-   [EF 4.2 Release Post](http://blogs.msdn.com/b/adonet/archive/2011/11/01/ef-4-2-released.aspx)
-   [Code First Walkthrough](http://blogs.msdn.com/b/adonet/archive/2011/09/28/ef-4-2-code-first-walkthrough.aspx)
-   [Model & Database First Walkthrough](http://blogs.msdn.com/b/adonet/archive/2011/09/28/ef-4-2-model-amp-database-first-walkthrough.aspx)

## EF 4.1

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) to install Entity Framework 4.1.

```
Install-Package EntityFramework -Version 4.1.10715
```

You’ll notice that the strange ‘10715’ patch version of the package. We used to use date based patch versions before we decided to adopt the \<http://semver.org> standard for semantic versioning. Think of this version as patch 1 (i.e. 4.1.1). There is also 4.1.10331 version of the package which should really have been 4.1. In addition there is a 4.1.10311 version which should have been 4.1.0-rc (the ‘rc’ stands for ‘release candidate’).

Here is a useful list of content we put together for the 4.1 release. Much of it still applies to later releases of the Entity Framework:

-   [EF 4.1.1 Release Post](http://blogs.msdn.com/b/adonet/archive/2011/07/25/ef-4-1-update-1-released.aspx)
-   [EF 4.1 Release Post](http://blogs.msdn.com/b/adonet/archive/2011/04/11/ef-4-1-released.aspx)
-   [Code First Walkthrough](http://blogs.msdn.com/b/adonet/archive/2011/03/15/ef-4-1-code-first-walkthrough.aspx)
-   [Model & Database First Walkthrough](http://blogs.msdn.com/b/adonet/archive/2011/03/15/ef-4-1-model-amp-database-first-walkthrough.aspx)
-   [SQL Azure Federations and the Entity Framework](http://blogs.msdn.com/b/adonet/archive/2012/01/10/sql-azure-federations-and-the-entity-framework.aspx)

## EF 4

Entity Framework 4 was included in [Visual Studio 2010](https://www.microsoft.com/visualstudio/en-us/products/2010-editions) and .NET Framework 4.0.

Here are some useful links to get you started:

-   [Entity Framework 4.0 MSDN Content](https://msdn.microsoft.com/library/bb399572(v=vs.100).aspx)
-   [Entity Framework 4.0 Sample Applications](https://msdn.microsoft.com/library/bb738547.aspx)
-   [Visual Studio 2010 and .NET Framework 4 Training Kit](https://www.microsoft.com/downloads/details.aspx?DisplayLang=en&FamilyID=752cb725-969b-4732-a383-ed5740f02e93)
-   Entity Framework Presentations
    -   [Overview of the Microsoft ADO.NET Entity Framework 4.0 TechEd Presentation](http://www.msteched.com/2010/NorthAmerica/DEV205)
    -   [Evolving ADO.NET Entity Framework in .NET 4 and Beyond](http://microsoftpdc.com/Sessions/FT10)

## EF 3.5

The first version of Entity Framework was included in [Visual Studio 2008 SP1](https://www.microsoft.com/visualstudio/en-us/products/2008-editions) and .NET Framework 3.5 SP1.

Here are some useful links to get you started:

-   [Entity Framework 3.5 MSDN Content](https://msdn.microsoft.com/library/bb399572(v=vs.90).aspx)
-   [.NET Framework 3.5 Enhancements Training Kit](https://www.microsoft.com/download/en/details.aspx?displaylang=en&id=13772)
-   [Introducing the Entity Framework (200 Level)](https://msdn.microsoft.com/data/bb399567)
-   [Achieve Flexible Data Modeling with the Entity Framework (300 level)](https://msdn.microsoft.com/magazine/cc700331.aspx)
-   Training
    -   [Visual Studio 2008: ADO.NET 3.5](https://www.microsoft.com/learning/en/us/course.aspx?ID=6464A&locale=en-us) (2 days)
    -   [Implementing the Entity Framework in ADO.NET 3.5 Applications](https://www.microsoftelearning.com/eLearning/courseDetail.aspx?courseId=92817&tab=overview) (2 hours)
    -   [Visual Studio 2008 ADO.NET 3.5](https://www.microsoftelearning.com/eLearning/offerDetail.aspx?offerPriceId=220335) (20 hours)
