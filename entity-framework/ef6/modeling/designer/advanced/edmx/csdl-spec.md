---
title: CSDL Specification - EF6
description: CSDL Specification in Entity Framework 6
author: SamMonoRT
ms.date: 08/23/2024
uid: ef6/modeling/designer/advanced/edmx/csdl-spec
---
# CSDL Specification

> [!NOTE]
> CSDL v1 is unsupported, please update to V3

Conceptual schema definition language (CSDL) is an XML-based language that describes the entities, relationships, and functions that make up a conceptual model of a data-driven application. This conceptual model can be used by the Entity Framework or WCF Data Services. The metadata that is described with CSDL is used by the Entity Framework to map entities and relationships that are defined in a conceptual model to a data source. For more information, see [SSDL Specification](xref:ef6/modeling/designer/advanced/edmx/ssdl-spec) and [MSL Specification](xref:ef6/modeling/designer/advanced/edmx/msl-spec).

CSDL is the Entity Framework's implementation of the Entity Data Model.

In an Entity Framework application, conceptual model metadata is loaded from a .csdl file (written in CSDL) into an instance of the System.Data.Metadata.Edm.EdmItemCollection and is accessible by using methods in the System.Data.Metadata.Edm.MetadataWorkspace class. Entity Framework uses conceptual model metadata to translate queries against the conceptual model to data source-specific commands.

The EF Designer stores conceptual model information in an .edmx file at design time. At build time, the EF Designer uses information in an .edmx file to create the .csdl file that is needed by Entity Framework at runtime.

Versions of CSDL are differentiated by XML namespaces.

| CSDL Version | XML Namespace                                |
|:-------------|:---------------------------------------------|
| CSDL v1      | `https://schemas.microsoft.com/ado/2006/04/edm` |
| CSDL v2      | `https://schemas.microsoft.com/ado/2008/09/edm` |
| CSDL v3      | `https://schemas.microsoft.com/ado/2009/11/edm` |

## Association Element (CSDL)

An **Association** element defines a relationship between two entity types. An association must specify the entity types that are involved in the relationship and the possible number of entity types at each end of the relationship, which is known as the multiplicity. The multiplicity of an association end can have a value of one (1), zero or one (0..1), or many (\*). This information is specified in two child End elements.

Entity type instances at one end of an association can be accessed through navigation properties or foreign keys, if they are exposed on an entity type.

In an application, an instance of an association represents a specific association between instances of entity types. Association instances are logically grouped in an association set.

An **Association** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   End (exactly 2 elements)
-   ReferentialConstraint (zero or one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **Association** element.

| Attribute Name | Is Required | Value                        |
|:---------------|:------------|:-----------------------------|
| **Name**       | Yes         | The name of the association. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Association** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **Association** element that defines the **CustomerOrders** association when foreign keys have not been exposed on the **Customer** and **Order** entity types. The **Multiplicity** values for each **End** of the association indicate that many **Orders** can be associated with a **Customer**, but only one **Customer** can be associated with an **Order**. Additionally, the **OnDelete** element indicates that all **Orders** that are related to a particular **Customer** and have been loaded into the ObjectContext will be deleted if the **Customer** is deleted.

``` xml
 <Association Name="CustomerOrders">
   <End Type="ExampleModel.Customer" Role="Customer" Multiplicity="1" >
         <OnDelete Action="Cascade" />
   </End>
   <End Type="ExampleModel.Order" Role="Order" Multiplicity="*" />
 </Association>
```
 

The following example shows an **Association** element that defines the **CustomerOrders** association when foreign keys have been exposed on the **Customer** and **Order** entity types. With foreign keys exposed, the relationship between the entities is managed with a **ReferentialConstraint** element. A corresponding AssociationSetMapping element is not necessary to map this association to the data source.

``` xml
 <Association Name="CustomerOrders">
   <End Type="ExampleModel.Customer" Role="Customer" Multiplicity="1" >
         <OnDelete Action="Cascade" />
   </End>
   <End Type="ExampleModel.Order" Role="Order" Multiplicity="*" />
   <ReferentialConstraint>
        <Principal Role="Customer">
            <PropertyRef Name="Id" />
        </Principal>
        <Dependent Role="Order">
             <PropertyRef Name="CustomerId" />
         </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## AssociationSet Element (CSDL)

The **AssociationSet** element in conceptual schema definition language (CSDL) is a logical container for association instances of the same type. An association set provides a definition for grouping association instances so that they can be mapped to a data source.  

The **AssociationSet** element can have the following child elements (in the order listed):

-   Documentation (zero or one elements allowed)
-   End (exactly two elements required)
-   Annotation elements (zero or more elements allowed)

The **Association** attribute specifies the type of association that an association set contains. The entity sets that make up the ends of an association set are specified with exactly two child **End** elements.

### Applicable Attributes

The table below describes the attributes that can be applied to the **AssociationSet** element.

| Attribute Name  | Is Required | Value                                                                                                                                                             |
|:----------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**        | Yes         | The name of the entity set. The value of the **Name** attribute cannot be the same as the value of the **Association** attribute.                                 |
| **Association** | Yes         | The fully-qualified name of the association that the association set contains instances of. The association must be in the same namespace as the association set. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **AssociationSet** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityContainer** element with two **AssociationSet** elements:

``` xml
 <EntityContainer Name="BooksContainer" >
   <EntitySet Name="Books" EntityType="BooksModel.Book" />
   <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
   <EntitySet Name="Authors" EntityType="BooksModel.Author" />
   <AssociationSet Name="PublishedBy" Association="BooksModel.PublishedBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Publisher" EntitySet="Publishers" />
   </AssociationSet>
   <AssociationSet Name="WrittenBy" Association="BooksModel.WrittenBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Author" EntitySet="Authors" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## CollectionType Element (CSDL)

The **CollectionType** element in conceptual schema definition language (CSDL) specifies that a function parameter or function return type is a collection. The **CollectionType** element can be a child of the Parameter element or the ReturnType (Function) element. The type of collection can be specified by using either the **Type** attribute or one of the following child elements:

-   **CollectionType**
-   ReferenceType
-   RowType
-   TypeRef

> [!NOTE]
> A model will not validate if the type of a collection is specified with both the **Type** attribute and a child element.

 

### Applicable Attributes

The following table describes the attributes that can be applied to the **CollectionType** element. Note that the **DefaultValue**, **MaxLength**, **FixedLength**, **Precision**, **Scale**, **Unicode**, and **Collation** attributes are only applicable to collections of **EDMSimpleTypes**.

| Attribute Name                                                          | Is Required | Value                                                                                                                                                                                                                            |
|:------------------------------------------------------------------------|:------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Type**                                                                | No          | The type of the collection.                                                                                                                                                                                                      |
| **Nullable**                                                            | No          | **True** (the default value) or **False** depending on whether the property can have a null value. <br/> [!NOTE]                                                                                                                 |
| > In the CSDL v1, a complex type property must have `Nullable="False"`. |             |                                                                                                                                                                                                                                  |
| **DefaultValue**                                                        | No          | The default value of the property.                                                                                                                                                                                               |
| **MaxLength**                                                           | No          | The maximum length of the property value.                                                                                                                                                                                        |
| **FixedLength**                                                         | No          | **True** or **False** depending on whether the property value will be stored as a fixed length string.                                                                                                                           |
| **Precision**                                                           | No          | The precision of the property value.                                                                                                                                                                                             |
| **Scale**                                                               | No          | The scale of the property value.                                                                                                                                                                                                 |
| **SRID**                                                                | No          | Spatial System Reference Identifier. Valid only for properties of spatial types.   For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx) |
| **Unicode**                                                             | No          | **True** or **False** depending on whether the property value will be stored as a Unicode string.                                                                                                                                |
| **Collation**                                                           | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                    |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **CollectionType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a model-defined function that that uses a **CollectionType** element to specify that the function returns a collection of **Person** entity types (as specified with the **ElementType** attribute).

``` xml
 <Function Name="LastNamesAfter">
        <Parameter Name="someString" Type="Edm.String"/>
        <ReturnType>
             <CollectionType  ElementType="SchoolModel.Person"/>
        </ReturnType>
        <DefiningExpression>
             SELECT VALUE p
             FROM SchoolEntities.People AS p
             WHERE p.LastName >= someString
        </DefiningExpression>
 </Function>
```
 

The following example shows a model-defined function that uses a **CollectionType** element to specify that the function returns a collection of rows (as specified in the **RowType** element).

``` xml
 <Function Name="LastNamesAfter">
   <Parameter Name="someString" Type="Edm.String" />
   <ReturnType>
    <CollectionType>
      <RowType>
        <Property Name="FirstName" Type="Edm.String" Nullable="false" />
        <Property Name="LastName" Type="Edm.String" Nullable="false" />
      </RowType>
    </CollectionType>
   </ReturnType>
   <DefiningExpression>
             SELECT VALUE ROW(p.FirstName, p.LastName)
             FROM SchoolEntities.People AS p
             WHERE p.LastName &gt;= somestring
   </DefiningExpression>
 </Function>
```
 

The following example shows a model-defined function that uses the **CollectionType** element to specify that the function accepts as a parameter a collection of **Department** entity types.

``` xml
 <Function Name="GetAvgBudget">
      <Parameter Name="Departments">
          <CollectionType>
             <TypeRef Type="SchoolModel.Department"/>
          </CollectionType>
           </Parameter>
       <ReturnType Type="Collection(Edm.Decimal)"/>
       <DefiningExpression>
             SELECT VALUE AVG(d.Budget) FROM Departments AS d
       </DefiningExpression>
 </Function>
```
 

 

## ComplexType Element (CSDL)

A **ComplexType** element defines a data structure composed of **EdmSimpleType** properties or other complex types.  A complex type can be a property of an entity type or another complex type. A complex type is similar to an entity type in that a complex type defines data. However, there are some key differences between complex types and entity types:

-   Complex types do not have identities (or keys) and therefore cannot exist independently. Complex types can only exist as properties of entity types or other complex types.
-   Complex types cannot participate in associations. Neither end of an association can be a complex type, and therefore navigation properties cannot be defined for complex types.
-   A complex type property cannot have a null value, though the scalar properties of a complex type may each be set to null.

