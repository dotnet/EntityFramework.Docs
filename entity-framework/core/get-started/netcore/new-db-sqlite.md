---
title: .NET Core Application to New SQLite Database
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 099d179e-dd7b-4755-8f3c-fcde914bf50b
ms.prod: entity-framework
uid: core/get-started/netcore/new-db-sqlite
---
# .NET Core Application to New SQLite Database

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

In this walkthrough, you will build a .NET Core console application that performs basic data access using Entity Framework. You will use migrations to create the database from your model.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/NetCore/ConsoleApp.SQLite) on GitHub.

## Prerequisites

Minimum system requirements
* An operating system that supports .NET Core

* [.NET Core SDK Preview 2](https://www.microsoft.com/net/core)

* A text editor

> [!WARNING]
> **Known Issues**
>
>  * Migrations on SQLite do not support more complex schema changes due to limitations in SQLite itself. See [SQLite Limitations](../../providers/sqlite/limitations.md)

### Install the .NET Core SDK

The .NET Core SDK provides the command-line tool `dotnet` which will be used to build and run our sample application.

See the [.NET Core website](https://www.microsoft.com/net/core) for instructions on installing the SDK on your operating system.

## Create a new project

* Create a new folder `ConsoleApp/` for your project. All files for the project should be in this folder.

 <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


````bash
mkdir ConsoleApp
cd ConsoleApp/
````

* Execute the following .NET Core CLI commands to create a new console application, download dependencies, and run the .NET Core app.

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


````bash
dotnet new
dotnet restore
dotnet run
````

## Install Entity Framework

*  To add EF to your project, modify `project.json` so it matches the following sample.

    <!-- [!code-json[Main](samples/core/GetStarted/NetCore/ConsoleApp.SQLite/project.json?highlight=8,9,10,11,12,25,26,27)] -->


    ````json
    {
      "version": "1.0.0-*",
      "buildOptions": {
        "debugType": "portable",
        "emitEntryPoint": true
      },
      "dependencies": {
        "Microsoft.EntityFrameworkCore.Sqlite": "1.0.0",
        "Microsoft.EntityFrameworkCore.Design": {
          "version": "1.0.0-preview2-final",
          "type": "build"
        }
      },
      "frameworks": {
        "netcoreapp1.0": {
          "dependencies": {
            "Microsoft.NETCore.App": {
              "type": "platform",
              "version": "1.0.0"
            }
          },
          "imports": "dnxcore50"
        }
      },
      "tools": {
        "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final"
      }
    }
    ````

*  Run `dotnet restore` again to install the new packages.

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


    ````bash
    dotnet restore
    ````

    * Verify that Entity Framework is installed by running `dotnet ef --help`.

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


    ````bash
    dotnet ef --help
    ````

    ## Create your model

    With this new project, you are ready to begin using Entity Framework. The next steps will add code to configure and access a SQLite database file.

    * Create a new file called `Model.cs`

     All classes in the following steps will be added to this file.

    <!-- [!code-csharp[Main](samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs)] -->


    ````csharp
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.EntityFrameworkCore;

    namespace ConsoleApp.SQLite
    {
    ````

*  Add a new class to represent the SQLite database.

    We will call this `BloggingContext`. The call to `UseSqlite()` configures EF to point to a *.db file.

    <!-- [!code-csharp[Main](samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs?highlight=1,8)] -->


    ````csharp
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./blog.db");
        }
    }
    ````

* Add classes to represent tables.

    Note that we will be using foreign keys to associate many posts to one blog.

    <!-- [!code-csharp[Main](samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs)] -->


    ````csharp
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

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
    ````

* To make sure the files are correct, you can compile the project on the command line by running `dotnet build`

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


    ````bash
    dotnet build
    ````

## Create your database

We can now use Entity Framework command line tools to create and manage the schema of the database.

* Create the first migration.

    Execute the command below to generate your first migration. This will find our context and models, and generate a migration for us in a folder named `Migrations/`

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


    ````bash
    dotnet ef migrations add MyFirstMigration
    ````

* Apply the migrations.

    You can now begin using the existing migration to create the database file and creates the tables.

     <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->


    ````bash
    dotnet ef database update
    ````

    This should create a new file `blog.db` in the output path. This SQLite file should now contain two empty tables.

> [!NOTE]
> When using relative paths with SQLite, the path will be relative to the application's main assembly. In this sample, the main binary is `bin/Debug/netcoreapp1.0/ConsoleApp.dll`, so the SQLite database will be in `bin/Debug/netcoreapp1.0/blog.db`

## Use your model

Now that we have configured our model and created the database schema, we can use BloggingContext to create, update, and delete objects.

<!-- [!code-csharp[Main](samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Program.cs)] -->
````csharp
using System;

namespace ConsoleApp.SQLite
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new BloggingContext())
            {
                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(" - {0}", blog.Url);
                }
            }
        }
    }
}
````

## Start your app

Run the application from the command line.

   <!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````bash
dotnet run
````

After adding the new post, you can verify the data has been added by inspecting the SQLite database file, `bin/Debug/netcoreapp1.0/blog.db`.
