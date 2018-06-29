---
title: "Code First Insert, Update, and Delete Stored Procedures - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 9a7ae7f9-4072-4843-877d-506dd7eef576
caps.latest.revision: 3
---
# Code First Insert, Update, and Delete Stored Procedures (EF6 onwards)
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

By default, Code First will configure all entities to perform insert, update and delete commands using direct table access. Starting in EF6 you can configure your Code First model to use stored procedures for some or all entities in your model.  

## Basic Entity Mapping  

You can opt into using stored procedures for insert, update and delete using the Fluent API.  

``` csharp
modelBuilder
  .Entity<Blog>()
  .MapToStoredProcedures();
```  

Doing this will cause Code First to use some conventions to build the expected shape of the stored procedures in the database.  

- Three stored procedures named **\<type_name\>_Insert**, **\<type_name\>_Update** and **\<type_name\>_Delete** (e.g. Blog_Insert, Blog_Update and Blog_Delete).  
- Parameter names correspond to the property names.  
    > [!NOTE]
> If you use HasColumnName() or the Column attribute to rename the column for a given property then this name is used for parameters instead of the property name.  
- **The insert stored procedure** will have a parameter for every property, except for those marked as store generated (identity or computed). The stored procedure should return a result set with a column for each store generated property.  
- **The update stored procedure** will have a parameter for every property, except for those marked with a store generated pattern of 'Computed'. Some concurrency tokens require a parameter for the original value, see the *Concurrency Tokens* section below for details. The stored procedure should return a result set with a column for each computed property.  
- **The delete stored procedure** should have a parameter for the key value of the entity (or multiple parameters if the entity has a composite key). Additionally, the delete procedure should also have parameters for any independent association foreign keys on the target table (relationships that do not have corresponding foreign key properties declared in the entity). Some concurrency tokens require a parameter for the original value, see the *Concurrency Tokens* section below for details.  

Using the following class as an example:  

``` csharp
public class Blog  
{  
  public int BlogId { get; set; }  
  public string Name { get; set; }  
  public string Url { get; set; }  
}
```  

The default stored procedures would be:  

``` SQL
CREATE PROCEDURE [dbo].[Blog_Insert]  
  @Name nvarchar(max),  
  @Url nvarchar(max)  
AS  
BEGIN
  INSERT INTO [dbo].[Blogs] ([Name], [Url])
  VALUES (@Name, @Url)

  SELECT SCOPE_IDENTITY() AS BlogId
END
CREATE PROCEDURE [dbo].[Blog_Update]  
  @BlogId int,  
  @Name nvarchar(max),  
  @Url nvarchar(max)  
AS  
  UPDATE [dbo].[Blogs]
  SET [Name] = @Name, [Url] = @Url     
  WHERE BlogId = @BlogId;
CREATE PROCEDURE [dbo].[Blog_Delete]  
  @BlogId int  
AS  
  DELETE FROM [dbo].[Blogs]
  WHERE BlogId = @BlogId
```  

### Overriding the Defaults  

You can override part or all of what was configured by default.  

You can change the name of one or more stored procedures. This example renames the update stored procedure only.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.HasName("modify_blog")));
```  

This example renames all three stored procedures.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.HasName("modify_blog"))  
     .Delete(d => d.HasName("delete_blog"))  
     .Insert(i => i.HasName("insert_blog")));
```  

In these examples the calls are chained together, but you can also use lambda block syntax.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    {  
      s.Update(u => u.HasName("modify_blog"));  
      s.Delete(d => d.HasName("delete_blog"));  
      s.Insert(i => i.HasName("insert_blog"));  
    });
```  

This example renames the parameter for the BlogId property on the update stored procedure.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.Parameter(b => b.BlogId, "blog_id")));
```  

These calls are all chainable and composable. Here is an example that renames all three stored procedures and their parameters.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.HasName("modify_blog")  
                   .Parameter(b => b.BlogId, "blog_id")  
                   .Parameter(b => b.Name, "blog_name")  
                   .Parameter(b => b.Url, "blog_url"))  
     .Delete(d => d.HasName("delete_blog")  
                   .Parameter(b => b.BlogId, "blog_id"))  
     .Insert(i => i.HasName("insert_blog")  
                   .Parameter(b => b.Name, "blog_name")  
                   .Parameter(b => b.Url, "blog_url")));
