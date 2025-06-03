---
title: Spatial - Code First - EF6
description: Spatial - Code First in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/code-first/data-types/spatial
---
# Spatial - Code First
> [!NOTE]
> **EF5 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 5. If you are using an earlier version, some or all of the information does not apply.

The video and step-by-step walkthrough shows how to map spatial types with Entity Framework Code First. It also demonstrates how to use a LINQ query to find a distance between two locations.

This walkthrough will use Code First to create a new database, but you can also use [Code First to an existing database](xref:ef6/modeling/code-first/workflows/existing-database).

Spatial type support was introduced in Entity Framework 5. Note that to use the new features like spatial type, enums, and Table-valued functions, you must target .NET Framework 4.5. Visual Studio 2012 targets .NET 4.5 by default.

To use spatial data types you must also use an Entity Framework provider that has spatial support. See [provider support for spatial types](xref:ef6/fundamentals/providers/spatial-support) for more information.

There are two main spatial data types: geography and geometry. The geography data type stores ellipsoidal data (for example, GPS latitude and longitude coordinates). The geometry data type represents Euclidean (flat) coordinate system.

## Watch the video
This video shows how to map spatial types with Entity Framework Code First. It also demonstrates how to use a LINQ query to find a distance between two locations.

**Presented By**: Julia Kornich

**Video**: [WMV](https://download.microsoft.com/download/9/1/3/913EA17E-6F97-41D8-A4FE-805A0D83D26A/HDI-ITPro-MSDN-winvideo-spatialwithcodefirst.wmv) | [MP4](https://download.microsoft.com/download/9/1/3/913EA17E-6F97-41D8-A4FE-805A0D83D26A/HDI-ITPro-MSDN-mp4video-spatialwithcodefirst.m4v) | [WMV (ZIP)](https://download.microsoft.com/download/9/1/3/913EA17E-6F97-41D8-A4FE-805A0D83D26A/HDI-ITPro-MSDN-winvideo-spatialwithcodefirst.zip)

## Pre-Requisites

You will need to have Visual Studio 2012, Ultimate, Premium, Professional, or Web Express edition installed to complete this walkthrough.

## Set up the Project

1.  Open Visual Studio 2012
2.  On the **File** menu, point to **New**, and then click **Project**
3.  In the left pane, click **Visual C\#**, and then select the **Console** template
4.  Enter **SpatialCodeFirst** as the name of the project and click **OK**

## Define a New Model using Code First

When using Code First development you usually begin by writing .NET Framework classes that define your conceptual (domain) model. The code below defines the University class.

The University has the Location property of the DbGeography type. To use the DbGeography type, you must add a reference to the System.Data.Entity assembly and also add the System.Data.Spatial using statement.

Open the Program.cs file and paste the following using statements at the top of the file:

``` csharp
using System.Data.Spatial;
```

Add the following University class definition to the Program.cs file.

``` csharp
public class University  
{
    public int UniversityID { get; set; }
    public string Name { get; set; }
    public DbGeography Location { get; set; }
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

Note, that in addition to the EntityFramework  assembly, a reference to the System.ComponentModel.DataAnnotations assembly is also added.

At the top of the Program.cs file, add the following using statement:

``` csharp
using System.Data.Entity;
```

In the Program.cs add the context definition. 

``` csharp
public partial class UniversityContext : DbContext
{
    public DbSet<University> Universities { get; set; }
}
```

## Persist and Retrieve Data

Open the Program.cs file where the Main method is defined. Add the following code into the Main function.

The code adds two new University objects to the context. Spatial properties are initialized by using the DbGeography.FromText method. The geography point represented as WellKnownText is passed to the method. The code then saves the data. Then, the LINQ query that that returns a University object where its location is closest to the specified location, is constructed and executed.

``` csharp
using (var context = new UniversityContext ())
{
    context.Universities.Add(new University()
        {
            Name = "Graphic Design Institute",
            Location = DbGeography.FromText("POINT(-122.336106 47.605049)"),
        });

    context. Universities.Add(new University()
        {
            Name = "School of Fine Art",
            Location = DbGeography.FromText("POINT(-122.335197 47.646711)"),
        });

    context.SaveChanges();

    var myLocation = DbGeography.FromText("POINT(-122.296623 47.640405)");

    var university = (from u in context.Universities
                        orderby u.Location.Distance(myLocation)
                        select u).FirstOrDefault();

    Console.WriteLine(
        "The closest University to you is: {0}.",
        university.Name);
}
```

Compile and run the application. The program produces the following output:

```console
The closest University to you is: School of Fine Art.
```

## View the Generated Database

When you run the application the first time, the Entity Framework creates a database for you. Because we have Visual Studio 2012 installed, the database will be created on the LocalDB instance. By default, the Entity Framework names the database after the fully qualified name of the derived context (in this example that is **SpatialCodeFirst.UniversityContext**). The subsequent times the existing database will be used.  

Note, that if you make any changes to your model after the database has been created, you should use Code First Migrations to update the database schema. See [Code First to a New Database](xref:ef6/modeling/code-first/workflows/new-database) for an example of using Migrations.

To view the database and data, do the following:

1.  In the Visual Studio 2012 main menu, select **View** -&gt; **SQL Server Object Explorer**.
2.  If LocalDB is not in the list of servers, click the right mouse button on **SQL Server** and select **Add SQL Server**
    Use the default **Windows Authentication** to connect to the LocalDB instance
3.  Expand the LocalDB node
4.  Unfold the **Databases** folder to see the new database and browse to the **Universities** table
5.  To view data, right-click on the table and select **View Data**

## Summary

In this walkthrough we looked at how to use spatial types with Entity Framework Code First. 
