---
title: Design-time services - EF Core
description: Information on Entity Framework Core design-time services
author: SamMonoRT
ms.date: 01/17/2025
uid: core/cli/services
---
# Design-time services

Some services used by the tools are only used at design time. These services are managed separately from EF Core's runtime services to prevent them from being deployed with your app. To override one of these services (for example the service to generate migration files), add an implementation of `IDesignTimeServices` to your startup project.

[!code-csharp[Main](../../../samples/core/Miscellaneous/CommandLine/DesignTimeServices.cs#DesignTimeServices)]

## Referencing Microsoft.EntityFrameworkCore.Design

Microsoft.EntityFrameworkCore.Design is a DevelopmentDependency package. This means that the dependency won't flow transitively into other projects, and that you cannot, by default, reference its types.

In order to reference its types and override design-time services, update the PackageReference item's metadata in your project file.

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
  <PrivateAssets>all</PrivateAssets>
  <!-- Remove IncludeAssets to allow compiling against the assembly -->
  <!--<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>-->
</PackageReference>
```

If the package is being referenced transitively via Microsoft.EntityFrameworkCore.Tools, you will need to add an explicit PackageReference to the package and change its metadata.

## List of services

The following is a list of the design-time services.

Service                                                                              | Description
------------------------------------------------------------------------------------ | -----------
<xref:Microsoft.EntityFrameworkCore.Design.IAnnotationCodeGenerator>                 | Generates the code for corresponding model annotations.
<xref:Microsoft.EntityFrameworkCore.Design.ICandidateNamingService>                  | Generates candidate names for entities and properties.
<xref:Microsoft.EntityFrameworkCore.Design.ICSharpHelper>                            | Helps with generating C# code.
<xref:Microsoft.EntityFrameworkCore.Design.ICSharpMigrationOperationGenerator>       | Generates C# code for migration operations.
<xref:Microsoft.EntityFrameworkCore.Design.ICSharpSnapshotGenerator>                 | Generates C# code for model snapshots.
<xref:Microsoft.EntityFrameworkCore.Design.ICSharpUtilities>                         | C# code generation utilities.
<xref:Microsoft.EntityFrameworkCore.Design.IPluralizer>                              | Pluralizes and singularizes words.
<xref:Microsoft.EntityFrameworkCore.Migrations.Design.IMigrationsCodeGenerator>      | Generates code for a migration.
<xref:Microsoft.EntityFrameworkCore.Migrations.Design.IMigrationsCodeGeneratorSelector> | Selects the appropriate migrations code generator.
<xref:Microsoft.EntityFrameworkCore.Migrations.Design.IMigrationsScaffolder>         | The main class for managing migration files.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.ICompiledModelCodeGenerator>         | Generates code for compiled model metadata.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.ICompiledModelCodeGeneratorSelector> | Selects the appropriate compiled model code generator.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.ICompiledModelScaffolder>            | The main class for scaffolding compiled models.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IDatabaseModelFactory>               | Creates a database model from a database.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IModelCodeGenerator>                 | Generates code for a model.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IModelCodeGeneratorSelector>         | Selects the appropriate model code generator.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IProviderConfigurationCodeGenerator> | Generates OnConfiguring code.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IReverseEngineerScaffolder>          | The main class for scaffolding reverse engineered models.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IScaffoldingModelFactory>            | Creates a model from a database model.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.IScaffoldingTypeMapper>              | Maps database types to .NET types during scaffolding.
<xref:Microsoft.EntityFrameworkCore.Scaffolding.ISnapshotModelProcessor>             | Processes model snapshots.
<xref:Microsoft.EntityFrameworkCore.Query.IPrecompiledQueryCodeGenerator>            | Generates code for precompiled queries.
<xref:Microsoft.EntityFrameworkCore.Query.IPrecompiledQueryCodeGeneratorSelector>    | Selects the appropriate precompiled query code generator.

## Using services

These services can also be useful for creating your own tools. For example, when you want to automate part of your design-time workflow.

You can build a service provider containing these services using the AddEntityFrameworkDesignTimeServices and AddDbContextDesignTimeServices extension methods.

[!code-csharp[](../../../samples/core/Miscellaneous/CommandLine/CustomTools.cs#CustomTools)]
