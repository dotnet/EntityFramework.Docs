---
title: Command-Line Reference - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/06/2017
---
Entity Framework Core Tools
===========================
The Entity Framework Core Tools help you during the development of EF Core apps. They're primarily used to scaffold a
DbContext and entity types by reverse engineering the schema of a database, and to manage Migrations.

The [EF Core Package Manager Console (PMC) Tools][1] provide a superior experience inside Visual Studio. Run them using
NuGet's [Package Manager Console][2]. These tools work with both .NET Framework and .NET Core projects.

The [EF Core .NET Command-line Tools][3] are an extension to the [.NET Core command-line interface (CLI) tools][4] that
are cross-platform and can run outside of Visual Studio. These tools require a .NET Core SDK project (one with
`Sdk="Microsoft.NET.Sdk"` or similar in the project file).

Both tools expose the same functionality. If you're developing in Visual Studio, we recommend using the PMC Tools since
they provide a more integrated experience.

Frameworks
----------
The tools support projects targeting .NET Framework or .NET Core.

If you want to use a class library, then consider using a .NET Core or .NET Framework class library if possible. This will result in the least issues with .NET tooling. If instead you wish to use a .NET Standard class library, then you will need to use a startup project that targets .NET Framework or .NET Core so that the tooling has a conrete target platform into which it can load your class library. This startup project can be a dummy project with no real code--it is only needed to provide a target for the tooling.

If your project targets another framework (for example, Universal Windows or Xamarin), then you will need to create a separate .NET Standard class library. In this case, follow the guidance above to also create a startup project that can be used by the tooling.

Startup and Target Projects
---------------------------
Whenever you invoke a command, there are two projects involved: the target project and the startup project.

The target project is where any files are added (or in some cases removed).

The startup project is the one emulated by the tools when executing your project's code.

Both the target project and the startup project can be the same.


  [1]: powershell.md
  [2]: https://docs.microsoft.com/nuget/tools/package-manager-console
  [3]: dotnet.md
  [4]: https://docs.microsoft.com/dotnet/core/tools/
