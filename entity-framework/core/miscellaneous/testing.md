---
title: Testing with InMemory
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 349a7ba9-d6fb-402e-b517-c0cc4c11cbe7
ms.prod: entity-framework
uid: core/miscellaneous/testing
---
# Testing with InMemory

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

This article covers how to use the InMemory provider to write efficient tests with minimal impact to the code being tested.

> [!WARNING]
> Currently you need to use `ServiceCollection` and `IServiceProvider` to control the scope of the InMemory database, which adds complexity to your tests. In the next release after RC2, there will be improvements to make this easier, [see issue #3253](https://github.com/aspnet/EntityFramework/issues/3253) for more details.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Testing) on GitHub.

## When to use InMemory for testing

The InMemory provider is useful when you want to test components using something that approximates connecting to the real database, without the overhead of actual database operations.

For example, consider the following service that allows application code to perform some operations related to blogs. Internally it uses a `DbContext` that connects to a SQL Server database. It would be useful to swap this context to connect to an InMemory database so that we can write efficient tests for this service without having to modify the code, or do a lot of work to create a test double of the context.

<!-- [!code-csharp[Main](samples/core/Miscellaneous/Testing/BusinessLogic/BlogService.cs)] -->
````csharp
public class BlogService
{
    private BloggingContext _context;

    public BlogService(BloggingContext context)
    {
        _context = context;
    }

    public void Add(string url)
    {
        var blog = new Blog { Url = url };
        _context.Blogs.Add(blog);
        _context.SaveChanges();
    }

    public IEnumerable<Blog> Find(string term)
    {
        return _context.Blogs
            .Where(b => b.Url.Contains(term))
            .OrderBy(b => b.Url)
            .ToList();
    }
}
````

### InMemory is not a relational database

EF Core database providers do not have to be relational databases. InMemory is designed to be a general purpose database for testing, and is not designed to mimic a relational database.

Some examples of this include:
* InMemory will allow you to save data that would violate referential integrity constraints in a relational database.

* If you use DefaultValueSql(string) for a property in your model, this is a relational database API and will have no effect when running against InMemory.

> [!TIP]
> For many test purposes these difference will not matter. However, if you want to test against something that behaves more like a true relational database, then consider using [SQLite in-memory mode](http://www.sqlite.org/inmemorydb.html).

## Get your context ready

### Avoid configuring two database providers

In your tests you are going to externally configure the context to use the InMemory provider. If you are configuring a database provider by overriding `OnConfiguring` in your context, then you need to add some conditional code to ensure that you only configure the database provider if one has not already been configured.

> [!NOTE]
> If you are using ASP.NET Core, then you should not need this code since your database provider is configured outside of the context (in Startup.cs).

<!-- [!code-csharp[Main](samples/core/Miscellaneous/Testing/BusinessLogic/BloggingContext.cs?highlight=3)] -->
````csharp
 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 {
     if (!optionsBuilder.IsConfigured)
     {
         optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
     }
````

### Add a constructor for testing

The simplest way to enable testing with the InMemory provider is to modify your context to expose a constructor that accepts a `DbContextOptions<TContext>`.

<!-- [!code-csharp[Main](samples/core/Miscellaneous/Testing/BusinessLogic/BloggingContext.cs?highlight=6,7,8)] -->
````csharp
public class BloggingContext : DbContext
{
    public BloggingContext()
    { }

    public BloggingContext(DbContextOptions<BloggingContext> options)
        : base(options)
    { }
````

> [!NOTE]
> `DbContextOptions<TContext>` tells the context all of its settings, such as which database to connect to. This is the same object that is built by running the OnConfiguring method in your context.

## Writing tests

The key to testing with this provider is the ability to tell the context to use the InMemory provider, and control the scope of the in-memory database. Typically you want a clean database for each test method.

`DbContextOptions<TContext>` exposes a `UseInternalServiceProvider` method that allows us to control the `IServiceProvider` the context will use. `IServiceProvider` is the container that EF will resolve all its services from (including the InMemory database instance). Typically, EF creates a single `IServiceProvider` for all contexts of a given type in an AppDomain - meaning all context instances share the same InMemory database instance. By allowing one to be passed in, you can control the scope of the InMemory database.

Here is an example of a test class that uses the InMemory database. Each test method creates a new `DbContextOptions<TContext>` with a new `IServiceProvider`, meaning each method has its own InMemory database.

<!-- [!code-csharp[Main](samples/core/Miscellaneous/Testing/TestProject/BlogServiceTests.cs)] -->
````csharp
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class BlogServiceTests
    {
        private static DbContextOptions<BloggingContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        [TestMethod]
        public void Add_writes_to_database()
        {
            // All contexts that share the same service provider will share the same InMemory database
            var options = CreateNewContextOptions();

            // Run the test against one instance of the context
            using (var context = new BloggingContext(options))
            {
                var service = new BlogService(context);
                service.Add("http://sample.com");
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new BloggingContext(options))
            {
                Assert.AreEqual(1, context.Blogs.Count());
                Assert.AreEqual("http://sample.com", context.Blogs.Single().Url);
            }
        }

        [TestMethod]
        public void Find_searches_url()
        {
            // All contexts that share the same service provider will share the same InMemory database
            var options = CreateNewContextOptions();

            // Insert seed data into the database using one instance of the context
            using (var context = new BloggingContext(options))
            {
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BloggingContext(options))
            {
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
````

## Sharing a database instance for read-only tests

If a test class has read-only tests that share the same seed data, then you can share the InMemory database instance for the whole class (rather than a new one for each method). This means you have a single  `DbContextOptions<TContext>` and `IServiceProvider` for the test class, rather than one for each test method.

<!-- [!code-csharp[Main](samples/core/Miscellaneous/Testing/TestProject/BlogServiceTestsReadOnly.cs)] -->
````csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace TestProject
{
    [TestClass]
    public class BlogServiceTestsReadOnly
    {
        private DbContextOptions<BloggingContext> _contextOptions;

        public BlogServiceTestsReadOnly()
        {
            // Create a service provider to be shared by all test methods
            var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();

            // Create options telling the context to use an
            // InMemory database and the service provider.
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            _contextOptions = builder.Options;

            // Insert the seed data that is expected by all test methods
            using (var context = new BloggingContext(_contextOptions))
            {
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Find_with_empty_term()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("");
                Assert.AreEqual(3, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_unmatched_term()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("horse");
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_some_matched()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
````