```  

You can also change the name of the columns in the result set that contains database generated values.  

``` csharp
modelBuilder
  .Entity<Blog>()
  .MapToStoredProcedures(s =>
    s.Insert(i => i.Result(b => b.BlogId, "generated_blog_identity")));
```

``` SQL
CREATE PROCEDURE [dbo].[Blog_Insert]  
  @Name nvarchar(max),  
  @Url nvarchar(max)  
AS  
BEGIN
  INSERT INTO [dbo].[Blogs] ([Name], [Url])
  VALUES (@Name, @Url)

  SELECT SCOPE_IDENTITY() AS generated_blog_id
END
```  

## Relationships Without a Foreign Key in the Class (Independent Associations)  

When a foreign key property is included in the class definition, the corresponding parameter can be renamed in the same way as any other property. When a relationship exists without a foreign key property in the class, the default parameter name is **\<navigation_property_name\>_\<primary_key_name\>**.  

For example, the following class definitions would result in a Blog_BlogId parameter being expected in the stored procedures to insert and update Posts.  

``` csharp
public class Blog  
{  
  public int BlogId { get; set; }  
  public string Name { get; set; }  
  public string Url { get; set; }

  public List<Post> Posts { get; set; }  
}  

public class Post  
{  
  public int PostId { get; set; }  
  public string Title { get; set; }  
  public string Content { get; set; }  

  public Blog Blog { get; set; }  
}
```  

### Overriding the Defaults  

You can change parameters for foreign keys that are not included in the class by supplying the path to the primary key property to the Parameter method.  

``` csharp
modelBuilder
  .Entity<Post>()  
  .MapToStoredProcedures(s =>  
    s.Insert(i => i.Parameter(p => p.Blog.BlogId, "blog_id")));
```  

If you don’t have a navigation property on the dependent entity (i.e no Post.Blog property) then you can use the Association method to identify the other end of the relationship and then configure the parameters that correspond to each of the key property(s).  

``` csharp
modelBuilder
  .Entity<Post>()  
  .MapToStoredProcedures(s =>  
    s.Insert(i => i.Navigation<Blog>(  
      b => b.Posts,  
      c => c.Parameter(b => b.BlogId, "blog_id"))));
```  

## Concurrency Tokens  

Update and delete stored procedures may also need to deal with concurrency:  

- If the entity contains concurrency tokens, the stored procedure can optionally have an output parameter that returns the number of rows updated/deleted (rows affected). Such a parameter must be configured using the RowsAffectedParameter method.  
By default EF uses the return value from ExecuteNonQuery to determine how many rows were affected. Specifying a rows affected output parameter is useful if you perform any logic in your sproc that would result in the return value of ExecuteNonQuery being incorrect (from EF's perspective) at the end of execution.  
- For each concurrency token there will be a parameter named **\<property_name\>_Original** (i.e. Timestamp_Original). This will be passed the original value of this property – the value when queried from the database.  
    - Concurrency tokens that are computed by the database – such as timestamps – will only have an original value parameter.  
    - Non-computed properties that are set as concurrency tokens will also have a parameter for the new value in the update procedure. This uses the naming conventions already discussed for new values. An example of such a token would be using a Blog's URL as a concurrency token, the new value is required because this can be updated to a new value by your code (unlike a Timestamp token which is only updated by the database).  

This is an example class and update stored procedure with a timestamp concurrency token.  

``` csharp
public class Blog  
{  
  public int BlogId { get; set; }  
  public string Name { get; set; }  
  public string Url { get; set; }  
  [Timestamp]
  public byte[] Timestamp { get; set; }
}
```

``` SQL
CREATE PROCEDURE [dbo].[Blog_Update]  
  @BlogId int,  
  @Name nvarchar(max),  
  @Url nvarchar(max),
  @Timestamp_Original rowversion  
