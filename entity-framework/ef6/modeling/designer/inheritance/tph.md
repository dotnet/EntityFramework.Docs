---
title: "Designer TPH Inheritance - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 72d26a8e-20ab-4500-bd13-394a08e73394
caps.latest.revision: 3
---
# Designer TPH Inheritance
This step-by-step walkthrough shows how to implement table-per-hierarchy (TPH) inheritance in your conceptual model with the Entity Framework Designer (EF Designer). TPH inheritance uses one database table to maintain data for all of the entity types in an inheritance hierarchy.

In this walkthrough we will map the Person table to three entity types: Person (the base type), Student (derives from Person), and Instructor (derives from Person). We'll create a conceptual model from the database (Database First) and then alter the model to implement the TPH inheritance using the EF Designer.

It is possible to map to a TPH inheritance using Model First but you would have to write your own database generation workflow which is complex. You would then assign this workflow to the **Database Generation Workflow** property in the EF Designer. An easier alternative is to use Code First.

## Other Inheritance Options

Table-per-Type (TPT) is another type of inheritance in which separate tables in the database are mapped to entities that participate in the inheritance.  For information about how to map Table-per-Type inheritance with the EF Designer, see [EF Designer TPT Inheritance](~/ef6/modeling/designer/inheritance/tpt.md).

Table-per-Concrete Type Inheritance (TPC) and mixed inheritance models are supported by the Entity Framework runtime but are not supported by the EF Designer. If you want to use TPC or mixed inheritance, you have two options: use Code First, or manually edit the EDMX file. If you choose to work with the EDMX file, the Mapping Details Window will be put into “safe mode” and you will not be able to use the designer to change the mappings.

## Prerequisites

To complete this walkthrough, you will need:

- A recent version of Visual Studio.
- The [School sample database](~/ef6/resources/school-database.md).

## Set up the Project

-   Open Visual Studio 2012.
-   Select **File-&gt; New -&gt; Project**
-   In the left pane, click **Visual C\#**, and then select the **Console** template.
-   Enter **TPHDBFirstSample** as the name.
-   Select **OK**.

## Create a Model

-   Right-click the project name in Solution Explorer, and select **Add -&gt; New Item**.
-   Select **Data** from the left menu and then select **ADO.NET Entity Data Model** in the Templates pane.
-   Enter **TPHModel.edmx** for the file name, and then click **Add**.
-   In the Choose Model Contents dialog box, select **Generate from database**, and then click **Next**.
-   Click **New Connection**.
    In the Connection Properties dialog box, enter the server name (for example, **(localdb)\\mssqllocaldb**), select the authentication method, type **School** for the database name, and then click **OK**.
    The Choose Your Data Connection dialog box is updated with your database connection setting.
-   In the Choose Your Database Objects dialog box, under the Tables node, select the **Person** table.
-   Click **Finish**.

The Entity Designer, which provides a design surface for editing your model, is displayed. All the objects that you selected in the Choose Your Database Objects dialog box are added to the model.

That is how the **Person** table looks in the database.

![PersonTable](~/ef6/media/persontable.png) 

## Implement Table-per-Hierarchy Inheritance

The **Person** table has the **Discriminator** column, which can have one of two values: “Student” and “Instructor”. Depending on the value the **Person** table will be mapped to the **Student** entity or the **Instructor** entity. The **Person** table also has two columns, **HireDate** and **EnrollmentDate**, which must be **nullable** because a person cannot be a student and an instructor at the same time (at least not in this walkthrough).

### Add new Entities

-   Add a new entity.
    To do this, right-click on an empty space of the design surface of the Entity Framework Designer, and select **Add-&gt;Entity**.
-   Type **Instructor** for the **Entity name** and select **Person** from the drop-down list for the **Base type**.
-   Click **OK**.
-   Add another new entity. Type **Student** for the **Entity name** and select **Person** from the drop-down list for the **Base type**.

Two new entity types were added to the design surface. An arrow points from the new entity types to the **Person** entity type; this indicates that **Person** is the base type for the new entity types.

-   Right-click the **HireDate** property of the **Person** entity. Select **Cut** (or use the Ctrl-X key).
-   Right-click the **Instructor** entity and select **Paste** (or use the Ctrl-V key).
-   Right-click the **HireDate** property and select **Properties**.
-   In the **Properties** window, set the **Nullable** property to **false**.
-   Right-click the **EnrollmentDate** property of the **Person** entity. Select **Cut** (or use the Ctrl-X key).
-   Right-click the **Student** entity and select **Paste(or use the Ctrl-V key).**
-   Select the **EnrollmentDate** property and set the **Nullable** property to **false**.
-   Select the **Person** entity type. In the **Properties** window, set its **Abstract** property to **true**.
-   Delete the **Discriminator** property from **Person**. The reason it should be deleted is explained in the following section.

### Map the entities

-   Right-click the **Instructor** and select **Table Mapping.**
    The Instructor entity is selected in the Mapping Details window.
-   Click **&lt;Add a Table or View&gt;** in the **Mapping Details** window.
    The **&lt;Add a Table or View&gt;** field becomes a drop-down list of tables or views to which the selected entity can be mapped.
-   Select **Person** from the drop-down list.
-   The **Mapping Details** window is updated with default column mappings and an option for adding a condition.
-   Click on **&lt;Add a Condition&gt;**.
    The **&lt;Add a Condition&gt;** field becomes a drop-down list of columns for which conditions can be set.
-   Select **Discriminator** from the drop-down list.
-   In the **Operator** column of the **Mapping Details** window, select = from the drop-down list.
-   In the **Value/Property** column, type **Instructor**. The end result should look like this:

    ![MappingDetails2](~/ef6/media/mappingdetails2.png)

-   Repeat these steps for the **Student** entity type, but make the condition equal to **Student** value.  
    *The reason we wanted to remove the **Discriminator** property, is because you cannot map a table column more than once. This column will be used for conditional mapping, so it cannot be used for property mapping as well. The only way it can be used for both, if a condition uses an **Is Null** or **Is Not Null** comparison.*

Table-per-hierarchy inheritance is now implemented.

![FinalTPH](~/ef6/media/finaltph.png)

## Use the Model

Open the **Program.cs** file where the **Main** method is defined. Paste the following code into the **Main** function. The code executes three queries. The first query brings back all **Person** objects. The second query uses the **OfType** method to return **Instructor** objects. The third query uses the **OfType** method to return **Student** objects.

``` csharp
    using (var context = new SchoolEntities())
    {
        Console.WriteLine("All people:");
        foreach (var person in context.People)
        {
            Console.WriteLine("    {0} {1}", person.FirstName, person.LastName);
        }

        Console.WriteLine("Instructors only: ");
        foreach (var person in context.People.OfType<Instructor>())
        {
            Console.WriteLine("    {0} {1}", person.FirstName, person.LastName);
        }

        Console.WriteLine("Students only: ");
        foreach (var person in context.People.OfType<Student>())
        {
            Console.WriteLine("    {0} {1}", person.FirstName, person.LastName);
        }
    }
```
