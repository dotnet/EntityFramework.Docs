---
title: Stored Procedures with Multiple Result Sets - EF6
description: Stored Procedures with Multiple Result Sets in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/modeling/designer/advanced/multiple-result-sets
---
# Stored Procedures with Multiple Result Sets
Sometimes when using stored procedures you will need to return more than one result set. This scenario is commonly used to reduce the number of database round trips required to compose a single screen. Prior to EF5, Entity Framework would allow the stored procedure to be called but would only return the first result set to the calling code.

This article will show you two ways that you can use to access more than one result set from a stored procedure in Entity Framework. One that uses just code and works with both Code first and the EF Designer and one that only works with the EF Designer. The tooling and API support for this should improve in future versions of Entity Framework.

## Model

The examples in this article use a basic Blog and Posts model where a blog has many posts and a post belongs to a single blog. We will use a stored procedure in the database that returns all blogs and posts, something like this:

``` SQL
    CREATE PROCEDURE [dbo].[GetAllBlogsAndPosts]
    AS
        SELECT * FROM dbo.Blogs
        SELECT * FROM dbo.Posts
```

## Accessing Multiple Result Sets with Code

We can execute use code to issue a raw SQL command to execute our stored procedure. The advantage of this approach is that it works with both Code first and the EF Designer.

In order to get multiple result sets working we need to drop to the ObjectContext API by using the IObjectContextAdapter interface.

Once we have an ObjectContext then we can use the Translate method to translate the results of our stored procedure into entities that can be tracked and used in EF as normal. The following code sample demonstrates this in action.

``` csharp
    using (var db = new BloggingContext())
    {
        // If using Code First we need to make sure the model is built before we open the connection
        // This isn't required for models created with the EF Designer
        db.Database.Initialize(force: false);

        // Create a SQL command to execute the sproc
        var cmd = db.Database.Connection.CreateCommand();
        cmd.CommandText = "[dbo].[GetAllBlogsAndPosts]";

        try
        {

            db.Database.Connection.Open();
            // Run the sproc
            var reader = cmd.ExecuteReader();

            // Read Blogs from the first result set
            var blogs = ((IObjectContextAdapter)db)
                .ObjectContext
                .Translate<Blog>(reader, "Blogs", MergeOption.AppendOnly);   


            foreach (var item in blogs)
            {
                Console.WriteLine(item.Name);
            }        

            // Move to second result set and read Posts
            reader.NextResult();
            var posts = ((IObjectContextAdapter)db)
                .ObjectContext
                .Translate<Post>(reader, "Posts", MergeOption.AppendOnly);


            foreach (var item in posts)
            {
                Console.WriteLine(item.Title);
            }
        }
        finally
        {
            db.Database.Connection.Close();
        }
    }
```

The Translate method accepts the reader that we received when we executed the procedure, an EntitySet name, and a MergeOption. The EntitySet name will be the same as the DbSet property on your derived context. The MergeOption enum controls how results are handled if the same entity already exists in memory.

Here we iterate through the collection of blogs before we call NextResult, this is important given the above code because the first result set must be consumed before moving to the next result set.

Once the two translate methods are called then the Blog and Post entities are tracked by EF the same way as any other entity and so can be modified or deleted and saved as normal.

>[!NOTE]
> EF does not take any mapping into account when it creates entities using the Translate method. It will simply match column names in the result set with property names on your classes.

>[!NOTE]
> That if you have lazy loading enabled, accessing the posts property on one of the blog entities then EF will connect to the database to lazily load all posts, even though we have already loaded them all. This is because EF cannot know whether or not you have loaded all posts or if there are more in the database. If you want to avoid this then you will need to disable lazy loading.

## Multiple Result Sets with Configured in EDMX

>[!NOTE]
> You must target .NET Framework 4.5 to be able to configure multiple result sets in EDMX. If you are targeting .NET 4.0 you can use the code-based method shown in the previous section.

If you are using the EF Designer, you can also modify your model so that it knows about the different result sets that will be returned. One thing to know before hand is that the tooling is not multiple result set aware, so you will need to manually edit the edmx file. Editing the edmx file like this will work but it will also break the validation of the model in VS. So if you validate your model you will always get errors.

