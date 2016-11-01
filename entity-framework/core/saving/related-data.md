---
title: Related Data
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 07b6680f-ffcf-412c-9857-f997486b386c
ms.prod: entity-framework
uid: core/saving/related-data
---
# Related Data

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

In addition to isolated entities, you can also make use of the relationships defined in your model.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Saving/Saving/RelatedData/) on GitHub.

## Adding a graph of new entities

If you create several new related entities, adding one of them to the context will cause the others to be added too.

In the following example, the blog and three related posts are all inserted into the database. The posts are found and added, because they are reachable via the `Blog.Posts` navigation property.

<!-- [!code-csharp[Main](samples/core/Saving/Saving/RelatedData/Sample.cs)] -->
````csharp
        using (var context = new BloggingContext())
        {
            var blog = new Blog
            {
                Url = "http://blogs.msdn.com/dotnet",
                Posts = new List<Post>
                {
                    new Post { Title = "Intro to C#" },
                    new Post { Title = "Intro to VB.NET" },
                    new Post { Title = "Intro to F#" }
                }
            };

            context.Blogs.Add(blog);
            context.SaveChanges();
        }
````

## Adding a related entity

If you reference a new entity from the navigation property of an entity that is already tracked by the context, the entity will be discovered and inserted into the database.

In the following example, the `post` entity is inserted because it is added to the `Posts` property of the `blog` entity which was fetched from the database.

<!-- [!code-csharp[Main](samples/core/Saving/Saving/RelatedData/Sample.cs)] -->
````csharp
        using (var context = new BloggingContext())
        {
            var blog = context.Blogs.First();
            var post = new Post { Title = "Intro to EF Core" };

            blog.Posts.Add(post);
            context.SaveChanges();
        }
````

## Changing relationships

If you change the navigation property of an entity, the corresponding changes will be made to the foreign key column in the database.

In the following example, the `post` entity is updated to belong to the new `blog` entity because its `Blog` navigation property is set to point to `blog`. Note that `blog` will also be inserted into the database because it is a new entity that is referenced by the navigation property of an entity that is already tracked by the context (`post`).

<!-- [!code-csharp[Main](samples/core/Saving/Saving/RelatedData/Sample.cs)] -->
````csharp
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://blogs.msdn.com/visualstudio" };
            var post = context.Posts.First();

            blog.Posts.Add(post);
            context.SaveChanges();
        }
````

## Removing relationships

You can remove a relationship by setting a reference navigation to `null`, or removing the related entity from a collection navigation.

If a cascade delete is configured, the child/dependent entity will be deleted from the database, see [Cascade Delete](cascade-delete.md) for more information. If no cascade delete is configured, the foreign key column in the database will be set to null (if the column does not accept nulls, an exception will be thrown).

In the following example, a cascade delete is configured on the relationship between `Blog` and `Post`, so the `post` entity is deleted from the database.

<!-- [!code-csharp[Main](samples/core/Saving/Saving/RelatedData/Sample.cs)] -->
````csharp
        using (var context = new BloggingContext())
        {
            var blog = context.Blogs.Include(b => b.Posts).First();
            var post = blog.Posts.First();

            blog.Posts.Remove(post);
            context.SaveChanges();
        }
````
