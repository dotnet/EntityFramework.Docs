---
title: Upgrading from EF Core 1.0 RC2 to RTM - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: c3c1940b-136d-45d8-aa4f-cb5040f8980a
uid: core/miscellaneous/rc2-rtm-upgrade
---
# Upgrading from EF Core 1.0 RC2 to RTM

This article provides guidance for moving an application built with the RC2 packages to 1.0.0 RTM.

## Package Versions

The names of the top level packages that you would typically install into an application did not change between RC2 and RTM.

**You need to upgrade the installed packages to the RTM versions:**

* Runtime packages (for example, `Microsoft.EntityFrameworkCore.SqlServer`) changed from `1.0.0-rc2-final` to `1.0.0`.

* The `Microsoft.EntityFrameworkCore.Tools` package changed from `1.0.0-preview1-final` to `1.0.0-preview2-final`. Note that tooling is still pre-release.

## Existing migrations may need maxLength added

In RC2, the column definition in a migration looked like `table.Column<string>(nullable: true)` and the length of the column was looked up in some metadata we store in the code behind the migration. In RTM, the length is now included in the scaffolded code `table.Column<string>(maxLength: 450, nullable: true)`.

Any existing migrations that were scaffolded prior to using RTM will not have the `maxLength` argument specified. This means the maximum length supported by the database will be used (`nvarchar(max)` on SQL Server). This may be fine for some columns, but columns that are part of a key, foreign key, or index need to be updated to include a maximum length. By convention, 450 is the maximum length used for keys, foreign keys, and indexed columns. If you have explicitly configured a length in the model, then you should use that length instead.

**ASP.NET Identity**

This change impacts projects that use ASP.NET Identity and were created from a pre-RTM project template. The project template includes a migration used to create the database. This migration must be edited to specify a maximum length of `256` for the following columns.

*  **AspNetRoles**

    * Name

    * NormalizedName

*  **AspNetUsers**

   * Email

   * NormalizedEmail

   * NormalizedUserName

   * UserName

Failure to make this change will result in the following exception when the initial migration is applied to a database.

    System.Data.SqlClient.SqlException (0x80131904): Column 'NormalizedName' in table 'AspNetRoles' is of a type that is invalid for use as a key column in an index.

## .NET Core: Remove "imports" in project.json

If you were targeting .NET Core with RC2, you needed to add `imports` to project.json as a temporary workaround for some of EF Core's dependencies not supporting .NET Standard. These can now be removed.

``` json
{
  "frameworks": {
    "netcoreapp1.0": {
      "imports": ["dnxcore50", "portable-net451+win8"]
    }
  }
}
```

> [!NOTE]  
> As of version 1.0 RTM, the [.NET Core SDK](https://www.microsoft.com/net/download/core) no longer supports `project.json` or developing .NET Core applications using Visual Studio 2015. We recommend you [migrate from project.json to csproj](https://docs.microsoft.com/dotnet/articles/core/migration/). If you are using Visual Studio, we recommend you upgrade to [Visual Studio 2017](https://www.visualstudio.com/downloads/).

## UWP: Add binding redirects

Attempting to run EF commands on Universal Windows Platform (UWP) projects results in the following error:

    System.IO.FileLoadException: Could not load file or assembly 'System.IO.FileSystem.Primitives, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference.

You need to manually add binding redirects to the UWP project. Create a file named `App.config` in the project root folder and add redirects to the correct assembly versions.

``` xml
<configuration>
 <runtime>
   <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
     <dependentAssembly>
       <assemblyIdentity name="System.IO.FileSystem.Primitives"
                         publicKeyToken="b03f5f7f11d50a3a"
                         culture="neutral" />
       <bindingRedirect oldVersion="4.0.0.0"
                        newVersion="4.0.1.0"/>
     </dependentAssembly>
     <dependentAssembly>
       <assemblyIdentity name="System.Threading.Overlapped"
                         publicKeyToken="b03f5f7f11d50a3a"
                         culture="neutral" />
       <bindingRedirect oldVersion="4.0.0.0"
                        newVersion="4.0.1.0"/>
     </dependentAssembly>
   </assemblyBinding>
 </runtime>
</configuration>
```
