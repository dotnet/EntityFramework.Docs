---
title: ASP.NET Core - Existing Database | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016
ms.assetid: 2bc68bea-ff77-4860-bf0b-cf00db6712a0
ms.technology: entity-framework-core
uid: core/get-started/aspnetcore/existing-db
---

# ASP.NET Core - Existing Database

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

In this walkthrough, you will build an ASP.NET Core MVC application that performs basic data access using Entity Framework.  You will use reverse engineering to create an Entity Framework model based on an existing database.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb) on GitHub.

## Prerequisites

The following prerequisites are needed to complete this walkthrough:

* [Visual Studio 2017 RC3](https://www.visualstudio.com/downloads/)
* [Blogging database](#blogging-database)

### Blogging database

This tutorial uses a **Blogging** database on your LocalDb instance as the existing database.

> [!NOTE]
> If you have already created the **Blogging** database as part of another tutorial, you can skip these steps.

* Open Visual Studio
* **Tools -> Connect to Database...**
* Select **Microsoft SQL Server** and click **Continue**
* Enter **(localdb)\mssqllocaldb** as the **Server Name**
* Enter **master** as the **Database Name** and click **OK**
* The master database is now displayed under **Data Connections** in **Server Explorer**
* Right-click on the database in **Server Explorer** and select **New Query**
* Copy the script, listed below, into the query editor
* Right-click on the query editor and select **Execute**

[!code-sql[Main](../_shared/create-blogging-database-script.sql)]

## Create a new project

* Open Visual Studio 2017
* **File -> New -> Project...**
* From the left menu select **Installed -> Templates -> Visual C# -> Web**
* Select the **ASP.NET Core Web Application (.NET Core)** project template
* Enter **EFGetStarted.AspNetCore.ExistingDb** as the name and click **OK**
* Wait for the **New ASP.NET Core Web Application** dialog to appear
* Under **ASP.NET Core Templates 1.1** select the **Web Application**
* Ensure that **Authentication** is set to **No Authentication**
* Click **OK**

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* **Tools -> NuGet Package Manager -> Package Manager Console**
* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

We will be using some Entity Framework commands to create a model from the database. So we will install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools -Version 1.1.0-msbuild3-final`
* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer.Design`

## Reverse engineer your model

Now it's time to create the EF model based on your existing database.

* **Tools –> NuGet Package Manager –> Package Manager Console**
* Run the following command to create a model from the existing database. If you receive an error stating `The term 'Scaffold-DbContext' is not recognized as the name of a cmdlet`, then close and reopen Visual Studio.

```
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

The reverse engineer process created entity classes (`Blog.cs` & `Post.cs`) and a derived context (`BloggingContext.cs`) based on the schema of the existing database.

 The entity classes are simple C# objects that represent the data you will be querying and saving.

 [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Models/Blog.cs)]

 The context represents a session with the database and allows you to query and save instances of the entity classes.

<!-- Static code listing, rather than a linked file, because the walkthrough modifies the context file heavily -->
 ```c#
public partial class BloggingContext : DbContext
{
    public virtual DbSet<Blog> Blog { get; set; }
    public virtual DbSet<Post> Post { get; set; }

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
}
```

## Register your context with dependency injection

The concept of dependency injection is central to ASP.NET Core. Services (such as `BloggingContext`) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the [Dependency Injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html) article on the ASP.NET site.

### Remove inline context configuration

In ASP.NET Core, configuration is generally performed in **Startup.cs**. To conform to this pattern, we will move configuration of the database provider to **Startup.cs**.

* Open `Models\BloggingContext.cs`
* Delete the `OnConfiguring(...)` method

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");
}
```

* Add the following constructor, which will allow configuration to be passed into the context by dependency injection

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Models/BloggingContext.cs#Constructor)]

### Register and configure your context in Startup.cs

In order for our MVC controllers to make use of `BloggingContext` we are going to register it as a service.

* Open **Startup.cs**
* Add the following `using` statements at the start of the file

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Startup.cs#AddedUsings)]

Now we can use the `AddDbContext(...)` method to register it as a service.
* Locate the `ConfigureServices(...)` method
* Add the following code to register the context as a service

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.ExistingDb/Startup.cs?name=ConfigureServices&highlight=7-8)]

> [!NOTE]
> In a real application you would typically put the connection string in a configuration file. For the sake of simplicity, we are defining it in code. For more information, see [Connection Strings](../../miscellaneous/connection-strings.md).

## Create a controller

Next, we'll enable scaffolding in our project.

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add -> Controller...**
* Select **Full Dependencies** and click **Add**
* You can ignore the instructions in the `ScaffoldingReadMe.txt` file that opens

Now that scaffolding is enabled, we can scaffold a controller for the `Blog` entity.

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add -> Controller...**
* Select **MVC Controller with views, using Entity Framework** and click **Ok**
* Set **Model class** to **Blog** and **Data context class** to **BloggingContext**
* Click **Add**

## Run the application

You can now run the application to see it in action.

* **Debug -> Start Without Debugging**
* The application will build and open in a web browser
* Navigate to `/Blogs`
* Click **Create New**
* Enter a **Url** for the new blog and click **Create**

![image](_static/create.png)

![image](_static/index-existing-db.png)
