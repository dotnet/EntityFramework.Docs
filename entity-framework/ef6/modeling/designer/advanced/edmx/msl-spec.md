---
title: MSL Specification - EF6
description: MSL Specification in Entity Framework 6
author: SamMonoRT
ms.date: 08/23/2024
uid: ef6/modeling/designer/advanced/edmx/msl-spec
---
# MSL Specification

> [!NOTE]
> MSL v1 is unsupported, please update to V3

Mapping specification language (MSL) is an XML-based language that describes the mapping between the conceptual model and storage model of an Entity Framework application.

In an Entity Framework application, mapping metadata is loaded from an .msl file (written in MSL) at build time. Entity Framework uses mapping metadata at runtime to translate queries against the conceptual model to store-specific commands.

The Entity Framework Designer (EF Designer) stores mapping information in an .edmx file at design time. At build time, the Entity Designer uses information in an .edmx file to create the .msl file that is needed by Entity Framework at runtime

Names of all conceptual or storage model types that are referenced in MSL must be qualified by their respective namespace names. For information about the conceptual model namespace name, see [CSDL Specification](xref:ef6/modeling/designer/advanced/edmx/csdl-spec). For information about the storage model namespace name, see [SSDL Specification](xref:ef6/modeling/designer/advanced/edmx/ssdl-spec).

Versions of MSL are differentiated by XML namespaces.

| MSL Version | XML Namespace                                        |
|:------------|:-----------------------------------------------------|
| MSL v1      | urn:schemas-microsoft-com:windows:storage:mapping:CS |
| MSL v2      | `https://schemas.microsoft.com/ado/2008/09/mapping/cs` |
| MSL v3      | `https://schemas.microsoft.com/ado/2009/11/mapping/cs` |

## Alias Element (MSL)

The **Alias** element in mapping specification language (MSL) is a child of the Mapping element that is used to define aliases for conceptual model and storage model namespaces. Names of all conceptual or storage model types that are referenced in MSL must be qualified by their respective namespace names. For information about the conceptual model namespace name, see Schema Element (CSDL). For information about the storage model namespace name, see Schema Element (SSDL).

The **Alias** element cannot have child elements.

### Applicable Attributes

The table below describes the attributes that can be applied to the **Alias** element.

| Attribute Name | Is Required | Value                                                                     |
|:---------------|:------------|:--------------------------------------------------------------------------|
| **Key**        | Yes         | The alias for the namespace that is specified by the **Value** attribute. |
| **Value**      | Yes         | The namespace for which the value of the **Key** element is an alias.     |

### Example

The following example shows an **Alias** element that defines an alias, `c`, for types that are defined in the conceptual model.

``` xml
 <Mapping Space="C-S"
          xmlns="https://schemas.microsoft.com/ado/2009/11/mapping/cs">
   <Alias Key="c" Value="SchoolModel"/>
   <EntityContainerMapping StorageEntityContainer="SchoolModelStoreContainer"
                           CdmEntityContainer="SchoolModelEntities">
     <EntitySetMapping Name="Courses">
       <EntityTypeMapping TypeName="c.Course">
         <MappingFragment StoreEntitySet="Course">
           <ScalarProperty Name="CourseID" ColumnName="CourseID" />
           <ScalarProperty Name="Title" ColumnName="Title" />
           <ScalarProperty Name="Credits" ColumnName="Credits" />
           <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
         </MappingFragment>
       </EntityTypeMapping>
     </EntitySetMapping>
     <EntitySetMapping Name="Departments">
       <EntityTypeMapping TypeName="c.Department">
         <MappingFragment StoreEntitySet="Department">
           <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
           <ScalarProperty Name="Name" ColumnName="Name" />
           <ScalarProperty Name="Budget" ColumnName="Budget" />
           <ScalarProperty Name="StartDate" ColumnName="StartDate" />
           <ScalarProperty Name="Administrator" ColumnName="Administrator" />
         </MappingFragment>
       </EntityTypeMapping>
     </EntitySetMapping>
   </EntityContainerMapping>
 </Mapping>
```

## AssociationEnd Element (MSL)

The **AssociationEnd** element in mapping specification language (MSL) is used when the modification functions of an entity type in the conceptual model are mapped to stored procedures in the underlying database. If a modification stored procedure takes a parameter whose value is held in an association property, the **AssociationEnd** element maps the property value to the parameter. For more information, see the example below.

For more information about mapping modification functions of entity types to stored procedures, see ModificationFunctionMapping Element (MSL) and Walkthrough: Mapping an Entity to Stored Procedures.

The **AssociationEnd** element can have the following child elements:

-   ScalarProperty

### Applicable Attributes

The following table describes the attributes that are applicable to the **AssociationEnd** element.

| Attribute Name     | Is Required | Value                                                                                                                                                                             |
|:-------------------|:------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AssociationSet** | Yes         | The name of the association that is being mapped.                                                                                                                                 |
| **From**           | Yes         | The value of the **FromRole** attribute of the navigation property that corresponds to the association being mapped. For more information, see NavigationProperty Element (CSDL). |
| **To**             | Yes         | The value of the **ToRole** attribute of the navigation property that corresponds to the association being mapped. For more information, see NavigationProperty Element (CSDL).   |

### Example

Consider the following conceptual model entity type:

``` xml
 <EntityType Name="Course">
   <Key>
     <PropertyRef Name="CourseID" />
   </Key>
   <Property Type="Int32" Name="CourseID" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" MaxLength="100"
             FixedLength="false" Unicode="true" />
   <Property Type="Int32" Name="Credits" Nullable="false" />
   <NavigationProperty Name="Department"
                       Relationship="SchoolModel.FK_Course_Department"
                       FromRole="Course" ToRole="Department" />
 </EntityType>
```

Also consider the following stored procedure:

``` SQL
 CREATE PROCEDURE [dbo].[UpdateCourse]
                                @CourseID int,
                                @Title nvarchar(50),
                                @Credits int,
                                @DepartmentID int
                                AS
                                UPDATE Course SET Title=@Title,
                                                              Credits=@Credits,
                                                              DepartmentID=@DepartmentID
                                WHERE CourseID=@CourseID;
```

In order to map the update function of the `Course` entity to this stored procedure, you must supply a value to the **DepartmentID** parameter. The value for `DepartmentID` does not correspond to a property on the entity type; it is contained in an independent association whose mapping is shown here:

``` xml
 <AssociationSetMapping Name="FK_Course_Department"
                        TypeName="SchoolModel.FK_Course_Department"
                        StoreEntitySet="Course">
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <EndProperty Name="Department">
     <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
   </EndProperty>
 </AssociationSetMapping>
```

The following code shows the **AssociationEnd** element used to map the **DepartmentID** property of the **FK\_Course\_Department** association to the **UpdateCourse** stored procedure (to which the update function of the **Course** entity type is mapped):

``` xml
 <EntitySetMapping Name="Courses">
   <EntityTypeMapping TypeName="SchoolModel.Course">
     <MappingFragment StoreEntitySet="Course">
       <ScalarProperty Name="Credits" ColumnName="Credits" />
       <ScalarProperty Name="Title" ColumnName="Title" />
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="SchoolModel.Course">
     <ModificationFunctionMapping>
       <UpdateFunction FunctionName="SchoolModel.Store.UpdateCourse">
         <AssociationEnd AssociationSet="FK_Course_Department"
                         From="Course" To="Department">
           <ScalarProperty Name="DepartmentID"
                           ParameterName="DepartmentID"
                           Version="Current" />
         </AssociationEnd>
         <ScalarProperty Name="Credits" ParameterName="Credits"
                         Version="Current" />
         <ScalarProperty Name="Title" ParameterName="Title"
                         Version="Current" />
         <ScalarProperty Name="CourseID" ParameterName="CourseID"
                         Version="Current" />
       </UpdateFunction>
     </ModificationFunctionMapping>
   </EntityTypeMapping>
 </EntitySetMapping>
```

## AssociationSetMapping Element (MSL)

