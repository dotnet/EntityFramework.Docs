---
title: Spatial - EF Designer - EF6
description: Spatial - EF Designer in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/designer/data-types/spatial
---
# Spatial - EF Designer
> [!NOTE]
> **EF5 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 5. If you are using an earlier version, some or all of the information does not apply.

The video and step-by-step walkthrough shows how to map spatial types with the Entity Framework Designer. It also demonstrates how to use a LINQ query to find a distance between two locations.

This walkthrough will use Model First to create a new database, but the EF Designer can also be used with the [Database First](xref:ef6/modeling/designer/workflows/database-first) workflow to map to an existing database.

Spatial type support was introduced in Entity Framework 5. Note that to use the new features like spatial type, enums, and Table-valued functions, you must target .NET Framework 4.5. Visual Studio 2012 targets .NET 4.5 by default.

To use spatial data types you must also use an Entity Framework provider that has spatial support. See [provider support for spatial types](xref:ef6/fundamentals/providers/spatial-support) for more information.

There are two main spatial data types: geography and geometry. The geography data type stores ellipsoidal data (for example, GPS latitude and longitude coordinates). The geometry data type represents Euclidean (flat) coordinate system.

## Watch the video
This video shows how to map spatial types with the Entity Framework Designer. It also demonstrates how to use a LINQ query to find a distance between two locations.

**Presented By**: Julia Kornich

**Video**: [WMV](https://download.microsoft.com/download/E/C/9/EC9E6547-8983-4C1F-A919-D33210E4B213/HDI-ITPro-MSDN-winvideo-spatialwithdesigner.wmv) | [MP4](https://download.microsoft.com/download/E/C/9/EC9E6547-8983-4C1F-A919-D33210E4B213/HDI-ITPro-MSDN-mp4video-spatialwithdesigner.m4v) | [WMV (ZIP)](https://download.microsoft.com/download/E/C/9/EC9E6547-8983-4C1F-A919-D33210E4B213/HDI-ITPro-MSDN-winvideo-spatialwithdesigner.zip)

## Pre-Requisites

You will need to have Visual Studio 2012, Ultimate, Premium, Professional, or Web Express edition installed to complete this walkthrough.

## Set up the Project

1.  Open Visual Studio 2012
2.  On the **File** menu, point to **New**, and then click **Project**
3.  In the left pane, click **Visual C\#**, and then select the **Console** template
4.  Enter **SpatialEFDesigner** as the name of the project and click **OK**

## Create a New Model using the EF Designer

1.  Right-click the project name in Solution Explorer, point to **Add**, and then click **New Item**
2.  Select **Data** from the left menu and then select **ADO.NET Entity Data Model** in the Templates pane
3.  Enter **UniversityModel.edmx** for the file name, and then click **Add**
4.  On the Entity Data Model Wizard page, select **Empty Model** in the Choose Model Contents dialog box
5.  Click **Finish**

The Entity Designer, which provides a design surface for editing your model, is displayed.

The wizard performs the following actions:

-   Generates the EnumTestModel.edmx file that defines the conceptual model, the storage model, and the mapping between them. Sets the Metadata Artifact Processing property of the .edmx file to Embed in Output Assembly so the generated metadata files get embedded into the assembly.
-   Adds a reference to the following assemblies: EntityFramework, System.ComponentModel.DataAnnotations, and System.Data.Entity.
-   Creates UniversityModel.tt and UniversityModel.Context.tt files and adds them under the .edmx file. These T4 template files generate the code that defines the DbContext derived type and POCO types that map to the entities in the .edmx model

## Add a New Entity Type

1.  Right-click an empty area of the design surface, select **Add -&gt; Entity**, the New Entity dialog box appears
2.  Specify **University** for the type name and specify **UniversityID** for the key property name, leave the type as **Int32**
3.  Click **OK**
4.  Right-click the entity and select **Add New -&gt; Scalar Property**
5.  Rename the new property to **Name**
6.  Add another scalar property and rename it to **Location**
    Open the Properties window and change the type of the new property to **Geography**
7.  Save the model and build the project
    > [!NOTE]
    > When you build, warnings about unmapped entities and associations may appear in the Error List. You can ignore these warnings because after we choose to generate the database from the model, the errors will go away.

## Generate Database from Model

Now we can generate a database that is based on the model.

1.  Right-click an empty space on the Entity Designer surface and select **Generate Database from Model**
2.  The Choose Your Data Connection Dialog Box of the Generate Database Wizard is displayed
    Click the **New Connection** button
    Specify **(localdb)\\mssqllocaldb** for the server name and **University** for the database and click **OK**
3.  A dialog asking if you want to create a new database will pop up, click **Yes**.
4.  Click **Next** and the Create Database Wizard generates data definition language (DDL) for creating a database
    The generated DDL is displayed in the Summary and Settings Dialog Box
    Note, that the DDL does not contain a definition for a table that maps to the enumeration type
5.  Click **Finish**
    Clicking Finish does not execute the DDL script.
6.  The Create Database Wizard does the following:
    Opens the **UniversityModel.edmx.sql** in T-SQL Editor
    Generates the store schema and mapping sections of the EDMX file
    Adds connection string information to the App.config file
7.  Click the right mouse button in T-SQL Editor and select **Execute**
    The Connect to Server dialog appears, enter the connection information from step 2 and click **Connect**
8.  To view the generated schema, right-click on the database name in SQL Server Object Explorer and select **Refresh**

## Persist and Retrieve Data

Open the Program.cs file where the Main method is defined. Add the following code into the Main function.

The code adds two new University objects to the context. Spatial properties are initialized by using the DbGeography.FromText method. The geography point represented as WellKnownText is passed to the method. The code then saves the data. Then, the LINQ query that that returns a University object where its location is closest to the specified location, is constructed and executed.

``` csharp
using (var context = new UniversityModelContainer())
{
    context.Universities.Add(new University()
    {
        Name = "Graphic Design Institute",
        Location = DbGeography.FromText("POINT(-122.336106 47.605049)"),
    });

    context.Universities.Add(new University()
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

To view data in the database, right-click on the database name in SQL Server Object Explorer and select **Refresh**. Then, click the right mouse button on the table and select **View Data**.

## Summary

In this walkthrough we looked at how to map spatial types using the Entity Framework Designer and how to use spatial types in code. 
