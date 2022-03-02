using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WorkerService1;

public class Blog
{
    public int Id { get; set; }

    public Uri Source { get; set; }
    public string Title { get; set; }

    public ICollection<Post> Posts { get; } = new HashSet<Post>();
}

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    public ICollection<Post> AuthoredPosts { get; } = new HashSet<Post>();
}

public class Post
{
    public int Id { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }

    public int AuthorId { get; set; }
    public Person Author { get; set; }
}

public class MediaPost : Post
{
    public Uri MediaUrl { get; set; }
    public string MediaType { get; set; }
}

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasIndex(p => p.Email).IsUnique();

        modelBuilder.Entity<MediaPost>();
    }
}