A **ComplexType** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Property (zero or more elements)
-   Annotation elements (zero or more elements)

The table below describes the attributes that can be applied to the **ComplexType** element.

| Attribute Name                                                                                                 | Is Required | Value                                                                                                                                                                               |
|:---------------------------------------------------------------------------------------------------------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Name                                                                                                           | Yes         | The name of the complex type. The name of a complex type cannot be the same as the name of another complex type, entity type, or association that is within the scope of the model. |
| BaseType                                                                                                       | No          | The name of another complex type that is the base type of the complex type that is being defined. <br/> [!NOTE]                                                                     |
| > This attribute is not applicable in CSDL v1. Inheritance for complex types is not supported in that version. |             |                                                                                                                                                                                     |
| Abstract                                                                                                       | No          | **True** or **False** (the default value) depending on whether the complex type is an abstract type. <br/> [!NOTE]                                                                  |
| > This attribute is not applicable in CSDL v1. Complex types in that version cannot be abstract types.         |             |                                                                                                                                                                                     |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **ComplexType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a complex type, **Address**, with the **EdmSimpleType** properties **StreetAddress**, **City**, **StateOrProvince**, **Country**, and **PostalCode**.

``` xml
 <ComplexType Name="Address" >
   <Property Type="String" Name="StreetAddress" Nullable="false" />
   <Property Type="String" Name="City" Nullable="false" />
   <Property Type="String" Name="StateOrProvince" Nullable="false" />
   <Property Type="String" Name="Country" Nullable="false" />
   <Property Type="String" Name="PostalCode" Nullable="false" />
 </ComplexType>
```
 

To define the complex type **Address** (above) as a property of an entity type, you must declare the property type in the entity type definition. The following example shows the **Address** property as a complex type on an entity type (**Publisher**):

``` xml
 <EntityType Name="Publisher">
       <Key>
         <PropertyRef Name="Id" />
       </Key>
       <Property Type="Int32" Name="Id" Nullable="false" />
       <Property Type="String" Name="Name" Nullable="false" />
       <Property Type="BooksModel.Address" Name="Address" Nullable="false" />
       <NavigationProperty Name="Books" Relationship="BooksModel.PublishedBy"
                           FromRole="Publisher" ToRole="Book" />
     </EntityType>
```
 

 

## DefiningExpression Element (CSDL)

The **DefiningExpression** element in conceptual schema definition language (CSDL) contains an Entity SQL expression that defines a function in the conceptual model.  

> [!NOTE]
> For validation purposes, a **DefiningExpression** element can contain arbitrary content. However, Entity Framework will throw an exception at runtime if a **DefiningExpression** element does not contain valid Entity SQL.

 

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **DefiningExpression** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example uses a **DefiningExpression** element to define a function that returns the number of years since a book was published. The content of the **DefiningExpression** element is written in Entity SQL.

``` xml
 <Function Name="GetYearsInPrint" ReturnType="Edm.Int32" >
       <Parameter Name="book" Type="BooksModel.Book" />
       <DefiningExpression>
         Year(CurrentDateTime()) - Year(cast(book.PublishedDate as DateTime))
       </DefiningExpression>
     </Function>
```
 

 

## Dependent Element (CSDL)

The **Dependent** element in conceptual schema definition language (CSDL) is a child element to the ReferentialConstraint element and defines the dependent end of a referential constraint. A **ReferentialConstraint** element defines functionality that is similar to a referential integrity constraint in a relational database. In the same way that a column (or columns) from a database table can reference the primary key of another table, a property (or properties) of an entity type can reference the entity key of another entity type. The entity type that is referenced is called the *principal end* of the constraint. The entity type that references the principal end is called the *dependent end* of the constraint. **PropertyRef** elements are used to specify which keys reference the principal end.

The **Dependent** element can have the following child elements (in the order listed):

-   PropertyRef (one or more elements)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **Dependent** element.

| Attribute Name | Is Required | Value                                                                |
|:---------------|:------------|:---------------------------------------------------------------------|
| **Role**       | Yes         | The name of the entity type on the dependent end of the association. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Dependent** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **ReferentialConstraint** element being used as part of the definition of the **PublishedBy** association. The **PublisherId** property of the **Book** entity type makes up the dependent end of the referential constraint.

``` xml
 <Association Name="PublishedBy">
   <End Type="BooksModel.Book" Role="Book" Multiplicity="*" >
   </End>
   <End Type="BooksModel.Publisher" Role="Publisher" Multiplicity="1" />
   <ReferentialConstraint>
     <Principal Role="Publisher">
       <PropertyRef Name="Id" />
     </Principal>
     <Dependent Role="Book">
       <PropertyRef Name="PublisherId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## Documentation Element (CSDL)

The **Documentation** element in conceptual schema definition language (CSDL) can be used to provide information about an object that is defined in a parent element. In an .edmx file, when the **Documentation** element is a child of an element that appears as an object on the design surface of the EF Designer  (such as an entity, association, or property), the contents of the **Documentation** element will appear in the Visual Studio **Properties** window for the object.

The **Documentation** element can have the following child elements (in the order listed):

-   **Summary**: A brief description of the parent element. (zero or one element)
-   **LongDescription**: An extensive description of the parent element. (zero or one element)
-   Annotation elements. (zero or more elements)

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **Documentation** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example shows the **Documentation** element as a child element of an EntityType element. If the snippet below were in the CSDL content of an .edmx file, the contents of the **Summary** and **LongDescription** elements would appear in the Visual Studio **Properties** window when you click on the `Customer` entity type.

``` xml
 <EntityType Name="Customer">
    <Documentation>
      <Summary>Summary here.</Summary>
      <LongDescription>Long description here.</LongDescription>
    </Documentation>
    <Key>
      <PropertyRef Name="CustomerId" />
    </Key>
    <Property Type="Int32" Name="CustomerId" Nullable="false" />
    <Property Type="String" Name="Name" Nullable="false" />
 </EntityType>
```
 

 

## End Element (CSDL)

The **End** element in conceptual schema definition language (CSDL) can be a child of the Association element or the AssociationSet element. In each case, the role of the **End** element is different and the applicable attributes are different.

### End Element as a Child of the Association Element

An **End** element (as a child of the **Association** element) identifies the entity type on one end of an association and the number of entity type instances that can exist at that end of an association. Association ends are defined as part of an association; an association must have exactly two association ends. Entity type instances at one end of an association can be accessed through navigation properties or foreign keys if they are exposed on an entity type.  

An **End** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   OnDelete (zero or one element)
-   Annotation elements (zero or more elements)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **End** element when it is the child of an **Association** element.

| Attribute Name   | Is Required | Value                                                                                                                                                                                                                                                                                                                                                                                                              |
|:-----------------|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Type**         | Yes         | The name of the entity type at one end of the association.                                                                                                                                                                                                                                                                                                                                                         |
| **Role**         | No          | A name for the association end. If no name is provided, the name of the entity type on the association end will be used.                                                                                                                                                                                                                                                                                           |
| **Multiplicity** | Yes         | **1**, **0..1**, or **\*** depending on the number of entity type instances that can be at the end of the association. <br/> **1** indicates that exactly one entity type instance exists at the association end. <br/> **0..1** indicates that zero or one entity type instances exist at the association end. <br/> **\*** indicates that zero, one, or more entity type instances exist at the association end. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **End** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows an **Association** element that defines the **CustomerOrders** association. The **Multiplicity** values for each **End** of the association indicate that many **Orders** can be associated with a **Customer**, but only one **Customer** can be associated with an **Order**. Additionally, the **OnDelete** element indicates that all **Orders** that are related to a particular **Customer** and that have been loaded into the ObjectContext will be deleted if the **Customer** is deleted.

``` xml
 <Association Name="CustomerOrders">
   <End Type="ExampleModel.Customer" Role="Customer" Multiplicity="1" />
   <End Type="ExampleModel.Order" Role="Order" Multiplicity="*">
         <OnDelete Action="Cascade" />
   </End>
 </Association>
```
 

### End Element as a Child of the AssociationSet Element

The **End** element specifies one end of an association set. The **AssociationSet** element must contain two **End** elements. The information contained in an **End** element is used in mapping an association set to a data source.

An **End** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Annotation elements (zero or more elements)

> [!NOTE]
> Annotation elements must appear after all other child elements. Annotation elements are only allowed in CSDL v2 and later.

 

#### Applicable Attributes

The following table describes the attributes that can be applied to the **End** element when it is the child of an **AssociationSet** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                                 |
|:---------------|:------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **EntitySet**  | Yes         | The name of the **EntitySet** element that defines one end of the parent **AssociationSet** element. The **EntitySet** element must be defined in the same entity container as the parent **AssociationSet** element. |
| **Role**       | No          | The name of the association set end. If the **Role** attribute is not used, the name of the association set end will be the name of the entity set.                                                                   |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **End** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows an **EntityContainer** element with two **AssociationSet** elements, each with two **End** elements:

``` xml
 <EntityContainer Name="BooksContainer" >
   <EntitySet Name="Books" EntityType="BooksModel.Book" />
   <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
   <EntitySet Name="Authors" EntityType="BooksModel.Author" />
   <AssociationSet Name="PublishedBy" Association="BooksModel.PublishedBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Publisher" EntitySet="Publishers" />
   </AssociationSet>
   <AssociationSet Name="WrittenBy" Association="BooksModel.WrittenBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Author" EntitySet="Authors" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntityContainer Element (CSDL)

The **EntityContainer** element in conceptual schema definition language (CSDL) is a logical container for entity sets, association sets, and function imports. A conceptual model entity container maps to a storage model entity container through the EntityContainerMapping element. A storage model entity container describes the structure of the database: entity sets describe tables, association sets describe foreign key constraints, and function imports describe stored procedures in a database.

An **EntityContainer** element can have zero or one Documentation elements. If a **Documentation** element is present, it must precede all **EntitySet**, **AssociationSet**, and **FunctionImport** elements.

An **EntityContainer** element can have zero or more of the following child elements (in the order listed):

