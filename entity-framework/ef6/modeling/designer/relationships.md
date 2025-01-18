---
title: Relationships - EF Designer - EF6
description: Relationships - EF Designer in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/designer/relationships
---
# Relationships - EF Designer
> [!NOTE]
> This page provides information about setting up relationships in your model using the EF Designer. For general information about relationships in EF and how to access and manipulate data using relationships, see [Relationships & Navigation Properties](xref:ef6/fundamentals/relationships).

Associations define relationships between entity types in a model. This topic shows how to map associations with the Entity Framework Designer (EF Designer). The following image shows the main windows that are used when working with the EF Designer.

![EF Designer](~/ef6/media/efdesigner.png)

> [!NOTE]
> When you build the conceptual model, warnings about unmapped entities and associations may appear in the Error List. You can ignore these warnings because after you choose to generate the database from the model, the errors will go away.

## Associations Overview

When you design your model using the EF Designer, an .edmx file represents your model. In the .edmx file, an **Association** element defines a relationship between two entity types. An association must specify the entity types that are involved in the relationship and the possible number of entity types at each end of the relationship, which is known as the multiplicity. The multiplicity of an association end can have a value of one (1), zero or one (0..1), or many (\*). This information is specified in two child **End** elements.

At run time, entity type instances at one end of an association can be accessed through navigation properties or foreign keys (if you choose to expose foreign keys in your entities). With foreign keys exposed, the relationship between the entities is managed with a **ReferentialConstraint** element (a child element of the **Association** element). It is recommended that you always expose foreign keys for relationships in your entities.

> [!NOTE]
> In many-to-many (\*:\*) you cannot add foreign keys to the entities. In a \*:\* relationship, the association information is managed with an independent object.

For information about CSDL elements (**ReferentialConstraint**, **Association**, etc.) see the [CSDL specification](xref:ef6/modeling/designer/advanced/edmx/csdl-spec).

## Create and Delete Associations

Creating an association with the EF Designer updates the model content of the .edmx file. After creating an association, you must create the mappings for the association (discussed later in this topic).

> [!NOTE]
> This section assumes that you already added the entities you wish to create an association between to your model.

### To create an association

1.  Right-click an empty area of the design surface, point to **Add New**, and select **Association…**.
2.  Fill in the settings for the association in the **Add Association** dialog.

    ![Add Association](~/ef6/media/addassociation.png)

    > [!NOTE]
    > You can choose to not add navigation properties or foreign key properties to the entities at the ends of the association by clearing the **Navigation Property **and **Add foreign key properties to the &lt;entity type name&gt; Entity **checkboxes. If you add only one navigation property, the association will be traversable in only one direction. If you add no navigation properties, you must choose to add foreign key properties in order to access entities at the ends of the association.
    
3.  Click **OK**.

### To delete an association

To delete an association do one of the following:

-   Right-click the association on the EF Designer surface and select **Delete**.

- OR -

-   Select one or more associations and press the DELETE key.

## Include Foreign Key Properties in Your Entities (Referential Constraints)

It is recommended that you always expose foreign keys for relationships in your entities. Entity Framework uses a referential constraint to identify that a property acts as the foreign key for a relationship.

If you checked the ***Add foreign key properties to the &lt;entity type name&gt; Entity*** checkbox when creating a relationship, this referential constraint was added for you.

When you use the EF Designer to add or edit a referential constraint, the EF Designer adds or modifies a **ReferentialConstraint** element in the CSDL content of the .edmx file.

-   Double-click the association that you want to edit.
    The **Referential Constraint** dialog box appears.
-   From the **Principal** drop-down list, select the principal entity in the referential constraint.
    The entity's key properties are added to the **Principal Key** list in the dialog box.
-   From the **Dependent** drop-down list, select the dependent entity in the referential constraint.
-   For each principal key that has a dependent key, select a corresponding dependent key from the drop-down lists in the **Dependent Key** column.

    ![Ref Constraint](~/ef6/media/refconstraint.png)

-   Click **OK**.

## Create and Edit Association Mappings

You can specify how an association maps to the database in the **Mapping Details** window of the EF Designer.

> [!NOTE]
> You can only map details for the associations that do not have a referential constraint specified. If a referential constraint is specified then a foreign key property is included in the entity and you can use the Mapping Details for the entity to control which column the foreign key maps to.

### Create an association mapping

-   Right-click an association in the design surface and select **Table Mapping**.
    This displays the association mapping in the **Mapping Details** window.
-   Click **Add a Table or View**.
    A drop-down list appears that includes all the tables in the storage model.
-   Select the table to which the association will map.
    The **Mapping Details** window displays both ends of the association and the key properties for the entity type at each **End**.
-   For each key property, click the **Column** field, and select the column to which the property will map.

    ![Mapping Details 4](~/ef6/media/mappingdetails4.png)

### Edit an association mapping

-   Right-click an association in the design surface and select **Table Mapping**.
    This displays the association mapping in the **Mapping Details** window.
-   Click **Maps to &lt;Table Name&gt;**.
    A drop-down list appears that includes all the tables in the storage model.
-   Select the table to which the association will map.
    The **Mapping Details** window displays both ends of the association and the key properties for the entity type at each End.
-   For each key property, click the **Column** field, and select the column to which the property will map.

## Edit and Delete Navigation Properties

Navigation properties are shortcut properties that are used to locate the entities at the ends of an association in a model. Navigation properties can be created when you create an association between two entity types.

#### To edit navigation properties

-   Select a navigation property on the EF Designer surface.
    Information about the navigation property is displayed in the Visual Studio **Properties** window.
-   Change the property settings in the **Properties** window.

#### To delete navigation properties

-   If foreign keys are not exposed on entity types in the conceptual model, deleting a navigation property may make the corresponding association traversable in only one direction or not traversable at all.
-   Right-click a navigation property on the EF Designer surface and select **Delete**.
