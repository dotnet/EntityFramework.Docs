Including & Excluding Properties
================================

Including a property in the model means that EF has metadata about that property and will attempt to read and write values from/to the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, public properties with a getter and a setter will be included in the model.

Data Annotations
----------------

You can use Data Annotations to exclude a property from the model.

.. includesamplefile:: Modeling/DataAnnotations/Samples/IgnoreProperty.cs
        :language: c#
        :lines: 12-19
        :emphasize-lines: 6
        :linenos:

Fluent API
----------

You can use the Fluent API to exclude a property from the model.

.. includesamplefile:: Modeling/FluentAPI/Samples/IgnoreProperty.cs
        :language: c#
        :lines: 6-23
        :emphasize-lines: 7-8
        :linenos:




Using Data Annotations with navigation property the related object type is automatically removed from model.
 
public class UserProfile : IdentityUser<long>
{
        [NotMapped]  
        public virtual List<UserTool> Tools { get; set; }
}

Using Fleunt API you have to explicitly remove the property and the related object type:
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.Entity<UserProfile>().Ignore(p => p.Tools);
    builder.Ignore<UserTool>();
}