-   EntitySet
-   AssociationSet
-   FunctionImport
-   Annotation elements

You can extend an **EntityContainer** element to include the contents of another **EntityContainer** that is within the same namespace. To include the contents of another **EntityContainer**, in the referencing **EntityContainer** element, set the value of the **Extends** attribute to the name of the **EntityContainer** element that you want to include. All child elements of the included **EntityContainer** element will be treated as child elements of the referencing **EntityContainer** element.

### Applicable Attributes

The table below describes the attributes that can be applied to the **Using** element.

| Attribute Name | Is Required | Value                                                           |
|:---------------|:------------|:----------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity container.                               |
| **Extends**    | No          | The name of another entity container within the same namespace. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntityContainer** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityContainer** element that defines three entity sets and two association sets.

``` xml
 <EntityContainer Name="BooksContainer" >
   <EntitySet Name="Books" EntityType="BooksModel.Book" />
   <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
   <EntitySet Name="Authors" EntityType="BooksModel.Author" />
   <AssociationSet Name="PublishedBy" Association="BooksModel.PublishedBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Publisher" EntitySet="Publishers" />
   </AssociationSet>
   <AssociationSet Name="WrittenBy" Association="BooksModel.WrittenBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Author" EntitySet="Authors" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntitySet Element (CSDL)

The **EntitySet** element in conceptual schema definition language is a logical container for instances of an entity type and instances of any type that is derived from that entity type. The relationship between an entity type and an entity set is analogous to the relationship between a row and a table in a relational database. Like a row, an entity type defines a set of related data, and, like a table, an entity set contains instances of that definition. An entity set provides a construct for grouping entity type instances so that they can be mapped to related data structures in a data source.  

More than one entity set for a particular entity type may be defined.

> [!NOTE]
> The EF Designer does not support conceptual models that contain multiple entity sets per type.

 

The **EntitySet** element can have the following child elements (in the order listed):

-   Documentation Element (zero or one elements allowed)
-   Annotation elements (zero or more elements allowed)

### Applicable Attributes

The table below describes the attributes that can be applied to the **EntitySet** element.

| Attribute Name | Is Required | Value                                                                                    |
|:---------------|:------------|:-----------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity set.                                                              |
| **EntityType** | Yes         | The fully-qualified name of the entity type for which the entity set contains instances. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntitySet** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityContainer** element with three **EntitySet** elements:

``` xml
 <EntityContainer Name="BooksContainer" >
   <EntitySet Name="Books" EntityType="BooksModel.Book" />
   <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
   <EntitySet Name="Authors" EntityType="BooksModel.Author" />
   <AssociationSet Name="PublishedBy" Association="BooksModel.PublishedBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Publisher" EntitySet="Publishers" />
   </AssociationSet>
   <AssociationSet Name="WrittenBy" Association="BooksModel.WrittenBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Author" EntitySet="Authors" />
   </AssociationSet>
 </EntityContainer>
```
 

It is possible to define multiple entity sets per type (MEST). The following example defines an entity container with two entity sets for the **Book** entity type:

``` xml
 <EntityContainer Name="BooksContainer" >
   <EntitySet Name="Books" EntityType="BooksModel.Book" />
   <EntitySet Name="FictionBooks" EntityType="BooksModel.Book" />
   <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
   <EntitySet Name="Authors" EntityType="BooksModel.Author" />
   <AssociationSet Name="PublishedBy" Association="BooksModel.PublishedBy">
     <End Role="Book" EntitySet="Books" />
     <End Role="Publisher" EntitySet="Publishers" />
   </AssociationSet>
   <AssociationSet Name="BookAuthor" Association="BooksModel.BookAuthor">
     <End Role="Book" EntitySet="Books" />
     <End Role="Author" EntitySet="Authors" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntityType Element (CSDL)

The **EntityType** element represents the structure of a top-level concept, such as a customer or order, in a conceptual model. An entity type is a template for instances of entity types in an application. Each template contains the following information:

-   A unique name. (Required.)
-   An entity key that is defined by one or more properties. (Required.)
-   Properties for containing data. (Optional.)
-   Navigation properties that allow for navigation from one end of an association to the other end. (Optional.)

In an application, an instance of an entity type represents a specific object (such as a specific customer or order). Each instance of an entity type must have a unique entity key within an entity set.

Two entity type instances are considered equal only if they are of the same type and the values of their entity keys are the same.

An **EntityType** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Key (zero or one element)
-   Property (zero or more elements)
-   NavigationProperty (zero or more elements)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **EntityType** element.

| Attribute Name                                                                                                                                  | Is Required | Value                                                                                            |
|:------------------------------------------------------------------------------------------------------------------------------------------------|:------------|:-------------------------------------------------------------------------------------------------|
| **Name**                                                                                                                                        | Yes         | The name of the entity type.                                                                     |
| **BaseType**                                                                                                                                    | No          | The name of another entity type that is the base type of the entity type that is being defined.  |
| **Abstract**                                                                                                                                    | No          | **True** or **False**, depending on whether the entity type is an abstract type.                 |
| **OpenType**                                                                                                                                    | No          | **True** or **False** depending on whether the entity type is an open entity type. <br/> [!NOTE] |
| > The **OpenType** attribute is only applicable to entity types that are defined in conceptual models that are used with ADO.NET Data Services. |             |                                                                                                  |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntityType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityType** element with three **Property** elements and two **NavigationProperty** elements:

``` xml
 <EntityType Name="Book">
   <Key>
     <PropertyRef Name="ISBN" />
   </Key>
   <Property Type="String" Name="ISBN" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" />
   <Property Type="Decimal" Name="Revision" Nullable="false" Precision="29" Scale="29" />
   <NavigationProperty Name="Publisher" Relationship="BooksModel.PublishedBy"
                       FromRole="Book" ToRole="Publisher" />
   <NavigationProperty Name="Authors" Relationship="BooksModel.WrittenBy"
                       FromRole="Book" ToRole="Author" />
 </EntityType>
```
 

 

## EnumType Element (CSDL)

The **EnumType** element represents an enumerated type.

An **EnumType** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Member (zero or more elements)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **EnumType** element.

| Attribute Name     | Is Required | Value                                                                                                                                                                                         |
|:-------------------|:------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**           | Yes         | The name of the entity type.                                                                                                                                                                  |
| **IsFlags**        | No          | **True** or **False**, depending on whether the enum type can be used as a set of flags. The default value is **False.**.                                                                     |
| **UnderlyingType** | No          | **Edm.Byte**, **Edm.Int16**, **Edm.Int32**, **Edm.Int64** or **Edm.SByte** defining the range of values of the type.   The default underlying type of enumeration elements is **Edm.Int32.**. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EnumType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EnumType** element with three **Member** elements:

``` xml
 <EnumType Name="Color" IsFlags=”false” UnderlyingTyp=”Edm.Byte”>
   <Member Name="Red" />
   <Member Name="Green" />
   <Member Name="Blue" />
 </EntityType>
```
 

 

## Function Element (CSDL)

The **Function** element in conceptual schema definition language (CSDL) is used to define or declare functions in the conceptual model. A function is defined by using a DefiningExpression element.  

A **Function** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Parameter (zero or more elements)
-   DefiningExpression (zero or one element)
-   ReturnType (Function) (zero or one element)
-   Annotation elements (zero or more elements)

A return type for a function must be specified with either the **ReturnType** (Function) element or the **ReturnType** attribute (see below), but not both. The possible return types are any EdmSimpleType, entity type, complex type, row type, or ref type (or a collection of one of these types).

### Applicable Attributes

The table below describes the attributes that can be applied to the **Function** element.

| Attribute Name | Is Required | Value                              |
|:---------------|:------------|:-----------------------------------|
| **Name**       | Yes         | The name of the function.          |
| **ReturnType** | No          | The type returned by the function. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Function** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example uses a **Function** element to define a function that returns the number of years since an instructor was hired.

``` xml
 <Function Name="YearsSince" ReturnType="Edm.Int32">
   <Parameter Name="date" Type="Edm.DateTime" />
   <DefiningExpression>
     Year(CurrentDateTime()) - Year(date)
   </DefiningExpression>
 </Function>
```
 

 

## FunctionImport Element (CSDL)

The **FunctionImport** element in conceptual schema definition language (CSDL) represents a function that is defined in the data source but available to objects through the conceptual model. For example, a Function element in the storage model can be used to represent a stored procedure in a database. A **FunctionImport** element in the conceptual model represents the corresponding function in an Entity Framework application and is mapped to the storage model function by using the FunctionImportMapping element. When the function is called in the application, the corresponding stored procedure is executed in the database.

The **FunctionImport** element can have the following child elements (in the order listed):

-   Documentation (zero or one elements allowed)
-   Parameter (zero or more elements allowed)
-   Annotation elements (zero or more elements allowed)
-   ReturnType (FunctionImport) (zero or more elements allowed)

One **Parameter** element should be defined for each parameter that the function accepts.

A return type for a function must be specified with either the **ReturnType** (FunctionImport) element or the **ReturnType** attribute (see below), but not both. The return type value must be a  collection of EdmSimpleType, EntityType, or ComplexType.

### Applicable Attributes

The table below describes the attributes that can be applied to the **FunctionImport** element.

| Attribute Name   | Is Required | Value                                                                                                                                                                                                 |
|:-----------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**         | Yes         | The name of the imported function.                                                                                                                                                                    |
| **ReturnType**   | No          | The type that the function returns. Do not use this attribute if the function does not return a value. Otherwise, the value must be a collection of ComplexType, EntityType, or EDMSimpleType.        |
| **EntitySet**    | No          | If the function returns a collection of entity types, the value of the **EntitySet** must be the entity set to which the collection belongs. Otherwise, the **EntitySet** attribute must not be used. |
| **IsComposable** | No          | If the value is set to true, the function is composable (Table-valued Function) and can be used in a LINQ query.  The default is **false**.                                                           |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **FunctionImport** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **FunctionImport** element that accepts one parameter and returns a collection of entity types:

