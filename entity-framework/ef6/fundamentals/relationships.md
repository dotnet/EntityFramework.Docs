---
title: Relationships, navigation properties, and foreign keys - EF6
description: Relationships, navigation properties, and foreign keys in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/relationships
---
# Relationships, navigation properties, and foreign keys

This article gives an overview of how Entity Framework manages relationships between entities. It also gives some guidance on how to map and manipulate relationships.

## Relationships in EF

In relational databases, relationships (also called associations) between tables are defined through foreign keys. A foreign key (FK) is a column or combination of columns that is used to establish and enforce a link between the data in two tables. There are generally three types of relationships: one-to-one, one-to-many, and many-to-many. In a one-to-many relationship, the foreign key is defined on the table that represents the many end of the relationship. The many-to-many relationship involves defining a third table (called a junction or join table), whose primary key is composed of the foreign keys from both related tables. In a one-to-one relationship, the primary key acts additionally as a foreign key and there is no separate foreign key column for either table.

The following image shows two tables that participate in one-to-many relationship. The **Course** table is the dependent table because it contains the **DepartmentID** column that links it to the **Department** table.

![Department and Course tables](~/ef6/media/database2.png)

In Entity Framework, an entity can be related to other entities through an association or relationship. Each relationship contains two ends that describe the entity type and the multiplicity of the type (one, zero-or-one, or many) for the two entities in that relationship. The relationship may be governed by a referential constraint, which describes which end in the relationship is a principal role and which is a dependent role.

Navigation properties provide a way to navigate an association between two entity types. Every object can have a navigation property for every relationship in which it participates. Navigation properties allow you to navigate and manage relationships in both directions, returning either a reference object (if the multiplicity is either one or zero-or-one) or a collection (if the multiplicity is many). You may also choose to have one-way navigation, in which case you define the navigation property on only one of the types that participates in the relationship and not on both.

It is recommended to include properties in the model that map to foreign keys in the database. With foreign key properties included, you can create or change a relationship by modifying the foreign key value on a dependent object. This kind of association is called a foreign key association. Using foreign keys is even more essential when working with disconnected entities. Note, that when working with 1-to-1 or 1-to-0..1 relationships, there is no separate foreign key column, the primary key property acts as the foreign key and is always included in the model.

When foreign key columns are not included in the model, the association information is managed as an independent object. Relationships are tracked through object references instead of foreign key properties. This type of association is called an *independent association*. The most common way to modify an *independent association* is to modify the navigation properties that are generated for each entity that participates in the association.

You can choose to use one or both types of associations in your model. However, if you have a pure many-to-many relationship that is connected by a join table that contains only foreign keys, the EF will use an independent association to manage such many-to-many relationship.   

The following image shows a conceptual model that was created with the Entity Framework Designer. The model contains two entities that participate in one-to-many relationship. Both entities have navigation properties. **Course** is the dependent entity and has the **DepartmentID** foreign key property defined.

![Department and Course tables with navigation properties](~/ef6/media/relationshipefdesigner.png)

The following code snippet shows the same model that was created with Code First.

``` csharp
public class Course
{
  public int CourseID { get; set; }
  public string Title { get; set; }
  public int Credits { get; set; }
  public int DepartmentID { get; set; }
  public virtual Department Department { get; set; }
}

public class Department
{
   public Department()
   {
     this.Courses = new HashSet<Course>();
   }  
   public int DepartmentID { get; set; }
   public string Name { get; set; }
   public decimal Budget { get; set; }
   public DateTime StartDate { get; set; }
   public int? Administrator {get ; set; }
   public virtual ICollection<Course> Courses { get; set; }
}
```

## Configuring or mapping relationships

The rest of this page covers how to access and manipulate data using relationships. For information on setting up relationships in your model, see the following pages.

-   To configure relationships in Code First, see [Data Annotations](xref:ef6/modeling/code-first/data-annotations) and [Fluent API – Relationships](xref:ef6/modeling/code-first/fluent/relationships).
-   To configure relationships using the Entity Framework Designer, see [Relationships with the EF Designer](xref:ef6/modeling/designer/relationships).

## Creating and modifying relationships

In a *foreign key association*, when you change the relationship, the state of a dependent object with an `EntityState.Unchanged` state changes to `EntityState.Modified`. In an independent relationship, changing the relationship does not update the state of the dependent object.

The following examples show how to use the foreign key properties and navigation properties to associate the related objects. With foreign key associations, you can use either method to change, create, or modify relationships. With independent associations, you cannot use the foreign key property.

- By assigning a new value to a foreign key property, as in the following example.  
  ``` csharp
  course.DepartmentID = newCourse.DepartmentID;
  ```

- The following code removes a relationship by setting the foreign key to **null**. Note, that the foreign key property must be nullable.  
  ``` csharp
  course.DepartmentID = null;
  ```

  >[!NOTE]
  > If the reference is in the added state (in this example, the course object), the reference navigation property will not be synchronized with the key values of a new object until SaveChanges is called. Synchronization does not occur because the object context does not contain permanent keys for added objects until they are saved. If you must have new objects fully synchronized as soon as you set the relationship, use one of the following methods.*

