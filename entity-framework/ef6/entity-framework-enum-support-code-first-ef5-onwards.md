---
title: "Entity Framework Enum Support - Code First (EF5 onwards) - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 77a42501-27c9-4f4b-96df-26c128021467
caps.latest.revision: 3
---
# Entity Framework Enum Support - Code First (EF5 onwards)
> **EF5 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 5. If you are using an earlier version, some or all of the information does not apply.

This video and step-by-step walkthrough shows how to use enum types with Entity Framework Code First. It also demonstrates how to use enums in a LINQ query.

This walkthrough will use Code First to create a new database, but you can also use [Code First to map to an existing database](../ef6/entity-framework-code-first-to-an-existing-database.md).

Enum support was introduced in Entity Framework 5. To use the new features like enums, spatial data types, and table-valued functions, you must target .NET Framework 4.5. Visual Studio 2012 targets .NET 4.5 by default.

In Entity Framework, an enumeration can have the following underlying types: **Byte**, **Int16**, **Int32**, **Int64** , or **SByte**.

[See the video that accompanies this step-by-step walkthrough.](../ef6/entity-framework-enum-support-code-first-ef5-onwards-video.md)

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

```
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

```
using System.Data.Entity;
```

In the Program.cs add the context definition. 

```
public partial class EnumTestContext : DbContext 
{ 
    public DbSet<Department> Departments { get; set; } 
}
```
 

## Persist and Retrieve Data

Open the Program.cs file where the Main method is defined. Add the following code into the Main function. The code adds a new Department object to the context. It then saves the data. The code also executes a LINQ query that returns a Department where the name is DepartmentNames.English.

```
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

```
DepartmentID: 1 Name: English
```
 

## View the Generated Database

When you run the application the first time, the Entity Framework creates a database for you. Because we have Visual Studio 2012 installed, the database will be created on the LocalDB instance. By default, the Entity Framework names the database after the fully qualified name of the derived context (for this example that is **EnumCodeFirst.EnumTestContext**). The subsequent times the existing database will be used.  

Note, that if you make any changes to your model after the database has been created, you should use Code First Migrations to update the database schema. See [Code First to a New Database](../ef6/entity-framework-code-first-to-a-new-database.md) for an example of using Migrations.

To view the database and data, do the following:

1.  In the Visual Studio 2012 main menu, select **View** -&gt; **SQL Server Object Explorer**.
2.  If **(localdb)\\v11.0** is not in the list of servers, click the right mouse button on **SQL Server** and select **Add SQL Server**
    Use the default **Windows Authentication** to connect to the **(localdb)\\v11.0** server
3.  Expand **(localdb)\\v11.0**
4.  Unfold the **Databases** folder to see the new database and browse to the **Department** table
    Note, that Code First does not create a table that maps to the enumeration type
5.  To view data, right-click on the table and select **View Data**

## Summary

In this walkthrough we looked at how to use enum types with Entity Framework Code First. 
