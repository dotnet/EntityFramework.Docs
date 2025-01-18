---
title: Local Data - EF6
description: Local Data in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/querying/local-data
---
# Local Data
Running a LINQ query directly against a DbSet will always send a query to the database, but you can access the data that is currently in-memory using the DbSet.Local property. You can also access the extra information EF is tracking about your entities using the DbContext.Entry and DbContext.ChangeTracker.Entries methods. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

## Using Local to look at local data  

The Local property of DbSet provides simple access to the entities of the set that are currently being tracked by the context and have not been marked as Deleted. Accessing the Local property never causes a query to be sent to the database. This means that it is usually used after a query has already been performed. The Load extension method can be used to execute a query so that the context tracks the results. For example:  

``` csharp
using (var context = new BloggingContext())
{
    // Load all blogs from the database into the context
    context.Blogs.Load();

    // Add a new blog to the context
    context.Blogs.Add(new Blog { Name = "My New Blog" });

    // Mark one of the existing blogs as Deleted
    context.Blogs.Remove(context.Blogs.Find(1));

    // Loop over the blogs in the context.
    Console.WriteLine("In Local: ");
    foreach (var blog in context.Blogs.Local)
    {
        Console.WriteLine(
            "Found {0}: {1} with state {2}",
            blog.BlogId,  
            blog.Name,
            context.Entry(blog).State);
    }

    // Perform a query against the database.
    Console.WriteLine("\nIn DbSet query: ");
    foreach (var blog in context.Blogs)
    {
        Console.WriteLine(
            "Found {0}: {1} with state {2}",
            blog.BlogId,  
            blog.Name,
            context.Entry(blog).State);
    }
}
```  

If we had two blogs in the database - 'ADO.NET Blog' with a BlogId of 1 and 'The Visual Studio Blog' with a BlogId of 2 - we could expect the following output:  

```console
In Local:
Found 0: My New Blog with state Added
Found 2: The Visual Studio Blog with state Unchanged

In DbSet query:
Found 1: ADO.NET Blog with state Deleted
Found 2: The Visual Studio Blog with state Unchanged
```  

This illustrates three points:  

- The new blog 'My New Blog' is included in the Local collection even though it has not yet been saved to the database. This blog has a primary key of zero because the database has not yet generated a real key for the entity.  
- The 'ADO.NET Blog' is not included in the local collection even though it is still being tracked by the context. This is because we removed it from the DbSet thereby marking it as deleted.  
- When DbSet is used to perform a query the blog marked for deletion (ADO.NET Blog) is included in the results and the new blog (My New Blog) that has not yet been saved to the database is not included in the results. This is because DbSet is performing a query against the database and the results returned always reflect what is in the database.  

## Using Local to add and remove entities from the context  