``` xml
 <FunctionImport Name="GetStudentGrades"
                 EntitySet="StudentGrade"
                 ReturnType="Collection(SchoolModel.StudentGrade)">
        <Parameter Name="StudentID" Mode="In" Type="Int32" />
 </FunctionImport>
```
 

 

## Key Element (CSDL)

The **Key** element is a child element of the EntityType element and defines an *entity key* (a property or a set of properties of an entity type that determine identity). The properties that make up an entity key are chosen at design time. The values of entity key properties must uniquely identify an entity type instance within an entity set at run time. The properties that make up an entity key should be chosen to guarantee uniqueness of instances in an entity set. The **Key** element defines an entity key by referencing one or more of the properties of an entity type.

The **Key** element can have the following child elements:

-   PropertyRef (one or more elements)
-   Annotation elements (zero or more elements)

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **Key** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The example below defines an entity type named **Book**. The entity key is defined by referencing the **ISBN** property of the entity type.

``` xml
 <EntityType Name="Book">
   <Key>
     <PropertyRef Name="ISBN" />
   </Key>
   <Property Type="String" Name="ISBN" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" />
   <Property Type="Decimal" Name="Revision" Nullable="false" Precision="29" Scale="29" />
   <NavigationProperty Name="Publisher" Relationship="BooksModel.PublishedBy"
                       FromRole="Book" ToRole="Publisher" />
   <NavigationProperty Name="Authors" Relationship="BooksModel.WrittenBy"
                       FromRole="Book" ToRole="Author" />
 </EntityType>
```
 

The **ISBN** property is a good choice for the entity key because an International Standard Book Number (ISBN) uniquely identifies a book.

The following example shows an entity type (**Author**) that has an entity key that consists of two properties, **Name** and **Address**.

``` xml
 <EntityType Name="Author">
   <Key>
     <PropertyRef Name="Name" />
     <PropertyRef Name="Address" />
   </Key>
   <Property Type="String" Name="Name" Nullable="false" />
   <Property Type="String" Name="Address" Nullable="false" />
   <NavigationProperty Name="Books" Relationship="BooksModel.WrittenBy"
                       FromRole="Author" ToRole="Book" />
 </EntityType>
```
 

Using **Name** and **Address** for the entity key is a reasonable choice, because two authors of the same name are unlikely to live at the same address. However, this choice for an entity key does not absolutely guarantee unique entity keys in an entity set. Adding a property, such as **AuthorId**, that could be used to uniquely identify an author would be recommended in this case.

 

## Member Element (CSDL)

The **Member** element is a child element of the EnumType element and defines a member of the enumerated type.

### Applicable Attributes

The table below describes the attributes that can be applied to the **FunctionImport** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                    |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the member.                                                                                                                                                                  |
| **Value**      | No          | The value of the member. By default, the first member has the value 0, and the value of each successive enumerator is incremented by 1. Multiple members with the same values may exist. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **FunctionImport** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EnumType** element with three **Member** elements:

``` xml
 <EnumType Name="Color">
   <Member Name="Red" Value=”1”/>
   <Member Name="Green" Value=”3” />
   <Member Name="Blue" Value=”5”/>
 </EntityType>
```
 

 

## NavigationProperty Element (CSDL)

A **NavigationProperty** element defines a navigation property, which provides a reference to the other end of an association. Unlike properties defined with the Property element, navigation properties do not define the shape and characteristics of data. They provide a way to navigate an association between two entity types.

Note that navigation properties are optional on both entity types at the ends of an association. If you define a navigation property on one entity type at the end of an association, you do not have to define a navigation property on the entity type at the other end of the association.

The data type returned by a navigation property is determined by the multiplicity of its remote association end. For example, suppose a navigation property, **OrdersNavProp**, exists on a **Customer** entity type and navigates a one-to-many association between **Customer** and **Order**. Because the remote association end for the navigation property has multiplicity many (\*), its data type is a collection (of **Order**). Similarly, if a navigation property, **CustomerNavProp**, exists on the **Order** entity type, its data type would be **Customer** since the multiplicity of the remote end is one (1).

A **NavigationProperty** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **NavigationProperty** element.

| Attribute Name   | Is Required | Value                                                                                                                                                                                                                                            |
|:-----------------|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**         | Yes         | The name of the navigation property.                                                                                                                                                                                                             |
| **Relationship** | Yes         | The name of an association that is within the scope of the model.                                                                                                                                                                                |
| **ToRole**       | Yes         | The end of the association at which navigation ends. The value of the **ToRole** attribute must be the same as the value of one of the **Role** attributes defined on one of the association ends (defined in the AssociationEnd element).       |
| **FromRole**     | Yes         | The end of the association from which navigation begins. The value of the **FromRole** attribute must be the same as the value of one of the **Role** attributes defined on one of the association ends (defined in the AssociationEnd element). |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **NavigationProperty** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example defines an entity type (**Book**) with two navigation properties (**PublishedBy** and **WrittenBy**):

``` xml
 <EntityType Name="Book">
   <Key>
     <PropertyRef Name="ISBN" />
   </Key>
   <Property Type="String" Name="ISBN" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" />
   <Property Type="Decimal" Name="Revision" Nullable="false" Precision="29" Scale="29" />
   <NavigationProperty Name="Publisher" Relationship="BooksModel.PublishedBy"
                       FromRole="Book" ToRole="Publisher" />
   <NavigationProperty Name="Authors" Relationship="BooksModel.WrittenBy"
                       FromRole="Book" ToRole="Author" />
 </EntityType>
```
 

 

## OnDelete Element (CSDL)

The **OnDelete** element in conceptual schema definition language (CSDL) defines behavior that is connected with an association. If the **Action** attribute is set to **Cascade** on one end of an association, related entity types on the other end of the association are deleted when the entity type on the first end is deleted. If the association between two entity types is a primary key-to-primary key relationship, then a loaded dependent object is deleted when the principal object on the other end of the association is deleted regardless of the **OnDelete** specification.  

> [!NOTE]
> The **OnDelete** element only affects the runtime behavior of an application; it does not affect behavior in the data source. The behavior defined in the data source should be the same as the behavior defined in the application.

 

An **OnDelete** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **OnDelete** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                                         |
|:---------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Action**     | Yes         | **Cascade** or **None**. If **Cascade**, dependent entity types will be deleted when the principal entity type is deleted. If **None**, dependent entity types will not be deleted when the principal entity type is deleted. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Association** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **Association** element that defines the **CustomerOrders** association. The **OnDelete** element indicates that all **Orders** that are related to a particular **Customer** and have been loaded into the ObjectContext will be deleted when the **Customer** is deleted.

``` xml
 <Association Name="CustomerOrders">
   <End Type="ExampleModel.Customer" Role="Customer" Multiplicity="1">
         <OnDelete Action="Cascade" />
   </End>
   <End Type="ExampleModel.Order" Role="Order" Multiplicity="*" />
 </Association>
```
 

 

## Parameter Element (CSDL)

The **Parameter** element in conceptual schema definition language (CSDL) can be a child of the FunctionImport element or the Function element.

### FunctionImport Element Application

A **Parameter** element (as a child of the **FunctionImport** element) is used to define input and output parameters for function imports that are declared in CSDL.

The **Parameter** element can have the following child elements (in the order listed):

-   Documentation (zero or one elements allowed)
-   Annotation elements (zero or more elements allowed)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **Parameter** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                                           |
|:---------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the parameter.                                                                                                                                                                                                      |
| **Type**       | Yes         | The parameter type. The value must be an **EDMSimpleType** or a complex type that is within the scope of the model.                                                                                                             |
| **Mode**       | No          | **In**, **Out**, or **InOut** depending on whether the parameter is an input, output, or input/output parameter.                                                                                                                |
| **MaxLength**  | No          | The maximum allowed length of the parameter.                                                                                                                                                                                    |
| **Precision**  | No          | The precision of the parameter.                                                                                                                                                                                                 |
| **Scale**      | No          | The scale of the parameter.                                                                                                                                                                                                     |
| **SRID**       | No          | Spatial System Reference Identifier. Valid only for parameters of spatial types. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Parameter** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows a **FunctionImport** element with one **Parameter** child element. The function accepts one input parameter and returns a collection of entity types.

``` xml
 <FunctionImport Name="GetStudentGrades"
                 EntitySet="StudentGrade"
                 ReturnType="Collection(SchoolModel.StudentGrade)">
        <Parameter Name="StudentID" Mode="In" Type="Int32" />
 </FunctionImport>
```
 

### Function Element Application

A **Parameter** element (as a child of the **Function** element) defines parameters for functions that are defined or declared in a conceptual model.

The **Parameter** element can have the following child elements (in the order listed):

-   Documentation (zero or one elements)
-   CollectionType (zero or one elements)
-   ReferenceType (zero or one elements)
-   RowType (zero or one elements)

> [!NOTE]
> Only one of the **CollectionType**, **ReferenceType**, or **RowType** elements can be a child element of a **Property** element.

 

-   Annotation elements (zero or more elements allowed)

> [!NOTE]
> Annotation elements must appear after all other child elements. Annotation elements are only allowed in CSDL v2 and later.

 

#### Applicable Attributes

The following table describes the attributes that can be applied to the **Parameter** element.

| Attribute Name   | Is Required | Value                                                                                                                                                                                                                           |
|:-----------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**         | Yes         | The name of the parameter.                                                                                                                                                                                                      |
| **Type**         | No          | The parameter type. A parameter can be any of the following types (or collections of these types): <br/> **EdmSimpleType** <br/> entity type <br/> complex type <br/> row type <br/> reference type                             |
| **Nullable**     | No          | **True** (the default value) or **False** depending on whether the property can have a **null** value.                                                                                                                          |
| **DefaultValue** | No          | The default value of the property.                                                                                                                                                                                              |
| **MaxLength**    | No          | The maximum length of the property value.                                                                                                                                                                                       |
| **FixedLength**  | No          | **True** or **False** depending on whether the property value will be stored as a fixed length string.                                                                                                                          |
| **Precision**    | No          | The precision of the property value.                                                                                                                                                                                            |
| **Scale**        | No          | The scale of the property value.                                                                                                                                                                                                |
| **SRID**         | No          | Spatial System Reference Identifier. Valid only for properties of spatial types. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |
| **Unicode**      | No          | **True** or **False** depending on whether the property value will be stored as a Unicode string.                                                                                                                               |
| **Collation**    | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                   |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Parameter** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows a **Function** element that uses one **Parameter** child element to define a function parameter.