The **AssociationSetMapping** element in mapping specification language (MSL) defines the mapping between an association in the conceptual model and table columns in the underlying database.

Associations in the conceptual model are types whose properties represent primary and foreign key columns in the underlying database. The **AssociationSetMapping** element uses two EndProperty elements to define the mappings between association type properties and columns in the database. You can place conditions on these mappings with the Condition element. Map the insert, update, and delete functions for associations to stored procedures in the database with the ModificationFunctionMapping element. Define read-only mappings between associations and table columns by using an Entity SQL string in a QueryView element.

> [!NOTE]
> If a referential constraint is defined for an association in the conceptual model, the association does not need to be mapped with an **AssociationSetMapping** element. If an **AssociationSetMapping** element is present for an association that has a referential constraint, the mappings defined in the **AssociationSetMapping** element will be ignored. For more information, see ReferentialConstraint Element (CSDL).

The **AssociationSetMapping** element can have the following child elements

-   QueryView (zero or one)
-   EndProperty (zero or two)
-   Condition (zero or more)
-   ModificationFunctionMapping (zero or one)

### Applicable Attributes

The following table describes the attributes that can be applied to the **AssociationSetMapping** element.

| Attribute Name     | Is Required | Value                                                                                       |
|:-------------------|:------------|:--------------------------------------------------------------------------------------------|
| **Name**           | Yes         | The name of the conceptual model association set that is being mapped.                      |
| **TypeName**       | No          | The namespace-qualified name of the conceptual model association type that is being mapped. |
| **StoreEntitySet** | No          | The name of the table that is being mapped.                                                 |

### Example

The following example shows an **AssociationSetMapping** element in which the **FK\_Course\_Department** association set in the conceptual model is mapped to the **Course** table in the database. Mappings between association type properties and table columns are specified in child **EndProperty** elements.

``` xml
 <AssociationSetMapping Name="FK_Course_Department"
                        TypeName="SchoolModel.FK_Course_Department"
                        StoreEntitySet="Course">
   <EndProperty Name="Department">
     <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
 </AssociationSetMapping>
```

## ComplexProperty Element (MSL)

A **ComplexProperty** element in mapping specification language (MSL) defines the mapping between a complex type property on a conceptual model entity type and table columns in the underlying database. The property-column mappings are specified in child ScalarProperty elements.

The **ComplexType** property element can have the following child elements:

-   ScalarProperty (zero or more)
-   **ComplexProperty** (zero or more)
-   ComplexTypeMapping (zero or more)
-   Condition (zero or more)

### Applicable Attributes

The following table describes the attributes that are applicable to the **ComplexProperty** element:

| Attribute Name | Is Required | Value                                                                                            |
|:---------------|:------------|:-------------------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the complex property of an entity type in the conceptual model that is being mapped. |
| **TypeName**   | No          | The namespace-qualified name of the conceptual model property type.                              |

### Example

The following example is based on the School model. The following complex type has been added to the conceptual model:

``` xml
 <ComplexType Name="FullName">
   <Property Type="String" Name="LastName"
             Nullable="false" MaxLength="50"
             FixedLength="false" Unicode="true" />
   <Property Type="String" Name="FirstName"
             Nullable="false" MaxLength="50"
             FixedLength="false" Unicode="true" />
 </ComplexType>
```

The **LastName** and **FirstName** properties of the **Person** entity type have been replaced with one complex property, **Name**:

``` xml
 <EntityType Name="Person">
   <Key>
     <PropertyRef Name="PersonID" />
   </Key>
   <Property Name="PersonID" Type="Int32" Nullable="false"
             annotation:StoreGeneratedPattern="Identity" />
   <Property Name="HireDate" Type="DateTime" />
   <Property Name="EnrollmentDate" Type="DateTime" />
   <Property Name="Name" Type="SchoolModel.FullName" Nullable="false" />
 </EntityType>
```

The following MSL shows the **ComplexProperty** element used to map the **Name** property to columns in the underlying database:

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <ScalarProperty Name="EnrollmentDate" ColumnName="EnrollmentDate" />
       <ComplexProperty Name="Name" TypeName="SchoolModel.FullName">
         <ScalarProperty Name="FirstName" ColumnName="FirstName" />
         <ScalarProperty Name="LastName" ColumnName="LastName" />  
       </ComplexProperty>
     </MappingFragment>
   </EntityTypeMapping>
 </EntitySetMapping>
```

## ComplexTypeMapping Element (MSL)

The **ComplexTypeMapping** element in mapping specification language (MSL) is a child of the ResultMapping element and defines the mapping between a function import in the conceptual model and a stored procedure in the underlying database when the following are true:

-   The function import returns a conceptual complex type.
-   The names of the columns returned by the stored procedure do not exactly match the names of the properties on the complex type.

By default, the mapping between the columns returned by a stored procedure and a complex type is based on column and property names. If column names do not exactly match property names, you must use the **ComplexTypeMapping** element to define the mapping. For an example of the default mapping, see FunctionImportMapping Element (MSL).

The **ComplexTypeMapping** element can have the following child elements:

-   ScalarProperty (zero or more)

### Applicable Attributes

The following table describes the attributes that are applicable to the **ComplexTypeMapping** element.

| Attribute Name | Is Required | Value                                                                  |
|:---------------|:------------|:-----------------------------------------------------------------------|
| **TypeName**   | Yes         | The namespace-qualified name of the complex type that is being mapped. |

### Example

Consider the following stored procedure:

``` SQL
 CREATE PROCEDURE [dbo].[GetGrades]
             @student_Id int
             AS
             SELECT     EnrollmentID as enroll_id,
                                                                             Grade as grade,
                                                                             CourseID as course_id,
                                                                             StudentID as student_id
                                               FROM dbo.StudentGrade
             WHERE StudentID = @student_Id
```

Also consider the following conceptual model complex type:

``` xml
 <ComplexType Name="GradeInfo">
   <Property Type="Int32" Name="EnrollmentID" Nullable="false" />
   <Property Type="Decimal" Name="Grade" Nullable="true"
             Precision="3" Scale="2" />
   <Property Type="Int32" Name="CourseID" Nullable="false" />
   <Property Type="Int32" Name="StudentID" Nullable="false" />
 </ComplexType>
```

In order to create a function import that returns instances of the previous complex type, the mapping between the columns returned by the stored procedure and the entity type must be defined in a **ComplexTypeMapping** element:

``` xml
 <FunctionImportMapping FunctionImportName="GetGrades"
                        FunctionName="SchoolModel.Store.GetGrades" >
   <ResultMapping>
     <ComplexTypeMapping TypeName="SchoolModel.GradeInfo">
       <ScalarProperty Name="EnrollmentID" ColumnName="enroll_id"/>
       <ScalarProperty Name="CourseID" ColumnName="course_id"/>
       <ScalarProperty Name="StudentID" ColumnName="student_id"/>
       <ScalarProperty Name="Grade" ColumnName="grade"/>
     </ComplexTypeMapping>
   </ResultMapping>
 </FunctionImportMapping>
