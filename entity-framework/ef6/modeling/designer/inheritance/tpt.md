---
title: "Designer TPT Inheritance - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: efc78c31-b4ea-4ea3-a0cd-c69eb507020e
caps.latest.revision: 3
---
# Designer TPT Inheritance
This step-by-step walkthrough shows how to implement table-per-type (TPT) inheritance in your model using the Entity Framework Designer (EF Designer). Table-per-type inheritance uses a separate table in the database to maintain data for non-inherited properties and key properties for each type in the inheritance hierarchy.

In this walkthrough we will map the **Course** (base type), **OnlineCourse** (derives from Course), and **OnsiteCourse** (derives from **Course**) entities to tables with the same names. We'll create a model from the database and then alter the model to implement the TPT inheritance.

You can also start with the Model First and then generate the database from the model. The EF Designer uses the TPT strategy by default and so any inheritance in the model will be mapped to separate tables.

## Other Inheritance Options

Table-per-Hierarchy (TPH) is another type of inheritance in which one database table is used to maintain data for all of the entity types in an inheritance hierarchy.  For information about how to map Table-per-Hierarchy inheritance with the Entity Designer, see [EF Designer TPH Inheritance](~/ef6/modeling/designer/inheritance/tph.md). 

Note that, the Table-per-Concrete Type Inheritance (TPC) and mixed inheritance models are supported by the Entity Framework runtime but are not supported by the EF Designer. If you want to use TPC or mixed inheritance, you have two options: use Code First, or manually edit the EDMX file. If you choose to work with the EDMX file, the Mapping Details Window will be put into “safe mode” and you will not be able to use the designer to change the mappings.

## Prerequisites

To complete this walkthrough, you will need:

- A recent version of Visual Studio.
- The [School sample database](~/ef6/resources/school-database.md).

## Set up the Project

-   Open Visual Studio 2012.
-   Select **File-&gt; New -&gt; Project**
-   In the left pane, click **Visual C\#**, and then select the **Console** template.
-   Enter **TPTDBFirstSample** as the name.
-   Select **OK**.

## Create a Model

-   Right-click the project in Solution Explorer, and select **Add -&gt; New Item**.
-   Select **Data** from the left menu and then select **ADO.NET Entity Data Model** in the Templates pane.
-   Enter **TPTModel.edmx** for the file name, and then click **Add**.
-   In the Choose Model Contents dialog box, select** Generate from database**, and then click **Next**.
-   Click **New Connection**.
    In the Connection Properties dialog box, enter the server name (for example, **(localdb)\\mssqllocaldb**), select the authentication method, type **School** for the database name, and then click **OK**.
    The Choose Your Data Connection dialog box is updated with your database connection setting.
-   In the Choose Your Database Objects dialog box, under the Tables node, select the **Department**, **Course, OnlineCourse, and OnsiteCourse** tables.
-   Click **Finish**.

The Entity Designer, which provides a design surface for editing your model, is displayed. All the objects that you selected in the Choose Your Database Objects dialog box are added to the model.

## Implement Table-per-Type Inheritance

-   On the design surface, right-click the **OnlineCourse** entity type and select **Properties**.
-   In the **Properties** window, set the Base Type property to **Course**.
-   Right-click the **OnsiteCourse** entity type and select **Properties**.
-   In the **Properties** window, set the Base Type property to **Course**.
-   Right-click the association (the line) between the **OnlineCourse** and **Course** entity types.
    Select **Delete from Model**.
-   Right-click the association between the **OnsiteCourse** and **Course** entity types.
    Select **Delete from Model**.

We will now delete the **CourseID** property from **OnlineCourse** and **OnsiteCourse** because these classes inherit **CourseID** from the **Course** base type.

-   Right-click the **CourseID** property of the **OnlineCourse** entity type, and then select **Delete from Model**.
-   Right-click the **CourseID** property of the **OnsiteCourse** entity type, and then select **Delete from Model**
-   Table-per-type inheritance is now implemented.

![TPT](~/ef6/media/tpt.png)

## Use the Model

Open the **Program.cs** file where the **Main** method is defined. Paste the following code into the **Main** function. The code executes three queries. The first query brings back all **Courses** related to the specified department. The second query uses the **OfType** method to return **OnlineCourses** related to the specified department. The third query returns **OnsiteCourses**.

``` csharp
    using (var context = new SchoolEntities())
    {
        foreach (var department in context.Departments)
        {
            Console.WriteLine("The {0} department has the following courses:",
                               department.Name);

            Console.WriteLine("   All courses");
            foreach (var course in department.Courses )
            {
                Console.WriteLine("     {0}", course.Title);
            }

            foreach (var course in department.Courses.
                OfType<OnlineCourse>())
            {
                Console.WriteLine("   Online - {0}", course.Title);
            }

            foreach (var course in department.Courses.
                OfType<OnsiteCourse>())
            {
                Console.WriteLine("   Onsite - {0}", course.Title);
            }
        }
    }
```
