---
title: Visual Studio Releases - EF6
description: Visual Studio Releases and Entity Framework 6
author: SamMonoRT
ms.date: 07/05/2018
uid: ef6/what-is-new/visual-studio
---
# Visual Studio Releases

We recommend to always use the latest version of Visual Studio because it contains the latest tools for .NET, NuGet, and Entity Framework.
In fact, the various samples and walkthroughs across the Entity Framework documentation assume that you are using a recent version of Visual Studio.

It is possible however to use older versions of Visual Studio with different versions of Entity Framework as long as you take into account some differences:

## Visual Studio 2017 15.7 and newer

- This version of Visual Studio includes the latest release of Entity Framework tools and the EF 6.2 runtime, and does not require additional setup steps.
See [What's New](xref:ef6/what-is-new/index) for more details on these releases.
- Adding Entity Framework to new projects using the EF tools will automatically add the EF 6.2 NuGet package.
You can manually install or upgrade to any EF NuGet package available online.
- By default, the SQL Server instance available with this version of Visual Studio is a LocalDB instance called MSSQLLocalDB.
The server section of connection string you should use is "(localdb)\\MSSQLLocalDB".
Remember to use a verbatim string prefixed with `@` or double back-slashes "\\\\" when specifying a connection string in C# code.  


## Visual Studio 2015 to Visual Studio 2017 15.6

- These versions of Visual Studio include Entity Framework tools and runtime 6.1.3.
See [Past Releases](xref:ef6/what-is-new/past-releases#ef-613) for more details on these releases.
- Adding Entity Framework to new projects using the EF tools will automatically add the EF 6.1.3 NuGet package.
You can manually install or upgrade to any EF NuGet package available online.
- By default, the SQL Server instance available with this version of Visual Studio is a LocalDB instance called MSSQLLocalDB.
The server section of connection string you should use is "(localdb)\\MSSQLLocalDB".
Remember to use a verbatim string prefixed with `@` or double back-slashes "\\\\" when specifying a connection string in C# code.  


## Visual Studio 2013
- This version of Visual Studio includes and older version of Entity Framework tools and runtime.
It is recommended that you upgrade to Entity Framework Tools 6.1.3, using the installer.
See [Past Releases](xref:ef6/what-is-new/past-releases#ef-613) for more details on these releases.
- Adding Entity Framework to new projects using the upgraded EF tools will automatically add the EF 6.1.3 NuGet package.
You can manually install or upgrade to any EF NuGet package available online.
- By default, the SQL Server instance available with this version of Visual Studio is a LocalDB instance called MSSQLLocalDB.
The server section of connection string you should use is "(localdb)\\MSSQLLocalDB".
Remember to use a verbatim string prefixed with `@` or double back-slashes "\\\\" when specifying a connection string in C# code.  

## Visual Studio 2012

- This version of Visual Studio includes and older version of Entity Framework tools and runtime.
It is recommended that you upgrade to Entity Framework Tools 6.1.3, using the installer.
See [Past Releases](xref:ef6/what-is-new/past-releases#ef-613) for more details on these releases.
- Adding Entity Framework to new projects using the upgraded EF tools will automatically add the EF 6.1.3 NuGet package.
You can manually install or upgrade to any EF NuGet package available online.
- By default, the SQL Server instance available with this version of Visual Studio is a LocalDB instance called v11.0.
The server section of connection string you should use is "(localdb)\\v11.0".
Remember to use a verbatim string prefixed with `@` or double back-slashes "\\\\" when specifying a connection string in C# code.  

## Visual Studio 2010

- The version of Entity Framework Tools available with this version of Visual Studio is not compatible with the Entity Framework 6 runtime and cannot be upgraded.
- By default, the Entity Framework tools will add Entity Framework 4.0 to your projects.
In order to create applications using any newer versions of EF, you will first need to install the [NuGet Package Manager extension](https://marketplace.visualstudio.com/items?itemName=NuGetTeam.NuGetPackageManager).
- By default, all code generation in the version of EF tools is based on EntityObject and Entity Framework 4.
We recommend that you switch the code generation to be based on DbContext and Entity Framework 5, by installing the DbContext code generation templates for [C#](https://marketplace.visualstudio.com/items?itemName=EntityFrameworkTeam.EF5xDbContextGeneratorforC) or [Visual Basic](https://marketplace.visualstudio.com/items?itemName=EntityFrameworkTeam.EF5xDbContextGeneratorforVBNET).
- Once you have installed the NuGet Package Manager extensions, you can manually install or upgrade to any EF NuGet package available online and use EF6 with Code First, which does not require a designer.
- By default, the SQL Server instance available with this version of Visual Studio is SQL Server Express named SQLEXPRESS.
The server section of connection string you should use is ".\\SQLEXPRESS".
Remember to use a verbatim string prefixed with `@` or double back-slashes "\\\\" when specifying a connection string in C# code.