```

## Condition Element (MSL)

The **Condition** element in mapping specification language (MSL) places conditions on mappings between the conceptual model and the underlying database. The mapping that is defined within an XML node is valid if all conditions, as specified in child **Condition** elements, are met. Otherwise, the mapping is not valid. For example, if a MappingFragment element contains one or more **Condition** child elements, the mapping defined within the **MappingFragment** node will only be valid if all the conditions of the child **Condition** elements are met.

Each condition can apply to either a **Name** (the name of a conceptual model entity property, specified by the **Name** attribute), or a **ColumnName** (the name of a column in the database, specified by the **ColumnName** attribute). When the **Name** attribute is set, the condition is checked against an entity property value. When the **ColumnName** attribute is set, the condition is checked against a column value. Only one of the **Name** or **ColumnName** attribute can be specified in a **Condition** element.

> [!NOTE]
> When the **Condition** element is used within a FunctionImportMapping element, only the **Name** attribute is not applicable.

The **Condition** element can be a child of the following elements:

-   AssociationSetMapping
-   ComplexProperty
-   EntitySetMapping
-   MappingFragment
-   EntityTypeMapping

The **Condition** element can have no child elements.

### Applicable Attributes

The following table describes the attributes that are applicable to the **Condition** element:

| Attribute Name | Is Required | Value                                                                                                                                                                                                                                                                                         |
|:---------------|:------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **ColumnName** | No          | The name of the table column whose value is used to evaluate the condition.                                                                                                                                                                                                                   |
| **IsNull**     | No          | **True** or **False**. If the value is **True** and the column value is **null**, or if the value is **False** and the column value is not **null**, the condition is true. Otherwise, the condition is false. <br/> The **IsNull** and **Value** attributes cannot be used at the same time. |
| **Value**      | No          | The value with which the column value is compared. If the values are the same, the condition is true. Otherwise, the condition is false. <br/> The **IsNull** and **Value** attributes cannot be used at the same time.                                                                       |
| **Name**       | No          | The name of the conceptual model entity property whose value is used to evaluate the condition. <br/> This attribute is not applicable if the **Condition** element is used within a FunctionImportMapping element.                                                                           |

### Example

The following example shows **Condition** elements as children of **MappingFragment** elements. When **HireDate** is not null and **EnrollmentDate** is null, data is mapped between the **SchoolModel.Instructor** type and the **PersonID** and **HireDate** columns of the **Person** table. When **EnrollmentDate** is not null and **HireDate** is null, data is mapped between the **SchoolModel.Student** type and the **PersonID** and **Enrollment** columns of the **Person** table.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Person)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Instructor)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <Condition ColumnName="HireDate" IsNull="false" />
       <Condition ColumnName="EnrollmentDate" IsNull="true" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Student)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="EnrollmentDate"
                       ColumnName="EnrollmentDate" />
       <Condition ColumnName="EnrollmentDate" IsNull="false" />
       <Condition ColumnName="HireDate" IsNull="true" />
     </MappingFragment>
   </EntityTypeMapping>
 </EntitySetMapping>
```

## DeleteFunction Element (MSL)

The **DeleteFunction** element in mapping specification language (MSL) maps the delete function of an entity type or association in the conceptual model to a stored procedure in the underlying database. Stored procedures to which modification functions are mapped must be declared in the storage model. For more information, see Function Element (SSDL).

> [!NOTE]
> If you do not map all three of the insert, update, or delete operations of an entity type to stored procedures, the unmapped operations will fail if executed at runtime and an UpdateException is thrown.

### DeleteFunction Applied to EntityTypeMapping

When applied to the EntityTypeMapping element, the **DeleteFunction** element maps the delete function of an entity type in the conceptual model to a stored procedure.

The **DeleteFunction** element can have the following child elements when applied to an **EntityTypeMapping** element:

-   AssociationEnd (zero or more)
-   ComplexProperty (zero or more)
-   ScalarProperty (zero or more)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **DeleteFunction** element when it is applied to an **EntityTypeMapping** element.

| Attribute Name            | Is Required | Value                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| **FunctionName**          | Yes         | The namespace-qualified name of the stored procedure to which the delete function is mapped. The stored procedure must be declared in the storage model. |
| **RowsAffectedParameter** | No          | The name of the output parameter that returns the number of rows affected.                                                                               |

#### Example

The following example is based on the School model and shows the **DeleteFunction** element mapping the delete function of the **Person** entity type to the **DeletePerson** stored procedure. The **DeletePerson** stored procedure is declared in the storage model.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <ScalarProperty Name="EnrollmentDate"
                       ColumnName="EnrollmentDate" />
     </MappingFragment>
 </EntityTypeMapping>
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <ModificationFunctionMapping>
       <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName" />
         <ScalarProperty Name="LastName" ParameterName="LastName" />
         <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
       </InsertFunction>
       <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate"
                         Version="Current" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate"
                         Version="Current" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName"
                         Version="Current" />
         <ScalarProperty Name="LastName" ParameterName="LastName"
                         Version="Current" />
         <ScalarProperty Name="PersonID" ParameterName="PersonID"
                         Version="Current" />
       </UpdateFunction>
       <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
         <ScalarProperty Name="PersonID" ParameterName="PersonID" />
       </DeleteFunction>
     </ModificationFunctionMapping>
   </EntityTypeMapping>
 </EntitySetMapping>
```

### DeleteFunction Applied to AssociationSetMapping

When applied to the AssociationSetMapping element, the **DeleteFunction** element maps the delete function of an association in the conceptual model to a stored procedure.

The **DeleteFunction** element can have the following child elements when applied to the **AssociationSetMapping** element:

-   EndProperty

#### Applicable Attributes

The following table describes the attributes that can be applied to the **DeleteFunction** element when it is applied to the **AssociationSetMapping** element.

| Attribute Name            | Is Required | Value                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| **FunctionName**          | Yes         | The namespace-qualified name of the stored procedure to which the delete function is mapped. The stored procedure must be declared in the storage model. |
| **RowsAffectedParameter** | No          | The name of the output parameter that returns the number of rows affected.                                                                               |

#### Example

The following example is based on the School model and shows the **DeleteFunction** element used to map delete function of the **CourseInstructor** association to the **DeleteCourseInstructor** stored procedure. The **DeleteCourseInstructor** stored procedure is declared in the storage model.

``` xml
 <AssociationSetMapping Name="CourseInstructor"
                        TypeName="SchoolModel.CourseInstructor"
                        StoreEntitySet="CourseInstructor">
   <EndProperty Name="Person">
     <ScalarProperty Name="PersonID" ColumnName="PersonID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertCourseInstructor" >   
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </InsertFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeleteCourseInstructor">
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </DeleteFunction>
   </ModificationFunctionMapping>
 </AssociationSetMapping>
```

## EndProperty Element (MSL)

The **EndProperty** element in mapping specification language (MSL) defines the mapping between an end or a modification function of a conceptual model association and the underlying database. The property-column mapping is specified in a child ScalarProperty element.

When an **EndProperty** element is used to define the mapping for the end of a conceptual model association, it is a child of an AssociationSetMapping element. When the **EndProperty** element is used to define the mapping for a modification function of a conceptual model association, it is a child of an InsertFunction element or DeleteFunction element.

The **EndProperty** element can have the following child elements:

-   ScalarProperty (zero or more)

### Applicable Attributes

The following table describes the attributes that are applicable to the **EndProperty** element:

| Attribute Name | Is Required | Value                                                 |
|:---------------|:------------|:------------------------------------------------------|
| Name           | Yes         | The name of the association end that is being mapped. |

### Example

The following example shows an **AssociationSetMapping** element in which the **FK\_Course\_Department** association in the conceptual model is mapped to the **Course** table in the database. Mappings between association type properties and table columns are specified in child **EndProperty** elements.

``` xml
 <AssociationSetMapping Name="FK_Course_Department"
                        TypeName="SchoolModel.FK_Course_Department"
                        StoreEntitySet="Course">
   <EndProperty Name="Department">
     <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
 </AssociationSetMapping>
```

### Example

The following example shows the **EndProperty** element mapping the insert and delete functions of an association (**CourseInstructor**) to stored procedures in the underlying database. The functions that are mapped to are declared in the storage model.

``` xml
 <AssociationSetMapping Name="CourseInstructor"
                        TypeName="SchoolModel.CourseInstructor"
                        StoreEntitySet="CourseInstructor">
   <EndProperty Name="Person">
     <ScalarProperty Name="PersonID" ColumnName="PersonID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertCourseInstructor" >   
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </InsertFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeleteCourseInstructor">
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </DeleteFunction>
   </ModificationFunctionMapping>
 </AssociationSetMapping>
