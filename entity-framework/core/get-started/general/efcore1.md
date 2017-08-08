---
title: Installing EF Core 1.x | Microsoft Docs
author: divega
ms.author: divega

ms.date: 08/06/2017

ms.assetid: e1bff2e3-6ea1-41e1-9ab1-d95ed2d86621
ms.technology: entity-framework-core

uid: core/get-started/general/efcore1
---
# Installing EF Core 1.x

## Runtime

You can install EF Core 1.x by installing a compatible version of an EF Core data provider from NuGet. E.g. you can install or upgrade the SQL Server provider in cross-platform .NET Core application executing this on the command line:

``` console
$ dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 1.1.2
```

For any type of application using Visual Studio’s Package Manager Console:

``` console
PM> install-package Microsoft.EntityFrameworkCore.SqlServer -Version 1.1.2
```

Or to upgrade to EF Core 1.1.2 from the Package Manager Console:

``` console
PM> update-package Microsoft.EntityFrameworkCore.SqlServer -Version 1.1.2
```

If you need to update an application that is using a third-party data provider, check for an update of the provider that is compatible with EF Core 1.x.

## Tooling

Install any pre-requisite updates to your development tools of choice, e.g. Visual Studio 2017, Visual Studio 2017 for Mac or Visual Studio Code, etc.

To use the `dotnet ef` command line tools in cross-platoform development, your application’s `csproj` file should contain the following:

``` xml
<ItemGroup>
  <DotNetCliToolReference
      Include="Microsoft.EntityFrameworkCore.Tools.DotNet"
      Version="1.0.1" />
</ItemGroup>
```

The Package Manager Console EF Core commands for Visual Studio can be upgraded by issuing the following command:

``` console
PM> update-package Microsoft.EntityFrameworkCore.Tools -Version 1.1.2
```

Some design-time functionality such as `DbContext` scaffolding  requires installing the design-time package for the specific database provider. E.g. for the SQL Server provider you can use:  

``` console
$ dotnet add package Microsoft.EntityFrameworkCore.SqlServer.Design -v 1.1.2
```

Or from the Package Manager Console:

``` console
PM> install-package Microsoft.EntityFrameworkCore.SqlServer.Design -Version 1.1.2
```
