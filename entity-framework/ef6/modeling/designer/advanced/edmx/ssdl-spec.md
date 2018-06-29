---
title: "SSDL Specification - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: a4af4b1a-40f4-48cc-b2e0-fa8f5d9d5419
caps.latest.revision: 3
---
# SSDL Specification
Store schema definition language (SSDL) is an XML-based language that describes the storage model of an Entity Framework application.

In an Entity Framework application, storage model metadata is loaded from a .ssdl file (written in SSDL) into an instance of the System.Data.Metadata.Edm.StoreItemCollection and is accessible by using methods in the System.Data.Metadata.Edm.MetadataWorkspace class. Entity Framework uses storage model metadata to translate queries against the conceptual model to store-specific commands.

The Entity Framework Designer (EF Designer) stores storage model information in an .edmx file at design time. At build time the Entity Designer uses information in an .edmx file to create the .ssdl file that is needed by Entity Framework at runtime.

Versions of SSDL are differentiated by XML namespaces.

| SSDL Version | XML Namespace                                     |
|:-------------|:--------------------------------------------------|
| SSDL v1      | http://schemas.microsoft.com/ado/2006/04/edm/ssdl |
| SSDL v2      | http://schemas.microsoft.com/ado/2009/02/edm/ssdl |
| SSDL v3      | http://schemas.microsoft.com/ado/2009/11/edm/ssdl |

## Association Element (SSDL)

An **Association** element in store schema definition language (SSDL) specifies table columns that participate in a foreign key constraint in the underlying database. Two required child End elements specify tables at the ends of the association and the multiplicity at each end. An optional ReferentialConstraint element specifies the principal and dependent ends of the association as well as the participating columns. If no **ReferentialConstraint** element is present, an AssociationSetMapping element must be used to specify the column mappings for the association.

