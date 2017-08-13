---
title: EF Core | Getting Started on UWP - New Database | Microsoft Docs
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: a0ae2f21-1eef-43c6-83ad-92275f9c0727
ms.technology: entity-framework-core

uid: core/get-started/uwp/getting-started
---

# Getting Started with EF Core on Universal Windows Platform (UWP) with a New Database

> [!NOTE]  
> Temporarily this tutorial uses EF Core 1.1. UWP has not been updated yet to support .NET Standard 2.0 which is required for compatibility with EF Core 2.0. Once it is, we will update the tutorial to use the new version. 

In this walkthrough, you will build a Universal Windows Platform (UWP) application that performs basic data access against a local SQLite database using Entity Framework.

> [!WARNING]
> **Avoid using anonymous types in LINQ queries on UWP**. Deploying a UWP application to the app store requires your application to be compiled with .NET Native. Queries with anonymous types have poor performance on .NET Native or may crash the application.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/UWP/UWP.SQLite) on GitHub.

## Prerequisites

The following items are required to complete this walkthrough:
* Windows 10

* [Visual Studio 2017](https://www.visualstudio.com/downloads/)

* The latest version of [Windows 10 Developer Tools](https://dev.windows.com/en-us/downloads)

## Create a new project

* Open Visual Studio

* File > New > Project...

* From the left menu select Templates > Visual C# > Windows Universal

* Select the **Blank App (Universal Windows)** project template

* Give the project a name and click **OK**

## Upgrade Microsoft.NETCore.UniversalWindowsPlatform

Depending on your version of Visual Studio, the template may have generated your project with an old version of .NET Core for UWP. EF Core requires `Microsoft.NETCore.UniversalWindowsPlatform` version **5.2.2** or greater.

* Tools > NuGet Package Manager > Package Manager Console

* Run `Update-Package Microsoft.NETCore.UniversalWindowsPlatform –Version 5.2.2`

> [!TIP]
> If you are using Visual Studio 2017, you can upgrade to the latest version of `Microsoft.NETCore.UniversalWindowsPlatform` and do not need to explicitly target `5.2.2`.

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see [Database Providers](../../providers/index.md).

* Tools > NuGet Package Manager > Package Manager Console

* Run `Install-Package Microsoft.EntityFrameworkCore.Sqlite`

Later in this walkthrough we will also be using some Entity Framework Tools to maintain the database. So we will install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Create your model

Now it's time to define a context and entity classes that make up your model.

* Project > Add Class...

* Enter *Model.cs* as the name and click **OK**

* Replace the contents of the file with the following code

<!-- [!code-csharp[Main](samples/core/GetStarted/UWP/UWP.SQLite/Model.cs)] -->
``` csharp
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFGetStarted.UWP
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

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

## Create your database

Now that you have a model, you can use migrations to create a database for you.

* Tools –> NuGet Package Manager –> Package Manager Console

* Run `Add-Migration MyFirstMigration` to scaffold a migration to create the initial set of tables for your model.

Since we want the database to be created on the device that the app runs on, we will add some code to apply any pending migrations to the local database on application startup. The first time that the app runs, this will take care of creating the local database for us.

* Right-click on **App.xaml** in **Solution Explorer** and select **View Code**

* Add the highlighted using to the start of the file

<!-- [!code-csharp[Main](samples/core/GetStarted/UWP/UWP.SQLite/App.xaml.cs?highlight=1)] -->
``` csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
```

* Add the highlighted code to apply any pending migrations

<!-- [!code-csharp[Main](samples/core/GetStarted/UWP/UWP.SQLite/App.xaml.cs?highlight=6,7,8,9)] -->
``` csharp
public App()
{
    this.InitializeComponent();
    this.Suspending += OnSuspending;

    using (var db = new BloggingContext())
    {
        db.Database.Migrate();
    }
}
```

> [!TIP]
> If you make future changes to your model, you can use the `Add-Migration` command to scaffold a new migration to apply the corresponding changes to the database. Any pending migrations will be applied to the local database on each device when the application starts.
>
>EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

## Use your model

You can now use your model to perform data access.

* Open *MainPage.xaml*

* Add the page load handler and UI content highlighted below

<!-- [!code-csharp[Main](samples/core/GetStarted/UWP/UWP.SQLite/MainPage.xaml?highlight=9,12,13,14,15,16,17,18,19,20,21,22)] -->
``` xaml
<Page
    x:Class="EFGetStarted.UWP.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:EFGetStarted.UWP"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Loaded="Page_Loaded">

    <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <StackPanel>
            <TextBox Name="NewBlogUrl"></TextBox>
            <Button Click="Add_Click">Add</Button>
            <ListView Name="Blogs">
                <ListView.ItemTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding Url}" />
                    </DataTemplate>
                </ListView.ItemTemplate>
            </ListView>
        </StackPanel>
    </Grid>
</Page>
```

Now we'll add code to wire up the UI with the database

* Right-click **MainPage.xaml** in **Solution Explorer** and select **View Code**

* Add the highlighted code from the following listing

<!-- [!code-csharp[Main](samples/core/GetStarted/UWP/UWP.SQLite/MainPage.xaml.cs?highlight=8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26)] -->
``` csharp
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        using (var db = new BloggingContext())
        {
            Blogs.ItemsSource = db.Blogs.ToList();
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        using (var db = new BloggingContext())
        {
            var blog = new Blog { Url = NewBlogUrl.Text };
            db.Blogs.Add(blog);
            db.SaveChanges();

            Blogs.ItemsSource = db.Blogs.ToList();
        }
    }
}
```

You can now run the application to see it in action.

* Debug > Start Without Debugging

* The application will build and launch

* Enter a URL and click the **Add** button

![image](_static/create.png)

![image](_static/list.png)

## Next steps

Tada! You now have a simple UWP app running Entity Framework.

Check out the numerous articles in this documentation to learn more about Entity Framework's features.
