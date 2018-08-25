---
title: Installing EF Core
author: divega
ms.date: 08/06/2017
ms.assetid: 608cc774-c570-4809-8a3e-cd2c8446b8b2
uid: core/get-started/install/index
---
# Installing EF Core

## Prerequisites

In order to develop .NET Core 2.1 applications (including ASP.NET Core 2.1 applications that target .NET Core) you will need to download and install a version of the [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) that is appropriate to your platform. **This is true even if you have installed Visual Studio 2017 version 15.7.**

In order to use EF Core 2.1 or any other .NET Standard 2.0 library with a .NET platform besides .NET Core 2.1 (for example, with .NET Framework 4.6.1 or greater) you will need a version of NuGet that is aware of the .NET Standard 2.0 and its compatible frameworks. Here are a few ways you can obtain this:

* Install Visual Studio 2017 version 15.7
* If you are using Visual Studio 2015, [download and upgrade NuGet client to version 3.6.0](https://www.nuget.org/downloads)

Projects created with previous versions of Visual Studio and targeting .NET Framework may need additional modifications in order to be compatible with .NET Standard 2.0 libraries:

* Edit the project file and make sure the following entry appears in the initial property group:
  ``` xml
  <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  ```

* For test projects, also make sure the following entry is present:
  ``` xml
  <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
  ```

## Getting the bits
The recommended way to add EF Core runtime libraries into an application is to install an EF Core database provider from NuGet.

Besides the runtime libraries, you can install tools which make it easier to perform several EF Core-related tasks in your project at design time, such as creating and applying migrations, and creating a model based on an existing database.

> [!TIP]  
> If you need to update an application that is using a third-party database provider, always check for an update of the provider that is compatible with the version of EF Core you want to use. For example, database providers for previous versions are not compatible with version 2.1 of the EF Core runtime.  

> [!TIP]  
> Applications targeting ASP.NET Core 2.1 can use EF Core 2.1 without additional dependencies besides third party database providers. Applications targeting previous versions of ASP.NET Core need to upgrade to ASP.NET Core 2.1 in order to use EF Core 2.1.

<a name="cli"></a>
### Cross-platform development using the .NET Core Command Line Interface (CLI)

To develop applications that target [.NET Core](https://www.microsoft.com/net/download/core) you can choose to use the [`dotnet` CLI commands](https://docs.microsoft.com/dotnet/core/tools/) in combination with your favorite text editor, or an Integrated Development Environment (IDE) such as Visual Studio, Visual Studio for Mac, or Visual Studio Code.

> [!IMPORTANT]  
> Applications that target .NET Core require specific versions of Visual Studio. For example, .NET Core 1.x development requires Visual Studio 2017, while .NET Core 2.1 development requires Visual Studio 2017 version 15.7.

To install or upgrade the SQL Server provider in a cross-platform .NET Core application, switch to the application's directory and run the following in a command line:

``` Console
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

You can indicate a specific version install in the `dotnet add package` command, using the `-v` modifier. For example, to install EF Core 2.1 packages, append `-v 2.1.0` to the command.

EF Core includes a set of [additional commands for the `dotnet` CLI](../../miscellaneous/cli/dotnet.md), starting with `dotnet ef`. The .NET Core CLI tools for EF Core require a package called `Microsoft.EntityFrameworkCore.Design`. You can add it to the project using:

 ``` Console	
dotnet add package Microsoft.EntityFrameworkCore.Design	
```	

> [!IMPORTANT]  	
> Always use the version of the tools package that matches the major version of the runtime packages.

<a name="visual-studio"></a>
### Visual Studio development

You can develop many different types of applications that target .NET Core, .NET Framework, or other platforms supported by EF Core using Visual Studio.

There are two ways you can install an EF Core database provider in your application from Visual Studio:

#### Using NuGet's [Package Manager User Interface](https://docs.microsoft.com/nuget/tools/package-manager-ui)

* Select on the menu **Project > Manage NuGet Packages**

* Click on the **Browse** or the **Updates** tab

* Select the `Microsoft.EntityFrameworkCore.SqlServer` package and the desired version and confirm

#### Using NuGet's [Package Manager Console (PMC)](https://docs.microsoft.com/nuget/tools/package-manager-console)

* Select on the menu **Tools > NuGet Package Manager > Package Manager Console**

* Type and run the following command in the PMC:

  ``` PowerShell  
  Install-Package Microsoft.EntityFrameworkCore.SqlServer
  ```
* You can use the `Update-Package` command instead to update a package that is already installed to a more recent  version

* To specify a specific version, you can use the `-Version` modifier. For example, to install EF Core 2.1 packages, append `-Version 2.1.0` to the commands

#### Tools

There is also a PowerShell version of the [EF Core commands which run inside the PMC](../../miscellaneous/cli/powershell.md) in Visual Studio, with similar capabilities to the `dotnet ef` commands. 

> [!TIP]  
> Although it is possible to use the `dotnet ef` commands from the PMC in Visual Studio, it is far more convenient to use the PowerShell version:
> * They automatically work with the current project selected in the PMC without requiring manually switching directories.  
> * They automatically open files generated by the commands in Visual Studio after the command is completed.

> [!IMPORTANT]  
> **Deprecated packages in EF Core 2.1:** If you're upgrading an existing application to EF Core 2.1, some references to older EF Core packages may need to be removed manually:
> * Database provider design-time packages such as `Microsoft.EntityFrameworkCore.SqlServer.Design` are no longer required or supported in EF Core 2.1, but will not be automatically removed when upgrading the other packages.
> * The .NET CLI tools are now included in the .NET SDK, so the reference to that package can be removed from the *.csproj* file:
>   ```
>   <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
>   ```
