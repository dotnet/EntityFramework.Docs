---
title: Entity Framework Core tools reference - EF Core
description: Reference guide for the Entity Framework Core CLI tool and the Visual Studio Package Manager Console
author: bricelam
ms.date: 09/19/2018
uid: core/miscellaneous/cli/index
---

# Entity Framework Core tools reference

The Entity Framework Core tools help with design-time development tasks. They're primarily used to manage Migrations and to scaffold a
`DbContext` and entity types by reverse engineering the schema of a database.

* The [EF Core Package Manager Console tools](xref:core/miscellaneous/cli/powershell) run in
the [Package Manager Console](/nuget/tools/package-manager-console) in Visual Studio.

* The [EF Core .NET command-line interface (CLI) tools](xref:core/miscellaneous/cli/dotnet) are an extension to the cross-platform [.NET Core CLI tools](/dotnet/core/tools/). These tools require a .NET Core SDK project (one with `Sdk="Microsoft.NET.Sdk"` or similar in the project file).

Both tools expose the same functionality. If you're developing in Visual Studio, we recommend using the **Package Manager Console** tools since
they provide a more integrated experience.

## Next steps

* [EF Core Package Manager Console tools reference](xref:core/miscellaneous/cli/powershell)
* [EF Core .NET CLI tools reference](xref:core/miscellaneous/cli/dotnet)
