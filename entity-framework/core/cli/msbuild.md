---
title: EF Core MSBuild tasks - EF Core
description: Reference guide for the Entity Framework Core .NET MSBuild tasks
author: AndriySvyryd
ms.date: 01/17/2025
uid: core/cli/msbuild
---

# Entity Framework Core MSBuild integration

Starting with EF 9, you can use an MSBuild task to generate the compiled model and precompiled queries automatically either when the project is built or when it's published. This is mainly intended to be used with NativeAOT publishing.

> [!WARNING]
> NativeAOT support and the MSBuild integration are experimental features, and are not yet suited for production use.

## Installing the tasks

To get started, install the [Microsoft.EntityFrameworkCore.Tasks](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tasks) NuGet package. For example:

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Tasks
```

> [!NOTE]
> **Every** project that needs to be compiled with generated files should reference the NuGet package, it is not transitive by default.

## Using the tasks

If the project specifies `<PublishAot>true</PublishAot>` then by default the MSBuild task will generate a compiled model and precompiled queries during publishing. Otherwise, you can set the following properties to control the generation behavior:

| MSBuild property   | Description                                                                                                                                                                                                     |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| EFOptimizeContext  | Set to `true` to enable MSBuild integration.                                                                                                                                                                |
| EFScaffoldModelStage | Set to `publish`, `build` or `none` to indicate at which stage the compiled model will be generated. Defaults to `publish`.                        |
| EFPrecompileQueriesStage | Set to `publish`, `build` or `none` to indicate at which stage the precompiled queries will be generated. Defaults to `publish`.                        |
| DbContextName      | The derived `DbContext` class to use. Class name only or fully qualified with namespaces. If this option is omitted, EF Core will perform generation for all context classes in the project. |
| EFTargetNamespace  | The namespace to use for all generated classes. If this option is omitted, EF Core will use `$(RootNamespace)`.                      |
| EFOutputDir        | The folder to put the generated files before the project is compiled. If this option is omitted, EF Core will use `$(IntermediateOutputPath)`.                      |
| EFNullable        | Whether nullable reference types will be used in the generated code. If this option is omitted, EF Core will use `$(Nullable)`.                      |

## Limitations

* When using the integration during the `publish` stage also set the rid in the startup project (e.g. \<RuntimeIdentifier\>win-x64\</RuntimeIdentifier\>)
* A different startup project cannot be specified when using this approach as it would introduce an inverse build dependency. This means that the context project needs to be autosuficient in terms of configuration, so if your app normally configures the context using a host builder in a different project you'd need to [implement _IDesignTimeDbContextFactory&lt;TContext&gt;_](xref:core/cli/dbcontext-creation#from-a-design-time-factory) in the context project.
* Since the project needs to be compilable before the compiled model is generated this approach doesn't support partial method implementations for customization of the compiled model.
* Currently, this will always generate additional code in the compiled model that's required for NativeAOT. If you are not planning to enable NativeAOT then [generate the compiled model using the CLI tools](xref:core/cli/dotnet#optimize).

## Additional resources

* [Compiled models](xref:core/performance/advanced-performance-topics#compiled-models)
