---
title: Custom Code First Conventions - EF6
description: Custom Code First Conventions in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/code-first/conventions/custom
---
# Custom Code First Conventions
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.

When using Code First your model is calculated from your classes using a set of conventions. The default [Code First Conventions](xref:ef6/modeling/code-first/conventions/built-in) determine things like which property becomes the primary key of an entity, the name of the table an entity maps to, and what precision and scale a decimal column has by default.

Sometimes these default conventions are not ideal for your model, and you have to work around them by configuring many individual entities using Data Annotations or the Fluent API. Custom Code First Conventions let you define your own conventions that provide configuration defaults for your model. In this walkthrough, we will explore the different types of custom conventions and how to create each of them.


## Model-Based Conventions

This page covers the DbModelBuilder API for custom conventions. This API should be sufficient for authoring most custom conventions. However, there is also the ability to author model-based conventions - conventions that manipulate the final model once it is created - to handle advanced scenarios. For more information, see [Model-Based Conventions](xref:ef6/modeling/code-first/conventions/model).

 

## Our Model

Let's start by defining a simple model that we can use with our conventions. Add the following classes to your project.

``` csharp
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class ProductContext : DbContext
    {
        static ProductContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ProductContext>());
        }

        public DbSet<Product> Products { get; set; }
    }

    public class Product
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public ProductCategory Category { get; set; }
    }

    public class ProductCategory
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
```

 

## Introducing Custom Conventions

Let’s write a convention that configures any property named Key to be the primary key for its entity type.

Conventions are enabled on the model builder, which can be accessed by overriding OnModelCreating in the context. Update the ProductContext class as follows:

``` csharp
    public class ProductContext : DbContext
    {
        static ProductContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ProductContext>());
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties()
                        .Where(p => p.Name == "Key")
                        .Configure(p => p.IsKey());
        }
    }
```

Now, any property in our model named Key will be configured as the primary key of whatever entity its part of.

We could also make our conventions more specific by filtering on the type of property that we are going to configure:

``` csharp
    modelBuilder.Properties<int>()
                .Where(p => p.Name == "Key")
                .Configure(p => p.IsKey());
```

This will configure all properties called Key to be the primary key of their entity, but only if they are an integer.

An interesting feature of the IsKey method is that it is additive. Which means that if you call IsKey on multiple properties and they will all become part of a composite key. The one caveat for this is that when you specify multiple properties for a key you must also specify an order for those properties. You can do this by calling the HasColumnOrder method like below:

``` csharp
    modelBuilder.Properties<int>()
                .Where(x => x.Name == "Key")
                .Configure(x => x.IsKey().HasColumnOrder(1));

    modelBuilder.Properties()
                .Where(x => x.Name == "Name")
                .Configure(x => x.IsKey().HasColumnOrder(2));
```

This code will configure the types in our model to have a composite key consisting of the int Key column and the string Name column. If we view the model in the designer it would look like this:

![composite Key](~/ef6/media/compositekey.png)

Another example of property conventions is to configure all DateTime properties in my model to map to the datetime2 type in SQL Server instead of datetime. You can achieve this with the following:

``` csharp
    modelBuilder.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));
```

 

## Convention Classes

Another way of defining conventions is to use a Convention Class to encapsulate your convention. When using a Convention Class then you create a type that inherits from the Convention class in the System.Data.Entity.ModelConfiguration.Conventions namespace.

We can create a Convention Class with the datetime2 convention that we showed earlier by doing the following:

``` csharp
    public class DateTime2Convention : Convention
    {
        public DateTime2Convention()
        {
            this.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));        
        }
    }
```

To tell EF to use this convention you add it to the Conventions collection in OnModelCreating, which if you’ve been following along with the walkthrough will look like this:

``` csharp
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Properties<int>()
                    .Where(p => p.Name.EndsWith("Key"))
                    .Configure(p => p.IsKey());

        modelBuilder.Conventions.Add(new DateTime2Convention());
    }
```