AS  
  UPDATE [dbo].[Blogs]
  SET [Name] = @Name, [Url] = @Url     
  WHERE BlogId = @BlogId AND [Timestamp] = @Timestamp_Original
```  

Here is an example class and update stored procedure with non-computed concurrency token.  

``` csharp
public class Blog  
{  
  public int BlogId { get; set; }  
  public string Name { get; set; }  
  [ConcurrencyCheck]
  public string Url { get; set; }  
}
```

``` SQL
CREATE PROCEDURE [dbo].[Blog_Update]  
  @BlogId int,  
  @Name nvarchar(max),  
  @Url nvarchar(max),
  @Url_Original nvarchar(max),
AS  
  UPDATE [dbo].[Blogs]
  SET [Name] = @Name, [Url] = @Url     
  WHERE BlogId = @BlogId AND [Url] = @Url_Original
```  

### Overriding the Defaults  

You can optionally introduce a rows affected parameter.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.RowsAffectedParameter("rows_affected")));
```  

For database computed concurrency tokens – where only the original value is passed – you can just use the standard parameter renaming mechanism to rename the parameter for the original value.  

``` csharp
modelBuilder  
  .Entity<Blog>()  
  .MapToStoredProcedures(s =>  
    s.Update(u => u.Parameter(b => b.Timestamp, "blog_timestamp")));
```  

For non-computed concurrency tokens – where both the original and new value are passed – you can use an overload of Parameter that allows you to supply a name for each parameter.  

``` csharp
modelBuilder
 .Entity<Blog>()
 .MapToStoredProcedures(s => s.Update(u => u.Parameter(b => b.Url, "blog_url", "blog_original_url")));
```  

## Many to Many Relationships  

We’ll use the following classes as an example in this section.  

``` csharp
public class Post  
{  
  public int PostId { get; set; }  
  public string Title { get; set; }  
  public string Content { get; set; }  

  public List<Tag> Tags { get; set; }  
}  

public class Tag  
{  
  public int TagId { get; set; }  
  public string TagName { get; set; }  

  public List<Post> Posts { get; set; }  
}
```  

Many to many relationships can be mapped to stored procedures with the following syntax.  

``` csharp
modelBuilder  
  .Entity<Post>()  
  .HasMany(p => p.Tags)  
  .WithMany(t => t.Posts)  
  .MapToStoredProcedures();
```  

If no other configuration is supplied then the following stored procedure shape is used by default.  

- Two stored procedures named **\<type_one\>\<type_two\>_Insert** and **\<type_one\>\<type_two\>_Delete** (i.e. PostTag_Insert and PostTag_Delete).  
- The parameters will be the key value(s) for each type. The name of each parameter being **\<type_name\>_\<property_name\>** (i.e. Post_PostId and Tag_TagId).

Here are example insert and update stored procedures.  

``` SQL
CREATE PROCEDURE [dbo].[PostTag_Insert]  
  @Post_PostId int,  
  @Tag_TagId int  
AS  
  INSERT INTO [dbo].[Post_Tags] (Post_PostId, Tag_TagId)   
  VALUES (@Post_PostId, @Tag_TagId)
CREATE PROCEDURE [dbo].[PostTag_Delete]  
  @Post_PostId int,  
  @Tag_TagId int  
AS  
  DELETE FROM [dbo].[Post_Tags]    
  WHERE Post_PostId = @Post_PostId AND Tag_TagId = @Tag_TagId
```  

### Overriding the Defaults  

The procedure and parameter names can be configured in a similar way to entity stored procedures.  

``` csharp
modelBuilder  
  .Entity<Post>()  
  .HasMany(p => p.Tags)  
  .WithMany(t => t.Posts)  
  .MapToStoredProcedures(s =>  
    s.Insert(i => i.HasName("add_post_tag")  
                   .LeftKeyParameter(p => p.PostId, "post_id")  
                   .RightKeyParameter(t => t.TagId, "tag_id"))  
     .Delete(d => d.HasName("remove_post_tag")  
                   .LeftKeyParameter(p => p.PostId, "post_id")  
                   .RightKeyParameter(t => t.TagId, "tag_id")));
```  
