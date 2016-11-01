---
title: Console Application to Existing Database (Database First)
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: a29a3d97-b2d8-4d33-9475-40ac67b3b2c6
ms.prod: entity-framework-
uid: core/get-started/full-dotnet/existing-db
---
# Console Application to Existing Database (Database First)

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

In this walkthrough, you will build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework. You will use reverse engineering to create an Entity Framework model based on an existing database.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb) on GitHub.

## Prerequisites

The following prerequisites are needed to complete this walkthrough:

* [Visual Studio 2015 Update 3](https://go.microsoft.com/fwlink/?LinkId=691129)

* [Latest version of NuGet Package Manager](https://visualstudiogallery.msdn.microsoft.com/5d345edc-2e2d-4a9c-b73b-d53956dc458d)

* [Latest version of Windows PowerShell](https://www.microsoft.com/en-us/download/details.aspx?id=40855)

* [Blogging database](#blogging-database)

### Blogging database

This tutorial uses a **Blogging** database on your LocalDb instance as the existing database.

> [!NOTE]
> If you have already created the **Blogging** database as part of another tutorial, you can skip these steps.

* Open Visual Studio

* Tools ‣ Connect to Database...

* Select **Microsoft SQL Server** and click **Continue**

* Enter **(localdb)\mssqllocaldb** as the **Server Name**

* Enter **master** as the **Database Name** and click **OK**

* The master database is now displayed under **Data Connections** in **Server Explorer**

* Right-click on the database in **Server Explorer** and select **New Query**

* Copy the script, listed below, into the query editor

* Right-click on the query editor and select **Execute**

[!code-sql[Main](../_shared/create-blogging-database-script.sql)]

## Create a new project

* Open Visual Studio 2015

* File ‣ New ‣ Project...

* From the left menu select Templates ‣ Visual C# ‣ Windows

* Select the **Console Application** project template

* Ensure you are targeting **.NET Framework 4.5.1** or later

* Give the project a name and click **OK**

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* Tools ‣ NuGet Package Manager ‣ Package Manager Console

* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

To enable reverse engineering from an existing database we need to install a couple of other packages too.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools –Pre`

* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer.Design`

## Reverse engineer your model

Now it's time to create the EF model based on your existing database.

* Tools –> NuGet Package Manager –> Package Manager Console

* Run the following command to create a model from the existing database

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
````

The reverse engineer process created entity classes and a derived context based on the schema of the existing database. The entity classes are simple C# objects that represent the data you will be querying and saving.

<!-- [!code-csharp[Main](samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/Blog.cs)] -->
````csharp
using System;
using System.Collections.Generic;

namespace EFGetStarted.ConsoleApp.ExistingDb
{
    public partial class Blog
    {
        public Blog()
        {
            Post = new HashSet<Post>();
        }

        public int BlogId { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
````

The context represents a session with the database and allows you to query and save instances of the entity classes.

<!-- [!code-csharp[Main](samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/BloggingContext.cs)] -->
````csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFGetStarted.ConsoleApp.ExistingDb
{
    public partial class BloggingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.Url).IsRequired();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(d => d.Blog)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.BlogId);
            });
        }

        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<Post> Post { get; set; }
    }
}
````

## Use your model

You can now use your model to perform data access.

* Open *Program.cs*

* Replace the contents of the file with the following code

<!-- [!code-csharp[Main](samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/Program.cs)] -->
````csharp
using System;

namespace EFGetStarted.ConsoleApp.ExistingDb
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                db.Blog.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blog)
                {
                    Console.WriteLine(" - {0}", blog.Url);
                }
            }
        }
    }
}
````

* Debug ‣ Start Without Debugging

You will see that one blog is saved to the database and then the details of all blogs are printed to the console.

![image](_static/output-existing-db.png)
