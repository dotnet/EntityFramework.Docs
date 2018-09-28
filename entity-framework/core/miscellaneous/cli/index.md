---
title: Entity Framework Core tools reference - EF Core
author: bricelam
ms.author: bricelam
ms.date: 09/19/2018
uid: core/miscellaneous/cli/index
---

# Entity Framework Core tools reference

The Entity Framework Core tools help with design-time development tasks. They're primarily used to manage Migrations and to scaffold a
`DbContext` and entity types by reverse engineering the schema of a database.

* The [EF Core Package Manager Console tools](powershell.md)) run in
the [Package Manager Console](https://docs.microsoft.com/nuget/tools/package-manager-console) in Visual Studio. These tools work with both .NET Framework and .NET Core projects.

* The [EF Core .NET command-line interface (CLI) tools](dotnet.md) are an extension to the cross-platform [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/). These tools require a .NET Core SDK project (one with `Sdk="Microsoft.NET.Sdk"` or similar in the project file).

Both tools expose the same functionality. If you're developing in Visual Studio, we recommend using the **Package Manager Console** tools since
they provide a more integrated experience.

## Next steps

* [EF Core Package Manager Console tools reference](powershell.md)
* [EF Core .NET CLI tools reference](dotnet.md)