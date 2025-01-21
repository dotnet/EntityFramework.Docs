---
title: Enum Support - Code First - EF6
description: Enum Support - Code First in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/code-first/data-types/enums
---
# Enum Support - Code First
> [!NOTE]
> **EF5 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 5. If you are using an earlier version, some or all of the information does not apply.

This video and step-by-step walkthrough shows how to use enum types with Entity Framework Code First. It also demonstrates how to use enums in a LINQ query.

This walkthrough will use Code First to create a new database, but you can also use [Code First to map to an existing database](xref:ef6/modeling/code-first/workflows/existing-database).

Enum support was introduced in Entity Framework 5. To use the new features like enums, spatial data types, and table-valued functions, you must target .NET Framework 4.5. Visual Studio 2012 targets .NET 4.5 by default.

In Entity Framework, an enumeration can have the following underlying types: **Byte**, **Int16**, **Int32**, **Int64** , or **SByte**.

## Watch the video
This video shows how to use enum types with Entity Framework Code First. It also demonstrates how to use enums in a LINQ query.

**Presented By**: Julia Kornich

**Video**: [WMV](https://download.microsoft.com/download/A/5/8/A583DEE8-FD5C-47EE-A4E1-966DDF39D1DA/HDI-ITPro-MSDN-winvideo-enumwithcodefirst.wmv) | [MP4](https://download.microsoft.com/download/A/5/8/A583DEE8-FD5C-47EE-A4E1-966DDF39D1DA/HDI-ITPro-MSDN-mp4video-enumwithcodefirst.m4v) | [WMV (ZIP)](https://download.microsoft.com/download/A/5/8/A583DEE8-FD5C-47EE-A4E1-966DDF39D1DA/HDI-ITPro-MSDN-winvideo-enumwithcodefirst.zip)

## Pre-Requisites

You will need to have Visual Studio 2012, Ultimate, Premium, Professional, or Web Express edition installed to complete this walkthrough.

 

## Set up the Project

1.  Open Visual Studio 2012
2.  On the **File** menu, point to **New**, and then click **Project**
3.  In the left pane, click **Visual C\#**, and then select the **Console** template
4.  Enter **EnumCodeFirst** as the name of the project and click **OK**

## Define a New Model using Code First

When using Code First development you usually begin by writing .NET Framework classes that define your conceptual (domain) model. The code below defines the Department class.

The code also defines the DepartmentNames enumeration. By default, the enumeration is of **int** type. The Name property on the Department class is of the DepartmentNames type.

Open the Program.cs file and paste the following class definitions.

``` csharp
public enum DepartmentNames
{
    English,
    Math,
    Economics
}     

public partial class Department
{
    public int DepartmentID { get; set; }
    public DepartmentNames Name { get; set; }
    public decimal Budget { get; set; }
}
```
 

## Define the DbContext Derived Type

In addition to defining entities, you need to define a class that derives from DbContext and exposes DbSet&lt;TEntity&gt; properties. The DbSet&lt;TEntity&gt; properties let the context know which types you want to include in the model.

An instance of the DbContext derived type manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database.

The DbContext and DbSet types are defined in the EntityFramework assembly. We will add a reference to this DLL by using the EntityFramework NuGet package.

1.  In Solution Explorer, right-click on the project name.
2.  Select **Manage NuGet Packages…**
3.  In the Manage NuGet Packages dialog, Select the **Online** tab and choose the **EntityFramework** package.
4.  Click **Install**

Note, that in addition to the EntityFramework  assembly, references to System.ComponentModel.DataAnnotations and System.Data.Entity assemblies are added as well.

At the top of the Program.cs file, add the following using statement:

``` csharp
using System.Data.Entity;
```

In the Program.cs add the context definition. 

``` csharp
public partial class EnumTestContext : DbContext
{
    public DbSet<Department> Departments { get; set; }
}
```
 

## Persist and Retrieve Data

Open the Program.cs file where the Main method is defined. Add the following code into the Main function. The code adds a new Department object to the context. It then saves the data. The code also executes a LINQ query that returns a Department where the name is DepartmentNames.English.

``` csharp
using (var context = new EnumTestContext())
{
    context.Departments.Add(new Department { Name = DepartmentNames.English });

    context.SaveChanges();

    var department = (from d in context.Departments
                        where d.Name == DepartmentNames.English
                        select d).FirstOrDefault();

    Console.WriteLine(
        "DepartmentID: {0} Name: {1}",
        department.DepartmentID,  
        department.Name);
}
```

Compile and run the application. The program produces the following output:

``` csharp
DepartmentID: 1 Name: English
```
 

## View the Generated Database

When you run the application the first time, the Entity Framework creates a database for you. Because we have Visual Studio 2012 installed, the database will be created on the LocalDB instance. By default, the Entity Framework names the database after the fully qualified name of the derived context (for this example that is **EnumCodeFirst.EnumTestContext**). The subsequent times the existing database will be used.  

Note, that if you make any changes to your model after the database has been created, you should use Code First Migrations to update the database schema. See [Code First to a New Database](xref:ef6/modeling/code-first/workflows/new-database) for an example of using Migrations.

To view the database and data, do the following:

1.  In the Visual Studio 2012 main menu, select **View** -&gt; **SQL Server Object Explorer**.
2.  If LocalDB is not in the list of servers, click the right mouse button on **SQL Server** and select **Add SQL Server**
    Use the default **Windows Authentication** to connect to the LocalDB instance
3.  Expand the LocalDB node
4.  Unfold the **Databases** folder to see the new database and browse to the **Department** table
    Note, that Code First does not create a table that maps to the enumeration type
5.  To view data, right-click on the table and select **View Data**

## Summary

In this walkthrough we looked at how to use enum types with Entity Framework Code First. 