```

## EntityContainerMapping Element (MSL)

The **EntityContainerMapping** element in mapping specification language (MSL) maps the entity container in the conceptual model to the entity container in the storage model. The **EntityContainerMapping** element is a child of the Mapping element.

The **EntityContainerMapping** element can have the following child elements (in the order listed):

-   EntitySetMapping (zero or more)
-   AssociationSetMapping (zero or more)
-   FunctionImportMapping (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **EntityContainerMapping** element.

| Attribute Name            | Is Required | Value                                                                                                                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **StorageModelContainer** | Yes         | The name of the storage model entity container that is being mapped.                                                                                                                                                                                     |
| **CdmEntityContainer**    | Yes         | The name of the conceptual model entity container that is being mapped.                                                                                                                                                                                  |
| **GenerateUpdateViews**   | No          | **True** or **False**. If **False**, no update views are generated. This attribute should be set to **False** when you have a read-only mapping that would be invalid because data may not round-trip successfully. <br/> The default value is **True**. |

### Example

The following example shows an **EntityContainerMapping** element that maps the **SchoolModelEntities** container (the conceptual model entity container) to the **SchoolModelStoreContainer** container (the storage model entity container):

``` xml
 <EntityContainerMapping StorageEntityContainer="SchoolModelStoreContainer"
                         CdmEntityContainer="SchoolModelEntities">
   <EntitySetMapping Name="Courses">
     <EntityTypeMapping TypeName="c.Course">
       <MappingFragment StoreEntitySet="Course">
         <ScalarProperty Name="CourseID" ColumnName="CourseID" />
         <ScalarProperty Name="Title" ColumnName="Title" />
         <ScalarProperty Name="Credits" ColumnName="Credits" />
         <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
       </MappingFragment>
     </EntityTypeMapping>
   </EntitySetMapping>
   <EntitySetMapping Name="Departments">
     <EntityTypeMapping TypeName="c.Department">
       <MappingFragment StoreEntitySet="Department">
         <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
         <ScalarProperty Name="Name" ColumnName="Name" />
         <ScalarProperty Name="Budget" ColumnName="Budget" />
         <ScalarProperty Name="StartDate" ColumnName="StartDate" />
         <ScalarProperty Name="Administrator" ColumnName="Administrator" />
       </MappingFragment>
     </EntityTypeMapping>
   </EntitySetMapping>
 </EntityContainerMapping>
```

## EntitySetMapping Element (MSL)

The **EntitySetMapping** element in mapping specification language (MSL) maps all types in a conceptual model entity set to entity sets in the storage model. An entity set in the conceptual model is a logical container for instances of entities of the same type (and derived types). An entity set in the storage model represents a table or view in the underlying database. The conceptual model entity set is specified by the value of the **Name** attribute of the **EntitySetMapping** element. The mapped-to table or view is specified by the **StoreEntitySet** attribute in each child MappingFragment element or in the **EntitySetMapping** element itself.

The **EntitySetMapping** element can have the following child elements:

-   EntityTypeMapping (zero or more)
-   QueryView (zero or one)
-   MappingFragment (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **EntitySetMapping** element.

| Attribute Name           | Is Required | Value                                                                                                                                                                                                                         |
|:-------------------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**                 | Yes         | The name of the conceptual model entity set that is being mapped.                                                                                                                                                             |
| **TypeName** **1**       | No          | The name of the conceptual model entity type that is being mapped.                                                                                                                                                            |
| **StoreEntitySet** **1** | No          | The name of the storage model entity set that is being mapped to.                                                                                                                                                             |
| **MakeColumnsDistinct**  | No          | **True** or **False** depending on whether only distinct rows are returned. <br/> If this attribute is set to **True**, the **GenerateUpdateViews** attribute of the EntityContainerMapping element must be set to **False**. |

 

**1** The **TypeName** and **StoreEntitySet** attributes can be used in place of the EntityTypeMapping and MappingFragment child elements to map a single entity type to a single table.

### Example

The following example shows an **EntitySetMapping** element that maps three types (a base type and two derived types) in the **Courses** entity set of the conceptual model to three different tables in the underlying database. The tables are specified by the **StoreEntitySet** attribute in each **MappingFragment** element.

``` xml
 <EntitySetMapping Name="Courses">
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel1.Course)">
     <MappingFragment StoreEntitySet="Course">
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
       <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
       <ScalarProperty Name="Credits" ColumnName="Credits" />
       <ScalarProperty Name="Title" ColumnName="Title" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel1.OnlineCourse)">
     <MappingFragment StoreEntitySet="OnlineCourse">
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
       <ScalarProperty Name="URL" ColumnName="URL" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel1.OnsiteCourse)">
     <MappingFragment StoreEntitySet="OnsiteCourse">
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
       <ScalarProperty Name="Time" ColumnName="Time" />
       <ScalarProperty Name="Days" ColumnName="Days" />
       <ScalarProperty Name="Location" ColumnName="Location" />
     </MappingFragment>
   </EntityTypeMapping>
 </EntitySetMapping>
```

## EntityTypeMapping Element (MSL)

The **EntityTypeMapping** element in mapping specification language (MSL) defines the mapping between an entity type in the conceptual model and tables or views in the underlying database. For information about conceptual model entity types and underlying database tables or views, see EntityType Element (CSDL) and EntitySet Element (SSDL). The conceptual model entity type that is being mapped is specified by the **TypeName** attribute of the **EntityTypeMapping** element. The table or view that is being mapped is specified by the **StoreEntitySet** attribute of the child MappingFragment element.

The ModificationFunctionMapping child element can be used to map the insert, update, or delete functions of entity types to stored procedures in the database.

The **EntityTypeMapping** element can have the following child elements:

-   MappingFragment (zero or more)
-   ModificationFunctionMapping (zero or one)
-   ScalarProperty
-   Condition

> [!NOTE]
> **MappingFragment** and **ModificationFunctionMapping** elements cannot be child elements of the **EntityTypeMapping** element at the same time.


> [!NOTE]
> The **ScalarProperty** and **Condition** elements can only be child elements of the **EntityTypeMapping** element when it is used within a FunctionImportMapping element.

### Applicable Attributes

The following table describes the attributes that can be applied to the **EntityTypeMapping** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **TypeName**   | Yes         | The namespace-qualified name of the conceptual model entity type that is being mapped. <br/> If the type is abstract or a derived type, the value must be `IsOfType(Namespace-qualified_type_name)`. |

### Example

The following example shows an EntitySetMapping element with two child **EntityTypeMapping** elements. In the first **EntityTypeMapping** element, the **SchoolModel.Person** entity type is mapped to the **Person** table. In the second **EntityTypeMapping** element, the update functionality of the **SchoolModel.Person** type is mapped to a stored procedure, **UpdatePerson**, in the database.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <ScalarProperty Name="EnrollmentDate" ColumnName="EnrollmentDate" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <ModificationFunctionMapping>
       <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
         <ScalarProperty Name="EnrollmentDate" ParameterName="EnrollmentDate"
                         Version="Current" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate"
                         Version="Current" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName"
                         Version="Current" />
         <ScalarProperty Name="LastName" ParameterName="LastName"
                         Version="Current" />
         <ScalarProperty Name="PersonID" ParameterName="PersonID"
                         Version="Current" />
       </UpdateFunction>
     </ModificationFunctionMapping>
   </EntityTypeMapping>
 </EntitySetMapping>
```

### Example

The next example shows the mapping of a type hierarchy in which the root type is abstract. Note the use of the `IsOfType` syntax for the **TypeName** attributes.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Person)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Instructor)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <Condition ColumnName="HireDate" IsNull="false" />
       <Condition ColumnName="EnrollmentDate" IsNull="true" />
     </MappingFragment>
   </EntityTypeMapping>
   <EntityTypeMapping TypeName="IsTypeOf(SchoolModel.Student)">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="EnrollmentDate"
                       ColumnName="EnrollmentDate" />
       <Condition ColumnName="EnrollmentDate" IsNull="false" />
       <Condition ColumnName="HireDate" IsNull="true" />
     </MappingFragment>
   </EntityTypeMapping>
 </EntitySetMapping>