``` xml
 <Function Name="GetYearsEmployed" ReturnType="Edm.Int32">
 <Parameter Name="Instructor" Type="SchoolModel.Person" />
   <DefiningExpression>
   Year(CurrentDateTime()) - Year(cast(Instructor.HireDate as DateTime))
   </DefiningExpression>
 </Function>
```

 

## Principal Element (CSDL)

The **Principal** element in conceptual schema definition language (CSDL) is a child element to the ReferentialConstraint element that defines the principal end of a referential constraint. A **ReferentialConstraint** element defines functionality that is similar to a referential integrity constraint in a relational database. In the same way that a column (or columns) from a database table can reference the primary key of another table, a property (or properties) of an entity type can reference the entity key of another entity type. The entity type that is referenced is called the *principal end* of the constraint. The entity type that references the principal end is called the *dependent end* of the constraint. **PropertyRef** elements are used to specify which keys are referenced by the dependent end.

The **Principal** element can have the following child elements (in the order listed):

-   PropertyRef (one or more elements)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **Principal** element.

| Attribute Name | Is Required | Value                                                                |
|:---------------|:------------|:---------------------------------------------------------------------|
| **Role**       | Yes         | The name of the entity type on the principal end of the association. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Principal** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **ReferentialConstraint** element that is part of the definition of the **PublishedBy** association. The **Id** property of the **Publisher** entity type makes up the principal end of the referential constraint.

``` xml
 <Association Name="PublishedBy">
   <End Type="BooksModel.Book" Role="Book" Multiplicity="*" >
   </End>
   <End Type="BooksModel.Publisher" Role="Publisher" Multiplicity="1" />
   <ReferentialConstraint>
     <Principal Role="Publisher">
       <PropertyRef Name="Id" />
     </Principal>
     <Dependent Role="Book">
       <PropertyRef Name="PublisherId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## Property Element (CSDL)

The **Property** element in conceptual schema definition language (CSDL) can be a child of the EntityType element, the ComplexType element, or the RowType element.

### EntityType and ComplexType Element Applications

**Property** elements (as children of **EntityType** or **ComplexType** elements) define the shape and characteristics of data that an entity type instance or complex type instance will contain. Properties in a conceptual model are analogous to properties that are defined on a class. In the same way that properties on a class define the shape of the class and carry information about objects, properties in a conceptual model define the shape of an entity type and carry information about entity type instances.

The **Property** element can have the following child elements (in the order listed):

-   Documentation Element (zero or one elements allowed)
-   Annotation elements (zero or more elements allowed)

The following facets can be applied to a **Property** element: **Nullable**, **DefaultValue**, **MaxLength**, **FixedLength**, **Precision**, **Scale**, **Unicode**, **Collation**, **ConcurrencyMode**. Facets are XML attributes that provide information about how property values are stored in the data store.

> [!NOTE]
> Facets can only be applied to properties of type **EDMSimpleType**.

 

#### Applicable Attributes

The following table describes the attributes that can be applied to the **Property** element.

| Attribute Name                                                         | Is Required | Value                                                                                                                                                                                                                           |
|:-----------------------------------------------------------------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**                                                               | Yes         | The name of the property.                                                                                                                                                                                                       |
| **Type**                                                               | Yes         | The type of the property value. The property value type must be an **EDMSimpleType** or a complex type (indicated by a fully-qualified name) that is within scope of the model.                                                 |
| **Nullable**                                                           | No          | **True** (the default value) or <strong>False</strong> depending on whether the property can have a null value. <br/> [!NOTE]                                                                                                   |
| > In the CSDL v1 a complex type property must have `Nullable="False"`. |             |                                                                                                                                                                                                                                 |
| **DefaultValue**                                                       | No          | The default value of the property.                                                                                                                                                                                              |
| **MaxLength**                                                          | No          | The maximum length of the property value.                                                                                                                                                                                       |
| **FixedLength**                                                        | No          | **True** or **False** depending on whether the property value will be stored as a fixed length string.                                                                                                                          |
| **Precision**                                                          | No          | The precision of the property value.                                                                                                                                                                                            |
| **Scale**                                                              | No          | The scale of the property value.                                                                                                                                                                                                |
| **SRID**                                                               | No          | Spatial System Reference Identifier. Valid only for properties of spatial types. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |
| **Unicode**                                                            | No          | **True** or **False** depending on whether the property value will be stored as a Unicode string.                                                                                                                               |
| **Collation**                                                          | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                   |
| **ConcurrencyMode**                                                    | No          | **None** (the default value) or **Fixed**. If the value is set to **Fixed**, the property value will be used in optimistic concurrency checks.                                                                                  |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Property** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows an **EntityType** element with three **Property** elements:

``` xml
 <EntityType Name="Book">
   <Key>
     <PropertyRef Name="ISBN" />
   </Key>
   <Property Type="String" Name="ISBN" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" />
   <Property Type="Decimal" Name="Revision" Nullable="false" Precision="29" Scale="29" />
   <NavigationProperty Name="Publisher" Relationship="BooksModel.PublishedBy"
                       FromRole="Book" ToRole="Publisher" />
   <NavigationProperty Name="Authors" Relationship="BooksModel.WrittenBy"
                       FromRole="Book" ToRole="Author" />
 </EntityType>
```
 

The following example shows a **ComplexType** element with five **Property** elements:

``` xml
 <ComplexType Name="Address" >
   <Property Type="String" Name="StreetAddress" Nullable="false" />
   <Property Type="String" Name="City" Nullable="false" />
   <Property Type="String" Name="StateOrProvince" Nullable="false" />
   <Property Type="String" Name="Country" Nullable="false" />
   <Property Type="String" Name="PostalCode" Nullable="false" />
 </ComplexType>
```
 

### RowType Element Application

**Property** elements (as the children of a **RowType** element) define the shape and characteristics of data that can be passed to or returned from a model-defined function.  

The **Property** element can have exactly one of the following child elements:

-   CollectionType
-   ReferenceType
-   RowType

The **Property** element can have any number child annotation elements.

> [!NOTE]
> Annotation elements are only allowed in CSDL v2 and later.

 

#### Applicable Attributes

The following table describes the attributes that can be applied to the **Property** element.

| Attribute Name                                                     | Is Required | Value                                                                                                                                                                                                                           |
|:-------------------------------------------------------------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**                                                           | Yes         | The name of the property.                                                                                                                                                                                                       |
| **Type**                                                           | Yes         | The type of the property value.                                                                                                                                                                                                 |
| **Nullable**                                                       | No          | **True** (the default value) or **False** depending on whether the property can have a null value. <br/> [!NOTE]                                                                                                                |
| > In CSDL v1 a complex type property must have `Nullable="False"`. |             |                                                                                                                                                                                                                                 |
| **DefaultValue**                                                   | No          | The default value of the property.                                                                                                                                                                                              |
| **MaxLength**                                                      | No          | The maximum length of the property value.                                                                                                                                                                                       |
| **FixedLength**                                                    | No          | **True** or **False** depending on whether the property value will be stored as a fixed length string.                                                                                                                          |
| **Precision**                                                      | No          | The precision of the property value.                                                                                                                                                                                            |
| **Scale**                                                          | No          | The scale of the property value.                                                                                                                                                                                                |
| **SRID**                                                           | No          | Spatial System Reference Identifier. Valid only for properties of spatial types. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |
| **Unicode**                                                        | No          | **True** or **False** depending on whether the property value will be stored as a Unicode string.                                                                                                                               |
| **Collation**                                                      | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                   |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Property** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows **Property** elements used to define the shape of the return type of a model-defined function.

``` xml
 <Function Name="LastNamesAfter">
   <Parameter Name="someString" Type="Edm.String" />
   <ReturnType>
    <CollectionType>
      <RowType>
        <Property Name="FirstName" Type="Edm.String" Nullable="false" />
        <Property Name="LastName" Type="Edm.String" Nullable="false" />
      </RowType>
    </CollectionType>
   </ReturnType>
   <DefiningExpression>
             SELECT VALUE ROW(p.FirstName, p.LastName)
             FROM SchoolEntities.People AS p
             WHERE p.LastName &gt;= somestring
   </DefiningExpression>
 </Function>
```
 

 

## PropertyRef Element (CSDL)

The **PropertyRef** element in conceptual schema definition language (CSDL) references a property of an entity type to indicate that the property will perform one of the following roles:

-   Part of the entity's key (a property or a set of properties of an entity type that determine identity). One or more **PropertyRef** elements can be used to define an entity key.
-   The dependent or principal end of a referential constraint.

The **PropertyRef** element can only have annotation elements (zero or more) as child elements.

> [!NOTE]
> Annotation elements are only allowed in CSDL v2 and later.

 

### Applicable Attributes

The table below describes the attributes that can be applied to the **PropertyRef** element.

| Attribute Name | Is Required | Value                                |
|:---------------|:------------|:-------------------------------------|
| **Name**       | Yes         | The name of the referenced property. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **PropertyRef** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The example below defines an entity type (**Book**). The entity key is defined by referencing the **ISBN** property of the entity type.

``` xml
 <EntityType Name="Book">
   <Key>
     <PropertyRef Name="ISBN" />
   </Key>
   <Property Type="String" Name="ISBN" Nullable="false" />
   <Property Type="String" Name="Title" Nullable="false" />
   <Property Type="Decimal" Name="Revision" Nullable="false" Precision="29" Scale="29" />
   <NavigationProperty Name="Publisher" Relationship="BooksModel.PublishedBy"
                       FromRole="Book" ToRole="Publisher" />
   <NavigationProperty Name="Authors" Relationship="BooksModel.WrittenBy"
                       FromRole="Book" ToRole="Author" />
 </EntityType>
