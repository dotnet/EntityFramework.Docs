using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public static class TemporaryValuesSample
{
    public static void Explicit_temporary_values_can_be_stored_in_entity_instance_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Explicit_temporary_values_can_be_stored_in_entity_instance_1)}");
        Console.WriteLine(">>>> Shows using explicit temporary values with FK values to relate posts to a blog.");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using var context = new SomeDbContext();

        var blog = new Blog { Id = -1 };
        var post1 = new Post { Id = -1, BlogId = -1 };
        var post2 = new Post { Id = -2, BlogId = -1 };

        context.Add(blog).Property(e => e.Id).IsTemporary = true;
        context.Add(post1).Property(e => e.Id).IsTemporary = true;
        context.Add(post2).Property(e => e.Id).IsTemporary = true;

        Console.WriteLine($"Blog has explicit temporary ID = {blog.Id}");
        Console.WriteLine($"Post 1 has explicit temporary ID = {post1.Id} and FK to Blog = {post1.BlogId}");
        Console.WriteLine($"Post 2 has explicit temporary ID = {post2.Id} and FK to Blog = {post2.BlogId}");

        Console.WriteLine("SavingChanges...");
        context.SaveChanges();

        Console.WriteLine($"Blog has ID = {blog.Id}");
        Console.WriteLine($"Post 1 has ID = {post1.Id} and FK to Blog = {post1.BlogId}");
        Console.WriteLine($"Post 2 has ID = {post2.Id} and FK to Blog = {post2.BlogId}");

        Console.WriteLine();
    }

    public static void Explicit_temporary_values_can_be_stored_in_entity_instance_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Explicit_temporary_values_can_be_stored_in_entity_instance_2)}");
        Console.WriteLine(">>>> Shows taking generated temporary values and setting them into entity instances.");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using var context = new SomeDbContext();

        var blog = new Blog();
        var post1 = new Post();
        var post2 = new Post();

        var blogIdEntry = context.Add(blog).Property(e => e.Id);
        blog.Id = blogIdEntry.CurrentValue;
        blogIdEntry.IsTemporary = true;

        var post1IdEntry = context.Add(post1).Property(e => e.Id);
        post1.Id = post1IdEntry.CurrentValue;
        post1IdEntry.IsTemporary = true;
        post1.BlogId = blog.Id;

        var post2IdEntry = context.Add(post2).Property(e => e.Id);
        post2.Id = post2IdEntry.CurrentValue;
        post2IdEntry.IsTemporary = true;
        post2.BlogId = blog.Id;

        Console.WriteLine($"Blog has generated temporary ID = {blog.Id}");
        Console.WriteLine($"Post 1 has generated temporary ID = {post1.Id} and FK to Blog = {post1.BlogId}");
        Console.WriteLine($"Post 2 has generated temporary ID = {post2.Id} and FK to Blog = {post2.BlogId}");

        Console.WriteLine("SavingChanges...");
        context.SaveChanges();

        Console.WriteLine($"Blog has ID = {blog.Id}");
        Console.WriteLine($"Post 1 has ID = {post1.Id} and FK to Blog = {post1.BlogId}");
        Console.WriteLine($"Post 2 has ID = {post2.Id} and FK to Blog = {post2.BlogId}");

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new SomeDbContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    public class Blog
    {
        public int Id { get; set; }

        public ICollection<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int Id { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class SomeDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        private readonly bool _quiet;

        public SomeDbContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