As you can see we add an instance of our convention to the conventions collection. Inheriting from Convention provides a convenient way of grouping and sharing conventions across teams or projects. You could, for example, have a class library with a common set of conventions that all of your organizations projects use.

 

## Custom Attributes

Another great use of conventions is to enable new attributes to be used when configuring a model. To illustrate this, let’s create an attribute that we can use to mark String properties as non-Unicode.

``` csharp
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NonUnicode : Attribute
    {
    }
```

Now, let’s create a convention to apply this attribute to our model:

``` csharp
    modelBuilder.Properties()
                .Where(x => x.GetCustomAttributes(false).OfType<NonUnicode>().Any())
                .Configure(c => c.IsUnicode(false));
```

With this convention we can add the NonUnicode attribute to any of our string properties, which means the column in the database will be stored as varchar instead of nvarchar.

One thing to note about this convention is that if you put the NonUnicode attribute on anything other than a string property then it will throw an exception. It does this because you cannot configure IsUnicode on any type other than a string. If this happens, then you can make your convention more specific, so that it filters out anything that isn’t a string.

While the above convention works for defining custom attributes there is another API that can be much easier to use, especially when you want to use properties from the attribute class.

For this example we are going to update our attribute and change it to an IsUnicode attribute, so it looks like this:

``` csharp
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class IsUnicode : Attribute
    {
        public bool Unicode { get; set; }

        public IsUnicode(bool isUnicode)
        {
            Unicode = isUnicode;
        }
    }
```

Once we have this, we can set a bool on our attribute to tell the convention whether or not a property should be Unicode. We could do this in the convention we have already by accessing the ClrProperty of the configuration class like this:

``` csharp
    modelBuilder.Properties()
                .Where(x => x.GetCustomAttributes(false).OfType<IsUnicode>().Any())
                .Configure(c => c.IsUnicode(c.ClrPropertyInfo.GetCustomAttribute<IsUnicode>().Unicode));
```

This is easy enough, but there is a more succinct way of achieving this by using the Having method of the conventions API. The Having method has a parameter of type Func&lt;PropertyInfo, T&gt; which accepts the PropertyInfo the same as the Where method, but is expected to return an object. If the returned object is null then the property will not be configured, which means you can filter out properties with it just like Where, but it is different in that it will also capture the returned object and pass it to the Configure method. This works like the following:

``` csharp
    modelBuilder.Properties()
                .Having(x => x.GetCustomAttributes(false).OfType<IsUnicode>().FirstOrDefault())
                .Configure((config, att) => config.IsUnicode(att.Unicode));
```

Custom attributes are not the only reason to use the Having method, it is useful anywhere that you need to reason about something that you are filtering on when configuring your types or properties.

 

## Configuring Types

So far all of our conventions have been for properties, but there is another area of the conventions API for configuring the types in your model. The experience is similar to the conventions we have seen so far, but the options inside configure will be at the entity instead of property level.

One of the things that Type level conventions can be really useful for is changing the table naming convention, either to map to an existing schema that differs from the EF default or to create a new database with a different naming convention. To do this we first need a method that can accept the TypeInfo for a type in our model and return what the table name for that type should be:

``` csharp
    private string GetTableName(Type type)
    {
        var result = Regex.Replace(type.Name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

        return result.ToLower();
    }
```

This method takes a type and returns a string that uses lower case with underscores instead of CamelCase. In our model this means that the ProductCategory class will be mapped to a table called product\_category instead of ProductCategories.

Once we have that method we can call it in a convention like this:

``` csharp
    modelBuilder.Types()
                .Configure(c => c.ToTable(GetTableName(c.ClrType)));
```

This convention configures every type in our model to map to the table name that is returned from our GetTableName method. This convention is the equivalent to calling the ToTable method for each entity in the model using the Fluent API.

