---
title: Installing EF Core 2.0 | Microsoft Docs
author: divega
ms.author: divega

ms.date: 08/06/2017

ms.assetid: 608cc774-c570-4809-8a3e-cd2c8446b8b2
ms.technology: entity-framework-core

uid: core/get-started/general/efcore2
---
# Installing EF Core 2.0

## Runtime

You can install EF Core 2.0 by installing a compatible version of an EF Core data provider from NuGet. E.g. you can install or upgrade the SQL Server provider in cross-platform .NET Core application executing this on the command line:

``` console
$ dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 2.0.0
```

For any type of application using Visual Studio’s Package Manager Console:

``` console
PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 2.0.0
```

Or to upgrade to EF Core 2.0 from the Package Manager Console:

``` console
PM> Update-Package Microsoft.EntityFrameworkCore.SqlServer -Version 2.0.0
```

If you need to update an application that is using a third-party data provider, check for an update of the provider that is compatible with EF Core 2.0. Data providers for previous versions are not compatible with version 2.0.

Applications targeting ASP.NET Core 2.0 can use EF Core 2.0 without additional dependencies besides third party data providers (applications targeting previous versions of ASP.NET Core will need to upgrade to ASP.NET Core 2.0).

## Tooling

If the application targets [.NET Core 2.0](https://www.microsoft.com/net/download/core), install any pre-requisite updates to your development tools of choice, e.g. Visual Studio 2017 15.3, Visual Studio 2017 for Mac or Visual Studio Code, etc.

To use the `dotnet ef` command line tools in cross-platoform development, your application’s `csproj` file should contain the following:

``` xml
<ItemGroup>
  <DotNetCliToolReference
      Include="Microsoft.EntityFrameworkCore.Tools.DotNet"
      Version="2.0.0" />
</ItemGroup>
```

The Package Manager Console EF Core commands for Visual Studio can be upgraded by issuing the following command:

``` console
PM> Update-Package Microsoft.EntityFrameworkCore.Tools -Version 2.0.0
```

If your existing project references any of the tooling and design packages, make sure you update the version to 2.0.

## Deprecated packages

If you are upgrading an existing application, some references to older EF Core packages may need to be removed manually. In particular, data provider design-time packages such as `Microsoft.EntityFrameworkCore.SqlServer.Design` are no longer required or supported in EF Core 2.0, but will not be automatically removed when upgrading the other packages.
