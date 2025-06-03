---
title: What's new - EF6
description: What's new in Entity Framework 6
author: SamMonoRT
ms.date: 05/17/2024
uid: ef6/what-is-new/index
---
# What's new in EF6

We highly recommend that you use the latest released version of Entity Framework to ensure you get the latest features and the highest stability.
However, we realize that you may need to use a previous version, or that you may want to experiment with new improvements in the latest pre-release.
To install specific versions of EF, see [Get Entity Framework](xref:ef6/fundamentals/install).

## EF 6.5.0

The EF 6.5.0 runtime was released to NuGet in June 2024. The primary goal of EF 6.5 is to include a new SQL Server / Azure SQL Database provider. See [list of important fixes](https://github.com/dotnet/ef6/milestone/17?closed=1) on Github. Here are some of the more notable ones:

- New SQL Server / Azure SQL Database provider (contributed by the community) - [Microsoft.EntityFramework.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFramework.SqlServer/). This new provider uses the modern SQL Server client [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient). For more information about configuration of this provider, see [Microsoft.EntityFramework.SqlServer Guide](xref:ef6/what-is-new/microsoft-ef6-sqlserver).
- The `ef6` utility was updated to only support .NET 6 and newer. It was also updated to support reading from app.config files, and support Windows ARM64.
- The System.Data.SqlClient driver was updated to version 4.8.6.

## EF 6.4.0

The EF 6.4.0 runtime was released to NuGet in December 2019. The primary goal of EF 6.4 is to polish the features and scenarios that was delivered in EF 6.3. See [list of important fixes](https://github.com/dotnet/ef6/milestone/14?closed=1) on Github.

## EF 6.3.0

The EF 6.3.0 runtime was released to NuGet in September 2019. The main goal of this release was to facilitate migrating existing applications that use EF 6 to .NET Core 3.0. The community has also contributed several bug fixes and enhancements. See the issues closed in each 6.3.0 [milestone](https://github.com/aspnet/EntityFramework6/milestones?state=closed) for details. Here are some of the more notable ones:

- Support for .NET Core 3.0
  - The EntityFramework package now targets .NET Standard 2.1 in addition to .NET Framework 4.x.
  - This means that EF 6.3 is cross-platform and supported on other operating systems besides Windows, like Linux and macOS.
  - The migrations commands have been rewritten to execute out of process and work with SDK-style projects.
- Support for SQL Server HierarchyId.
- Improved compatibility with Roslyn and NuGet PackageReference.
- Added `ef6.exe` utility for enabling, adding, scripting, and applying migrations from assemblies. This replaces `migrate.exe`.

### EF designer support

There's currently no support for using the EF designer directly on .NET Core or .NET Standard projects or on an SDK-style .NET Framework project. 

You can work around this limitation by adding the EDMX file and the generated classes for the entities and the DbContext as linked files to a .NET Core 3.0 or .NET Standard 2.1 project in the same solution.

The linked files will look like this in the project file:

``` csproj 
<ItemGroup>
  <EntityDeploy Include="..\EdmxDesignHost\Entities.edmx" Link="Model\Entities.edmx" />
  <Compile Include="..\EdmxDesignHost\Entities.Context.cs" Link="Model\Entities.Context.cs" />
  <Compile Include="..\EdmxDesignHost\Thing.cs" Link="Model\Thing.cs" />
  <Compile Include="..\EdmxDesignHost\Person.cs" Link="Model\Person.cs" />
</ItemGroup>
```

Note that the EDMX file is linked with the EntityDeploy build action. This is a special MSBuild task (now included in the EF 6.3 package) that takes care of adding the EF model into the target assembly as embedded resources (or copying it as files in the output folder, depending on the Metadata Artifact Processing setting in the EDMX). For more details on how to get this set up, see our [EDMX .NET Core sample](https://aka.ms/EdmxDotNetCoreSample).

Warning: make sure the old style (i.e. non-SDK-style) .NET Framework project defining the "real" .edmx file comes _before_ the project defining the link inside the .sln file. Otherwise, when you open the .edmx file in the designer, you see the error message "The Entity Framework is not available in the target framework currently specified for the project. You can change the target framework of the project or edit the model in the XmlEditor".

## Past Releases

The [Past Releases](xref:ef6/what-is-new/past-releases) page contains an archive of all previous versions of EF and the major features that were introduced on each release.