The **Association** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   End (exactly two)
-   ReferentialConstraint (zero or one)
-   Annotation elements (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **Association** element.

| Attribute Name | Is Required | Value                                                                            |
|:---------------|:------------|:---------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the corresponding foreign key constraint in the underlying database. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Association** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **Association** element that uses a **ReferentialConstraint** element to specify the columns that participate in the **FK\_CustomerOrders** foreign key constraint:

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## AssociationSet Element (SSDL)

The **AssociationSet** element in store schema definition language (SSDL) represents a foreign key constraint between two tables in the underlying database. The table columns that participate in the foreign key constraint are specified in an Association element. The **Association** element that corresponds to a given **AssociationSet** element is specified in the **Association** attribute of the **AssociationSet** element.

SSDL association sets are mapped to CSDL association sets by an AssociationSetMapping element. However, if the CSDL association for a given CSDL association set is defined by using a ReferentialConstraint element , no corresponding **AssociationSetMapping** element is necessary. In this case, if an **AssociationSetMapping** element is present, the mappings it defines will be overridden by the **ReferentialConstraint** element.

The **AssociationSet** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   End (zero or two)
-   Annotation elements (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **AssociationSet** element.

| Attribute Name  | Is Required | Value                                                                                                |
|:----------------|:------------|:-----------------------------------------------------------------------------------------------------|
| **Name**        | Yes         | The name of the foreign key constraint that the association set represents.                          |
| **Association** | Yes         | The name of the association that defines the columns that participate in the foreign key constraint. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **AssociationSet** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **AssociationSet** element that represents the `FK_CustomerOrders` foreign key constraint in the underlying database:

``` xml
 <AssociationSet Name="FK_CustomerOrders"
                 Association="ExampleModel.Store.FK_CustomerOrders">
   <End Role="Customers" EntitySet="Customers" />
   <End Role="Orders" EntitySet="Orders" />
 </AssociationSet>
```
 

 

## CollectionType Element (SSDL)

The **CollectionType** element in store schema definition language (SSDL) specifies that a function’s return type is a collection. The **CollectionType** element is a child of the ReturnType element. The type of collection is specified by using the RowType child element:

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **CollectionType** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a function that uses a **CollectionType** element to specify that the function returns a collection of rows.

``` xml
   <Function Name="GetProducts" IsComposable="true" Schema="dbo">
     <ReturnType>
       <CollectionType>
         <RowType>
           <Property Name="ProductID" Type="int" Nullable="false" />
           <Property Name="CategoryID" Type="bigint" Nullable="false" />
           <Property Name="ProductName" Type="nvarchar" MaxLength="40" Nullable="false" />
           <Property Name="UnitPrice" Type="money" />
           <Property Name="Discontinued" Type="bit" />
         </RowType>
       </CollectionType>
     </ReturnType>
   </Function>
```
 

 

## CommandText Element (SSDL)

The **CommandText** element in store schema definition language (SSDL) is a child of the Function element that allows you to define a SQL statement that is executed at the database. The **CommandText** element allows you to add functionality that is similar to a stored procedure in the database, but you define the **CommandText** element in the storage model.

The **CommandText** element cannot have child elements. The body of the **CommandText** element must be a valid SQL statement for the underlying database.

No attributes are applicable to the **CommandText** element.

### Example

The following example shows a **Function** element with a child **CommandText** element. Expose the **UpdateProductInOrder** function as a method on the ObjectContext by importing it into the conceptual model.  

``` xml
 <Function Name="UpdateProductInOrder" IsComposable="false">
   <CommandText>
     UPDATE Orders
     SET ProductId = @productId
     WHERE OrderId = @orderId;
   </CommandText>
   <Parameter Name="productId"
              Mode="In"
              Type="int"/>
   <Parameter Name="orderId"
              Mode="In"
              Type="int"/>
 </Function>
```
 

 

## DefiningQuery Element (SSDL)

 The **DefiningQuery** element in store schema definition language (SSDL) allows you to execute a SQL statement directly in the underlying database. The **DefiningQuery** element is commonly used like a database view, but the view is defined in the storage model instead of the database. The view defined in a **DefiningQuery** element can be mapped to an entity type in the conceptual model through an EntitySetMapping element. These mappings are read-only.  

The following SSDL syntax shows the declaration of an **EntitySet** followed by the **DefiningQuery** element that contains a query used to retrieve the view.

``` xml
 <Schema>
     <EntitySet Name="Tables" EntityType="Self.STable">
         <DefiningQuery>
           SELECT  TABLE_CATALOG,
                   'test' as TABLE_SCHEMA,
                   TABLE_NAME
           FROM    INFORMATION_SCHEMA.TABLES
         </DefiningQuery>
     </EntitySet>
 </Schema>
```
 

You can use stored procedures in the Entity Framework to enable read-write scenarios over views. You can use either a data source view or an Entity SQL view as the base table for retrieving data and for change processing by stored procedures.

You can use the **DefiningQuery** element to target Microsoft SQL Server Compact 3.5. Though SQL Server Compact 3.5 does not support stored procedures, you can implement similar functionality with the **DefiningQuery** element. Another place where it can be useful is in creating stored procedures to overcome a mismatch between the data types used in the programming language and those of the data source. You could write a **DefiningQuery** that takes a certain set of parameters and then calls a stored procedure with a different set of parameters, for example, a stored procedure that deletes data.

 

## Dependent Element (SSDL)

The **Dependent** element in store schema definition language (SSDL) is a child element to the ReferentialConstraint element that defines the dependent end of a foreign key constraint (also called a referential constraint). The **Dependent** element specifies the column (or columns) in a table that reference a primary key column (or columns). **PropertyRef** elements specify which columns are referenced. The Principal element specifies the primary key columns that are referenced by columns that are specified in the **Dependent** element.

The **Dependent** element can have the following child elements (in the order listed):

-   PropertyRef (one or more)
-   Annotation elements (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **Dependent** element.

| Attribute Name | Is Required | Value                                                                                                                                                       |
|:---------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Role**       | Yes         | The same value as the **Role** attribute (if used) of the corresponding End element; otherwise, the name of the table that contains the referencing column. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Dependent** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an Association element that uses a **ReferentialConstraint** element to specify the columns that participate in the **FK\_CustomerOrders** foreign key constraint. The **Dependent** element specifies the **CustomerId** column of the **Order** table as the dependent end of the constraint.

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## Documentation Element (SSDL)

The **Documentation** element in store schema definition language (SSDL) can be used to provide information about an object that is defined in a parent element.

The **Documentation** element can have the following child elements (in the order listed):

-   **Summary**: A brief description of the parent element. (zero or one element)
-   **LongDescription**: An extensive description of the parent element. (zero or one element)

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **Documentation** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example shows the **Documentation** element as a child element of an EntityType element.

``` xml
 <EntityType Name="Customers">
   <Documentation>
     <Summary>Summary here.</Summary>
     <LongDescription>Long description here.</LongDescription>
   </Documentation>
   <Key>
     <PropertyRef Name="CustomerId" />
   </Key>
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
 </EntityType>
```
 

 

## End Element (SSDL)

The **End** element in store schema definition language (SSDL) specifies the table and number of rows at one end of a foreign key constraint in the underlying database. The **End** element can be a child of the Association element or the AssociationSet element. In each case, the possible child elements and applicable attributes are different.

### End Element as a Child of the Association Element

An **End** element (as a child of the **Association** element) specifies the table and number of rows at the end of a foreign key constraint with the **Type** and **Multiplicity** attributes respectively. Ends of a foreign key constraint are defined as part of an SSDL association; an SSDL association must have exactly two ends.

An **End** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   OnDelete (zero or one element)
-   Annotation elements (zero or more elements)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **End** element when it is the child of an **Association** element.

| Attribute Name   | Is Required | Value                                                                                                                                                                                                                                                                                                                                                                                      |
|:-----------------|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Type**         | Yes         | The fully qualified name of the SSDL entity set that is at the end of the foreign key constraint.                                                                                                                                                                                                                                                                                          |
| **Role**         | No          | The value of the **Role** attribute in either the Principal or Dependent element of the corresponding ReferentialConstraint element (if used).                                                                                                                                                                                                                                             |
| **Multiplicity** | Yes         | **1**, **0..1**, or **\*** depending on the number of rows that can be at the end of the foreign key constraint. <br/> **1** indicates that exactly one row exists at the foreign key constraint end. <br/> **0..1** indicates that zero or one row exists at the foreign key constraint end. <br/> **\*** indicates that zero, one, or more rows exist at the foreign key constraint end. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **End** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows an **Association** element that defines the **FK\_CustomerOrders** foreign key constraint. The **Multiplicity** values specified on each **End** element indicate that many rows in the **Orders** table can be associated with a row in the **Customers** table, but only one row in the **Customers** table can be associated with a row in the **Orders** table. Additionally, the **OnDelete** element indicates that all rows in the **Orders** table that reference a particular row in the **Customers** table will be deleted if the row in the **Customers** table is deleted.

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

### End Element as a Child of the AssociationSet Element

The **End** element (as a child of the **AssociationSet** element) specifies a table at one end of a foreign key constraint in the underlying database.

An **End** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   Annotation elements (zero or more)

#### Applicable Attributes

The following table describes the attributes that can be applied to the **End** element when it is the child of an **AssociationSet** element.

| Attribute Name | Is Required | Value                                                                                                                  |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------|
| **EntitySet**  | Yes         | The name of the SSDL entity set that is at the end of the foreign key constraint.                                      |
| **Role**       | No          | The value of one of the **Role** attributes specified on one **End** element of the corresponding Association element. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **End** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

#### Example

The following example shows an **EntityContainer** element with an **AssociationSet** element with two **End** elements:

``` xml
 <EntityContainer Name="ExampleModelStoreContainer">
   <EntitySet Name="Customers"
              EntityType="ExampleModel.Store.Customers"
              Schema="dbo" />
   <EntitySet Name="Orders"
              EntityType="ExampleModel.Store.Orders"
              Schema="dbo" />
   <AssociationSet Name="FK_CustomerOrders"
                   Association="ExampleModel.Store.FK_CustomerOrders">
     <End Role="Customers" EntitySet="Customers" />
     <End Role="Orders" EntitySet="Orders" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntityContainer Element (SSDL)

An **EntityContainer** element in store schema definition language (SSDL) describes the structure of the underlying data source in an Entity Framework application: SSDL entity sets (defined in EntitySet elements) represent tables in a database, SSDL entity types (defined in EntityType elements) represent rows in a table, and association sets (defined in AssociationSet elements) represent foreign key constraints in a database. A storage model entity container maps to a conceptual model entity container through the EntityContainerMapping element.

An **EntityContainer** element can have zero or one Documentation elements. If a **Documentation** element is present, it must precede all other child elements.

An **EntityContainer** element can have zero or more of the following child elements (in the order listed):

-   EntitySet
-   AssociationSet
-   Annotation elements

### Applicable Attributes

The table below describes the attributes that can be applied to the **EntityContainer** element.

| Attribute Name | Is Required | Value                                                                   |
|:---------------|:------------|:------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity container. This name cannot contain periods (.). |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntityContainer** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityContainer** element that defines two entity sets and one association set. Note that entity type and association type names are qualified by the conceptual model namespace name.

``` xml
 <EntityContainer Name="ExampleModelStoreContainer">
   <EntitySet Name="Customers"
              EntityType="ExampleModel.Store.Customers"
              Schema="dbo" />
   <EntitySet Name="Orders"
              EntityType="ExampleModel.Store.Orders"
              Schema="dbo" />
   <AssociationSet Name="FK_CustomerOrders"
                   Association="ExampleModel.Store.FK_CustomerOrders">
     <End Role="Customers" EntitySet="Customers" />
     <End Role="Orders" EntitySet="Orders" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntitySet Element (SSDL)

 An **EntitySet** element in store schema definition language (SSDL) represents a table or view in the underlying database. An EntityType element in SSDL represents a row in the table or view. The **EntityType** attribute of an **EntitySet** element specifies the particular SSDL entity type that represents rows in an SSDL entity set. The mapping between a CSDL entity set and an SSDL entity set is specified in an EntitySetMapping element.

The **EntitySet** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   DefiningQuery (zero or one element)
-   Annotation elements

### Applicable Attributes

The following table describes the attributes that can be applied to the **EntitySet** element.

> [!NOTE]
> Some attributes (not listed here) may be qualified with the **store** alias. These attributes are used by the Update Model Wizard when updating a model.

 

| Attribute Name | Is Required | Value                                                                                    |
|:---------------|:------------|:-----------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity set.                                                              |
| **EntityType** | Yes         | The fully-qualified name of the entity type for which the entity set contains instances. |
| **Schema**     | No          | The database schema.                                                                     |
| **Table**      | No          | The database table.                                                                      |
 
 
 
 
 
 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntitySet** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityContainer** element that has two **EntitySet** elements and one **AssociationSet** element:

``` xml
 <EntityContainer Name="ExampleModelStoreContainer">
   <EntitySet Name="Customers"
              EntityType="ExampleModel.Store.Customers"
              Schema="dbo" />
   <EntitySet Name="Orders"
              EntityType="ExampleModel.Store.Orders"
              Schema="dbo" />
   <AssociationSet Name="FK_CustomerOrders"
                   Association="ExampleModel.Store.FK_CustomerOrders">
     <End Role="Customers" EntitySet="Customers" />
     <End Role="Orders" EntitySet="Orders" />
   </AssociationSet>
 </EntityContainer>
```
 

 

## EntityType Element (SSDL)

An **EntityType** element in store schema definition language (SSDL) represents a row in a table or view of the underlying database. An EntitySet element in SSDL represents the table or view in which rows occur. The **EntityType** attribute of an **EntitySet** element specifies the particular SSDL entity type that represents rows in an SSDL entity set. The mapping between an SSDL entity type and a CSDL entity type is specified in an EntityTypeMapping element.

The **EntityType** element can have the following child elements (in the order listed):

-   Documentation (zero or one element)
-   Key (zero or one element)
-   Annotation elements

### Applicable Attributes

The table below describes the attributes that can be applied to the **EntityType** element.

| Attribute Name | Is Required | Value                                                                                                                                                                  |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the entity type. This value is usually the same as the name of the table in which the entity type represents a row. This value can contain no periods (.). |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **EntityType** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityType** element with two properties:

``` xml
 <EntityType Name="Customers">
   <Documentation>
     <Summary>Summary here.</Summary>
     <LongDescription>Long description here.</LongDescription>
   </Documentation>
   <Key>
     <PropertyRef Name="CustomerId" />
   </Key>
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
 </EntityType>
```
 

 

## Function Element (SSDL)

The **Function** element in store schema definition language (SSDL) specifies a stored procedure that exists in the underlying database.

The **Function** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   Parameter (zero or more)
-   CommandText (zero or one)
-   ReturnType (zero or more)
-   Annotation elements (zero or more)

A return type for a function must be specified with either the **ReturnType** element or the **ReturnType** attribute (see below), but not both.

Stored procedures that are specified in the storage model can be imported into the conceptual model of an application. For more information, see [Querying with Stored Procedures](~/ef6/modeling/designer/stored-procedures/query.md). The **Function** element can also be used to define custom functions in the storage model.  

### Applicable Attributes

The following table describes the attributes that can be applied to the **Function** element.

> [!NOTE]
> Some attributes (not listed here) may be qualified with the **store** alias. These attributes are used by the Update Model Wizard when updating a model.

 

| Attribute Name             | Is Required | Value                                                                                                                                                                                                              |
|:---------------------------|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**                   | Yes         | The name of the stored procedure.                                                                                                                                                                                  |
| **ReturnType**             | No          | The return type of the stored procedure.                                                                                                                                                                           |
| **Aggregate**              | No          | **True** if the stored procedure returns an aggregate value; otherwise **False**.                                                                                                                                  |
| **BuiltIn**                | No          | **True** if the function is a built-in<sup>1</sup> function; otherwise **False**.                                                                                                                                  |
| **StoreFunctionName**      | No          | The name of the stored procedure.                                                                                                                                                                                  |
| **NiladicFunction**        | No          | **True** if the function is a niladic<sup>2</sup> function; **False** otherwise.                                                                                                                                   |
| **IsComposable**           | No          | **True** if the function is a composable<sup>3</sup> function; **False** otherwise.                                                                                                                                |
| **ParameterTypeSemantics** | No          | The enumeration that defines the type semantics used to resolve function overloads. The enumeration is defined in the provider manifest per function definition. The default value is **AllowImplicitConversion**. |
| **Schema**                 | No          | The name of the schema in which the stored procedure is defined.                                                                                                                                                   |

 

<sup>1</sup> A built-in function is a function that is defined in the database. For information about functions that are defined in the storage model, see CommandText Element (SSDL).

<sup>2</sup> A niladic function is a function that accepts no parameters and, when called, does not require parentheses.

<sup>3</sup> Two functions are composable if the output of one function can be the input for the other function.

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Function** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **Function** element that corresponds to the **UpdateOrderQuantity** stored procedure. The stored procedure accepts two parameters and does not return a value.

``` xml
 <Function Name="UpdateOrderQuantity"
           Aggregate="false"
           BuiltIn="false"
           NiladicFunction="false"
           IsComposable="false"
           ParameterTypeSemantics="AllowImplicitConversion"
           Schema="dbo">
   <Parameter Name="orderId" Type="int" Mode="In" />
   <Parameter Name="newQuantity" Type="int" Mode="In" />
 </Function>
```
 

 

## Key Element (SSDL)

The **Key** element in store schema definition language (SSDL) represents the primary key of a table in the underlying database. **Key** is a child element of an EntityType element, which represents a row in a table. The primary key is defined in the **Key** element by referencing one or more Property elements that are defined on the **EntityType** element.

The **Key** element can have the following child elements (in the order listed):

-   PropertyRef (one or more)
-   Annotation elements

No attributes are applicable to the **Key** element.

### Example

The following example shows an **EntityType** element with a key that references one property:

``` xml
 <EntityType Name="Customers">
   <Documentation>
     <Summary>Summary here.</Summary>
     <LongDescription>Long description here.</LongDescription>
   </Documentation>
   <Key>
     <PropertyRef Name="CustomerId" />
   </Key>
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
 </EntityType>
```
 

 

## OnDelete Element (SSDL)

The **OnDelete** element in store schema definition language (SSDL) reflects the database behavior when a row that participates in a foreign key constraint is deleted. If the action is set to **Cascade**, then rows that reference a row that is being deleted will also be deleted. If the action is set to **None**, then rows that reference a row that is being deleted are not also deleted. An **OnDelete** element is a child element of an End element.

An **OnDelete** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   Annotation elements (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **OnDelete** element.

| Attribute Name | Is Required | Value                                                                                               |
|:---------------|:------------|:----------------------------------------------------------------------------------------------------|
| **Action**     | Yes         | **Cascade** or **None**. (The value **Restricted** is valid but has the same behavior as **None**.) |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **OnDelete** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **Association** element that defines the **FK\_CustomerOrders** foreign key constraint. The **OnDelete** element indicates that all rows in the **Orders** table that reference a particular row in the **Customers** table will be deleted if the row in the **Customers** table is deleted.

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## Parameter Element (SSDL)

The **Parameter** element in store schema definition language (SSDL) is a child of the Function element that specifies parameters for a stored procedure in the database.

The **Parameter** element can have the following child elements (in the order listed):

-   Documentation (zero or one)
-   Annotation elements (zero or more)

### Applicable Attributes

The table below describes the attributes that can be applied to the **Parameter** element.

| Attribute Name | Is Required | Value                                                                                                                                                                                                                           |
|:---------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**       | Yes         | The name of the parameter.                                                                                                                                                                                                      |
| **Type**       | Yes         | The parameter type.                                                                                                                                                                                                             |
| **Mode**       | No          | **In**, **Out**, or **InOut** depending on whether the parameter is an input, output, or input/output parameter.                                                                                                                |
| **MaxLength**  | No          | The maximum length of the parameter.                                                                                                                                                                                            |
| **Precision**  | No          | The precision of the parameter.                                                                                                                                                                                                 |
| **Scale**      | No          | The scale of the parameter.                                                                                                                                                                                                     |
| **SRID**       | No          | Spatial System Reference Identifier. Valid only for parameters of spatial types. For more information, see [SRID](http://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Parameter** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **Function** element that has two **Parameter** elements that specify input parameters:

``` xml
 <Function Name="UpdateOrderQuantity"
           Aggregate="false"
           BuiltIn="false"
           NiladicFunction="false"
           IsComposable="false"
           ParameterTypeSemantics="AllowImplicitConversion"
           Schema="dbo">
   <Parameter Name="orderId" Type="int" Mode="In" />
   <Parameter Name="newQuantity" Type="int" Mode="In" />
 </Function>
```
 

 

## Principal Element (SSDL)

The **Principal** element in store schema definition language (SSDL) is a child element to the ReferentialConstraint element that defines the principal end of a foreign key constraint (also called a referential constraint). The **Principal** element specifies the primary key column (or columns) in a table that is referenced by another column (or columns). **PropertyRef** elements specify which columns are referenced. The Dependent element specifies columns that reference the primary key columns that are specified in the **Principal** element.

The **Principal** element can have the following child elements (in the order listed):

-   PropertyRef (one or more)
-   Annotation elements (zero or more)

### Applicable Attributes

The following table describes the attributes that can be applied to the **Principal** element.

| Attribute Name | Is Required | Value                                                                                                                                                      |
|:---------------|:------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Role**       | Yes         | The same value as the **Role** attribute (if used) of the corresponding End element; otherwise, the name of the table that contains the referenced column. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Principal** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an Association element that uses a **ReferentialConstraint** element to specify the columns that participate in the **FK\_CustomerOrders** foreign key constraint. The **Principal** element specifies the **CustomerId** column of the **Customer** table as the principal end of the constraint.

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```
 

 

## Property Element (SSDL)

The **Property** element in store schema definition language (SSDL) represents a column in a table in the underlying database. **Property** elements are children of EntityType elements, which represent rows in a table. Each **Property** element defined on an **EntityType** element represents a column.

A **Property** element cannot have any child elements.

### Applicable Attributes

The following table describes the attributes that can be applied to the **Property** element.

| Attribute Name            | Is Required | Value                                                                                                                                                                                                                           |
|:--------------------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Name**                  | Yes         | The name of the corresponding column.                                                                                                                                                                                           |
| **Type**                  | Yes         | The type of the corresponding column.                                                                                                                                                                                           |
| **Nullable**              | No          | **True** (the default value) or **False** depending on whether the corresponding column can have a null value.                                                                                                                  |
| **DefaultValue**          | No          | The default value of the corresponding column.                                                                                                                                                                                  |
| **MaxLength**             | No          | The maximum length of the corresponding column.                                                                                                                                                                                 |
| **FixedLength**           | No          | **True** or **False** depending on whether the corresponding column value will be stored as a fixed length string.                                                                                                              |
| **Precision**             | No          | The precision of the corresponding column.                                                                                                                                                                                      |
| **Scale**                 | No          | The scale of the corresponding column.                                                                                                                                                                                          |
| **Unicode**               | No          | **True** or **False** depending on whether the corresponding column value will be stored as a Unicode string.                                                                                                                   |
| **Collation**             | No          | A string that specifies the collating sequence to be used in the data source.                                                                                                                                                   |
| **SRID**                  | No          | Spatial System Reference Identifier. Valid only for properties of spatial types. For more information, see [SRID](http://en.wikipedia.org/wiki/SRID) and [SRID (SQL Server)](https://msdn.microsoft.com/library/bb964707.aspx). |
| **StoreGeneratedPattern** | No          | **None**, **Identity** (if the corresponding column value is an identity that is generated in the database), or **Computed** (if the corresponding column value is computed in the database). Not Valid for RowType properties. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **Property** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows an **EntityType** element with two child **Property** elements:

``` xml
 <EntityType Name="Customers">
   <Documentation>
     <Summary>Summary here.</Summary>
     <LongDescription>Long description here.</LongDescription>
   </Documentation>
   <Key>
     <PropertyRef Name="CustomerId" />
   </Key>
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
 </EntityType>
```
 

 

## PropertyRef Element (SSDL)

The **PropertyRef** element in store schema definition language (SSDL) references a property defined on an EntityType element to indicate that the property will perform one of the following roles:

-   Be part of the primary key of the table that the **EntityType** represents. One or more **PropertyRef** elements can be used to define a primary key. For more information, see Key element.
-   Be the dependent or principal end of a referential constraint. For more information, see ReferentialConstraint element.

The **PropertyRef** element can only have the following child elements:

-   Documentation (zero or one)
-   Annotation elements

### Applicable Attributes

The table below describes the attributes that can be applied to the **PropertyRef** element.

| Attribute Name | Is Required | Value                                |
|:---------------|:------------|:-------------------------------------|
| **Name**       | Yes         | The name of the referenced property. |

 

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **PropertyRef** element. However, custom attributes may not belong to any XML namespace that is reserved for CSDL. The fully-qualified names for any two custom attributes cannot be the same.

 

### Example

The following example shows a **PropertyRef** element used to define a primary key by referencing a property that is defined on an **EntityType** element.

``` xml
 <EntityType Name="Customers">
   <Documentation>
     <Summary>Summary here.</Summary>
     <LongDescription>Long description here.</LongDescription>
   </Documentation>
   <Key>
     <PropertyRef Name="CustomerId" />
   </Key>
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
 </EntityType>
```
 

 

## ReferentialConstraint Element (SSDL)

The **ReferentialConstraint** element in store schema definition language (SSDL) represents a foreign key constraint (also called a referential integrity constraint) in the underlying database. The principal and dependent ends of the constraint are specified by the Principal and Dependent child elements, respectively. Columns that participate in the principal and dependent ends are referenced with PropertyRef elements.

The **ReferentialConstraint** element is an optional child element of the Association element. If a **ReferentialConstraint** element is not used to map the foreign key constraint that is specified in the **Association** element, an AssociationSetMapping element must be used to do this.

The **ReferentialConstraint** element can have the following child elements:

-   Documentation (zero or one)
-   Principal (exactly one)
-   Dependent (exactly one)
-   Annotation elements (zero or more)

### Applicable Attributes

Any number of annotation attributes (custom XML attributes) may be applied to the **ReferentialConstraint** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example shows an **Association** element that uses a **ReferentialConstraint** element to specify the columns that participate in the **FK\_CustomerOrders** foreign key constraint:

``` xml
 <Association Name="FK_CustomerOrders">
   <End Role="Customers"
        Type="ExampleModel.Store.Customers" Multiplicity="1">
     <OnDelete Action="Cascade" />
   </End>
   <End Role="Orders"
        Type="ExampleModel.Store.Orders" Multiplicity="*" />
   <ReferentialConstraint>
     <Principal Role="Customers">
       <PropertyRef Name="CustomerId" />
     </Principal>
     <Dependent Role="Orders">
       <PropertyRef Name="CustomerId" />
     </Dependent>
   </ReferentialConstraint>
 </Association>
```

## ReturnType Element (SSDL)

The **ReturnType** element in store schema definition language (SSDL) specifies the return type for a function that is defined in a **Function** element. A function return type can also be specified with a **ReturnType** attribute.

The return type of a function is specified with the **Type** attribute or the **ReturnType** element.

The **ReturnType** element can have the following child elements:

- CollectionType (one)  

> [!NOTE]
> Any number of annotation attributes (custom XML attributes) may be applied to the **ReturnType** element. However, custom attributes may not belong to any XML namespace that is reserved for SSDL. The fully-qualified names for any two custom attributes cannot be the same.

### Example

The following example uses a **Function** that returns a collection of rows.

``` xml
   <Function Name="GetProducts" IsComposable="true" Schema="dbo">
     <ReturnType>
       <CollectionType>
         <RowType>
           <Property Name="ProductID" Type="int" Nullable="false" />
           <Property Name="CategoryID" Type="bigint" Nullable="false" />
           <Property Name="ProductName" Type="nvarchar" MaxLength="40" Nullable="false" />
           <Property Name="UnitPrice" Type="money" />
           <Property Name="Discontinued" Type="bit" />
         </RowType>
       </CollectionType>
     </ReturnType>
   </Function>
```


## RowType Element (SSDL)

A **RowType** element in store schema definition language (SSDL) defines an unnamed structure as a return type for a function defined in the store.

A **RowType** element is the child element of **CollectionType** element:

A **RowType** element can have the following child elements:

- Property (one or more)  

### Example

The following example shows a store function that uses a **CollectionType** element to specify that the function returns a collection of rows (as specified in the **RowType** element).


``` xml
   <Function Name="GetProducts" IsComposable="true" Schema="dbo">
     <ReturnType>
       <CollectionType>
         <RowType>
           <Property Name="ProductID" Type="int" Nullable="false" />
           <Property Name="CategoryID" Type="bigint" Nullable="false" />
           <Property Name="ProductName" Type="nvarchar" MaxLength="40" Nullable="false" />
           <Property Name="UnitPrice" Type="money" />
           <Property Name="Discontinued" Type="bit" />
         </RowType>
       </CollectionType>
     </ReturnType>
   </Function>
```
 

 

## Schema Element (SSDL)

The **Schema** element in store schema definition language (SSDL) is the root element of a storage model definition. It contains definitions for the objects, functions, and containers that make up a storage model.

The **Schema** element may contain zero or more of the following child elements:

-   Association
-   EntityType
-   EntityContainer
-   Function

The **Schema** element uses the **Namespace** attribute to define the namespace for the entity type and association objects in a storage model. Within a namespace, no two objects can have the same name.

A storage model namespace is different from the XML namespace of the **Schema** element. A storage model namespace (as defined by the **Namespace** attribute) is a logical container for entity types and association types. The XML namespace (indicated by the **xmlns** attribute) of a **Schema** element is the default namespace for child elements and attributes of the **Schema** element. XML namespaces of the form http://schemas.microsoft.com/ado/YYYY/MM/edm/ssdl (where YYYY and MM represent a year and month respectively) are reserved for SSDL. Custom elements and attributes cannot be in namespaces that have this form.

### Applicable Attributes

The table below describes the attributes can be applied to the **Schema** element.

| Attribute Name            | Is Required | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
|:--------------------------|:------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Namespace**             | Yes         | The namespace of the storage model. The value of the **Namespace** attribute is used to form the fully qualified name of a type. For example, if an **EntityType** named *Customer* is in the ExampleModel.Store namespace, then the fully qualified name of the **EntityType** is ExampleModel.Store.Customer. <br/> The following strings cannot be used as the value for the **Namespace** attribute: **System**, **Transient**, or **Edm**. The value for the **Namespace** attribute cannot be the same as the value for the **Namespace** attribute in the CSDL Schema element. |
| **Alias**                 | No          | An identifier used in place of the namespace name. For example, if an **EntityType** named *Customer* is in the ExampleModel.Store namespace and the value of the **Alias** attribute is *StorageModel*, then you can use StorageModel.Customer as the fully qualified name of the **EntityType.**                                                                                                                                                                                                                                                                                    |
| **Provider**              | Yes         | The data provider.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| **ProviderManifestToken** | Yes         | A token that indicates to the provider which provider manifest to return. No format for the token is defined. Values for the token are defined by the provider. For information about SQL Server provider manifest tokens, see SqlClient for Entity Framework.                                                                                                                                                                                                                                                                                                                        |

 

### Example

The following example shows a **Schema** element that contains an **EntityContainer** element, two **EntityType** elements, and one **Association** element.

``` xml
 <Schema Namespace="ExampleModel.Store"
       Alias="Self" Provider="System.Data.SqlClient"
       ProviderManifestToken="2008"
       xmlns="http://schemas.microsoft.com/ado/2009/11/edm/ssdl">
   <EntityContainer Name="ExampleModelStoreContainer">
     <EntitySet Name="Customers"
                EntityType="ExampleModel.Store.Customers"
                Schema="dbo" />
     <EntitySet Name="Orders"
                EntityType="ExampleModel.Store.Orders"
                Schema="dbo" />
     <AssociationSet Name="FK_CustomerOrders"
                     Association="ExampleModel.Store.FK_CustomerOrders">
       <End Role="Customers" EntitySet="Customers" />
       <End Role="Orders" EntitySet="Orders" />
     </AssociationSet>
   </EntityContainer>
   <EntityType Name="Customers">
     <Documentation>
       <Summary>Summary here.</Summary>
       <LongDescription>Long description here.</LongDescription>
     </Documentation>
     <Key>
       <PropertyRef Name="CustomerId" />
     </Key>
     <Property Name="CustomerId" Type="int" Nullable="false" />
     <Property Name="Name" Type="nvarchar(max)" Nullable="false" />
   </EntityType>
   <EntityType Name="Orders" xmlns:c="http://CustomNamespace">
     <Key>
       <PropertyRef Name="OrderId" />
     </Key>
     <Property Name="OrderId" Type="int" Nullable="false"
               c:CustomAttribute="someValue"/>
     <Property Name="ProductId" Type="int" Nullable="false" />
     <Property Name="Quantity" Type="int" Nullable="false" />
     <Property Name="CustomerId" Type="int" Nullable="false" />
     <c:CustomElement>
       Custom data here.
     </c:CustomElement>
   </EntityType>
   <Association Name="FK_CustomerOrders">
     <End Role="Customers"
          Type="ExampleModel.Store.Customers" Multiplicity="1">
       <OnDelete Action="Cascade" />
     </End>
     <End Role="Orders"
          Type="ExampleModel.Store.Orders" Multiplicity="*" />
     <ReferentialConstraint>
       <Principal Role="Customers">
         <PropertyRef Name="CustomerId" />
       </Principal>
       <Dependent Role="Orders">
         <PropertyRef Name="CustomerId" />
       </Dependent>
     </ReferentialConstraint>
   </Association>
   <Function Name="UpdateOrderQuantity"
             Aggregate="false"
             BuiltIn="false"
             NiladicFunction="false"
             IsComposable="false"
             ParameterTypeSemantics="AllowImplicitConversion"
             Schema="dbo">
     <Parameter Name="orderId" Type="int" Mode="In" />
     <Parameter Name="newQuantity" Type="int" Mode="In" />
   </Function>
   <Function Name="UpdateProductInOrder" IsComposable="false">
     <CommandText>
       UPDATE Orders
       SET ProductId = @productId
       WHERE OrderId = @orderId;
     </CommandText>
     <Parameter Name="productId"
                Mode="In"
                Type="int"/>
     <Parameter Name="orderId"
                Mode="In"
                Type="int"/>
   </Function>
 </Schema>
```
 

 

## Annotation Attributes

Annotation attributes in store schema definition language (SSDL) are custom XML attributes in the storage model that provide extra metadata about the elements in the storage model. In addition to having valid XML structure, the following constraints apply to annotation attributes:

-   Annotation attributes must not be in any XML namespace that is reserved for SSDL.
-   The fully-qualified names of any two annotation attributes must not be the same.

More than one annotation attribute may be applied to a given SSDL element. Metadata contained in annotation elements can be accessed at runtime by using classes in the System.Data.Metadata.Edm namespace.

### Example

The following example shows an EntityType element that has an annotation attribute applied to the **OrderId** property. The example also show an annotation element added to the **EntityType** element.

``` xml
 <EntityType Name="Orders" xmlns:c="http://CustomNamespace">
   <Key>
     <PropertyRef Name="OrderId" />
   </Key>
   <Property Name="OrderId" Type="int" Nullable="false"
             c:CustomAttribute="someValue"/>
   <Property Name="ProductId" Type="int" Nullable="false" />
   <Property Name="Quantity" Type="int" Nullable="false" />
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <c:CustomElement>
     Custom data here.
   </c:CustomElement>
 </EntityType>
```
 

 

## Annotation Elements (SSDL)

Annotation elements in store schema definition language (SSDL) are custom XML elements in the storage model that provide extra metadata about the storage model. In addition to having valid XML structure, the following constraints apply to annotation elements:

-   Annotation elements must not be in any XML namespace that is reserved for SSDL.
-   The fully-qualified names of any two annotation elements must not be the same.
-   Annotation elements must appear after all other child elements of a given SSDL element.

More than one annotation element may be a child of a given SSDL element. Starting with the .NET Framework version 4, metadata contained in annotation elements can be accessed at runtime by using classes in the System.Data.Metadata.Edm namespace.

### Example

The following example shows an EntityType element that has an annotation element (**CustomElement**). The example also shows an annotation attribute applied to the **OrderId** property.

``` xml
 <EntityType Name="Orders" xmlns:c="http://CustomNamespace">
   <Key>
     <PropertyRef Name="OrderId" />
   </Key>
   <Property Name="OrderId" Type="int" Nullable="false"
             c:CustomAttribute="someValue"/>
   <Property Name="ProductId" Type="int" Nullable="false" />
   <Property Name="Quantity" Type="int" Nullable="false" />
   <Property Name="CustomerId" Type="int" Nullable="false" />
   <c:CustomElement>
     Custom data here.
   </c:CustomElement>
 </EntityType>
```
 

 

## Facets (SSDL)

Facets in store schema definition language (SSDL) represent constraints on column types that are specified in Property elements. Facets are implemented as XML attributes on **Property** elements.

The following table describes the facets that are supported in SSDL:

| Facet           | Description                                                                                                                                                                                                                                                 |
|:----------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Collation**   | Specifies the collating sequence (or sorting sequence) to be used when performing comparison and ordering operations on values of the property.                                                                                                             |
| **FixedLength** | Specifies whether the length of the column value can vary.                                                                                                                                                                                                  |
| **MaxLength**   | Specifies the maximum length of the column value.                                                                                                                                                                                                           |
| **Precision**   | For properties of type **Decimal**, specifies the number of digits a property value can have. For properties of type **Time**, **DateTime**, and **DateTimeOffset**, specifies the number of digits for the fractional part of seconds of the column value. |
| **Scale**       | Specifies the number of digits to the right of the decimal point for the column value.                                                                                                                                                                      |
| **Unicode**     | Indicates whether the column value is stored as Unicode.                                                                                                                                                                                                    |