```
 

In the next example, two **PropertyRef** elements are used to indicate that two properties (**Id** and **PublisherId**) are the principal and dependent ends of a referential constraint.

``` xml
 <Association Name="PublishedBy">
   <End Type="BooksModel.Book" Role="Book" Multiplicity="*" >
   </End>
   <End Type="BooksModel.Publisher" Role="Publisher" Multiplicity="1" />
   <ReferentialConstraint>
     <Principal Role="Publisher">
       <PropertyRef Name="Id" />
     </Principal>
     <Dependent Role="Book">
       <PropertyRef Name="PublisherId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## ReferenceType Element (CSDL)

The **ReferenceType** element in conceptual schema definition language (CSDL) specifies a reference to an entity type. The **ReferenceType** element can be a child of the following elements:

-   ReturnType (Function)
-   Parameter
-   CollectionType

The **ReferenceType** element is used when defining a parameter or return type for a function.

A **ReferenceType** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The table below describes the attributes that can be applied to the **ReferenceType** element.

| Attribute Name | Is Required | Value                                         |
|:---------------|:------------|:----------------------------------------------|
| **Type**       | Yes         | The name of the entity type being referenced. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **ReferenceType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows the **ReferenceType** element used as a child of a **Parameter** element in a model-defined function that accepts a reference to a **Person** entity type:

``` xml
 <Function Name="GetYearsEmployed" ReturnType="Edm.Int32">
   <Parameter Name="instructor">
     <ReferenceType Type="SchoolModel.Person" />
   </Parameter>
   <DefiningExpression>
   Year(CurrentDateTime()) - Year(cast(instructor.HireDate as DateTime))
   </DefiningExpression>
 </Function>
```
 

The following example shows the **ReferenceType** element used as a child of a **ReturnType** (Function) element in a model-defined function that returns a reference to a **Person** entity type:

``` xml
 <Function Name="GetPersonReference">
     <Parameter Name="p" Type="SchoolModel.Person" />
     <ReturnType>
         <ReferenceType Type="SchoolModel.Person" />
     </ReturnType>
     <DefiningExpression>
           REF(p)
     </DefiningExpression>
 </Function>
```
 

 

## ReferentialConstraint Element (CSDL)

A **ReferentialConstraint** element in conceptual schema definition language (CSDL) defines functionality that is similar to a referential integrity constraint in a relational database. In the same way that a column (or columns) from a database table can reference the primary key of another table, a property (or properties) of an entity type can reference the entity key of another entity type. The entity type that is referenced is called the *principal end* of the constraint. The entity type that references the principal end is called the *dependent end* of the constraint.

If a foreign key that is exposed on one entity type references a property on another entity type, the **ReferentialConstraint** element defines an association between the two entity types. Because the **ReferentialConstraint** element provides information about how two entity types are related, no corresponding AssociationSetMapping element is necessary in the mapping specification language (MSL). An association between two entity types that do not have foreign keys exposed must have a corresponding **AssociationSetMapping** element in order to map association information to the data source.

If a foreign key is not exposed on an entity type, the **ReferentialConstraint** element can only define a primary key-to-primary key constraint between the entity type and another entity type.