```

## FunctionImportMapping Element (MSL)

The **FunctionImportMapping** element in mapping specification language (MSL) defines the mapping between a function import in the conceptual model and a stored procedure or function in the underlying database. Function imports must be declared in the conceptual model and stored procedures must be declared in the storage model. For more information, see FunctionImport Element (CSDL) and Function Element (SSDL).

> [!NOTE]
> By default, if a function import returns a conceptual model entity type or complex type, then the names of the columns returned by the underlying stored procedure must exactly match the names of the properties on the conceptual model type. If the column names do not exactly match the property names, the mapping must be defined in a ResultMapping element.

The **FunctionImportMapping** element can have the following child elements:

-   ResultMapping (zero or more)

### Applicable Attributes

The following table describes the attributes that are applicable to the **FunctionImportMapping** element:

| Attribute Name         | Is Required | Value                                                                                   |
|:-----------------------|:------------|:----------------------------------------------------------------------------------------|
| **FunctionImportName** | Yes         | The name of the function import in the conceptual model that is being mapped.           |
| **FunctionName**       | Yes         | The namespace-qualified name of the function in the storage model that is being mapped. |

### Example

The following example is based on the School model. Consider the following function in the storage model:

``` xml
 <Function Name="GetStudentGrades" Aggregate="false"
           BuiltIn="false" NiladicFunction="false"
           IsComposable="false" ParameterTypeSemantics="AllowImplicitConversion"
           Schema="dbo">
   <Parameter Name="StudentID" Type="int" Mode="In" />
 </Function>
```

Also consider this function import in the conceptual model:

``` xml
 <FunctionImport Name="GetStudentGrades" EntitySet="StudentGrades"
                 ReturnType="Collection(SchoolModel.StudentGrade)">
   <Parameter Name="StudentID" Mode="In" Type="Int32" />
 </FunctionImport>
```

The following example shows a **FunctionImportMapping** element used to map the function and function import above to each other:

``` xml
 <FunctionImportMapping FunctionImportName="GetStudentGrades"
                        FunctionName="SchoolModel.Store.GetStudentGrades" />
```
 
## InsertFunction Element (MSL)

The **InsertFunction** element in mapping specification language (MSL) maps the insert function of an entity type or association in the conceptual model to a stored procedure in the underlying database. Stored procedures to which modification functions are mapped must be declared in the storage model. For more information, see Function Element (SSDL).

> [!NOTE]
> If you do not map all three of the insert, update, or delete operations of an entity type to stored procedures, the unmapped operations will fail if executed at runtime and an UpdateException is thrown.

The **InsertFunction** element can be a child of the ModificationFunctionMapping element and applied to the EntityTypeMapping element or the AssociationSetMapping element.

### InsertFunction Applied to EntityTypeMapping

When applied to the EntityTypeMapping element, the **InsertFunction** element maps the insert function of an entity type in the conceptual model to a stored procedure.

The **InsertFunction** element can have the following child elements when applied to an **EntityTypeMapping** element:

-   AssociationEnd (zero or more)
-   ComplexProperty (zero or more)
-   ResultBinding (zero or one)
-   ScalarProperty (zero or more)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **InsertFunction** element when applied to an **EntityTypeMapping** element.

| Attribute Name            | Is Required | Value                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| **FunctionName**          | Yes         | The namespace-qualified name of the stored procedure to which the insert function is mapped. The stored procedure must be declared in the storage model. |
| **RowsAffectedParameter** | No          | The name of the output parameter that returns the number of affected rows.                                                                               |

#### Example

The following example is based on the School model and shows the **InsertFunction** element used to map insert function of the Person entity type to the **InsertPerson** stored procedure. The **InsertPerson** stored procedure is declared in the storage model.

``` xml
 <EntityTypeMapping TypeName="SchoolModel.Person">
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName" />
       <ScalarProperty Name="LastName" ParameterName="LastName" />
       <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
     </InsertFunction>
     <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate"
                       Version="Current" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate"
                       Version="Current" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName"
                       Version="Current" />
       <ScalarProperty Name="LastName" ParameterName="LastName"
                       Version="Current" />
       <ScalarProperty Name="PersonID" ParameterName="PersonID"
                       Version="Current" />
     </UpdateFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
       <ScalarProperty Name="PersonID" ParameterName="PersonID" />
     </DeleteFunction>
   </ModificationFunctionMapping>
 </EntityTypeMapping>
```
### InsertFunction Applied to AssociationSetMapping

When applied to the AssociationSetMapping element, the **InsertFunction** element maps the insert function of an association in the conceptual model to a stored procedure.

The **InsertFunction** element can have the following child elements when applied to the **AssociationSetMapping** element:

-   EndProperty

#### Applicable Attributes

The following table describes the attributes that can be applied to the **InsertFunction** element when it is applied to the **AssociationSetMapping** element.

| Attribute Name            | Is Required | Value                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| **FunctionName**          | Yes         | The namespace-qualified name of the stored procedure to which the insert function is mapped. The stored procedure must be declared in the storage model. |
| **RowsAffectedParameter** | No          | The name of the output parameter that returns the number of rows affected.                                                                               |

#### Example

The following example is based on the School model and shows the **InsertFunction** element used to map insert function of the **CourseInstructor** association to the **InsertCourseInstructor** stored procedure. The **InsertCourseInstructor** stored procedure is declared in the storage model.

``` xml
 <AssociationSetMapping Name="CourseInstructor"
                        TypeName="SchoolModel.CourseInstructor"
                        StoreEntitySet="CourseInstructor">
   <EndProperty Name="Person">
     <ScalarProperty Name="PersonID" ColumnName="PersonID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertCourseInstructor" >   
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </InsertFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeleteCourseInstructor">
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </DeleteFunction>
   </ModificationFunctionMapping>
 </AssociationSetMapping>
```

## Mapping Element (MSL)

The **Mapping** element in mapping specification language (MSL) contains information for mapping objects that are defined in a conceptual model to a database (as described in a storage model). For more information, see CSDL Specification and SSDL Specification.

The **Mapping** element is the root element for a mapping specification. The XML namespace for mapping specifications is `https://schemas.microsoft.com/ado/2009/11/mapping/cs`.

The mapping element can have the following child elements (in the order listed):

-   Alias (zero or more)
-   EntityContainerMapping (exactly one)

Names of conceptual and storage model types that are referenced in MSL must be qualified by their respective namespace names. For information about the conceptual model namespace name, see Schema Element (CSDL). For information about the storage model namespace name, see Schema Element (SSDL). Aliases for namespaces that are used in MSL can be defined with the Alias element.

### Applicable Attributes

The table below describes the attributes that can be applied to the **Mapping** element.

| Attribute Name | Is Required | Value                                                 |
|:---------------|:------------|:------------------------------------------------------|
| **Space**      | Yes         | **C-S**. This is a fixed value and cannot be changed. |

### Example

The following example shows a **Mapping** element that is based on part of the School model. For more information about the School model, see Quickstart (Entity Framework):

``` xml
 <Mapping Space="C-S"
          xmlns="https://schemas.microsoft.com/ado/2009/11/mapping/cs">
   <Alias Key="c" Value="SchoolModel"/>
   <EntityContainerMapping StorageEntityContainer="SchoolModelStoreContainer"
                           CdmEntityContainer="SchoolModelEntities">
     <EntitySetMapping Name="Courses">
       <EntityTypeMapping TypeName="c.Course">
         <MappingFragment StoreEntitySet="Course">
           <ScalarProperty Name="CourseID" ColumnName="CourseID" />
           <ScalarProperty Name="Title" ColumnName="Title" />
           <ScalarProperty Name="Credits" ColumnName="Credits" />
           <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
         </MappingFragment>
       </EntityTypeMapping>
     </EntitySetMapping>
     <EntitySetMapping Name="Departments">
       <EntityTypeMapping TypeName="c.Department">
         <MappingFragment StoreEntitySet="Department">
           <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
           <ScalarProperty Name="Name" ColumnName="Name" />
           <ScalarProperty Name="Budget" ColumnName="Budget" />
           <ScalarProperty Name="StartDate" ColumnName="StartDate" />
           <ScalarProperty Name="Administrator" ColumnName="Administrator" />
         </MappingFragment>
       </EntityTypeMapping>
     </EntitySetMapping>
   </EntityContainerMapping>
 </Mapping>
```

