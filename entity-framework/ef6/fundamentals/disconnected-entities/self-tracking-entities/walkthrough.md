---
title: Self-Tracking Entities Walkthrough - EF6
description: Self-tracking entities walkthrough for Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/disconnected-entities/self-tracking-entities/walkthrough
---
# Self-Tracking Entities Walkthrough
> [!IMPORTANT]
> We no longer recommend using the self-tracking-entities template. It will only continue to be available to support existing applications. If your application requires working with disconnected graphs of entities, consider other alternatives such as [Trackable Entities](https://trackableentities.github.io/), which is a technology similar to Self-Tracking-Entities that is more actively developed by the community, or writing custom code using the low-level change tracking APIs.

This walkthrough demonstrates the scenario in which a Windows Communication Foundation (WCF) service exposes an operation that returns an entity graph. Next, a client application manipulates that graph and submits the modifications to a service operation that validates and saves the updates to a database using Entity Framework.

Before completing this walkthrough make sure you read the [Self-Tracking Entities](xref:ef6/fundamentals/disconnected-entities/self-tracking-entities/index) page.

This walkthrough completes the following actions:

-   Creates a database to access.
-   Creates a class library that contains the model.
-   Swaps to the Self-Tracking Entity Generator template.
-   Moves the entity classes to a separate project.
-   Creates a WCF service that exposes operations to query and save entities.
-   Creates client applications (Console and WPF) that consume the service.

We'll use Database First in this walkthrough but the same techniques apply equally to Model First.

## Pre-Requisites

To complete this walkthrough you will need a recent version of Visual Studio.

## Create a Database

The database server that is installed with Visual Studio is different depending on the version of Visual Studio you have installed:

-   If you are using Visual Studio 2012 then you'll be creating a LocalDB database.
-   If you are using Visual Studio 2010 you'll be creating a SQL Express database.

Let's go ahead and generate the database.

-   Open Visual Studio
-   **View -&gt; Server Explorer**
-   Right click on **Data Connections -&gt; Add Connection…**
-   If you haven’t connected to a database from Server Explorer before you’ll need to select **Microsoft SQL Server** as the data source
-   Connect to either LocalDB or SQL Express, depending on which one you have installed
-   Enter **STESample** as the database name
-   Select **OK** and you will be asked if you want to create a new database, select **Yes**
-   The new database will now appear in Server Explorer
-   If you are using Visual Studio 2012
    -   Right-click on the database in Server Explorer and select **New Query**
    -   Copy the following SQL into the new query, then right-click on the query and select **Execute**
-   If you are using Visual Studio 2010
    -   Select **Data -&gt; Transact SQL Editor -&gt; New Query Connection...**
    -   Enter **.\\SQLEXPRESS** as the server name and click **OK**
    -   Select the **STESample** database from the drop down at the top of the query editor
    -   Copy the following SQL into the new query, then right-click on the query and select **Execute SQL**

``` SQL
    CREATE TABLE [dbo].[Blogs] (
        [BlogId] INT IDENTITY (1, 1) NOT NULL,
        [Name] NVARCHAR (200) NULL,
        [Url]  NVARCHAR (200) NULL,
        CONSTRAINT [PK_dbo.Blogs] PRIMARY KEY CLUSTERED ([BlogId] ASC)
    );

    CREATE TABLE [dbo].[Posts] (
        [PostId] INT IDENTITY (1, 1) NOT NULL,
        [Title] NVARCHAR (200) NULL,
        [Content] NTEXT NULL,
        [BlogId] INT NOT NULL,
        CONSTRAINT [PK_dbo.Posts] PRIMARY KEY CLUSTERED ([PostId] ASC),
        CONSTRAINT [FK_dbo.Posts_dbo.Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blogs] ([BlogId]) ON DELETE CASCADE
    );

    SET IDENTITY_INSERT [dbo].[Blogs] ON
    INSERT INTO [dbo].[Blogs] ([BlogId], [Name], [Url]) VALUES (1, N'ADO.NET Blog', N'blogs.msdn.com/adonet')
    SET IDENTITY_INSERT [dbo].[Blogs] OFF
    INSERT INTO [dbo].[Posts] ([Title], [Content], [BlogId]) VALUES (N'Intro to EF', N'Interesting stuff...', 1)
    INSERT INTO [dbo].[Posts] ([Title], [Content], [BlogId]) VALUES (N'What is New', N'More interesting stuff...', 1)
```

## Create the Model

First up, we need a project to put the model in.

-   **File -&gt; New -&gt; Project...**
-   Select **Visual C\#** from the left pane and then **Class Library**
-   Enter **STESample** as the name and click **OK**

Now we'll create a simple model in the EF Designer to access our database:

-   **Project -&gt; Add New Item...**
-   Select **Data** from the left pane and then **ADO.NET Entity Data Model**
-   Enter **BloggingModel** as the name and click **OK**
-   Select **Generate from database** and click **Next**
-   Enter the connection information for the database that you created in the previous section
-   Enter **BloggingContext** as the name for the connection string and click **Next**
-   Check the box next to **Tables** and click **Finish**

## Swap to STE Code Generation

Now we need to disable the default code generation and swap to Self-Tracking Entities.

### If you are using Visual Studio 2012

-   Expand **BloggingModel.edmx** in **Solution Explorer** and delete the **BloggingModel.tt** and **BloggingModel.Context.tt**
    *This will disable the default code generation*
-   Right-click an empty area on the EF Designer surface and select **Add Code Generation Item...**
-   Select **Online** from the left pane and search for **STE Generator**
-   Select the **STE Generator for C\#** template, enter **STETemplate** as the name and click **Add**
-   The **STETemplate.tt** and **STETemplate.Context.tt** files are added nested under the BloggingModel.edmx file

### If you are using Visual Studio 2010

-   Right-click an empty area on the EF Designer surface and select **Add Code Generation Item...**
-   Select **Code** from the left pane and then **ADO.NET Self-Tracking Entity Generator**
-   Enter **STETemplate** as the name and click **Add**
-   The **STETemplate.tt** and **STETemplate.Context.tt** files are added directly to your project

## Move Entity Types into Separate Project

To use Self-Tracking Entities our client application needs access to the entity classes generated from our model. Because we don't want to expose the whole model to the client application we're going to move the entity classes into a separate project.

The first step is to stop generating entity classes in the existing project:

-   Right-click on **STETemplate.tt** in **Solution Explorer** and select **Properties**
-   In the **Properties** window clear **TextTemplatingFileGenerator** from the **CustomTool** property
-   Expand **STETemplate.tt** in **Solution Explorer** and delete all files nested under it

Next, we are going to add a new project and generate the entity classes in it

-   **File -&gt; Add -&gt; Project...**
-   Select **Visual C\#** from the left pane and then **Class Library**
-   Enter **STESample.Entities** as the name and click **OK**
-   **Project -&gt; Add Existing Item...**
-   Navigate to the **STESample** project folder
-   Select to view **All Files (\*.\*)**
-   Select the **STETemplate.tt** file
-   Click on the drop down arrow next to the **Add** button and select **Add As Link**

    ![Add Linked Template](~/ef6/media/addlinkedtemplate.png)

We're also going to make sure the entity classes get generated in the same namespace as the context. This just reduces the number of using statements we need to add throughout our application.

-   Right-click on the linked **STETemplate.tt** in **Solution Explorer** and select **Properties**
-   In the **Properties** window set **Custom Tool Namespace** to **STESample**

The code generated by the STE template will need a reference to **System.Runtime.Serialization** in order to compile. This library is needed for the WCF **DataContract** and **DataMember** attributes that are used on the serializable entity types.

-   Right click on the **STESample.Entities** project in **Solution Explorer** and select **Add Reference...**
    -   In Visual Studio 2012 - check the box next to **System.Runtime.Serialization** and click **OK**
    -   In Visual Studio 2010 - select **System.Runtime.Serialization** and click **OK**

Finally, the project with our context in it will need a reference to the entity types.

-   Right click on the **STESample** project in **Solution Explorer** and select **Add Reference...**
    -   In Visual Studio 2012 - select **Solution** from the left pane, check the box next to **STESample.Entities** and click **OK**
    -   In Visual Studio 2010 - select the **Projects** tab, select **STESample.Entities** and click **OK**

>[!NOTE]
> Another option for moving the entity types to a separate project is to move the template file, rather than linking it from its default location. If you do this, you will need to update the **inputFile** variable in the template to provide the relative path to the edmx file (in this example that would be **..\\BloggingModel.edmx**).

## Create a WCF Service

Now it's time to add a WCF Service to expose our data, we'll start by creating the project.

-   **File -&gt; Add -&gt; Project...**
-   Select **Visual C\#** from the left pane and then **WCF Service Application**
-   Enter **STESample.Service** as the name and click **OK**
-   Add a reference to the **System.Data.Entity** assembly
-   Add a reference to the **STESample** and **STESample.Entities** projects

We need to copy the EF connection string to this project so that it is found at runtime.

-   Open the **App.Config** file for the **STESample **project and copy the **connectionStrings** element
-   Paste the **connectionStrings** element as a child element of the **configuration** element of the **Web.Config** file in the **STESample.Service** project

Now it's time to implement the actual service.

-   Open **IService1.cs** and replace the contents with the following code

``` csharp
    using System.Collections.Generic;
    using System.ServiceModel;

    namespace STESample.Service
    {
        [ServiceContract]
        public interface IService1
        {
            [OperationContract]
            List<Blog> GetBlogs();

            [OperationContract]
            void UpdateBlog(Blog blog);
        }
    }
```

-   Open **Service1.svc** and replace the contents with the following code

``` csharp
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    namespace STESample.Service
    {
        public class Service1 : IService1
        {
            /// <summary>
            /// Gets all the Blogs and related Posts.
            /// </summary>
            public List<Blog> GetBlogs()
            {
                using (BloggingContext context = new BloggingContext())
                {
                    return context.Blogs.Include("Posts").ToList();
                }
            }

            /// <summary>
            /// Updates Blog and its related Posts.
            /// </summary>
            public void UpdateBlog(Blog blog)
            {
                using (BloggingContext context = new BloggingContext())
                {
                    try
                    {
                        // TODO: Perform validation on the updated order before applying the changes.

                        // The ApplyChanges method examines the change tracking information
                        // contained in the graph of self-tracking entities to infer the set of operations
                        // that need to be performed to reflect the changes in the database.
                        context.Blogs.ApplyChanges(blog);
                        context.SaveChanges();

                    }
                    catch (UpdateException)
                    {
                        // To avoid propagating exception messages that contain sensitive data to the client tier
                        // calls to ApplyChanges and SaveChanges should be wrapped in exception handling code.
                        throw new InvalidOperationException("Failed to update. Try your request again.");
                    }
                }
            }        
        }
    }
```

## Consume the Service from a Console Application

Let's create a console application that uses our service.

-   **File -&gt; New -&gt; Project...**
-   Select **Visual C\#** from the left pane and then **Console Application**
-   Enter **STESample.ConsoleTest** as the name and click **OK**
-   Add a reference to the **STESample.Entities** project

We need a service reference to our WCF service

-   Right-click the **STESample.ConsoleTest** project in **Solution Explorer** and select **Add Service Reference...**
-   Click **Discover**
-   Enter **BloggingService** as the namespace and click **OK**

Now we can write some code to consume the service.

-   Open **Program.cs** and replace the contents with the following code.

``` csharp
    using STESample.ConsoleTest.BloggingService;
    using System;
    using System.Linq;

    namespace STESample.ConsoleTest
    {
        class Program
        {
            static void Main(string[] args)
            {
                // Print out the data before we change anything
                Console.WriteLine("Initial Data:");
                DisplayBlogsAndPosts();

                // Add a new Blog and some Posts
                AddBlogAndPost();
                Console.WriteLine("After Adding:");
                DisplayBlogsAndPosts();

                // Modify the Blog and one of its Posts
                UpdateBlogAndPost();
                Console.WriteLine("After Update:");
                DisplayBlogsAndPosts();

                // Delete the Blog and its Posts
                DeleteBlogAndPost();
                Console.WriteLine("After Delete:");
                DisplayBlogsAndPosts();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

            static void DisplayBlogsAndPosts()
            {
                using (var service = new Service1Client())
                {
                    // Get all Blogs (and Posts) from the service
                    // and print them to the console
                    var blogs = service.GetBlogs();
                    foreach (var blog in blogs)
                    {
                        Console.WriteLine(blog.Name);
                        foreach (var post in blog.Posts)
                        {
                            Console.WriteLine(" - {0}", post.Title);
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            static void AddBlogAndPost()
            {
                using (var service = new Service1Client())
                {
                    // Create a new Blog with a couple of Posts
                    var newBlog = new Blog
                    {
                        Name = "The New Blog",
                        Posts =
                        {
                            new Post { Title = "Welcome to the new blog"},
                            new Post { Title = "What's new on the new blog"}
                        }
                    };

                    // Save the changes using the service
                    service.UpdateBlog(newBlog);
                }
            }

            static void UpdateBlogAndPost()
            {
                using (var service = new Service1Client())
                {
                    // Get all the Blogs
                    var blogs = service.GetBlogs();

                    // Use LINQ to Objects to find The New Blog
                    var blog = blogs.First(b => b.Name == "The New Blog");

                    // Update the Blogs name
                    blog.Name = "The Not-So-New Blog";

                    // Update one of the related posts
                    blog.Posts.First().Content = "Some interesting content...";

                    // Save the changes using the service
                    service.UpdateBlog(blog);
                }
            }

            static void DeleteBlogAndPost()
            {
                using (var service = new Service1Client())
                {
                    // Get all the Blogs
                    var blogs = service.GetBlogs();

                    // Use LINQ to Objects to find The Not-So-New Blog
                    var blog = blogs.First(b => b.Name == "The Not-So-New Blog");

                    // Mark all related Posts for deletion
                    // We need to call ToList because each Post will be removed from the
                    // Posts collection when we call MarkAsDeleted
                    foreach (var post in blog.Posts.ToList())
                    {
                        post.MarkAsDeleted();
                    }

                    // Mark the Blog for deletion
                    blog.MarkAsDeleted();

                    // Save the changes using the service
                    service.UpdateBlog(blog);
                }
            }
        }
    }
```

You can now run the application to see it in action.

-   Right-click the **STESample.ConsoleTest** project in **Solution Explorer** and select **Debug -&gt; Start new instance**

You'll see the following output when the application executes.

```console
Initial Data:
ADO.NET Blog
- Intro to EF
- What is New

After Adding:
ADO.NET Blog
- Intro to EF
- What is New
The New Blog
- Welcome to the new blog
- What's new on the new blog

After Update:
ADO.NET Blog
- Intro to EF
- What is New
The Not-So-New Blog
- Welcome to the new blog
- What's new on the new blog

After Delete:
ADO.NET Blog
- Intro to EF
- What is New

Press any key to exit...
```

## Consume the Service from a WPF Application

Let's create a WPF application that uses our service.

-   **File -&gt; New -&gt; Project...**
-   Select **Visual C\#** from the left pane and then **WPF Application**
-   Enter **STESample.WPFTest** as the name and click **OK**
-   Add a reference to the **STESample.Entities** project

We need a service reference to our WCF service

-   Right-click the **STESample.WPFTest** project in **Solution Explorer** and select **Add Service Reference...**
-   Click **Discover**
-   Enter **BloggingService** as the namespace and click **OK**

Now we can write some code to consume the service.

-   Open **MainWindow.xaml** and replace the contents with the following code.

``` xaml
    <Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:STESample="clr-namespace:STESample;assembly=STESample.Entities"
        mc:Ignorable="d" x:Class="STESample.WPFTest.MainWindow"
        Title="MainWindow" Height="350" Width="525" Loaded="Window_Loaded">

        <Window.Resources>
            <CollectionViewSource
                x:Key="blogViewSource"
                d:DesignSource="{d:DesignInstance {x:Type STESample:Blog}, CreateList=True}"/>
            <CollectionViewSource
                x:Key="blogPostsViewSource"
                Source="{Binding Posts, Source={StaticResource blogViewSource}}"/>
        </Window.Resources>

        <Grid DataContext="{StaticResource blogViewSource}">
            <DataGrid AutoGenerateColumns="False" EnableRowVirtualization="True"
                      ItemsSource="{Binding}" Margin="10,10,10,179">
                <DataGrid.Columns>
                    <DataGridTextColumn Binding="{Binding BlogId}" Header="Id" Width="Auto" IsReadOnly="True" />
                    <DataGridTextColumn Binding="{Binding Name}" Header="Name" Width="Auto"/>
                    <DataGridTextColumn Binding="{Binding Url}" Header="Url" Width="Auto"/>
                </DataGrid.Columns>
            </DataGrid>
            <DataGrid AutoGenerateColumns="False" EnableRowVirtualization="True"
                      ItemsSource="{Binding Source={StaticResource blogPostsViewSource}}" Margin="10,145,10,38">
                <DataGrid.Columns>
                    <DataGridTextColumn Binding="{Binding PostId}" Header="Id" Width="Auto"  IsReadOnly="True"/>
                    <DataGridTextColumn Binding="{Binding Title}" Header="Title" Width="Auto"/>
                    <DataGridTextColumn Binding="{Binding Content}" Header="Content" Width="Auto"/>
                </DataGrid.Columns>
            </DataGrid>
            <Button Width="68" Height="23" HorizontalAlignment="Right" VerticalAlignment="Bottom"
                    Margin="0,0,10,10" Click="buttonSave_Click">Save</Button>
        </Grid>
    </Window>
```

-   Open the code behind for MainWindow (**MainWindow.xaml.cs**) and replace the contents with the following code

``` csharp
    using STESample.WPFTest.BloggingService;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    namespace STESample.WPFTest
    {
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                using (var service = new Service1Client())
                {
                    // Find the view source for Blogs and populate it with all Blogs (and related Posts)
                    // from the Service. The default editing functionality of WPF will allow the objects
                    // to be manipulated on the screen.
                    var blogsViewSource = (CollectionViewSource)this.FindResource("blogViewSource");
                    blogsViewSource.Source = service.GetBlogs().ToList();
                }
            }

            private void buttonSave_Click(object sender, RoutedEventArgs e)
            {
                using (var service = new Service1Client())
                {
                    // Get the blogs that are bound to the screen
                    var blogsViewSource = (CollectionViewSource)this.FindResource("blogViewSource");
                    var blogs = (List<Blog>)blogsViewSource.Source;

                    // Save all Blogs and related Posts
                    foreach (var blog in blogs)
                    {
                        service.UpdateBlog(blog);
                    }

                    // Re-query for data to get database-generated keys etc.
                    blogsViewSource.Source = service.GetBlogs().ToList();
                }
            }
        }
    }
```

You can now run the application to see it in action.

-   Right-click the **STESample.WPFTest** project in **Solution Explorer** and select **Debug -&gt; Start new instance**
-   You can manipulate the data using the screen and save it via the service using the **Save** button

![WPF Main window](~/ef6/media/wpf.png)
