---
title: Getting Started on ASP.NET Core - Existing Database - EF Core
author: rowanmiller
ms.date: 08/02/2018
ms.assetid: 2bc68bea-ff77-4860-bf0b-cf00db6712a0
uid: core/get-started/aspnetcore/existing-db
---

# Getting Started with EF Core on ASP.NET Core with an Existing Database

In this tutorial, you build an ASP.NET Core MVC application that performs basic data access using Entity Framework Core. You reverse engineer an existing database to create an Entity Framework model.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb).

## Prerequisites

Install the following software:

* [Visual Studio 2017 15.7](https://www.visualstudio.com/downloads/) with these workloads:
  * **ASP.NET and web development** (under **Web & Cloud**)
  * **.NET Core cross-platform development** (under **Other Toolsets**)
* [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core).

## Create Blogging database

This tutorial uses a **Blogging** database on your LocalDb instance as the existing database. If you have already created the **Blogging** database as part of another tutorial, skip these steps.

* Open Visual Studio
* **Tools -> Connect to Database...**
* Select **Microsoft SQL Server** and click **Continue**
* Enter **(localdb)\mssqllocaldb** as the **Server Name**
* Enter **master** as the **Database Name** and click **OK**
* The master database is now displayed under **Data Connections** in **Server Explorer**
* Right-click on the database in **Server Explorer** and select **New Query**
* Copy the script listed below into the query editor
* Right-click on the query editor and select **Execute**

[!code-sql[Main](../_shared/create-blogging-database-script.sql)]

## Create a new project

* Open Visual Studio 2017
* **File > New > Project...**
* From the left menu select **Installed > Visual C# > Web**
* Select the **ASP.NET Core Web Application** project template
* Enter **EFGetStarted.AspNetCore.ExistingDb** as the name (it has to match exactly the namespace later used in the code) and click **OK** 
* Wait for the **New ASP.NET Core Web Application** dialog to appear
* Make sure that the target framework dropdown is set to **.NET Core**, and the version dropdown is set to **ASP.NET Core 2.1**
* Select the **Web Application (Model-View-Controller)** template
* Ensure that **Authentication** is set to **No Authentication**
* Click **OK**

## Install Entity Framework Core

To install EF Core, you install the package for the EF Core database provider(s) you want to target. For a list of available providers see [Database Providers](../../providers/index.md). 

For this tutorial, you don't have to install a provider package because the tutorial uses SQL Server. The SQL Server provider package is included in the [Microsoft.AspnetCore.App metapackage](https://docs.microsoft.com/aspnet/core/fundamentals/metapackage-app?view=aspnetcore-2.1).

## Reverse engineer your model

Now it's time to create the EF model based on your existing database.

* **Tools –> NuGet Package Manager –> Package Manager Console**
* Run the following command to create a model from the existing database:

``` powershell
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

If you receive an error stating `The term 'Scaffold-DbContext' is not recognized as the name of a cmdlet`, then close and reopen Visual Studio.

> [!TIP]  
> You can specify which tables you want to generate entities for by adding the `-Tables` argument to the command above. For example, `-Tables Blog,Post`.

The reverse engineer process created entity classes (`Blog.cs` & `Post.cs`) and a derived context (`BloggingContext.cs`) based on the schema of the existing database.

 The entity classes are simple C# objects that represent the data you will be querying and saving. Here are the `Blog` and `Post` entity classes:

 [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Models/Blog.cs)]

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Models/Post.cs)]

> [!TIP]  
> To enable lazy loading, you can make navigation properties `virtual` (Blog.Post and Post.Blog).

 The context represents a session with the database and allows you to query and save instances of the entity classes.

<!-- Static code listing, rather than a linked file, because the tutorial modifies the context file heavily -->
 ``` csharp
public partial class BloggingContext : DbContext
{
    public BloggingContext()
    {
    }

    public BloggingContext(DbContextOptions<BloggingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blog { get; set; }
    public virtual DbSet<Post> Post { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");
        }
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
}
```

## Register your context with dependency injection

The concept of dependency injection is central to ASP.NET Core. Services (such as `BloggingContext`) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the [Dependency Injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html) article on the ASP.NET site.

### Register and configure your context in Startup.cs

To make `BloggingContext` available to MVC controllers, register it as a service.

* Open **Startup.cs**
* Add the following `using` statements at the start of the file

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Startup.cs#AddedUsings)]

Now you can use the `AddDbContext(...)` method to register it as a service.
* Locate the `ConfigureServices(...)` method
* Add the following highlighted code to register the context as a service

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Startup.cs?name=ConfigureServices&highlight=14-15)]

> [!TIP]  
> In a real application you would typically put the connection string in a configuration file or environment variable. For the sake of simplicity, this tutorial has you define it in code. For more information, see [Connection Strings](../../miscellaneous/connection-strings.md).

## Create a controller and views

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add -> Controller...**
* Select **MVC Controller with views, using Entity Framework** and click **Ok**
* Set **Model class** to **Blog** and **Data context class** to **BloggingContext**
* Click **Add**

## Run the application

You can now run the application to see it in action.

* **Debug -> Start Without Debugging**
* The application builds and opens in a web browser
* Navigate to `/Blogs`
* Click **Create New**
* Enter a **Url** for the new blog and click **Create**

  ![Create page](_static/create.png)

  ![Index page](_static/index-existing-db.png)

## Next steps

For more information about how to scaffold a context and entity classes, see the following articles:
* [Reverse Engineering](xref:core/managing-schemas/scaffolding)
* [Entity Framework Core tools reference - .NET CLI](xref:core/miscellaneous/cli/dotnet#dotnet-ef-dbcontext-scaffold)
* [Entity Framework Core tools reference - Package Manager Console](xref:core/miscellaneous/cli/powershell#scaffold-dbcontext)