The Local property on DbSet returns an [ObservableCollection](https://msdn.microsoft.com/library/ms668604.aspx) with events hooked up such that it stays in sync with the contents of the context. This means that entities can be added or removed from either the Local collection or the DbSet. It also means that queries that bring new entities into the context will result in the Local collection being updated with those entities. For example:  

``` csharp
using (var context = new BloggingContext())
{
    // Load some posts from the database into the context
    context.Posts.Where(p => p.Tags.Contains("entity-framework")).Load();  

    // Get the local collection and make some changes to it
    var localPosts = context.Posts.Local;
    localPosts.Add(new Post { Name = "What's New in EF" });
    localPosts.Remove(context.Posts.Find(1));  

    // Loop over the posts in the context.
    Console.WriteLine("In Local after entity-framework query: ");
    foreach (var post in context.Posts.Local)
    {
        Console.WriteLine(
            "Found {0}: {1} with state {2}",
            post.Id,  
            post.Title,
            context.Entry(post).State);
    }

    var post1 = context.Posts.Find(1);
    Console.WriteLine(
        "State of post 1: {0} is {1}",
        post1.Name,  
        context.Entry(post1).State);  

    // Query some more posts from the database
    context.Posts.Where(p => p.Tags.Contains("asp.net")).Load();  

    // Loop over the posts in the context again.
    Console.WriteLine("\nIn Local after asp.net query: ");
    foreach (var post in context.Posts.Local)
    {
        Console.WriteLine(
            "Found {0}: {1} with state {2}",
            post.Id,  
            post.Title,
            context.Entry(post).State);
    }
}
```  

Assuming we had a few posts tagged with 'entity-framework' and 'asp.net' the output may look something like this:  

```console
In Local after entity-framework query:
Found 3: EF Designer Basics with state Unchanged
Found 5: EF Code First Basics with state Unchanged
Found 0: What's New in EF with state Added
State of post 1: EF Beginners Guide is Deleted

In Local after asp.net query:
Found 3: EF Designer Basics with state Unchanged
Found 5: EF Code First Basics with state Unchanged
Found 0: What's New in EF with state Added
Found 4: ASP.NET Beginners Guide with state Unchanged
```  

This illustrates three points:  

- The new post 'What's New in EF' that was added to the Local collection becomes tracked by the context in the Added state. It will therefore be inserted into the database when SaveChanges is called.  
- The post that was removed from the Local collection (EF Beginners Guide) is now marked as deleted in the context. It will therefore be deleted from the database when SaveChanges is called.  
- The additional post (ASP.NET Beginners Guide) loaded into the context with the second query is automatically added to the Local collection.  

One final thing to note about Local is that because it is an ObservableCollection performance is not great for large numbers of entities. Therefore if you are dealing with thousands of entities in your context it may not be advisable to use Local.  

## Using Local for WPF data binding  

The Local property on DbSet can be used directly for data binding in a WPF application because it is an instance of ObservableCollection. As described in the previous sections this means that it will automatically stay in sync with the contents of the context and the contents of the context will automatically stay in sync with it. Note that you do need to pre-populate the Local collection with data for there to be anything to bind to since Local never causes a database query.  

This is not an appropriate place for a full WPF data binding sample but the key elements are:  

- Setup a binding source  
- Bind it to the Local property of your set  
- Populate Local using a query to the database.  

## WPF binding to navigation properties  

If you are doing master/detail data binding you may want to bind the detail view to a navigation property of one of your entities. An easy way to make this work is to use an ObservableCollection for the navigation property. For example:  

``` csharp
public class Blog
{
    private readonly ObservableCollection<Post> _posts =
        new ObservableCollection<Post>();

    public int BlogId { get; set; }
    public string Name { get; set; }

    public virtual ObservableCollection<Post> Posts
    {
        get { return _posts; }
    }
}
```  

## Using Local to clean up entities in SaveChanges  

In most cases entities removed from a navigation property will not be automatically marked as deleted in the context. For example, if you remove a Post object from the Blog.Posts collection then that post will not be automatically deleted when SaveChanges is called. If you need it to be deleted then you may need to find these dangling entities and mark them as deleted before calling SaveChanges or as part of an overridden SaveChanges. For example:  

``` csharp
public override int SaveChanges()
{
    foreach (var post in this.Posts.Local.ToList())
    {
        if (post.Blog == null)
        {
            this.Posts.Remove(post);
        }
    }

    return base.SaveChanges();
}
```  

The code above uses the Local collection to find all posts and marks any that do not have a blog reference as deleted. The ToList call is required because otherwise the collection will be modified by the Remove call while it is being enumerated. In most other situations you can query directly against the Local property without using ToList first.  

## Using Local and ToBindingList for Windows Forms data binding  

Windows Forms does not support full fidelity data binding using ObservableCollection directly. However, you can still use the DbSet Local property for data binding to get all the benefits described in the previous sections. This is achieved through the ToBindingList extension method which creates an [IBindingList](https://msdn.microsoft.com/library/system.componentmodel.ibindinglist.aspx) implementation backed by the Local ObservableCollection.  

This is not an appropriate place for a full Windows Forms data binding sample but the key elements are:  

- Setup an object binding source  
- Bind it to the Local property of your set using Local.ToBindingList()  
- Populate Local using a query to the database  

## Getting detailed information about tracked entities  

Many of the examples in this series use the Entry method to return a DbEntityEntry instance for an entity. This entry object then acts as the starting point for gathering information about the entity such as its current state, as well as for performing operations on the entity such as explicitly loading a related entity.  

The Entries methods return DbEntityEntry objects for many or all entities being tracked by the context. This allows you to gather information or perform operations on many entities rather than just a single entry. For example:  

``` csharp
using (var context = new BloggingContext())
{
    // Load some entities into the context
    context.Blogs.Load();
    context.Authors.Load();
    context.Readers.Load();

    // Make some changes
    context.Blogs.Find(1).Title = "The New ADO.NET Blog";
    context.Blogs.Remove(context.Blogs.Find(2));
    context.Authors.Add(new Author { Name = "Jane Doe" });
    context.Readers.Find(1).Username = "johndoe1987";

    // Look at the state of all entities in the context
    Console.WriteLine("All tracked entities: ");
    foreach (var entry in context.ChangeTracker.Entries())
    {
        Console.WriteLine(
            "Found entity of type {0} with state {1}",
            ObjectContext.GetObjectType(entry.Entity.GetType()).Name,
            entry.State);
    }

    // Find modified entities of any type
    Console.WriteLine("\nAll modified entities: ");
    foreach (var entry in context.ChangeTracker.Entries()
                              .Where(e => e.State == EntityState.Modified))
    {
        Console.WriteLine(
            "Found entity of type {0} with state {1}",
            ObjectContext.GetObjectType(entry.Entity.GetType()).Name,
            entry.State);
    }

    // Get some information about just the tracked blogs
    Console.WriteLine("\nTracked blogs: ");
    foreach (var entry in context.ChangeTracker.Entries<Blog>())
    {
        Console.WriteLine(
            "Found Blog {0}: {1} with original Name {2}",
            entry.Entity.BlogId,  
            entry.Entity.Name,
            entry.Property(p => p.Name).OriginalValue);
    }

    // Find all people (author or reader)
    Console.WriteLine("\nPeople: ");
    foreach (var entry in context.ChangeTracker.Entries<IPerson>())
    {
        Console.WriteLine("Found Person {0}", entry.Entity.Name);
    }
}
```  

You'll notice we are introducing an Author and Reader class into the example - both of these classes implement the IPerson interface.  

``` csharp
public class Author : IPerson
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
}

public class Reader : IPerson
{
    public int ReaderId { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
}

public interface IPerson
{
    string Name { get; }
}
```  

Let's assume we have the following data in the database:

Blog with BlogId = 1 and Name = 'ADO.NET Blog'  
Blog with BlogId = 2 and Name = 'The Visual Studio Blog'  
Blog with BlogId = 3 and Name = '.NET Framework Blog'  
Author with AuthorId = 1 and Name = 'Joe Bloggs'  
Reader with ReaderId = 1 and Name = 'John Doe'  

The output from running the code would be:  

```console
All tracked entities:
Found entity of type Blog with state Modified
Found entity of type Blog with state Deleted
Found entity of type Blog with state Unchanged
Found entity of type Author with state Unchanged
Found entity of type Author with state Added
Found entity of type Reader with state Modified

All modified entities:
Found entity of type Blog with state Modified
Found entity of type Reader with state Modified

Tracked blogs:
Found Blog 1: The New ADO.NET Blog with original Name ADO.NET Blog
Found Blog 2: The Visual Studio Blog with original Name The Visual Studio Blog
Found Blog 3: .NET Framework Blog with original Name .NET Framework Blog

People:
Found Person John Doe
Found Person Joe Bloggs
Found Person Jane Doe
```  

These examples illustrate several points:  

- The Entries methods return entries for entities in all states, including Deleted. Compare this to Local which excludes Deleted entities.  
- Entries for all entity types are returned when the non-generic Entries method is used. When the generic entries method is used entries are only returned for entities that are instances of the generic type. This was used above to get entries for all blogs. It was also used to get entries for all entities that implement IPerson. This demonstrates that the generic type does not have to be an actual entity type.  
- LINQ to Objects can be used to filter the results returned. This was used above to find entities of any type as long as they are modified.  

Note that DbEntityEntry instances always contain a non-null Entity. Relationship entries and stub entries are not represented as DbEntityEntry instances so there is no need to filter for these.
