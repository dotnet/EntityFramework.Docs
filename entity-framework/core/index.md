---
title: Overview - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: bc2a2676-bc46-493f-bf49-e3cc97994d57
uid: core/index
---

# Entity Framework Core

Entity Framework (EF) Core is a lightweight, extensible, [open source](https://github.com/aspnet/EntityFrameworkCore) and cross-platform version of the popular Entity Framework data access technology.

EF Core can serve as an object-relational mapper (O/RM), enabling .NET developers to work with a database using .NET objects, and eliminating the need for most of the data-access code they usually need to write.

EF Core supports many database engines, see [Database Providers](providers/index.md) for details.

## The Model

With EF Core, data access is performed using a model. A model is made up of entity classes and a context object that represents a session with the database, allowing you to query and save data. See [Creating a Model](modeling/index.md) to learn more.

You can generate a model from an existing database, hand code a model to match your database, or use [EF Migrations](managing-schemas/migrations/index.md) to create a database from your model, and then evolve it as your model changes over time.

``` csharp
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Intro
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
```

```vb
Imports Microsoft.EntityFrameworkCore
Imports System.Collections.Generic

Namespace Intro
    Public Class BloggingContext
        Inherits DbContext
        
        Public Property Blogs As DbSet(Of Blog)
        Public Property Posts As DbSet(Of Post)
    
        Protected Overrides Sub OnConfiguring(ByVal optionsBuilder As DbContextOptionsBuilder)
            optionsBuilder.UseSqlServer("Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True")
        End Sub
    End Class

    Public Class Blog
        Public Property BlogId As Integer
        Public Property Url As String
        Public Property Rating As Integer
        Public Property Posts As List(Of Post)
    End Class

    Public Class Post
        Public Property PostId As Integer
        Public Property Title As String
        Public Property Content As String

        Public Propery BlogId As Integer
        Public Property Blog As Blog
    End Class
End Namespace
```
## Querying

Instances of your entity classes are retrieved from the database using Language Integrated Query (LINQ). See [Querying Data](querying/index.md) to learn more.

``` csharp
using (var db = new BloggingContext())
{
    var blogs = db.Blogs
        .Where(b => b.Rating > 3)
        .OrderBy(b => b.Url)
        .ToList();
}
```

```vb
Using db As New BloggingContext()
    Dim blogs = db.Blogs.Where(Function(b) b.Rating > 3).
        OrderBy(Function(b) b.Url).
        ToList()
End Using
```

## Saving Data

Data is created, deleted, and modified in the database using instances of your entity classes. See [Saving Data](saving/index.md) to learn more.

``` csharp
using (var db = new BloggingContext())
{
    var blog = new Blog { Url = "http://sample.com" };
    db.Blogs.Add(blog);
    db.SaveChanges();
}
```

```vb
Using db As New BloggingContext()
    Dim blog = New Blog() With { .Url = "http://sample.com" }
    db.Blogs.Add(blog)
    db.SaveChanges()
End Using
```

## Next steps

For introductory tutorials, see [Getting Started with Entity Framework Core](get-started/index.md).