- By assigning a new object to a navigation property. The following code creates a relationship between a course and a `department`. If the objects are attached to the context, the `course` is also added to the `department.Courses` collection, and the corresponding foreign key property on the `course` object is set to the key property value of the department.  
  ``` csharp
  course.Department = department;
  ```

- To delete the relationship, set the navigation property to `null`. If you are working with Entity Framework that is based on .NET 4.0, then the related end needs to be loaded before you set it to null. For example:   
  ``` csharp
  context.Entry(course).Reference(c => c.Department).Load();
  course.Department = null;
  ```

  Starting with Entity Framework 5.0, that is based on .NET 4.5, you can set the relationship to null without loading the related end. You can also set the current value to null using the following method.   
  ``` csharp
  context.Entry(course).Reference(c => c.Department).CurrentValue = null;
  ```

- By deleting or adding an object in an entity collection. For example, you can add an object of type `Course` to the `department.Courses` collection. This operation creates a relationship between a particular **course** and a particular `department`. If the objects are attached to the context, the department reference and the foreign key property on the **course** object will be set to the appropriate `department`.  
  ``` csharp
  department.Courses.Add(newCourse);
  ```

- By using the `ChangeRelationshipState` method to change the state of the specified relationship between two entity objects. This method is most commonly used when working with N-Tier applications and an *independent association* (it cannot be used with a foreign key association). Also, to use this method you must drop down to `ObjectContext`, as shown in the example below.  
In the following example, there is a many-to-many relationship between Instructors and Courses. Calling the `ChangeRelationshipState` method and passing the `EntityState.Added` parameter, lets the `SchoolContext` know that a relationship has been added between the two objects:
  ``` csharp

  ((IObjectContextAdapter)context).ObjectContext.
    ObjectStateManager.
    ChangeRelationshipState(course, instructor, c => c.Instructor, EntityState.Added);
  ```

  Note that if you are updating (not just adding) a relationship, you must delete the old relationship after adding the new one:

  ``` csharp
  ((IObjectContextAdapter)context).ObjectContext.
    ObjectStateManager.
    ChangeRelationshipState(course, oldInstructor, c => c.Instructor, EntityState.Deleted);
  ```

## Synchronizing the changes between the foreign keys and navigation properties

When you change the relationship of the objects attached to the context by using one of the methods described above, Entity Framework needs to keep foreign keys, references, and collections in sync. Entity Framework automatically manages this synchronization (also known as relationship fix-up) for the POCO entities with proxies. For more information, see [Working with Proxies](xref:ef6/fundamentals/proxies).

If you are using POCO entities without proxies, you must make sure that the **DetectChanges** method is called to synchronize the related objects in the context. Note that the following APIs automatically trigger a **DetectChanges** call.

-   `DbSet.Add`
-   `DbSet.AddRange`
-   `DbSet.Remove`
-   `DbSet.RemoveRange`
-   `DbSet.Find`
-   `DbSet.Local`
-   `DbContext.SaveChanges`
-   `DbSet.Attach`
-   `DbContext.GetValidationErrors`
-   `DbContext.Entry`
-   `DbChangeTracker.Entries`
-   Executing a LINQ query against a `DbSet`

## Loading related objects

In Entity Framework you commonly use navigation properties to load entities that are related to the returned entity by the defined association. For more information, see [Loading Related Objects](xref:ef6/querying/related-data).

> [!NOTE]
> In a foreign key association, when you load a related end of a dependent object, the related object will be loaded based on the foreign key value of the dependent that is currently in memory:

``` csharp
    // Get the course where currently DepartmentID = 2.
    Course course = context.Courses.First(c => c.DepartmentID == 2);

    // Use DepartmentID foreign key property
    // to change the association.
    course.DepartmentID = 3;

    // Load the related Department where DepartmentID = 3
    context.Entry(course).Reference(c => c.Department).Load();
```

In an independent association, the related end of a dependent object is queried based on the foreign key value that is currently in the database. However, if the relationship was modified, and the reference property on the dependent object points to a different principal object that is loaded in the object context, Entity Framework will try to create a relationship as it is defined on the client.

## Managing concurrency

In both foreign key and independent associations, concurrency checks are based on the entity keys and other entity properties that are defined in the model. When using the EF Designer to create a model, set the `ConcurrencyMode` attribute to **fixed** to specify that the property should be checked for concurrency. When using Code First to define a model, use the `ConcurrencyCheck` annotation on properties that you want to be checked for concurrency. When working with Code First you can also use the `TimeStamp` annotation to specify that the property should be checked for concurrency. You can have only one timestamp property in a given class. Code First maps this property to a non-nullable field in the database.

We recommend that you always use the foreign key association when working with entities that participate in concurrency checking and resolution.

For more information, see [Handling Concurrency Conflicts](xref:ef6/saving/concurrency).

## Working with overlapping Keys

Overlapping keys are composite keys where some properties in the key are also part of another key in the entity. You cannot have an overlapping key in an independent association. To change a foreign key association that includes overlapping keys, we recommend that you modify the foreign key values instead of using the object references.