-   In order to do this you need to add the stored procedure to your model as you would for a single result set query.
-   Once you have this then you need to right click on your model and select **Open With..** then **Xml**

    ![Open As](~/ef6/media/openas.png)

Once you have the model opened as XML then you need to do the following steps:

-   Find the complex type and function import in your model:

``` xml
    <!-- CSDL content -->
    <edmx:ConceptualModels>

    ...

      <FunctionImport Name="GetAllBlogsAndPosts" ReturnType="Collection(BlogModel.GetAllBlogsAndPosts_Result)" />

    ...

      <ComplexType Name="GetAllBlogsAndPosts_Result">
        <Property Type="Int32" Name="BlogId" Nullable="false" />
        <Property Type="String" Name="Name" Nullable="false" MaxLength="255" />
        <Property Type="String" Name="Description" Nullable="true" />
      </ComplexType>

    ...

    </edmx:ConceptualModels>
```

 

-   Remove the complex type
-   Update the function import so that it maps to your entities, in our case it will look like the following:

``` xml
    <FunctionImport Name="GetAllBlogsAndPosts">
      <ReturnType EntitySet="Blogs" Type="Collection(BlogModel.Blog)" />
      <ReturnType EntitySet="Posts" Type="Collection(BlogModel.Post)" />
    </FunctionImport>
```

This tells the model that the stored procedure will return two collections, one of blog entries and one of post entries.

-   Find the function mapping element:

``` xml
    <!-- C-S mapping content -->
    <edmx:Mappings>

    ...

      <FunctionImportMapping FunctionImportName="GetAllBlogsAndPosts" FunctionName="BlogModel.Store.GetAllBlogsAndPosts">
        <ResultMapping>
          <ComplexTypeMapping TypeName="BlogModel.GetAllBlogsAndPosts_Result">
            <ScalarProperty Name="BlogId" ColumnName="BlogId" />
            <ScalarProperty Name="Name" ColumnName="Name" />
            <ScalarProperty Name="Description" ColumnName="Description" />
          </ComplexTypeMapping>
        </ResultMapping>
      </FunctionImportMapping>

    ...

    </edmx:Mappings>
```

-   Replace the result mapping with one for each entity being returned, such as the following:

``` xml
    <ResultMapping>
      <EntityTypeMapping TypeName ="BlogModel.Blog">
        <ScalarProperty Name="BlogId" ColumnName="BlogId" />
        <ScalarProperty Name="Name" ColumnName="Name" />
        <ScalarProperty Name="Description" ColumnName="Description" />
      </EntityTypeMapping>
    </ResultMapping>
    <ResultMapping>
      <EntityTypeMapping TypeName="BlogModel.Post">
        <ScalarProperty Name="BlogId" ColumnName="BlogId" />
        <ScalarProperty Name="PostId" ColumnName="PostId"/>
        <ScalarProperty Name="Title" ColumnName="Title" />
        <ScalarProperty Name="Text" ColumnName="Text" />
      </EntityTypeMapping>
    </ResultMapping>
```

It is also possible to map the result sets to complex types, such as the one created by default. To do this you create a new complex type, instead of removing them, and use the complex types everywhere that you had used the entity names in the examples above.

Once these mappings have been changed then you can save the model and execute the following code to use the stored procedure:

``` csharp
    using (var db = new BlogEntities())
    {
        var results = db.GetAllBlogsAndPosts();

        foreach (var result in results)
        {
            Console.WriteLine("Blog: " + result.Name);
        }

        var posts = results.GetNextResult<Post>();

        foreach (var result in posts)
        {
            Console.WriteLine("Post: " + result.Title);
        }

        Console.ReadLine();
    }
```

>[!NOTE]
> If you manually edit the edmx file for your model it will be overwritten if you ever regenerate the model from the database.

## Summary

Here we have shown two different methods of accessing multiple result sets using Entity Framework. Both of them are equally valid depending on your situation and preferences and you should choose the one that seems best for your circumstances. It is planned that the support for multiple result sets will be improved in future versions of Entity Framework and that performing the steps in this document will no longer be necessary.
