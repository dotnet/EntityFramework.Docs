---
title: Defining Query - EF Designer - EF6
description: Defining Query - EF Designer in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/designer/advanced/defining-query
---
# Defining Query - EF Designer
This walkthrough demonstrates how to add a defining query and a corresponding entity type to a model using the EF Designer. A defining query is commonly used to provide functionality similar to that provided by a database view, but the view is defined in the model, not the database. A defining query allows you to execute a SQL statement that is specified in the **DefiningQuery** element of an .edmx file. For more information, see **DefiningQuery** in the [SSDL Specification](xref:ef6/modeling/designer/advanced/edmx/ssdl-spec).

When using defining queries, you also have to define an entity type in your model. The entity type is used to surface data exposed by the defining query. Note that data surfaced through this entity type is read-only.

Parameterized queries cannot be executed as defining queries. However, the data can be updated by mapping the insert, update, and delete functions of the entity type that surfaces the data to stored procedures. For more information, see [Insert, Update, and Delete with Stored Procedures](xref:ef6/modeling/designer/stored-procedures/cud).

This topic shows how to perform the following tasks.

-   Add a Defining Query
-   Add an Entity Type to the Model
-   Map the Defining Query to the Entity Type

## Prerequisites

To complete this walkthrough, you will need:

- A recent version of Visual Studio.
- The [School sample database](xref:ef6/resources/school-database).

## Set up the Project

This walkthrough is using Visual Studio 2012 or newer.

-   Open Visual Studio.
-   On the **File** menu, point to **New**, and then click **Project**.
-   In the left pane, click **Visual C\#**, and then select the **Console Application** template.
-   Enter **DefiningQuerySample** as the name of the project and click **OK**.

 

## Create a Model based on the School Database

-   Right-click the project name in Solution Explorer, point to **Add**, and then click **New Item**.
-   Select **Data** from the left menu and then select **ADO.NET Entity Data Model** in the Templates pane.
-   Enter **DefiningQueryModel.edmx** for the file name, and then click **Add**.
-   In the Choose Model Contents dialog box, select **Generate from database**, and then click **Next**.
-   Click New Connection. In the Connection Properties dialog box, enter the server name (for example, **(localdb)\\mssqllocaldb**), select the authentication method, type **School** for the database name, and then click **OK**.
    The Choose Your Data Connection dialog box is updated with your database connection setting.
-   In the Choose Your Database Objects dialog box, check the **Tables** node. This will add all the tables to the **School** model.
-   Click **Finish**.
-   In Solution Explorer, right-click the **DefiningQueryModel.edmx** file and select **Open With…**.
-   Select **XML (Text) Editor**.

    ![XML Editor](~/ef6/media/xmleditor.png)

-   Click **Yes** if prompted with the following message:

    ![Warning 2](~/ef6/media/warning2.png)

 

## Add a Defining Query

In this step we will use the XML Editor to add a defining query and an entity type to the SSDL section of the .edmx file. 

-   Add an **EntitySet** element to the SSDL section of the .edmx file (line 5 thru 13). Specify the following:
    -   Only the **Name** and **EntityType** attributes of the **EntitySet** element are specified.
    -   The fully-qualified name of the entity type is used in the **EntityType** attribute.
    -   The SQL statement to be executed is specified in the **DefiningQuery** element.

``` xml
    <!-- SSDL content -->
    <edmx:StorageModels>
      <Schema Namespace="SchoolModel.Store" Alias="Self" Provider="System.Data.SqlClient" ProviderManifestToken="2008" xmlns:store="http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator" xmlns="http://schemas.microsoft.com/ado/2009/11/edm/ssdl">
        <EntityContainer Name="SchoolModelStoreContainer">
           <EntitySet Name="GradeReport" EntityType="SchoolModel.Store.GradeReport">
              <DefiningQuery>
                SELECT CourseID, Grade, FirstName, LastName
                FROM StudentGrade
                JOIN
                (SELECT * FROM Person WHERE EnrollmentDate IS NOT NULL) AS p
                ON StudentID = p.PersonID
              </DefiningQuery>
          </EntitySet>
          <EntitySet Name="Course" EntityType="SchoolModel.Store.Course" store:Type="Tables" Schema="dbo" />
```