A **ReferentialConstraint** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Principal (exactly one element)
-   Dependent (exactly one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The **ReferentialConstraint** element can have any number of annotation attributes (custom XML attributes). However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example shows a **ReferentialConstraint** element being used as part of the definition of the **PublishedBy** association.

``` xml
 <Association Name="PublishedBy">
   <End Type="BooksModel.Book" Role="Book" Multiplicity="*" >
   </End>
   <End Type="BooksModel.Publisher" Role="Publisher" Multiplicity="1" />
   <ReferentialConstraint>
     <Principal Role="Publisher">
       <PropertyRef Name="Id" />
     </Principal>
     <Dependent Role="Book">
       <PropertyRef Name="PublisherId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## ReturnType (Function) Element (CSDL)

The **ReturnType** (Function) element in conceptual schema definition language (CSDL) specifies the return type for a function that is defined in a Function element. A function return type can also be specified with a **ReturnType** attribute.

Return types can be any **EdmSimpleType**, entity type, complex type, row type, ref type, or a collection of one of these types.

The return type of a function can be specified with either the **Type** attribute of the **ReturnType** (Function) element, or with one of the following child elements:

-   CollectionType
-   ReferenceType
-   RowType

> [!NOTE]
> A model will not validate if you specify a function return type with both the **Type** attribute of the **ReturnType** (Function) element and one of the child elements.

 

### Applicable Attributes

The following table describes the attributes that can be applied to the **ReturnType** (Function) element.

| Attribute Name | Is Required | Value                              |
|:---------------|:------------|:-----------------------------------|
| **ReturnType** | No          | The type returned by the function. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **ReturnType** (Function) element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example uses a **Function** element to define a function that returns the number of years a book has been in print. Note that the return type is specified by the **Type** attribute of a **ReturnType** (Function) element.

``` xml
 <Function Name="GetYearsInPrint">
   <ReturnType Type=="Edm.Int32">
   <Parameter Name="book" Type="BooksModel.Book" />
   <DefiningExpression>
    Year(CurrentDateTime()) - Year(cast(book.PublishedDate as DateTime))
   </DefiningExpression>
 </Function>
```
 

 

## ReturnType (FunctionImport) Element (CSDL)

The **ReturnType** (FunctionImport) element in conceptual schema definition language (CSDL) specifies the return type for a function that is defined in a FunctionImport element. A function return type can also be specified with a **ReturnType** attribute.

Return types can be any collection of entity type, complex type,or **EdmSimpleType**,

The return type of a function is specified with the **Type** attribute of the **ReturnType** (FunctionImport) element.

### Applicable Attributes

The following table describes the attributes that can be applied to the **ReturnType** (FunctionImport) element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                 |
|:---------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Type**       | No          | The type that the function returns. The value must be a collection of ComplexType, EntityType, or EDMSimpleType.                                                                                      |
| **EntitySet**  | No          | If the function returns a collection of entity types, the value of the **EntitySet** must be the entity set to which the collection belongs. Otherwise, the **EntitySet** attribute must not be used. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **ReturnType** (FunctionImport) element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example uses a **FunctionImport** that returns books and publishers. Note that the function returns two result sets and therefore two **ReturnType** (FunctionImport) elements are specified.

``` xml
 <FunctionImport Name="GetBooksAndPublishers">
   <ReturnType Type=="Collection(BooksModel.Book )" EntitySet=”Books”>
   <ReturnType Type=="Collection(BooksModel.Publisher)" EntitySet=”Publishers”>
 </FunctionImport>
```
 

 

## RowType Element (CSDL)

A **RowType** element in conceptual schema definition language (CSDL) defines an unnamed structure as a parameter or return type for a function defined in the conceptual model.

A **RowType** element can be the child of the following elements:

-   CollectionType
-   Parameter
-   ReturnType (Function)

A **RowType** element can have the following child elements (in the order listed):

-   Property (one or more)
-   Annotation elements (zero or more)

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **RowType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example shows a model-defined function that uses a **CollectionType** element to specify that the function returns a collection of rows (as specified in the **RowType** element).

``` xml
 <Function Name="LastNamesAfter">
   <Parameter Name="someString" Type="Edm.String" />
   <ReturnType>
    <CollectionType>
      <RowType>
        <Property Name="FirstName" Type="Edm.String" Nullable="false" />
        <Property Name="LastName" Type="Edm.String" Nullable="false" />
      </RowType>
    </CollectionType>
   </ReturnType>
   <DefiningExpression>
             SELECT VALUE ROW(p.FirstName, p.LastName)
             FROM SchoolEntities.People AS p
             WHERE p.LastName &gt;= somestring
   </DefiningExpression>
 </Function>
```

## Schema Element (CSDL)

The **Schema** element is the root element of a conceptual model definition. It contains definitions for the objects, functions, and containers that make up a conceptual model.

The **Schema** element may contain zero or more of the following child elements:

-   Using
-   EntityContainer
-   EntityType
-   EnumType
-   Association
-   ComplexType
-   Function

A **Schema** element may contain zero or one Annotation elements.

> [!NOTE]
> The **Function** element and annotation elements are only allowed in CSDL v2 and later.

 

The **Schema** element uses the **Namespace** attribute to define the namespace for the entity type, complex type, and association objects in a conceptual model. Within a namespace, no two objects can have the same name. Namespaces can span multiple **Schema** elements and multiple .csdl files.

A conceptual model namespace is different from the XML namespace of the **Schema** element. A conceptual model namespace (as defined by the **Namespace** attribute) is a logical container for entity types, complex types, and association types. The XML namespace (indicated by the **xmlns** attribute) of a **Schema** element is the default namespace for child elements and attributes of the **Schema** element. XML namespaces of the form `https://schemas.microsoft.com/ado/YYYY/MM/edm` (where YYYY and MM represent a year and month respectively) are reserved for CSDL. Custom elements and attributes cannot be in namespaces that have this form.

### Applicable Attributes

The table below describes the attributes can be applied to the **Schema** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
|:---------------|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Namespace**  | Yes         | The namespace of the conceptual model. The value of the **Namespace** attribute is used to form the fully qualified name of a type. For example, if an **EntityType** named *Customer* is in the Simple.Example.Model namespace, then the fully qualified name of the **EntityType** is SimpleExampleModel.Customer. <br/> The following strings cannot be used as the value for the **Namespace** attribute: **System**, **Transient**, or **Edm**. The value for the **Namespace** attribute cannot be the same as the value for the **Namespace** attribute in the SSDL Schema element. |
| **Alias**      | No          | An identifier used in place of the namespace name. For example, if an **EntityType** named *Customer* is in the Simple.Example.Model namespace and the value of the **Alias** attribute is *Model*, then you can use Model.Customer as the fully qualified name of the **EntityType.**                                                                                                                                                                                                                                                                                                     |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Schema** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **Schema** element that contains an **EntityContainer** element, two **EntityType** elements, and one **Association** element.

``` xml
 <Schema xmlns="https://schemas.microsoft.com/ado/2009/11/edm"
      xmlns:cg="https://schemas.microsoft.com/ado/2009/11/codegeneration"
      xmlns:store="https://schemas.microsoft.com/ado/2009/11/edm/EntityStoreSchemaGenerator"
       Namespace="ExampleModel" Alias="Self">
         <EntityContainer Name="ExampleModelContainer">
           <EntitySet Name="Customers"
                      EntityType="ExampleModel.Customer" />
           <EntitySet Name="Orders" EntityType="ExampleModel.Order" />
           <AssociationSet
                       Name="CustomerOrder"
                       Association="ExampleModel.CustomerOrders">
             <End Role="Customer" EntitySet="Customers" />
             <End Role="Order" EntitySet="Orders" />
           </AssociationSet>
         </EntityContainer>
         <EntityType Name="Customer">
           <Key>
             <PropertyRef Name="CustomerId" />
           </Key>
           <Property Type="Int32" Name="CustomerId" Nullable="false" />
           <Property Type="String" Name="Name" Nullable="false" />
           <NavigationProperty
                    Name="Orders"
                    Relationship="ExampleModel.CustomerOrders"
                    FromRole="Customer" ToRole="Order" />
         </EntityType>
         <EntityType Name="Order">
           <Key>
             <PropertyRef Name="OrderId" />
           </Key>
           <Property Type="Int32" Name="OrderId" Nullable="false" />
           <Property Type="Int32" Name="ProductId" Nullable="false" />
           <Property Type="Int32" Name="Quantity" Nullable="false" />
           <NavigationProperty
                    Name="Customer"
                    Relationship="ExampleModel.CustomerOrders"
                    FromRole="Order" ToRole="Customer" />
           <Property Type="Int32" Name="CustomerId" Nullable="false" />
         </EntityType>
         <Association Name="CustomerOrders">
           <End Type="ExampleModel.Customer"
                Role="Customer" Multiplicity="1" />
           <End Type="ExampleModel.Order"
                Role="Order" Multiplicity="*" />
           <ReferentialConstraint>
             <Principal Role="Customer">
               <PropertyRef Name="CustomerId" />
             </Principal>
             <Dependent Role="Order">
               <PropertyRef Name="CustomerId" />
             </Dependent>
           </ReferentialConstraint>
         </Association>
       </Schema>
```
 

 

## TypeRef Element (CSDL)

The **TypeRef** element in conceptual schema definition language (CSDL) provides a reference to an existing named type. The **TypeRef** element can be a child of the CollectionType element, which is used to specify that a function has a collection as a parameter or return type.

A **TypeRef** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Annotation elements (zero or more elements)

### Applicable Attributes

The following table describes the attributes that can be applied to the **TypeRef** element. Note that the **DefaultValue**, **MaxLength**, **FixedLength**, **Precision**, **Scale**, **Unicode**, and **Collation** attributes are only applicable to **EDMSimpleTypes**.

| Attribute Name                                                     | Is Required | Value                                                                                                                                                                                                                           |
|:-------------------------------------------------------------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Type**                                                           | No          | The name of the type being referenced.                                                                                                                                                                                          |
| **Nullable**                                                       | No          | **True** (the default value) or **False** depending on whether the property can have a null value. <br/> [!NOTE]                                                                                                                |
| > In CSDL v1 a complex type property must have `Nullable="False"`. |             |                                                                                                                                                                                                                                 |
| **DefaultValue**                                                   | No          | The default value of the property.                                                                                                                                                                                              |
| **MaxLength**                                                      | No          | The maximum length of the property value.                                                                                                                                                                                       |
| **FixedLength**                                                    | No          | **True** or **False** depending on whether the property value will be stored as a fixed length string.                                                                                                                          |
| **Precision**                                                      | No          | The precision of the property value.                                                                                                                                                                                            |
| **Scale**                                                          | No          | The scale of the property value.                                                                                                                                                                                                |
| **SRID**                                                           | No          | Spatial System Reference Identifier. Valid only for properties of spatial types. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |
| **Unicode**                                                        | No          | **True** or **False** depending on whether the property value will be stored as a Unicode string.                                                                                                                               |
| **Collation**                                                      | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                   |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **CollectionType** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a model-defined function that uses the **TypeRef** element (as a child of a **CollectionType** element) to specify that the function accepts a collection of **Department** entity types.

``` xml
 <Function Name="GetAvgBudget">
      <Parameter Name="Departments">
          <CollectionType>
             <TypeRef Type="SchoolModel.Department"/>
          </CollectionType>
           </Parameter>
       <ReturnType Type="Collection(Edm.Decimal)"/>
       <DefiningExpression>
             SELECT VALUE AVG(d.Budget) FROM Departments AS d
       </DefiningExpression>
 </Function>
```
 

 

## Using Element (CSDL)

The **Using** element in conceptual schema definition language (CSDL) imports the contents of a conceptual model that exists in a different namespace. By setting the value of the **Namespace** attribute, you can refer to entity types, complex types, and association types that are defined in another conceptual model. More than one **Using** element can be a child of a **Schema** element.

> [!NOTE]
> The **Using** element in CSDL does not function exactly like a **using** statement in a programming language. By importing a namespace with a **using** statement in a programming language, you do not affect objects in the original namespace. In CSDL, an imported namespace can contain an entity type that is derived from an entity type in the original namespace. This can affect entity sets declared in the original namespace.

 

The **Using** element can have the following child elements:

-   Documentation (zero or one elements allowed)
-   Annotation elements (zero or more elements allowed)

### Applicable Attributes

The table below describes the attributes can be applied to the **Using** element.

| Attribute Name | Is Required | Value                                                                                                                                                                              |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Namespace**  | Yes         | The name of the imported namespace.                                                                                                                                                |
| **Alias**      | Yes         | An identifier used in place of the namespace name. Although this attribute is required, it is not required that it be used in place of the namespace name to qualify object names. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Using** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example demonstrates the **Using** element being used to import a namespace that is defined elsewhere. Note that the namespace for the **Schema** element shown is `BooksModel`. The `Address` property on the `Publisher`**EntityType** is a complex type that is defined in the `ExtendedBooksModel` namespace (imported with the **Using** element).

``` xml
 <Schema xmlns="https://schemas.microsoft.com/ado/2009/11/edm"
           xmlns:cg="https://schemas.microsoft.com/ado/2009/11/codegeneration"
           xmlns:store="https://schemas.microsoft.com/ado/2009/11/edm/EntityStoreSchemaGenerator"
           Namespace="BooksModel" Alias="Self">

     <Using Namespace="BooksModel.Extended" Alias="BMExt" />

 <EntityContainer Name="BooksContainer" >
       <EntitySet Name="Publishers" EntityType="BooksModel.Publisher" />
     </EntityContainer>

 <EntityType Name="Publisher">
       <Key>
         <PropertyRef Name="Id" />
       </Key>
       <Property Type="Int32" Name="Id" Nullable="false" />
       <Property Type="String" Name="Name" Nullable="false" />
       <Property Type="BMExt.Address" Name="Address" Nullable="false" />
     </EntityType>

 </Schema>
```
 

 

## Annotation Attributes (CSDL)

Annotation attributes in conceptual schema definition language (CSDL) are custom XML attributes in the conceptual model. In addition to having valid XML structure, the following must be true of annotation attributes:

-   Annotation attributes must not be in any XML namespace that is reserved for CSDL.
-   More than one annotation attribute may be applied to a given CSDL element.
-   The fully-qualified names of any two annotation attributes must not be the same.

Annotation attributes can be used to provide extra metadata about the elements in a conceptual model. Metadata contained in annotation elements can be accessed at runtime by using classes in the System.Data.Metadata.Edm namespace.

### Example

The following example shows an **EntityType** element with an annotation attribute (**CustomAttribute**). The example also shows an annotation element applied to the entity type element.

``` xml
 <Schema Namespace="SchoolModel" Alias="Self"
         xmlns:annotation="https://schemas.microsoft.com/ado/2009/02/edm/annotation"
         xmlns="https://schemas.microsoft.com/ado/2009/11/edm">
   <EntityContainer Name="SchoolEntities" annotation:LazyLoadingEnabled="true">
     <EntitySet Name="People" EntityType="SchoolModel.Person" />
   </EntityContainer>
   <EntityType Name="Person" xmlns:p="http://CustomNamespace.com"
               p:CustomAttribute="Data here.">
     <Key>
       <PropertyRef Name="PersonID" />
     </Key>
     <Property Name="PersonID" Type="Int32" Nullable="false"
               annotation:StoreGeneratedPattern="Identity" />
     <Property Name="LastName" Type="String" Nullable="false"
               MaxLength="50" Unicode="true" FixedLength="false" />
     <Property Name="FirstName" Type="String" Nullable="false"
               MaxLength="50" Unicode="true" FixedLength="false" />
     <Property Name="HireDate" Type="DateTime" />
     <Property Name="EnrollmentDate" Type="DateTime" />
     <p:CustomElement>
       Custom metadata.
     </p:CustomElement>
   </EntityType>
 </Schema>
```
 

The following code retrieves the metadata in the annotation attribute and writes it to the console:

``` xml
 EdmItemCollection collection = new EdmItemCollection("School.csdl");
 MetadataWorkspace workspace = new MetadataWorkspace();
 workspace.RegisterItemCollection(collection);
 EdmType contentType;
 workspace.TryGetType("Person", "SchoolModel", DataSpace.CSpace, out contentType);
 if (contentType.MetadataProperties.Contains("http://CustomNamespace.com:CustomAttribute"))
 {
     MetadataProperty annotationProperty =
         contentType.MetadataProperties["http://CustomNamespace.com:CustomAttribute"];
     object annotationValue = annotationProperty.Value;
     Console.WriteLine(annotationValue.ToString());
 }
```
 

The code above assumes that the `School.csdl` file is in the project's output directory and that you have added the following `Imports` and `Using` statements to your project:

``` csharp
 using System.Data.Metadata.Edm;
```
 

 

## Annotation Elements (CSDL)

Annotation elements in conceptual schema definition language (CSDL) are custom XML elements in the conceptual model. In addition to having valid XML structure, the following must be true of annotation elements:

-   Annotation elements must not be in any XML namespace that is reserved for CSDL.
-   More than one annotation element may be a child of a given CSDL element.
-   The fully-qualified names of any two annotation elements must not be the same.
-   Annotation elements must appear after all other child elements of a given CSDL element.

Annotation elements can be used to provide extra metadata about the elements in a conceptual model. Starting with the .NET Framework version 4, metadata contained in annotation elements can be accessed at runtime by using classes in the System.Data.Metadata.Edm namespace.

### Example

The following example shows an **EntityType** element with an annotation element (**CustomElement**). The example also show an annotation attribute applied to the entity type element.

``` xml
 <Schema Namespace="SchoolModel" Alias="Self"
         xmlns:annotation="https://schemas.microsoft.com/ado/2009/02/edm/annotation"
         xmlns="https://schemas.microsoft.com/ado/2009/11/edm">
   <EntityContainer Name="SchoolEntities" annotation:LazyLoadingEnabled="true">
     <EntitySet Name="People" EntityType="SchoolModel.Person" />
   </EntityContainer>
   <EntityType Name="Person" xmlns:p="http://CustomNamespace.com"
               p:CustomAttribute="Data here.">
     <Key>
       <PropertyRef Name="PersonID" />
     </Key>
     <Property Name="PersonID" Type="Int32" Nullable="false"
               annotation:StoreGeneratedPattern="Identity" />
     <Property Name="LastName" Type="String" Nullable="false"
               MaxLength="50" Unicode="true" FixedLength="false" />
     <Property Name="FirstName" Type="String" Nullable="false"
               MaxLength="50" Unicode="true" FixedLength="false" />
     <Property Name="HireDate" Type="DateTime" />
     <Property Name="EnrollmentDate" Type="DateTime" />
     <p:CustomElement>
       Custom metadata.
     </p:CustomElement>
   </EntityType>
 </Schema>
```
 

The following code retrieves the metadata in the annotation element and writes it to the console:

``` csharp
 EdmItemCollection collection = new EdmItemCollection("School.csdl");
 MetadataWorkspace workspace = new MetadataWorkspace();
 workspace.RegisterItemCollection(collection);
 EdmType contentType;
 workspace.TryGetType("Person", "SchoolModel", DataSpace.CSpace, out contentType);
 if (contentType.MetadataProperties.Contains("http://CustomNamespace.com:CustomElement"))
 {
     MetadataProperty annotationProperty =
         contentType.MetadataProperties["http://CustomNamespace.com:CustomElement"];
     object annotationValue = annotationProperty.Value;
     Console.WriteLine(annotationValue.ToString());
 }
```
 

The code above assumes that the School.csdl file is in the project's output directory and that you have added the following `Imports` and `Using` statements to your project:

``` csharp
 using System.Data.Metadata.Edm;
```
 

 

## Conceptual Model Types (CSDL)

Conceptual schema definition language (CSDL) supports a set of abstract primitive data types, called **EDMSimpleTypes**, that define properties in a conceptual model. **EDMSimpleTypes** are proxies for primitive data types that are supported in the storage or hosting environment.

The table below lists the primitive data types that are supported by CSDL. The table also lists the facets that can be applied to each **EDMSimpleType**.

| EDMSimpleType                    | Description                                                | Applicable Facets                                                        |
|:---------------------------------|:-----------------------------------------------------------|:-------------------------------------------------------------------------|
| **Edm.Binary**                   | Contains binary data.                                      | MaxLength, FixedLength, Nullable, Default                                |
| **Edm.Boolean**                  | Contains the value **true** or **false**.                  | Nullable, Default                                                        |
| **Edm.Byte**                     | Contains an unsigned 8-bit integer value.                  | Precision, Nullable, Default                                             |
| **Edm.DateTime**                 | Represents a date and time.                                | Precision, Nullable, Default                                             |
| **Edm.DateTimeOffset**           | Contains a date and time as an offset in minutes from GMT. | Precision, Nullable, Default                                             |
| **Edm.Decimal**                  | Contains a numeric value with fixed precision and scale.   | Precision, Nullable, Default                                             |
| **Edm.Double**                   | Contains a floating point number with 15-digit precision   | Precision, Nullable, Default                                             |
| **Edm.Float**                    | Contains a floating point number with 7-digit precision.   | Precision, Nullable, Default                                             |
| **Edm.Guid**                     | Contains a 16-byte unique identifier.                      | Precision, Nullable, Default                                             |
| **Edm.Int16**                    | Contains a signed 16-bit integer value.                    | Precision, Nullable, Default                                             |
| **Edm.Int32**                    | Contains a signed 32-bit integer value.                    | Precision, Nullable, Default                                             |
| **Edm.Int64**                    | Contains a signed 64-bit integer value.                    | Precision, Nullable, Default                                             |
| **Edm.SByte**                    | Contains a signed 8-bit integer value.                     | Precision, Nullable, Default                                             |
| **Edm.String**                   | Contains character data.                                   | Unicode, FixedLength, MaxLength, Collation, Precision, Nullable, Default |
| **Edm.Time**                     | Contains a time of day.                                    | Precision, Nullable, Default                                             |
| **Edm.Geography**                |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyPoint**           |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyLineString**      |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyPolygon**         |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyMultiPoint**      |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyMultiLineString** |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyMultiPolygon**    |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeographyCollection**      |                                                            | Nullable, Default, SRID                                                  |
| **Edm.Geometry**                 |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryPoint**            |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryLineString**       |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryPolygon**          |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryMultiPoint**       |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryMultiLineString**  |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryMultiPolygon**     |                                                            | Nullable, Default, SRID                                                  |
| **Edm.GeometryCollection**       |                                                            | Nullable, Default, SRID                                                  |

## Facets (CSDL)

Facets in conceptual schema definition language (CSDL) represent constraints on properties of entity types and complex types. Facets appear as XML attributes on the following CSDL elements:

-   Property
-   TypeRef
-   Parameter

The following table describes the facets that are supported in CSDL. All facets are optional. Some facets listed below are used by the Entity Framework when generating a database from a conceptual model.

> [!NOTE]
> For information about data types in a conceptual model, see Conceptual Model Types (CSDL).

| Facet               | Description                                                                                                                                                                                                                                                   | Applies to                                                                                                                                                                                                                                                                                                                                                                           | Used for the database generation | Used by the runtime |
|:--------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:---------------------------------|:--------------------|
| **Collation**       | Specifies the collating sequence (or sorting sequence) to be used when performing comparison and ordering operations on values of the property.                                                                                                               | **Edm.String**                                                                                                                                                                                                                                                                                                                                                                       | Yes                              | No                  |
| **ConcurrencyMode** | Indicates that the value of the property should be used for optimistic concurrency checks.                                                                                                                                                                    | All **EDMSimpleType** properties                                                                                                                                                                                                                                                                                                                                                     | No                               | Yes                 |
| **Default**         | Specifies the default value of the property if no value is supplied upon instantiation.                                                                                                                                                                       | All **EDMSimpleType** properties                                                                                                                                                                                                                                                                                                                                                     | Yes                              | Yes                 |
| **FixedLength**     | Specifies whether the length of the property value can vary.                                                                                                                                                                                                  | **Edm.Binary**, **Edm.String**                                                                                                                                                                                                                                                                                                                                                       | Yes                              | No                  |
| **MaxLength**       | Specifies the maximum length of the property value.                                                                                                                                                                                                           | **Edm.Binary**, **Edm.String**                                                                                                                                                                                                                                                                                                                                                       | Yes                              | No                  |
| **Nullable**        | Specifies whether the property can have a **null** value.                                                                                                                                                                                                     | All **EDMSimpleType** properties                                                                                                                                                                                                                                                                                                                                                     | Yes                              | Yes                 |
| **Precision**       | For properties of type **Decimal**, specifies the number of digits a property value can have. For properties of type **Time**, **DateTime**, and **DateTimeOffset**, specifies the number of digits for the fractional part of seconds of the property value. | **Edm.DateTime**, **Edm.DateTimeOffset**, **Edm.Decimal**, **Edm.Time**                                                                                                                                                                                                                                                                                                              | Yes                              | No                  |
| **Scale**           | Specifies the number of digits to the right of the decimal point for the property value.                                                                                                                                                                      | **Edm.Decimal**                                                                                                                                                                                                                                                                                                                                                                      | Yes                              | No                  |
| **SRID**            | Specifies the Spatial System Reference System ID. For more information, see [SRID](https://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx).                                                              | **Edm.Geography, Edm.GeographyPoint, Edm.GeographyLineString, Edm.GeographyPolygon, Edm.GeographyMultiPoint, Edm.GeographyMultiLineString, Edm.GeographyMultiPolygon, Edm.GeographyCollection, Edm.Geometry, Edm.GeometryPoint, Edm.GeometryLineString, Edm.GeometryPolygon, Edm.GeometryMultiPoint, Edm.GeometryMultiLineString, Edm.GeometryMultiPolygon, Edm.GeometryCollection** | No                               | Yes                 |
| **Unicode**         | Indicates whether the property value is stored as Unicode.                                                                                                                                                                                                    | **Edm.String**                                                                                                                                                                                                                                                                                                                                                                       | Yes                              | Yes                 |

>[!NOTE]
> When generating a database from a conceptual model, the Generate Database Wizard will recognize the value of the **StoreGeneratedPattern** attribute on a **Property** element if it is in the following namespace: `https://schemas.microsoft.com/ado/2009/02/edm/annotation`. The supported values for the attribute are **Identity** and **Computed**. A value of **Identity** will produce a database column with an identity value that is generated in the database. A value of **Computed** will produce a column with a value that is computed in the database.

### Example

The following example shows facets applied to the properties of an entity type:

``` xml
 <EntityType Name="Product">
   <Key>
     <PropertyRef Name="ProductId" />
   </Key>
   <Property Type="Int32"
             Name="ProductId" Nullable="false"
             a:StoreGeneratedPattern="Identity"
    xmlns:a="https://schemas.microsoft.com/ado/2009/02/edm/annotation" />
   <Property Type="String"
             Name="ProductName"
             Nullable="false"
             MaxLength="50" />
   <Property Type="String"
             Name="Location"
             Nullable="true"
             MaxLength="25" />
 </EntityType>
```
