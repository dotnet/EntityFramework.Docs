---
title: Async query and save - EF6
description: Async query and save in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/async
---
# Async query and save
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.

EF6 introduced support for asynchronous query and save using the [async and await keywords](https://msdn.microsoft.com/library/vstudio/hh191443.aspx) that were introduced in .NET 4.5. While not all applications may benefit from asynchrony, it can be used to improve client responsiveness and server scalability when handling long-running, network or I/O-bound tasks.

## When to really use async

The purpose of this walkthrough is to introduce the async concepts in a way that makes it easy to observe the difference between asynchronous and synchronous program execution. This walkthrough is not intended to illustrate any of the key scenarios where async programming provides benefits.

Async programming is primarily focused on freeing up the current managed thread (thread running .NET code) to do other work while it waits for an operation that does not require any compute time from a managed thread. For example, whilst the database engine is processing a query there is nothing to be done by .NET code.

In client applications (WinForms, WPF, etc.) the current thread can be used to keep the UI responsive while the async operation is performed. In server applications (ASP.NET etc.) the thread can be used to process other incoming requests - this can reduce memory usage and/or increase throughput of the server.

In most applications using async will have no noticeable benefits and even could be detrimental. Use tests, profiling and common sense to measure the impact of async in your particular scenario before committing to it.

Here are some more resources to learn about async:

-   [Brandon Bray’s overview of async/await in .NET 4.5](https://devblogs.microsoft.com/dotnet/async-in-4-5-worth-the-await/)
-   [Asynchronous Programming](https://msdn.microsoft.com/library/hh191443.aspx) pages in the MSDN Library

## Create the model

We’ll be using the [Code First workflow](xref:ef6/modeling/code-first/workflows/new-database) to create our model and generate the database, however the asynchronous functionality will work with all EF models including those created with the EF Designer.

-   Create a Console Application and call it **AsyncDemo**
-   Add the EntityFramework NuGet package
    -   In Solution Explorer, right-click on the **AsyncDemo** project
    -   Select **Manage NuGet Packages…**
    -   In the Manage NuGet Packages dialog, Select the **Online** tab and choose the **EntityFramework** package
    -   Click **Install**
-   Add a **Model.cs** class with the following implementation

``` csharp
    using System.Collections.Generic;
    using System.Data.Entity;

    namespace AsyncDemo
    {
        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Name { get; set; }

            public virtual List<Post> Posts { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public virtual Blog Blog { get; set; }
        }
    }
```

 

## Create a synchronous program

Now that we have an EF model, let's write some code that uses it to perform some data access.

-   Replace the contents of **Program.cs** with the following code

``` csharp
    using System;
    using System.Linq;

    namespace AsyncDemo
    {
        class Program
        {
            static void Main(string[] args)
            {
                PerformDatabaseOperations();

                Console.WriteLine("Quote of the day");
                Console.WriteLine(" Don't worry about the world coming to an end today... ");
                Console.WriteLine(" It's already tomorrow in Australia.");

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

            public static void PerformDatabaseOperations()
            {
                using (var db = new BloggingContext())
                {
                    // Create a new blog and save it
                    db.Blogs.Add(new Blog
                    {
                        Name = "Test Blog #" + (db.Blogs.Count() + 1)
                    });
                    Console.WriteLine("Calling SaveChanges.");
                    db.SaveChanges();
                    Console.WriteLine("SaveChanges completed.");

                    // Query for all blogs ordered by name
                    Console.WriteLine("Executing query.");
                    var blogs = (from b in db.Blogs
                                orderby b.Name
                                select b).ToList();

                    // Write all blogs out to Console
                    Console.WriteLine("Query completed with following results:");
                    foreach (var blog in blogs)
                    {
                        Console.WriteLine(" " + blog.Name);
                    }
                }
            }
        }
    }
```

This code calls the `PerformDatabaseOperations` method which saves a new **Blog** to the database and then retrieves all **Blogs** from the database and prints them to the **Console**. After this, the program writes a quote of the day to the **Console**.

Since the code is synchronous, we can observe the following execution flow when we run the program:

1.  `SaveChanges` begins to push the new **Blog** to the database
2.  `SaveChanges` completes
3.  Query for all **Blogs** is sent to the database
4.  Query returns and results are written to **Console**
5.  Quote of the day is written to **Console**

![Sync Output](~/ef6/media/syncoutput.png) 

 

## Making it asynchronous

Now that we have our program up and running, we can begin making use of the new async and await keywords. We've made the following changes to Program.cs

1.  Line 2: The using statement for the `System.Data.Entity` namespace gives us access to the EF async extension methods.
2.  Line 4: The using statement for the `System.Threading.Tasks` namespace allows us to use the `Task` type.
3.  Line 12 & 18: We are capturing as task that monitors the progress of `PerformSomeDatabaseOperations` (line 12) and then blocking program execution for this task to complete once all the work for the program is done (line 18).
4.  Line 25: We've update `PerformSomeDatabaseOperations` to be marked as `async` and return a `Task`.
5.  Line 35: We're now calling the Async version of `SaveChanges` and awaiting it's completion.
6.  Line 42: We're now calling the Async version of `ToList` and awaiting on the result.

For a comprehensive list of available extension methods in the `System.Data.Entity` namespace, refer to the `QueryableExtensions` class. *You’ll also need to add `using System.Data.Entity` to your using statements.*

``` csharp
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    namespace AsyncDemo
    {
        class Program
        {
            static void Main(string[] args)
            {
                var task = PerformDatabaseOperations();

                Console.WriteLine("Quote of the day");
                Console.WriteLine(" Don't worry about the world coming to an end today... ");
                Console.WriteLine(" It's already tomorrow in Australia.");

                task.Wait();

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

            public static async Task PerformDatabaseOperations()
            {
                using (var db = new BloggingContext())
                {
                    // Create a new blog and save it
                    db.Blogs.Add(new Blog
                    {
                        Name = "Test Blog #" + (db.Blogs.Count() + 1)
                    });
                    Console.WriteLine("Calling SaveChanges.");
                    await db.SaveChangesAsync();
                    Console.WriteLine("SaveChanges completed.");

                    // Query for all blogs ordered by name
                    Console.WriteLine("Executing query.");
                    var blogs = await (from b in db.Blogs
                                orderby b.Name
                                select b).ToListAsync();

                    // Write all blogs out to Console
                    Console.WriteLine("Query completed with following results:");
                    foreach (var blog in blogs)
                    {
                        Console.WriteLine(" - " + blog.Name);
                    }
                }
            }
        }
    }
```

Now that the code is asynchronous, we can observe a different execution flow when we run the program:

1. `SaveChanges` begins to push the new **Blog** to the database  
    *Once the command is sent to the database no more compute time is needed on the current managed thread. The `PerformDatabaseOperations` method returns (even though it hasn't finished executing) and program flow in the Main method continues.*
2. **Quote of the day is written to Console**  
    *Since there is no more work to do in the Main method, the managed thread is blocked on the `Wait` call until the database operation completes. Once it completes, the remainder of our `PerformDatabaseOperations` will be executed.*
3.  `SaveChanges` completes  
4.  Query for all **Blogs** is sent to the database  
    *Again, the managed thread is free to do other work while the query is processed in the database. Since all other execution has completed, the thread will just halt on the Wait call though.*
5.  Query returns and results are written to **Console**  

![Async Output](~/ef6/media/asyncoutput.png) 

 

## The takeaway

We now saw how easy it is to make use of EF’s asynchronous methods. Although the advantages of async may not be very apparent with a simple console app, these same strategies can be applied in situations where long-running or network-bound activities might otherwise block the application, or cause a large number of threads to increase the memory footprint.