-   Add the **EntityType** element to the SSDL section of the .edmx. file as shown below. Note the following:
    -   The value of the **Name** attribute corresponds to the value of the **EntityType** attribute in the **EntitySet** element above, although the fully-qualified name of the entity type is used in the **EntityType** attribute.
    -   The property names correspond to the column names returned by the SQL statement in the **DefiningQuery** element (above).
    -   In this example, the entity key is composed of three properties to ensure a unique key value.

``` xml
    <EntityType Name="GradeReport">
      <Key>
        <PropertyRef Name="CourseID" />
        <PropertyRef Name="FirstName" />
        <PropertyRef Name="LastName" />
      </Key>
      <Property Name="CourseID"
                Type="int"
                Nullable="false" />
      <Property Name="Grade"
                Type="decimal"
                Precision="3"
                Scale="2" />
      <Property Name="FirstName"
                Type="nvarchar"
                Nullable="false"
                MaxLength="50" />
      <Property Name="LastName"
                Type="nvarchar"
                Nullable="false"
                MaxLength="50" />
    </EntityType>
```

>[!NOTE]
> If later you run the **Update Model Wizard** dialog, any changes made to the storage model, including defining queries, will be overwritten.

 

## Add an Entity Type to the Model

In this step we will add the entity type to the conceptual model using the EF Designer.  Note the following:

-   The **Name** of the entity corresponds to the value of the **EntityType** attribute in the **EntitySet** element above.
-   The property names correspond to the column names returned by the SQL statement in the **DefiningQuery** element above.
-   In this example, the entity key is composed of three properties to ensure a unique key value.

Open the model in the EF Designer.

-   Double-click the DefiningQueryModel.edmx.
-   Say **Yes** to the following message:

    ![Warning 2](~/ef6/media/warning2.png)

 

The Entity Designer, which provides a design surface for editing your model, is displayed.

-   Right-click the designer surface and select **Add New**-&gt;**Entity…**.
-   Specify **GradeReport** for the entity name and **CourseID** for the **Key Property**.
-   Right-click the **GradeReport** entity and select **Add New**-&gt; **Scalar Property**.
-   Change the default name of the property to **FirstName**.
-   Add another scalar property and specify **LastName** for the name.
-   Add another scalar property and specify **Grade** for the name.
-   In the **Properties** window, change the **Grade**’s **Type** property to **Decimal**.
-   Select the **FirstName** and **LastName** properties.
-   In the **Properties** window, change the **EntityKey** property value to **True**.

As a result, the following elements were added to the **CSDL** section of the .edmx file.

``` xml
    <EntitySet Name="GradeReport" EntityType="SchoolModel.GradeReport" />

    <EntityType Name="GradeReport">
    . . .
    </EntityType>
```

 

## Map the Defining Query to the Entity Type

In this step, we will use the Mapping Details window to map the conceptual and storage entity types.

-   Right-click the **GradeReport** entity on the design surface and select **Table Mapping**.  
    The **Mapping Details** window is displayed.
-   Select **GradeReport** from the **&lt;Add a Table or View&gt;** dropdown list (located under **Table**s).  
    Default mappings between the conceptual and storage **GradeReport** entity type appear.  
    ![Mapping Details3](~/ef6/media/mappingdetails.png)

As a result, the **EntitySetMapping** element is added to the mapping section of the .edmx file. 

``` xml
    <EntitySetMapping Name="GradeReports">
      <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.GradeReport)">
        <MappingFragment StoreEntitySet="GradeReport">
          <ScalarProperty Name="LastName" ColumnName="LastName" />
          <ScalarProperty Name="FirstName" ColumnName="FirstName" />
          <ScalarProperty Name="Grade" ColumnName="Grade" />
          <ScalarProperty Name="CourseID" ColumnName="CourseID" />
        </MappingFragment>
      </EntityTypeMapping>
    </EntitySetMapping>
```

-   Compile the application.

 

## Call the Defining Query in your Code

You can now execute the defining query by using the **GradeReport** entity type. 

``` csharp
    using (var context = new SchoolEntities())
    {
        var report = context.GradeReports.FirstOrDefault();
        Console.WriteLine("{0} {1} got {2}",
            report.FirstName, report.LastName, report.Grade);
    }
```
