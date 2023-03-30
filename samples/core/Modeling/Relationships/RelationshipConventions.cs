#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships;

public class RelationshipConventions
{
    public class ReferenceNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Reference navigation discovery model: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ReferenceNavigations
        public class Blog
        {
            // Not discovered as reference navigations:
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public Uri? Uri { get; set; }
            public ConsoleKeyInfo ConsoleKeyInfo { get; set; }
            public Author DefaultAuthor => new() { Name = $"Author of the blog {Title}" };

            // Discovered as a reference navigation:
            public Author? Author { get; private set; }
        }

        public class Author
        {
            // Not discovered as reference navigations:
            public Guid Id { get; set; }
            public string Name { get; set; } = null!;
            public int BlogId { get; set; }

            // Discovered as a reference navigation:
            public Blog Blog { get; init; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ReferenceNavigations{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>().Ignore(e => e.ConsoleKeyInfo);
            }
        }
    }

    public class CollectionNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Collection navigation discovery model: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region CollectionNavigations
        public class Blog
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; set; } = null!;
        }

        public class Tag
        {
            public Guid Id { get; set; }
            public IEnumerable<Blog> Blogs { get; } = new List<Blog>();
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = CollectionNavigations{GetType().Name}.db");
        }
    }

    public class OneToManySingleRelationship
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Single one-to-many relationship: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            Console.WriteLine(context0.Database.GenerateCreateScript());
            Console.WriteLine();
        }

        #region OneToManySingleRelationship
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OneToManySingleRelationship{GetType().Name}.db");
        }
    }

    public class OneToOneSingleRelationship
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Single one-to-one relationship: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            Console.WriteLine(context0.Database.GenerateCreateScript());
            Console.WriteLine();
        }

        #region OneToOneSingleRelationship
        public class Blog
        {
            public int Id { get; set; }
            public Author? Author { get; set; }
        }

        public class Author
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OneToOneSingleRelationship{GetType().Name}.db");
        }
    }

    public class ManyToManySingleRelationship
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Single many-to-many relationship: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            Console.WriteLine(context0.Database.GenerateCreateScript());
            Console.WriteLine();
        }

        #region ManyToManySingleRelationship
        public class Post
        {
            public int Id { get; set; }
            public ICollection<Tag> Tags { get; } = new List<Tag>();
        }

        public class Tag
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManySingleRelationship{GetType().Name}.db");
        }
    }

    public class NavigationPrincipalKeyFKName
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Foreign key property discovered because it matches <navigation property name><principal key property name>: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region NavigationPrincipalKeyFKName
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? TheBlogKey { get; set; }
            public Blog? TheBlog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = NavigationPrincipalKeyFKName{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>().HasKey(e => e.Key);
            }
        }
    }

    public class NavigationIdFKName
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Foreign key property discovered because it matches <navigation property name>Id: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region NavigationIdFKName
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? TheBlogID { get; set; }
            public Blog? TheBlog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = NavigationIdFKName{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>().HasKey(e => e.Key);
            }
        }
    }

    public class PrincipalTypePrincipalKeyFKName
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Foreign key property discovered because it matches <principal entity type name><principal key property name>: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region PrincipalTypePrincipalKeyFKName
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogKey { get; set; }
            public Blog? TheBlog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = PrincipalTypePrincipalKeyFKName{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>().HasKey(e => e.Key);
            }
        }
    }

    public class PrincipalTypeIdFKName
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Foreign key property discovered because it matches <principal entity type name>Id: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region PrincipalTypeIdFKName
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? Blogid { get; set; }
            public Blog? TheBlog { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = PrincipalTypeIdFKName{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>().HasKey(e => e.Key);
            }
        }
    }
}
