---
title: Enum Support - EF Designer - EF6
description: Enum Support - EF Designer in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/designer/data-types/enums
---
# Enum Support - EF Designer
> [!NOTE]
> **EF5 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 5. If you are using an earlier version, some or all of the information does not apply.

This video and step-by-step walkthrough shows how to use enum types with the Entity Framework Designer. It also demonstrates how to use enums in a LINQ query.

This walkthrough will use Model First to create a new database, but the EF Designer can also be used with the [Database First](xref:ef6/modeling/designer/workflows/database-first) workflow to map to an existing database.

Enum support was introduced in Entity Framework 5. To use the new features like enums, spatial data types, and table-valued functions, you must target .NET Framework 4.5. Visual Studio 2012 targets .NET 4.5 by default.

In Entity Framework, an enumeration can have the following underlying types: **Byte**, **Int16**, **Int32**, **Int64** , or **SByte**.

## Watch the Video
This video shows how to use enum types with the Entity Framework Designer. It also demonstrates how to use enums in a LINQ query.

**Presented By**: Julia Kornich

**Video**: [WMV](https://download.microsoft.com/download/0/7/A/07ADECC9-7893-415D-9F20-8B97D46A37EC/HDI-ITPro-MSDN-winvideo-enumwithdesiger.wmv) | [MP4](https://download.microsoft.com/download/0/7/A/07ADECC9-7893-415D-9F20-8B97D46A37EC/HDI-ITPro-MSDN-mp4video-enumwithdesiger.m4v) | [WMV (ZIP)](https://download.microsoft.com/download/0/7/A/07ADECC9-7893-415D-9F20-8B97D46A37EC/HDI-ITPro-MSDN-winvideo-enumwithdesiger.zip)

## Pre-Requisites

You will need to have Visual Studio 2012, Ultimate, Premium, Professional, or Web Express edition installed to complete this walkthrough.

## Set up the Project

1.  Open Visual Studio 2012
2.  On the **File** menu, point to **New**, and then click **Project**
3.  In the left pane, click **Visual C\#**, and then select the **Console** template
4.  Enter **EnumEFDesigner** as the name of the project and click **OK**

## Create a New Model using the EF Designer

1.  Right-click the project name in Solution Explorer, point to **Add**, and then click **New Item**
2.  Select **Data** from the left menu and then select **ADO.NET Entity Data Model** in the Templates pane
3.  Enter **EnumTestModel.edmx** for the file name, and then click **Add**
4.  On the Entity Data Model Wizard page, select **Empty Model** in the Choose Model Contents dialog box
5.  Click **Finish**

The Entity Designer, which provides a design surface for editing your model, is displayed.

The wizard performs the following actions:

-   Generates the EnumTestModel.edmx file that defines the conceptual model, the storage model, and the mapping between them. Sets the Metadata Artifact Processing property of the .edmx file to Embed in Output Assembly so the generated metadata files get embedded into the assembly.
-   Adds a reference to the following assemblies: EntityFramework, System.ComponentModel.DataAnnotations, and System.Data.Entity.
-   Creates EnumTestModel.tt and EnumTestModel.Context.tt files and adds them under the .edmx file. These T4 template files generate the code that defines the DbContext derived type and POCO types that map to the entities in the .edmx model.

## Add a New Entity Type

1.  Right-click an empty area of the design surface, select **Add -&gt; Entity**, the New Entity dialog box appears
2.  Specify **Department** for the type name and specify **DepartmentID** for the key property name, leave the type as **Int32**
3.  Click **OK**
4.  Right-click the entity and select **Add New -&gt; Scalar Property**
5.  Rename the new property to **Name**
6.  Change the type of the new property to **Int32** (by default, the new property is of String type)
    To change the type, open the Properties window and change the Type property to **Int32**
7.  Add another scalar property and rename it to **Budget**, change the type to **Decimal**

## Add an Enum Type

1.  In the Entity Framework Designer, right-click the Name property, select **Convert to enum**

    ![Convert To Enum](~/ef6/media/converttoenum.png)

2.  In the **Add Enum** dialog box type **DepartmentNames** for the Enum Type Name, change the Underlying Type to **Int32**, and then add the following members to the type: English, Math, and Economics

    ![Add Enum Type](~/ef6/media/addenumtype.png)

3.  Press **OK**
4.  Save the model and build the project
    > [!NOTE]
    > When you build, warnings about unmapped entities and associations may appear in the Error List. You can ignore these warnings because after we choose to generate the database from the model, the errors will go away.

If you look at the Properties window, you will notice that the type of the Name property was changed to **DepartmentNames** and the newly added enum type was added to the list of types.

If you switch to the Model Browser window, you will see that the type was also added to the Enum Types node.

![Model Browser](~/ef6/media/modelbrowser.png)

>[!NOTE]
> You can also add new enum types from this window by clicking the right mouse button and selecting **Add Enum Type**. Once the type is created it will appear in the list of types and you would be able to associate with a property

## Generate Database from Model

Now we can generate a database that is based on the model.

1.  Right-click an empty space on the Entity Designer surface and select **Generate Database from Model**
2.  The Choose Your Data Connection Dialog Box of the Generate Database Wizard is displayed
    Click the **New Connection** button
    Specify **(localdb)\\mssqllocaldb** for the server name and **EnumTest** for the database and click **OK**
3.  A dialog asking if you want to create a new database will pop up, click **Yes**.
4.  Click **Next** and the Create Database Wizard generates data definition language (DDL) for creating a database
    The generated DDL is displayed in the Summary and Settings Dialog Box
    Note, that the DDL does not contain a definition for a table that maps to the enumeration type
5.  Click **Finish**
    Clicking Finish does not execute the DDL script.
6.  The Create Database Wizard does the following:
    Opens the **EnumTest.edmx.sql** in T-SQL Editor
    Generates the store schema and mapping sections of the EDMX file
    Adds connection string information to the App.config file
7.  Click the right mouse button in T-SQL Editor and select **Execute**
    The Connect to Server dialog appears, enter the connection information from step 2 and click **Connect**
8.  To view the generated schema, right-click on the database name in SQL Server Object Explorer and select **Refresh**

## Persist and Retrieve Data

Open the Program.cs file where the Main method is defined. Add the following code into the Main function. The code adds a new Department object to the context. It then saves the data. The code also executes a LINQ query that returns a Department where the name is DepartmentNames.English.

``` csharp
using (var context = new EnumTestModelContainer())
{
    context.Departments.Add(new Department{ Name = DepartmentNames.English });

    context.SaveChanges();

    var department = (from d in context.Departments
                        where d.Name == DepartmentNames.English
                        select d).FirstOrDefault();

    Console.WriteLine(
        "DepartmentID: {0} and Name: {1}",
        department.DepartmentID,  
        department.Name);
}
```

Compile and run the application. The program produces the following output:

```console
DepartmentID: 1 Name: English
```

To view data in the database, right-click on the database name in SQL Server Object Explorer and select **Refresh**. Then, click the right mouse button on the table and select **View Data**.

## Summary

In this walkthrough we looked at how to map enum types using the Entity Framework Designer and how to use enums in code. 
