---
title: "Entity Framework Code First Conventions - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: bc644573-c2b2-4ed7-8745-3c37c41058ad
caps.latest.revision: 4
---
# Entity Framework Code First Conventions
Code First enables you to describe a model by using C# or Visual Basic .NET classes. The basic shape of the model is detected by using conventions. Conventions are sets of rules that are used to automatically configure a conceptual model based on class definitions when working with Code First. The conventions are defined in the System.Data.Entity.ModelConfiguration.Conventions namespace.  
  
You can further configure your model by using data annotations or the fluent API. Precedence is given to configuration through the fluent API followed by data annotations and then conventions. For more information see [Data Annotations](../ef6/entity-framework-code-first-data-annotations.md), [Fluent API - Relationships](../ef6/entity-framework-fluent-api-relationships.md), [Fluent API - Types & Properties](../ef6/entity-framework-fluent-api-configuring-and-mapping-properties-and-types.md) and [Fluent API with VB.NET](../ef6/entity-framework-fluent-api-with-vb-net.md).  
  
A detailed list of Code First conventions is available in the [API Documentation](https://msdn.microsoft.com/library/system.data.entity.modelconfiguration.conventions.aspx). This topic provides an overview of the conventions used by Code First.  
  
## Type Discovery  
  
When using Code First development you usually begin by writing .NET Framework classes that define your conceptual (domain) model. In addition to defining the classes, you also need to let **DbContext** know which types you want to include in the model. To do this, you define a context class that derives from **DbContext** and exposes **DbSet** properties for the types that you want to be part of the model. Code First will include these types and also will pull in any referenced types, even if the referenced types are defined in a different assembly.  
  
If your types participate in an inheritance hierarchy, it is enough to define a **DbSet** property for the base class, and the derived types will be automatically included, if they are in the same assembly as the base class.  
  
In the following example, there is only one **DbSet** property defined on the **SchoolEntities** class (**Departments**). Code First uses this property to discover and pull in any referenced types.  
  
```  
public class SchoolEntities : DbContext 
{ 
    public DbSet<Department> Departments { get; set; } 
} 
 
public class Department 
{ 
    // Primary key 
    public int DepartmentID { get; set; } 
    public string Name { get; set; } 
 
    // Navigation property 
    public virtual ICollection<Course> Courses { get; set; } 
} 
 
public class Course 
{ 
    // Primary key 
    public int CourseID { get; set; } 
 
    public string Title { get; set; } 
    public int Credits { get; set; } 
 
    // Foreign key 
    public int DepartmentID { get; set; } 
 
    // Navigation properties 
    public virtual Department Department { get; set; } 
} 
     
public partial class OnlineCourse : Course 
{ 
    public string URL { get; set; } 
} 
 
public partial class OnsiteCourse : Course 
{ 
    public string Location { get; set; } 
    public string Days { get; set; } 
    public System.DateTime Time { get; set; } 
}
```  
  
If you want to exclude a type from the model, use the **NotMapped** attribute or the **DbModelBuilder.Ignore** fluent API.  
  
```  
modelBuilder.Ignore<Department>();
```  
  
## Primary Key Convention  
  
Code First infers that a property is a primary key if a property on a class is named “ID” (not case sensitive), or the class name followed by "ID". If the type of the primary key property is numeric or GUID it will be configured as an identity column.  
  
```  
public class Department 
{ 
    // Primary key 
    public int DepartmentID { get; set; } 
 
    . . .  
 
}
```  
  
## Relationship Convention  
  
In the Entity Framework, navigation properties provide a way to navigate a relationship between two entity types. Every object can have a navigation property for every relationship in which it participates. Navigation properties allow you to navigate and manage relationships in both directions, returning either a reference object (if the multiplicity is either one or zero-or-one) or a collection (if the multiplicity is many). Code First infers relationships based on the navigation properties defined on your types.  
  
In addition to navigation properties, we recommend that you include foreign key properties on the types that represent dependent objects. Any property with the same data type as the principal primary key property and with a name that follows one of the following formats represents a foreign key for the relationship: '\<navigation property name\>\<principal primary key property name\>', '\<principal class name\>\<primary key property name\>', or '\<principal primary key property name\>'. If multiple matches are found then precedence is given in the order listed above. Foreign key detection is not case sensitive. When a foreign key property is detected, Code First infers the multiplicity of the relationship based on the nullability of the foreign key. If the property is nullable then the relationship is registered as optional; otherwise the relationship is registered as required.  
  
If a foreign key on the dependent entity is not nullable, then Code First sets cascade delete on the relationship. If a foreign key on the dependent entity is nullable, Code First does not set cascade delete on the relationship, and when the principal is deleted the foreign key will be set to null. The multiplicity and cascade delete behavior detected by convention can be overridden by using the fluent API.  
  
In the following example the navigation properties and a foreign key are used to define the relationship between the Department and Course classes.  
  
```  
public class Department 
{ 
    // Primary key 
    public int DepartmentID { get; set; } 
    public string Name { get; set; } 
 
    // Navigation property 
    public virtual ICollection<Course> Courses { get; set; } 
} 
 
public class Course 
{ 
    // Primary key 
    public int CourseID { get; set; } 
 
    public string Title { get; set; } 
    public int Credits { get; set; } 
 
    // Foreign key 
    public int DepartmentID { get; set; } 
 
    // Navigation properties 
    public virtual Department Department { get; set; } 
}
```  
  
> **Note**: If you have multiple relationships between the same types (for example, suppose you define the **Person** and **Book** classes, where the **Person** class contains the **ReviewedBooks** and **AuthoredBooks** navigation properties and the **Book** class contains the **Author** and **Reviewer** navigation properties) you need to manually configure the relationships by using Data Annotations or the fluent API. For more information, see [Data Annotations - Relationships](../ef6/entity-framework-code-first-data-annotations.md) and [Fluent API - Relationships](../ef6/entity-framework-fluent-api-relationships.md).  
  
## Complex Types Convention  
  
When Code First discovers a class definition where a primary key cannot be inferred, and no primary key is registered through data annotations or the fluent API, then the type is automatically registered as a complex type. Complex type detection also requires that the type does not have properties that reference entity types and is not referenced from a collection property on another type. Given the following class definitions Code First would infer that **Details** is a complex type because it has no primary key.  
  
```  
public partial class OnsiteCourse : Course 
{ 
    public OnsiteCourse() 
    { 
        Details = new Details(); 
    } 
 
    public Details Details { get; set; } 
} 
 
public class Details 
{ 
    public System.DateTime Time { get; set; } 
    public string Location { get; set; } 
    public string Days { get; set; } 
}
```  
  
## Connection String Convention  
  
To learn about the conventions that DbContext uses to discover the connection to use see [Connections and Models](../ef6/entity-framework-connections-and-models.md).  
  
## Removing Conventions  
  
You can remove any of the conventions defined in the System.Data.Entity.ModelConfiguration.Conventions namespace. The following example removes **PluralizingTableNameConvention**.  
  
```  
public class SchoolEntities : DbContext 
{ 
     . . . 
 
    protected override void OnModelCreating(DbModelBuilder modelBuilder) 
    { 
        // Configure Code First to ignore PluralizingTableName convention 
        // If you keep this convention, the generated tables  
        // will have pluralized names. 
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
    } 
}
```  
  
## Custom Conventions  
  
Custom conventions are supported in EF6 onwards. For more information see [Custom Code First Conventions](../ef6/entity-framework-custom-code-first-conventions-ef6-onwards.md).