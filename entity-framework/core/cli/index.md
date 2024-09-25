---
title: Entity Framework Core tools reference - EF Core
description: Reference guide for the Entity Framework Core CLI tool and the Visual Studio Package Manager Console
author: SamMonoRT
ms.date: 09/19/2018
uid: core/cli/index
---

# Entity Framework Core tools reference

The Entity Framework Core tools help with design-time development tasks. They're primarily used to manage Migrations and to scaffold a `DbContext` and entity types by reverse engineering the schema of a database.

Either of the following tools can be installed, as both tools expose the same functionality:

* The [EF Core Package Manager Console tools](xref:core/cli/powershell) run in the [Package Manager Console](/nuget/tools/package-manager-console) in Visual Studio. We recommend using these tools if you are developing in Visual Studio as they provide a more integrated experience.

* The [EF Core .NET command-line interface (CLI) tools](xref:core/cli/dotnet) are an extension to the cross-platform [.NET Core CLI tools](/dotnet/core/tools/). These tools require a .NET Core SDK project (one with `Sdk="Microsoft.NET.Sdk"` or similar in the project file).

## Next steps

* [EF Core Package Manager Console tools reference](xref:core/cli/powershell)
* [EF Core .NET CLI tools reference](xref:core/cli/dotnet)
