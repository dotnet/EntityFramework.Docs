---
title: Command-Line Reference - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/6/2017
ms.technology: entity-framework-core
---
Entity Framework Core Tools
===========================
The Entity Framework Core Tools help you during the development of EF Core apps. They're primarily used to scaffold a
DbContext and entity types by reverse engineering the schema of a database, and to manage Migrations.

The [EF Core Package Manager Console (PMC) Tools][1] provide a superior experience inside Visual Studio. Run them using
NuGet's [Package Manager Console][2]. These tools work with both .NET Framework and .NET Core projects.

The [EF Core .NET Command Line Tools][3] are an extension to the [.NET Core command-line interface (CLI) tools][4] that
are cross-platform and can run outside of Visual Studio. These tools require a .NET Core SDK project (one with
`Sdk="Microsoft.NET.Sdk"` or similar in the project file).

Both tools expose the same functionality. If you're developing in Visual Studio, we recommend using the PMC Tools since
they provide a more integrated experience.

Frameworks
----------
The tools support projects targeting .NET Framework or .NET Core.

If your project targets another framework (e.g. Universal Windows or Xamarin), we recommend creating a separate .NET
Standard project and cross-targeting one of the supported frameworks.

To cross-target .NET Core, for example, right-click on the project and select **Edit \*.csproj**. Update the
`TargetFramework` property as follows. (Note, the property name becomes plural.)

``` xml
<TargetFrameworks>netcoreapp2.0;netstandard2.0</TargetFrameworks>
```

If you're using a .NET Standard class library, you don't need to cross-target if your startup project targets .NET
Framework or .NET Core.

Startup and Target Projects
---------------------------
Whenever you invoke a command, there are two projects involved: the target project and the startup project.

The target project is where any files are added (or in some cases removed).

The startup project is the one emulated by the tools when executing your project's code.

Both the target project and the startup project can be the same.


  [1]: pmc.md
  [2]: https://docs.microsoft.com/nuget/tools/package-manager-console
  [3]: dotnet.md
  [4]: https://docs.microsoft.com/dotnet/core/tools/