## MappingFragment Element (MSL)

The **MappingFragment** element in mapping specification language (MSL) defines the mapping between the properties of a conceptual model entity type and a table or view in the database. For information about conceptual model entity types and underlying database tables or views, see EntityType Element (CSDL) and EntitySet Element (SSDL). The **MappingFragment** can be a child element of the EntityTypeMapping element or the EntitySetMapping element.

The **MappingFragment** element can have the following child elements:

-   ComplexType (zero or more)
-   ScalarProperty (zero or more)
-   Condition (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **MappingFragment** element.

| Attribute Name          | Is Required | Value                                                                                                                                                                                                                         |
|:------------------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **StoreEntitySet**      | Yes         | The name of the table or view that is being mapped.                                                                                                                                                                           |
| **MakeColumnsDistinct** | No          | **True** or **False** depending on whether only distinct rows are returned. <br/> If this attribute is set to **True**, the **GenerateUpdateViews** attribute of the EntityContainerMapping element must be set to **False**. |

### Example

The following example shows a **MappingFragment** element as the child of an **EntityTypeMapping** element. In this example, properties of the **Course** type in the conceptual model are mapped to columns of the **Course** table in the database.

``` xml
 <EntitySetMapping Name="Courses">
   <EntityTypeMapping TypeName="SchoolModel.Course">
     <MappingFragment StoreEntitySet="Course">
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
       <ScalarProperty Name="Title" ColumnName="Title" />
       <ScalarProperty Name="Credits" ColumnName="Credits" />
       <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
     </MappingFragment>
   </EntityTypeMapping>
 </EntitySetMapping>
```

### Example

The following example shows a **MappingFragment** element as the child of an **EntitySetMapping** element. As in the example above, properties of the **Course** type in the conceptual model are mapped to columns of the **Course** table in the database.

``` xml
 <EntitySetMapping Name="Courses" TypeName="SchoolModel.Course">
     <MappingFragment StoreEntitySet="Course">
       <ScalarProperty Name="CourseID" ColumnName="CourseID" />
       <ScalarProperty Name="Title" ColumnName="Title" />
       <ScalarProperty Name="Credits" ColumnName="Credits" />
       <ScalarProperty Name="DepartmentID" ColumnName="DepartmentID" />
     </MappingFragment>
 </EntitySetMapping>
```

## ModificationFunctionMapping Element (MSL)

The **ModificationFunctionMapping** element in mapping specification language (MSL) maps the insert, update, and delete functions of a conceptual model entity type to stored procedures in the underlying database. The **ModificationFunctionMapping** element can also map the insert and delete functions for many-to-many associations in the conceptual model to stored procedures in the underlying database. Stored procedures to which modification functions are mapped must be declared in the storage model. For more information, see Function Element (SSDL).

> [!NOTE]
> If you do not map all three of the insert, update, or delete operations of an entity type to stored procedures, the unmapped operations will fail if executed at runtime and an UpdateException is thrown.


> [!NOTE]
> If the modification functions for one entity in an inheritance hierarchy are mapped to stored procedures, then modification functions for all types in the hierarchy must be mapped to stored procedures.

The **ModificationFunctionMapping** element can be a child of the EntityTypeMapping element or the AssociationSetMapping element.

The **ModificationFunctionMapping** element can have the following child elements:

-   DeleteFunction (zero or one)
-   InsertFunction (zero or one)
-   UpdateFunction (zero or one)

No attributes are applicable to the **ModificationFunctionMapping** element.

### Example

The following example shows the entity set mapping for the **People** entity set in the School model. In addition to the column mapping for the **Person** entity type, the mapping of the insert, update, and delete functions of the **Person** type are shown. The functions that are mapped to are declared in the storage model.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <ScalarProperty Name="EnrollmentDate"
                       ColumnName="EnrollmentDate" />
     </MappingFragment>
 </EntityTypeMapping>
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <ModificationFunctionMapping>
       <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName" />
         <ScalarProperty Name="LastName" ParameterName="LastName" />
         <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
       </InsertFunction>
       <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate"
                         Version="Current" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate"
                         Version="Current" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName"
                         Version="Current" />
         <ScalarProperty Name="LastName" ParameterName="LastName"
                         Version="Current" />
         <ScalarProperty Name="PersonID" ParameterName="PersonID"
                         Version="Current" />
       </UpdateFunction>
       <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
         <ScalarProperty Name="PersonID" ParameterName="PersonID" />
       </DeleteFunction>
     </ModificationFunctionMapping>
   </EntityTypeMapping>
 </EntitySetMapping>
```

### Example

The following example shows the association set mapping for the **CourseInstructor** association set in the School model. In addition to the column mapping for the **CourseInstructor** association, the mapping of the insert and delete functions of the **CourseInstructor** association are shown. The functions that are mapped to are declared in the storage model.

``` xml
 <AssociationSetMapping Name="CourseInstructor"
                        TypeName="SchoolModel.CourseInstructor"
                        StoreEntitySet="CourseInstructor">
   <EndProperty Name="Person">
     <ScalarProperty Name="PersonID" ColumnName="PersonID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertCourseInstructor" >   
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </InsertFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeleteCourseInstructor">
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </DeleteFunction>
   </ModificationFunctionMapping>
 </AssociationSetMapping>
```
 

 

## QueryView Element (MSL)

The **QueryView** element in mapping specification language (MSL) defines a read-only mapping between an entity type or association in the conceptual model and a table in the underlying database. The mapping is defined with an Entity SQL query that is evaluated against the storage model, and you express the result set in terms of an entity or association in the conceptual model. Because query views are read-only, you cannot use standard update commands to update types that are defined by query views. You can make updates to these types by using modification functions. For more information, see How to: Map Modification Functions to Stored Procedures.

> [!NOTE]
> In the **QueryView** element, Entity SQL expressions that contain **GroupBy**, group aggregates, or navigation properties are not supported.

 

The **QueryView** element can be a child of the EntitySetMapping element or the AssociationSetMapping element. In the former case, the query view defines a read-only mapping for an entity in the conceptual model. In the latter case, the query view defines a read-only mapping for an association in the conceptual model.

> [!NOTE]
> If the **AssociationSetMapping** element is for an association with a referential constraint, the **AssociationSetMapping** element is ignored. For more information, see ReferentialConstraint Element (CSDL).

The **QueryView** element cannot have any child elements.

### Applicable Attributes

The following table describes the attributes that can be applied to the **QueryView** element.

| Attribute Name | Is Required | Value                                                                         |
|:---------------|:------------|:------------------------------------------------------------------------------|
| **TypeName**   | No          | The name of the conceptual model type that is being mapped by the query view. |

### Example

The following example shows the **QueryView** element as a child of the **EntitySetMapping** element and defines a query view mapping for the **Department** entity type in the School Model.

``` xml
 <EntitySetMapping Name="Departments" >
   <QueryView>
     SELECT VALUE SchoolModel.Department(d.DepartmentID,
                                         d.Name,
                                         d.Budget,
                                         d.StartDate)
     FROM SchoolModelStoreContainer.Department AS d
     WHERE d.Budget > 150000
   </QueryView>
 </EntitySetMapping>
```

Because the query only returns a subset of the members of the **Department** type in the storage model, the **Department** type in the School model has been modified based on this mapping as follows:

``` xml
 <EntityType Name="Department">
   <Key>
     <PropertyRef Name="DepartmentID" />
   </Key>
   <Property Type="Int32" Name="DepartmentID" Nullable="false" />
   <Property Type="String" Name="Name" Nullable="false"
             MaxLength="50" FixedLength="false" Unicode="true" />
   <Property Type="Decimal" Name="Budget" Nullable="false"
             Precision="19" Scale="4" />
   <Property Type="DateTime" Name="StartDate" Nullable="false" />
   <NavigationProperty Name="Courses"
                       Relationship="SchoolModel.FK_Course_Department"
                       FromRole="Department" ToRole="Course" />
 </EntityType>
```

### Example

The next example shows the **QueryView** element as the child of an **AssociationSetMapping** element and defines a read-only mapping for the `FK_Course_Department` association in the School model.

``` xml
 <EntityContainerMapping StorageEntityContainer="SchoolModelStoreContainer"
                         CdmEntityContainer="SchoolEntities">
   <EntitySetMapping Name="Courses" >
     <QueryView>
       SELECT VALUE SchoolModel.Course(c.CourseID,
                                       c.Title,
                                       c.Credits)
       FROM SchoolModelStoreContainer.Course AS c
     </QueryView>
   </EntitySetMapping>
   <EntitySetMapping Name="Departments" >
     <QueryView>
       SELECT VALUE SchoolModel.Department(d.DepartmentID,
                                           d.Name,
                                           d.Budget,
                                           d.StartDate)
       FROM SchoolModelStoreContainer.Department AS d
       WHERE d.Budget > 150000
     </QueryView>
   </EntitySetMapping>
   <AssociationSetMapping Name="FK_Course_Department" >
     <QueryView>
       SELECT VALUE SchoolModel.FK_Course_Department(
         CREATEREF(SchoolEntities.Departments, row(c.DepartmentID), SchoolModel.Department),
         CREATEREF(SchoolEntities.Courses, row(c.CourseID)) )
       FROM SchoolModelStoreContainer.Course AS c
     </QueryView>
   </AssociationSetMapping>
 </EntityContainerMapping>
```
 
### Comments

You can define query views to enable the following scenarios:

-   Define an entity in the conceptual model that doesn't include all the properties of the entity in the storage model. This includes properties that do not have default values and do not support **null** values.
-   Map computed columns in the storage model to properties of entity types in the conceptual model.
-   Define a mapping where conditions used to partition entities in the conceptual model are not based on equality. When you specify a conditional mapping using the **Condition** element, the supplied condition must equal the specified value. For more information, see Condition Element (MSL).
-   Map the same column in the storage model to multiple types in the conceptual model.
-   Map multiple types to the same table.
-   Define associations in the conceptual model that are not based on foreign keys in the relational schema.
-   Use custom business logic to set the value of properties in the conceptual model. For example, you could map the string value "T" in the data source to a value of **true**, a Boolean, in the conceptual model.
-   Define conditional filters for query results.
-   Enforce fewer restrictions on data in the conceptual model than in the storage model. For example, you could make a property in the conceptual model nullable even if the column to which it is mapped does not support **null**values.

The following considerations apply when you define query views for entities:

-   Query views are read-only. You can only make updates to entities by using modification functions.
-   When you define an entity type by a query view, you must also define all related entities by query views.
-   When you map a many-to-many association to an entity in the storage model that represents a link table in the relational schema, you must define a **QueryView** element in the **AssociationSetMapping** element for this link table.
-   Query views must be defined for all types in a type hierarchy. You can do this in the following ways:
-   -   With a single **QueryView** element that specifies a single Entity SQL query that returns a union of all of the entity types in the hierarchy.
    -   With a single **QueryView** element that specifies a single Entity SQL query that uses the CASE operator to return a specific entity type in the hierarchy based on a specific condition.
    -   With an additional **QueryView** element for a specific type in the hierarchy. In this case, use the **TypeName** attribute of the **QueryView** element to specify the entity type for each view.
-   When a query view is defined, you cannot specify the **StorageSetName** attribute on the **EntitySetMapping** element.
-   When a query view is defined, the **EntitySetMapping**element cannot also contain **Property** mappings.

## ResultBinding Element (MSL)

The **ResultBinding** element in mapping specification language (MSL) maps column values that are returned by stored procedures to entity properties in the conceptual model when entity type modification functions are mapped to stored procedures in the underlying database. For example, when the value of an identity column is returned by an insert stored procedure, the **ResultBinding** element maps the returned value to an entity type property in the conceptual model.

The **ResultBinding** element can be child of the InsertFunction element or the UpdateFunction element.

The **ResultBinding** element cannot have any child elements.

### Applicable Attributes

The following table describes the attributes that are applicable to the **ResultBinding** element:

| Attribute Name | Is Required | Value                                                                         |
|:---------------|:------------|:------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity property in the conceptual model that is being mapped. |
| **ColumnName** | Yes         | The name of the column being mapped.                                          |

### Example

The following example is based on the School model and shows an **InsertFunction** element used to map the insert function of the **Person** entity type to the **InsertPerson** stored procedure. (The **InsertPerson** stored procedure is shown below and is declared in the storage model.) A **ResultBinding** element is used to map a column value that is returned by the stored procedure (**NewPersonID**) to an entity type property (**PersonID**).

``` xml
 <EntityTypeMapping TypeName="SchoolModel.Person">
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName" />
       <ScalarProperty Name="LastName" ParameterName="LastName" />
       <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
     </InsertFunction>
     <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate"
                       Version="Current" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate"
                       Version="Current" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName"
                       Version="Current" />
       <ScalarProperty Name="LastName" ParameterName="LastName"
                       Version="Current" />
       <ScalarProperty Name="PersonID" ParameterName="PersonID"
                       Version="Current" />
     </UpdateFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
       <ScalarProperty Name="PersonID" ParameterName="PersonID" />
     </DeleteFunction>
   </ModificationFunctionMapping>
 </EntityTypeMapping>
```

The following Transact-SQL describes the **InsertPerson** stored procedure:

``` SQL
 CREATE PROCEDURE [dbo].[InsertPerson]
                                @LastName nvarchar(50),
                                @FirstName nvarchar(50),
                                @HireDate datetime,
                                @EnrollmentDate datetime
                                AS
                                INSERT INTO dbo.Person (LastName,
                                                                             FirstName,
                                                                             HireDate,
                                                                             EnrollmentDate)
                                VALUES (@LastName,
                                               @FirstName,
                                               @HireDate,
                                               @EnrollmentDate);
                                SELECT SCOPE_IDENTITY() as NewPersonID;
```

## ResultMapping Element (MSL)

The **ResultMapping** element in mapping specification language (MSL) defines the mapping between a function import in the conceptual model and a stored procedure in the underlying database when the following are true:

-   The function import returns a conceptual model entity type or complex type.
-   The names of the columns returned by the stored procedure do not exactly match the names of the properties on the entity type or complex type.

By default, the mapping between the columns returned by a stored procedure and an entity type or complex type is based on column and property names. If column names do not exactly match property names, you must use the **ResultMapping** element to define the mapping. For an example of the default mapping, see FunctionImportMapping Element (MSL).

The **ResultMapping** element is a child element of the FunctionImportMapping element.

The **ResultMapping** element can have the following child elements:

-   EntityTypeMapping (zero or more)
-   ComplexTypeMapping

No attributes are applicable to the **ResultMapping** Element.

### Example

Consider the following stored procedure:

``` SQL
 CREATE PROCEDURE [dbo].[GetGrades]
             @student_Id int
             AS
             SELECT     EnrollmentID as enroll_id,
                                                                             Grade as grade,
                                                                             CourseID as course_id,
                                                                             StudentID as student_id
                                               FROM dbo.StudentGrade
             WHERE StudentID = @student_Id
```

Also consider the following conceptual model entity type:

``` xml
 <EntityType Name="StudentGrade">
   <Key>
     <PropertyRef Name="EnrollmentID" />
   </Key>
   <Property Name="EnrollmentID" Type="Int32" Nullable="false"
             annotation:StoreGeneratedPattern="Identity" />
   <Property Name="CourseID" Type="Int32" Nullable="false" />
   <Property Name="StudentID" Type="Int32" Nullable="false" />
   <Property Name="Grade" Type="Decimal" Precision="3" Scale="2" />
 </EntityType>
```

In order to create a function import that returns instances of the previous entity type, the mapping between the columns returned by the stored procedure and the entity type must be defined in a **ResultMapping** element:

``` xml
 <FunctionImportMapping FunctionImportName="GetGrades"
                        FunctionName="SchoolModel.Store.GetGrades" >
   <ResultMapping>
     <EntityTypeMapping TypeName="SchoolModel.StudentGrade">
       <ScalarProperty Name="EnrollmentID" ColumnName="enroll_id"/>
       <ScalarProperty Name="CourseID" ColumnName="course_id"/>
       <ScalarProperty Name="StudentID" ColumnName="student_id"/>
       <ScalarProperty Name="Grade" ColumnName="grade"/>
     </EntityTypeMapping>
   </ResultMapping>
 </FunctionImportMapping>
```

## ScalarProperty Element (MSL)

The **ScalarProperty** element in mapping specification language (MSL) maps a property on a conceptual model entity type, complex type, or association to a table column or stored procedure parameter in the underlying database.

> [!NOTE]
> Stored procedures to which modification functions are mapped must be declared in the storage model. For more information, see Function Element (SSDL).

The **ScalarProperty** element can be a child of the following elements:

-   MappingFragment
-   InsertFunction
-   UpdateFunction
-   DeleteFunction
-   EndProperty
-   ComplexProperty
-   ResultMapping

As a child of the **MappingFragment**, **ComplexProperty**, or **EndProperty** element, the **ScalarProperty** element maps a property in the conceptual model to a column in the database. As a child of the **InsertFunction**, **UpdateFunction**, or **DeleteFunction** element, the **ScalarProperty** element maps a property in the conceptual model to a stored procedure parameter.

The **ScalarProperty** element cannot have any child elements.

### Applicable Attributes

The attributes that apply to the **ScalarProperty** element differ depending on the role of the element.

The following table describes the attributes that are applicable when the **ScalarProperty** element is used to map a conceptual model property to a column in the database:

| Attribute Name | Is Required | Value                                                           |
|:---------------|:------------|:----------------------------------------------------------------|
| **Name**       | Yes         | The name of the conceptual model property that is being mapped. |
| **ColumnName** | Yes         | The name of the table column that is being mapped.              |

The following table describes the attributes that are applicable to the **ScalarProperty** element when it is used to map a conceptual model property to a stored procedure parameter:

| Attribute Name    | Is Required | Value                                                                                                                                           |
|:------------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**          | Yes         | The name of the conceptual model property that is being mapped.                                                                                 |
| **ParameterName** | Yes         | The name of the parameter that is being mapped.                                                                                                 |
| **Version**       | No          | **Current** or **Original** depending on whether the current value or the original value of the property should be used for concurrency checks. |

### Example

The following example shows the **ScalarProperty** element used in two ways:

-   To map the properties of the **Person** entity type to the columns of the **Person**table.
-   To map the properties of the **Person** entity type to the parameters of the **UpdatePerson** stored procedure. The stored procedures are declared in the storage model.

``` xml
 <EntitySetMapping Name="People">
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <MappingFragment StoreEntitySet="Person">
       <ScalarProperty Name="PersonID" ColumnName="PersonID" />
       <ScalarProperty Name="LastName" ColumnName="LastName" />
       <ScalarProperty Name="FirstName" ColumnName="FirstName" />
       <ScalarProperty Name="HireDate" ColumnName="HireDate" />
       <ScalarProperty Name="EnrollmentDate"
                       ColumnName="EnrollmentDate" />
     </MappingFragment>
 </EntityTypeMapping>
   <EntityTypeMapping TypeName="SchoolModel.Person">
     <ModificationFunctionMapping>
       <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName" />
         <ScalarProperty Name="LastName" ParameterName="LastName" />
         <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
       </InsertFunction>
       <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
         <ScalarProperty Name="EnrollmentDate"
                         ParameterName="EnrollmentDate"
                         Version="Current" />
         <ScalarProperty Name="HireDate" ParameterName="HireDate"
                         Version="Current" />
         <ScalarProperty Name="FirstName" ParameterName="FirstName"
                         Version="Current" />
         <ScalarProperty Name="LastName" ParameterName="LastName"
                         Version="Current" />
         <ScalarProperty Name="PersonID" ParameterName="PersonID"
                         Version="Current" />
       </UpdateFunction>
       <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
         <ScalarProperty Name="PersonID" ParameterName="PersonID" />
       </DeleteFunction>
     </ModificationFunctionMapping>
   </EntityTypeMapping>
 </EntitySetMapping>
```

### Example

The next example shows the **ScalarProperty** element used to map the insert and delete functions of a conceptual model association to stored procedures in the database. The stored procedures are declared in the storage model.

``` xml
 <AssociationSetMapping Name="CourseInstructor"
                        TypeName="SchoolModel.CourseInstructor"
                        StoreEntitySet="CourseInstructor">
   <EndProperty Name="Person">
     <ScalarProperty Name="PersonID" ColumnName="PersonID" />
   </EndProperty>
   <EndProperty Name="Course">
     <ScalarProperty Name="CourseID" ColumnName="CourseID" />
   </EndProperty>
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertCourseInstructor" >   
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </InsertFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeleteCourseInstructor">
       <EndProperty Name="Course">
         <ScalarProperty Name="CourseID" ParameterName="courseId"/>
       </EndProperty>
       <EndProperty Name="Person">
         <ScalarProperty Name="PersonID" ParameterName="instructorId"/>
       </EndProperty>
     </DeleteFunction>
   </ModificationFunctionMapping>
 </AssociationSetMapping>
```

## UpdateFunction Element (MSL)

The **UpdateFunction** element in mapping specification language (MSL) maps the update function of an entity type in the conceptual model to a stored procedure in the underlying database. Stored procedures to which modification functions are mapped must be declared in the storage model. For more information, see Function Element (SSDL).

> [!NOTE]
>  If you do not map all three of the insert, update, or delete operations of an entity type to stored procedures, the unmapped operations will fail if executed at runtime and an UpdateException is thrown.

The **UpdateFunction** element can be a child of the ModificationFunctionMapping element and applied to the EntityTypeMapping element.

The **UpdateFunction** element can have the following child elements:

-   AssociationEnd (zero or more)
-   ComplexProperty (zero or more)
-   ResultBinding (zero or one)
-   ScalarProperty (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **UpdateFunction** element.

| Attribute Name            | Is Required | Value                                                                                                                                                    |
|:--------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|
| **FunctionName**          | Yes         | The namespace-qualified name of the stored procedure to which the update function is mapped. The stored procedure must be declared in the storage model. |
| **RowsAffectedParameter** | No          | The name of the output parameter that returns the number of rows affected.                                                                               |

### Example

The following example is based on the School model and shows the **UpdateFunction** element used to map update function of the **Person** entity type to the **UpdatePerson** stored procedure. The **UpdatePerson** stored procedure is declared in the storage model.

``` xml
 <EntityTypeMapping TypeName="SchoolModel.Person">
   <ModificationFunctionMapping>
     <InsertFunction FunctionName="SchoolModel.Store.InsertPerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName" />
       <ScalarProperty Name="LastName" ParameterName="LastName" />
       <ResultBinding Name="PersonID" ColumnName="NewPersonID" />
     </InsertFunction>
     <UpdateFunction FunctionName="SchoolModel.Store.UpdatePerson">
       <ScalarProperty Name="EnrollmentDate"
                       ParameterName="EnrollmentDate"
                       Version="Current" />
       <ScalarProperty Name="HireDate" ParameterName="HireDate"
                       Version="Current" />
       <ScalarProperty Name="FirstName" ParameterName="FirstName"
                       Version="Current" />
       <ScalarProperty Name="LastName" ParameterName="LastName"
                       Version="Current" />
       <ScalarProperty Name="PersonID" ParameterName="PersonID"
                       Version="Current" />
     </UpdateFunction>
     <DeleteFunction FunctionName="SchoolModel.Store.DeletePerson">
       <ScalarProperty Name="PersonID" ParameterName="PersonID" />
     </DeleteFunction>
   </ModificationFunctionMapping>
 </EntityTypeMapping>
```
