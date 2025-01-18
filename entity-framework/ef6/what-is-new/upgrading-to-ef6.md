---
title: Upgrading to Entity Framework 6 - EF6
description: Upgrading to Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/what-is-new/upgrading-to-ef6
---
# Upgrading to Entity Framework 6

In previous versions of EF the code was split between core libraries (primarily System.Data.Entity.dll) shipped as part of the .NET Framework and out-of-band (OOB) libraries (primarily EntityFramework.dll) shipped in a NuGet package. EF6 takes the code from the core libraries and incorporates it into the OOB libraries. This was necessary in order to allow EF to be made open source and for it to be able to evolve at a different pace from .NET Framework. The consequence of this is that applications will need to be rebuilt against the moved types.

This should be straightforward for applications that make use of DbContext as shipped in EF 4.1 and later. A little more work is required for applications that make use of ObjectContext but it still isn’t hard to do.

Here is a checklist of the things you need to do to upgrade an existing application to EF6.

## 1. Install the EF6 NuGet package

You need to upgrade to the new Entity Framework 6 runtime.

1. Right-click on your project and select **Manage NuGet Packages...**  
2. Under the **Online** tab select **EntityFramework** and click **Install**  
   > [!NOTE]
   > If a previous version of the EntityFramework NuGet package was installed this will upgrade it to EF6.

Alternatively, you can run the following command from Package Manager Console:

``` powershell
Install-Package EntityFramework
```

## 2. Ensure that assembly references to System.Data.Entity.dll are removed

Installing the EF6 NuGet package should automatically remove any references to System.Data.Entity from your project for you.

## 3. Swap any EF Designer (EDMX) models to use EF 6.x code generation

If you have any models created with the EF Designer, you will need to update the code generation templates to generate EF6 compatible code.

> [!NOTE]
> There are currently only EF 6.x DbContext Generator templates available for Visual Studio 2012 and 2013.

1. Delete existing code-generation templates. These files will typically be named **\<edmx_file_name\>.tt** and **\<edmx_file_name\>.Context.tt** and be nested under your edmx file in Solution Explorer. You can select the templates in Solution Explorer and press the **Del** key to delete them.  
   > [!NOTE]
   > In Web Site projects the templates will not be nested under your edmx file, but listed alongside it in Solution Explorer.  

   > [!NOTE]
   > In VB.NET projects you will need to enable 'Show All Files' to be able to see the nested template files.
2. Add the appropriate EF 6.x code generation template. Open your model in the EF Designer, right-click on the design surface and select **Add Code Generation Item...**
    - If you are using the DbContext API (recommended) then **EF 6.x DbContext Generator** will be available under the **Data** tab.  
      > [!NOTE]
      > If you are using Visual Studio 2012, you will need to install the EF 6 Tools to have this template. See [Get Entity Framework](xref:ef6/fundamentals/install) for details.  

    - If you are using the ObjectContext API then you will need to select the **Online** tab and search for **EF 6.x EntityObject Generator**.  
3. If you applied any customizations to the code generation templates you will need to re-apply them to the updated templates.

## 4. Update namespaces for any core EF types being used

The namespaces for DbContext and Code First types have not changed. This means for many applications that use EF 4.1 or later you will not need to change anything.

Types like ObjectContext that were previously in System.Data.Entity.dll have been moved to new namespaces. This means you may need to update your *using* or *Import* directives to build against EF6.

The general rule for namespace changes is that any type in System.Data.* is moved to System.Data.Entity.Core.*. In other words, just insert **Entity.Core.** after System.Data. For example:

- System.Data.EntityException => System.Data.**Entity.Core**.EntityException  
- System.Data.Objects.ObjectContext => System.Data.**Entity.Core**.Objects.ObjectContext  
- System.Data.Objects.DataClasses.RelationshipManager => System.Data.**Entity.Core**.Objects.DataClasses.RelationshipManager  

These types are in the *Core* namespaces because they are not used directly for most DbContext-based applications. Some types that were part of System.Data.Entity.dll are still used commonly and directly for DbContext-based applications and so have not been moved into the *Core* namespaces. These are:

- System.Data.EntityState => System.Data.**Entity**.EntityState  
- System.Data.Objects.DataClasses.EdmFunctionAttribute => System.Data.**Entity.DbFunctionAttribute**  
  > [!NOTE]
  > This class has been renamed; a class with the old name still exists and works, but it now marked as obsolete.  
- System.Data.Objects.EntityFunctions => System.Data.**Entity.DbFunctions**  
  > [!NOTE]
  > This class has been renamed; a class with the old name still exists and works, but it now marked as obsolete.)  
- Spatial classes (for example, DbGeography, DbGeometry) have moved from System.Data.Spatial => System.Data.**Entity**.Spatial

> [!NOTE]
> Some types in the System.Data namespace are in System.Data.dll which is not an EF assembly. These types have not moved and so their namespaces remain unchanged.
