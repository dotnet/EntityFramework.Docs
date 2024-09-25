---
title: Installing Entity Framework Core - EF Core
description: Installation instructions for Entity Framework Core
author: SamMonoRT
ms.date: 03/23/2023
uid: core/get-started/overview/install
---
# Installing Entity Framework Core

## Prerequisites

* EF requires the most recent [.NET SDK](https://dotnet.microsoft.com/en-us/download).
  * At runtime, EF Core requires a recent version of .NET. See [_EF Core releases_](xref:core/what-is-new/index) to find the minimal .NET version needed for the version of EF Core that you want to use.

* You can use EF Core to develop applications on Windows using Visual Studio. The latest version of [Visual Studio](https://visualstudio.microsoft.com/vs) is recommended.

## Get Entity Framework Core

EF Core is shipped as [NuGet packages](https://www.nuget.org/). To add EF Core to an application, install the NuGet package for the database provider you want to use. See [_Providers_](xref:core/providers/index) for a list of the database providers available.

To install or update NuGet packages, you can use the .NET Core command-line interface (CLI), the Visual Studio Package Manager Dialog, or the Visual Studio Package Manager Console.

### .NET Core CLI

* Use the following .NET Core CLI command from the operating system's command line to install or update the EF Core SQL Server provider:

  ```dotnetcli
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  ```

* You can indicate a specific version in the `dotnet add package` command, using the `-v` modifier. For example, to install EF Core 6.0.14 packages, append `-v 6.0.14` to the command.

For more information, see [.NET command-line interface (CLI) tools](/dotnet/core/tools/).

### Visual Studio NuGet Package Manager Dialog

* From the Visual Studio menu, select **Project > Manage NuGet Packages**

* Click on the **Browse** or the **Updates** tab

* To install or update the SQL Server provider, select the `Microsoft.EntityFrameworkCore.SqlServer` package, and confirm.

For more information, see [NuGet Package Manager Dialog](/nuget/tools/package-manager-ui).

### Visual Studio NuGet Package Manager Console

* From the Visual Studio menu, select **Tools > NuGet Package Manager > Package Manager Console**

* To install the SQL Server provider, run the following command in the Package Manager Console:

  ```powershell
  Install-Package Microsoft.EntityFrameworkCore.SqlServer
  ```

* To update the provider, use the `Update-Package` command.

* To specify a specific version, use the `-Version` modifier. For example, to install EF Core 6.0.14 packages, append `-Version 6.0.14` to the commands

For more information, see [Package Manager Console](/nuget/tools/package-manager-console).

## Get the Entity Framework Core tools

You can install tools to carry out EF Core-related tasks in your project, like creating and applying database migrations, or creating an EF Core model based on an existing database.

Two sets of tools are available:

* The [.NET Core command-line interface (CLI) tools](xref:core/cli/dotnet) can be used on Windows, Linux, or macOS. These commands begin with `dotnet ef`.

* The [Package Manager Console (PMC) tools](xref:core/cli/powershell) run in Visual Studio on Windows. These commands start with a verb, for example `Add-Migration`, `Update-Database`.

<a name="cli"></a>

### Get the .NET Core CLI tools

.NET Core CLI tools require the .NET Core SDK, mentioned earlier in [Prerequisites](#prerequisites).

* `dotnet ef` must be installed as a global or local tool. Most developers prefer installing `dotnet ef` as a global tool using the following command:

  ```dotnetcli
  dotnet tool install --global dotnet-ef
  ```

  `dotnet ef` can also be used as a local tool. To use it as a local tool, restore the dependencies of a project that declares it as a tooling dependency using a [tool manifest file](/dotnet/core/tools/global-tools#install-a-local-tool).

* To update the tools, use the `dotnet tool update` command.

* Install the latest `Microsoft.EntityFrameworkCore.Design` package.

  ```dotnetcli
  dotnet add package Microsoft.EntityFrameworkCore.Design
  ```

> [!IMPORTANT]
> Always use the version of the tools package that matches the major version of the runtime packages.

### Get the Package Manager Console tools

To get the Package Manager Console tools for EF Core, install the `Microsoft.EntityFrameworkCore.Tools` package. For example, from Visual Studio:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
```

## Upgrading to the latest EF Core

* Any time we release a new version of EF Core, we also release a new version of the providers that are part of the EF Core project, like [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer), [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite), [Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Cosmos), and [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory). You can just upgrade to the new version of the provider to get all the improvements.

* If you need to update an application that is using a third-party database provider, always check for an update of the provider that is compatible with the version of EF Core you want to use. For example, database providers for version 1.0 are not compatible with version 2.0 of the EF Core runtime, and so on.

* Third-party providers for EF Core usually don't release patch versions alongside the EF Core runtime. To upgrade an application that uses a third-party provider to a patch version of EF Core, you may need to add a direct reference to individual EF Core runtime components, most notably [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/), and [Microsoft.EntityFrameworkCore.Relational](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational/).
