---
title: "Entity Framework Provider Support for Spatial Types | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: 1097cb00-15f5-453d-90ed-bff9403d23e3
caps.latest.revision: 3
---
# Entity Framework Provider Support for Spatial Types
Entity Framework supports working with spatial data through the DbGeography or DbGeometry classes. These classes rely on database-specific functionality provided by the Entity Framework provider. Not all providers support spatial data and those that do may have additional prerequisites such as the installation of low-level spatial type assemblies. More information about provider support for spatial types is provided below.  
  
Additional information on how to use spatial types in an application can be found in two walkthroughs, one for Code First, the other for Database First or Model First:  
  
- [Spatial Data Types in Code First](../ef6/entity-framework-spatial-code-first-ef5-onwards.md)  
- [Spatial Data Types in EF Designer](../ef6/entity-framework-spatial-ef-designer-ef5-onwards.md)  
  
## Entity Framework releases that support spatial types  
  
Support for spatial types was introduced in EF5. However, in EF5 spatial types are only supported when the application targets and runs on .NET 4.5.  
  
Starting with EF6 spatial types are supported for applications targeting both .NET 4 and .NET 4.5.  
  
## Entity Framework providers that support spatial types  
  
### EF5  
  
The Entity Framework providers for EF5 that we are aware of that support spatial types are:  
  
- Microsoft SQL Server provider  
    - This provider is shipped as part of EF5.  
    - This provider depends on some additional low-level libraries that may need to be installed—see below for details.  
- [Devart dotConnect for Oracle](http://www.devart.com/dotconnect/oracle/)  
    - This is a third-party provider from Devart.  
  
If you know of an EF5 provider that supports spatial types then please get in contact and we will be happy to add it to this list.  
  
### EF6  
  
The Entity Framework providers for EF6 that we are aware of that support spatial types are:  
  
- Microsoft SQL Server provider  
    - This provider is shipped as part of EF6.  
    - This provider depends on some additional low-level libraries that may need to be installed—see below for details.  
- [Devart dotConnect for Oracle](http://www.devart.com/dotconnect/oracle/)  
    - This is a third-party provider from Devart.  
  
If you know of an EF6 provider that supports spatial types then please get in contact and we will be happy to add it to this list.  
  
## Prerequisites for spatial types with Microsoft SQL Server  
  
SQL Server spatial support depends on the low-level, SQL Server-specific types SqlGeography and SqlGeometry. These types live in Microsoft.SqlServer.Types.dll assembly, and this assembly is not shipped as part of EF or as part of the .NET Framework.  
  
When Visual Studio is installed it will often also install a version of SQL Server, and this will include installation of the Microsoft.SqlServer.Types.dll.  
  
If SQL Server is not installed on the machine where you want to use spatial types, or if spatial types were excluded from the SQL Server installation, then you will need to install them manually. The types are included in the SQL Server Feature Packs, and different assemblies exist for SQL Server 2008 and SQL Server 2012:  
  
- [SQL Server 2008 Feature Pack](https://www.microsoft.com/en-gb/download/details.aspx?id=30440)  
- [SQL Server 2012 Feature Pack](https://www.microsoft.com/en-us/download/details.aspx?id=29065)  
  