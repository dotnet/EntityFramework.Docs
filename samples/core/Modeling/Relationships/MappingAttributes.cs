#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships;

public class MappingAttributes
{
    public class RequiredOnForeignKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [Required] on a foreign key property: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region RequiredOnForeignKey
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [Required]
            public string BlogId { get; set; }

            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredOnForeignKey{GetType().Name}.db");
        }
    }

    public class RequiredOnDependentNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [Required] on a dependent navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region RequiredOnDependentNavigation
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            public string BlogId { get; set; }

            [Required]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredOnDependentNavigation{GetType().Name}.db");
        }
    }

    public class RequiredOnDependentNavigationShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [Required] on a dependent navigation with a shadow foreign key: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region RequiredOnDependentNavigationShadowFk
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [Required]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredOnDependentNavigationShadowFk{GetType().Name}.db");
        }
    }

    public class ForeignKeyOnProperty
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [ForeignKey] on a foreign key property: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyOnProperty
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [ForeignKey(nameof(Blog))]
            public string BlogKey { get; set; }

            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyOnProperty{GetType().Name}.db");
        }
    }

    public class ForeignKeyOnDependentNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [ForeignKey] on a dependent navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyOnDependentNavigation
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            public string BlogKey { get; set; }

            [ForeignKey(nameof(BlogKey))]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyOnDependentNavigation{GetType().Name}.db");
        }
    }

    public class ForeignKeyOnPrincipalNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [ForeignKey] on a principal navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyOnPrincipalNavigation
        public class Blog
        {
            public string Id { get; set; }

            [ForeignKey(nameof(Post.BlogKey))]
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public string BlogKey { get; set; }
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyOnPrincipalNavigation{GetType().Name}.db");
        }
    }

    public class ForeignKeyOnDependentNavigationShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [ForeignKey] on a dependent navigation with a shadow foreign key: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyOnDependentNavigationShadowFk
        public class Blog
        {
            public string Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }

            [ForeignKey("BlogKey")]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyOnDependentNavigationShadowFk{GetType().Name}.db");
        }
    }

    public class ForeignKeyOnPrincipalNavigationShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [ForeignKey] on a principal navigation with a shadow foreign key: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyOnPrincipalNavigationShadowFk
        public class Blog
        {
            public string Id { get; set; }

            [ForeignKey("BlogKey")]
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyOnPrincipalNavigationShadowFk{GetType().Name}.db");
        }
    }

    public class InverseOnDependentNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [InverseProperty] on a dependent navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region InverseOnDependentNavigation
        public class Blog
        {
            public int Id { get; set; }

            public List<Post> Posts { get; } = new();

            public int FeaturedPostId { get; set; }
            public Post FeaturedPost { get; set; }
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            [InverseProperty("Posts")]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = InverseOnDependentNavigation{GetType().Name}.db");
        }
    }

    public class InverseOnPrincipalNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [InverseProperty] on a principal navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region InverseOnPrincipalNavigation
        public class Blog
        {
            public int Id { get; set; }

            [InverseProperty("Blog")]
            public List<Post> Posts { get; } = new();

            public int FeaturedPostId { get; set; }
            public Post FeaturedPost { get; set; }
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = InverseOnPrincipalNavigation{GetType().Name}.db");
        }
    }

    public class DeleteBehaviorOnDependentNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [DeleteBehavior] on the dependent navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region DeleteBehaviorOnDependentNavigation
        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            [DeleteBehavior(DeleteBehavior.Restrict)]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = DeleteBehaviorOnDependentNavigation{GetType().Name}.db");
        }
    }

    public class DeleteBehaviorOnPrincipalNavigation
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Using [DeleteBehavior] on the principal navigation: ");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region DeleteBehaviorOnPrincipalNavigation
        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; }

            [DeleteBehavior(DeleteBehavior.Restrict)]
            public Blog Blog { get; init; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = DeleteBehaviorOnPrincipalNavigation{GetType().Name}.db");
        }
    }
}
