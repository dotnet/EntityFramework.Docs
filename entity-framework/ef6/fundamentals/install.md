---
title: Get Entity Framework - EF6
description: Get Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/install
---
# Get Entity Framework
Entity Framework is made up of the EF Tools for Visual Studio and the EF Runtime.

## EF Tools for Visual Studio

The [Entity Framework Tools for Visual Studio](/visualstudio/data-tools/entity-data-model-tools-in-visual-studio) include the EF Designer and the EF Model Wizard and are required for the database first and model first workflows. EF Tools are included in all recent versions of Visual Studio. If you perform a custom install of Visual Studio you will need to ensure that the item "Entity Framework 6 Tools" is selected by either choosing a workload that includes it or by selecting it as an individual component.

For some past versions of Visual Studio, updated EF Tools are available as a download. See [Visual Studio Versions](xref:ef6/what-is-new/visual-studio) for guidance on how to get the latest version of EF Tools available for your version of Visual Studio.

## EF Runtime

The latest version of Entity Framework is available as the [EntityFramework NuGet package](https://nuget.org/packages/EntityFramework/). If you are not familiar with the NuGet Package Manager, we encourage you to read the [NuGet Overview](/nuget/consume-packages/overview-and-workflow).

### Installing the EF NuGet Package

You can install the EntityFramework package by right-clicking on the **References** folder of your project and selecting **Manage NuGet Packages…**

![Manage NuGet Packages](~/ef6/media/managenugetpackages.png)

### Installing from Package Manager Console

Alternatively, you can install EntityFramework by running the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

``` powershell
Install-Package EntityFramework
```

## Installing a specific version of EF

From EF 4.1 onwards, new versions of the EF runtime have been released as the [EntityFramework NuGet Package](https://www.nuget.org/packages/EntityFramework/). Any of those versions can be added to a .NET Framework-based project by running the following command in Visual Studio's [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console):

``` powershell
Install-Package EntityFramework -Version <number>
```

Note that `<number>` represents the specific version of EF to install. For example, 6.2.0 is the version of number for EF 6.2.   

EF runtimes before 4.1 were part of .NET Framework and cannot be installed separately.

### Installing the Latest Preview

The above methods will give you the latest fully supported release of Entity Framework. There are often prerelease versions of Entity Framework available that we would love you to try out and give us feedback on.

To install the latest preview of EntityFramework you can select **Include Prerelease** in the Manage NuGet Packages window. If no prerelease versions are available you will automatically get the latest fully supported version of Entity Framework.

![Include PreRelease](~/ef6/media/includeprerelease.png)

Alternatively, you can run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

``` powershell
Install-Package EntityFramework -Pre
```