One thing to note about this is that when you call ToTable EF will take the string that you provide as the exact table name, without any of the pluralization that it would normally do when determining table names. This is why the table name from our convention is product\_category instead of product\_categories. We can resolve that in our convention by making a call to the pluralization service ourselves.

In the following code we will use the [Dependency Resolution](xref:ef6/fundamentals/configuring/dependency-resolution) feature added in EF6 to retrieve the pluralization service that EF would have used and pluralize our table name.

``` csharp
    private string GetTableName(Type type)
    {
        var pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();

        var result = pluralizationService.Pluralize(type.Name);

        result = Regex.Replace(result, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

        return result.ToLower();
    }
```

> [!NOTE]
> The generic version of GetService is an extension method in the System.Data.Entity.Infrastructure.DependencyResolution namespace, you will need to add a using statement to your context in order to use it.

### ToTable and Inheritance

Another important aspect of ToTable is that if you explicitly map a type to a given table, then you can alter the mapping strategy that EF will use. If you call ToTable for every type in an inheritance hierarchy, passing the type name as the name of the table like we did above, then you will change the default Table-Per-Hierarchy (TPH) mapping strategy to Table-Per-Type (TPT). The best way to describe this is whith a concrete example:

``` csharp
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Manager : Employee
    {
        public string SectionManaged { get; set; }
    }
```

By default both employee and manager are mapped to the same table (Employees) in the database. The table will contain both employees and managers with a discriminator column that will tell you what type of instance is stored in each row. This is TPH mapping as there is a single table for the hierarchy. However, if you call ToTable on both classe then each type will instead be mapped to its own table, also known as TPT since each type has its own table.

``` csharp
    modelBuilder.Types()
                .Configure(c=>c.ToTable(c.ClrType.Name));
```

The code above will map to a table structure that looks like the following:

![tpt Example](~/ef6/media/tptexample.jpg)

You can avoid this, and maintain the default TPH mapping, in a couple ways:

1.  Call ToTable with the same table name for each type in the hierarchy.
2.  Call ToTable only on the base class of the hierarchy, in our example that would be employee.

 

## Execution Order

Conventions operate in a last wins manner, the same as the Fluent API. What this means is that if you write two conventions that configure the same option of the same property, then the last one to execute wins. As an example, in the code below the max length of all strings is set to 500 but we then configure all properties called Name in the model to have a max length of 250.

``` csharp
    modelBuilder.Properties<string>()
                .Configure(c => c.HasMaxLength(500));

    modelBuilder.Properties<string>()
                .Where(x => x.Name == "Name")
                .Configure(c => c.HasMaxLength(250));
```

Because the convention to set max length to 250 is after the one that sets all strings to 500, all the properties called Name in our model will have a MaxLength of 250 while any other strings, such as descriptions, would be 500. Using conventions in this way means that you can provide a general convention for types or properties in your model and then overide them for subsets that are different.

The Fluent API and Data Annotations can also be used to override a convention in specific cases. In our example above if we had used the Fluent API to set the max length of a property then we could have put it before or after the convention, because the more specific Fluent API will win over the more general Configuration Convention.

 

## Built-in Conventions

Because custom conventions could be affected by the default Code First conventions, it can be useful to add conventions to run before or after another convention. To do this you can use the AddBefore and AddAfter methods of the Conventions collection on your derived DbContext. The following code would add the convention class we created earlier so that it will run before the built in key discovery convention.

``` csharp
    modelBuilder.Conventions.AddBefore<IdKeyDiscoveryConvention>(new DateTime2Convention());
```

This is going to be of the most use when adding conventions that need to run before or after the built in conventions, a list of the built in conventions can be found here: [System.Data.Entity.ModelConfiguration.Conventions Namespace](https://msdn.microsoft.com/library/system.data.entity.modelconfiguration.conventions.aspx).

You can also remove conventions that you do not want applied to your model. To remove a convention, use the Remove method. Here is an example of removing the PluralizingTableNameConvention.

``` csharp
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
```
