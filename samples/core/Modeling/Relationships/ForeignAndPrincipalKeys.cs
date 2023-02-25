#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.Relationships;

public class ForeignAndPrincipalKeys
{
    public class ForeignKeyConfigurationByLambda
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a foreign key using HasForeignKey with a lambda expression:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? ContainingBlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyConfigurationByLambda{GetType().Name}.db");

            #region ForeignKeyByLambda
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.ContainingBlogId);
            }
            #endregion
        }
    }

    public class ForeignKeyConfigurationByString
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a foreign key using HasForeignKey with a string:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            private int? ContainingBlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyConfigurationByString{GetType().Name}.db");

            #region ForeignKeyByString
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("ContainingBlogId");
            }
            #endregion
        }
    }

    public class CompositeForeignKeyConfigurationByLambda
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a composite foreign key using HasForeignKey with a lambda expression:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        [PrimaryKey(nameof(Id1), nameof(Id2))]
        public class Blog
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? ContainingBlogId1 { get; set; }
            public int? ContainingBlogId2 { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = CompositeForeignKeyConfigurationByLambda{GetType().Name}.db");

            #region CompositeForeignKeyByLambda
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => new { e.ContainingBlogId1, e.ContainingBlogId2 });
            }
            #endregion
        }
    }

    public class CompositeForeignKeyConfigurationByString
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a composite foreign key using HasForeignKey with a string:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        [PrimaryKey(nameof(Id1), nameof(Id2))]
        public class Blog
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = CompositeForeignKeyConfigurationByString{GetType().Name}.db");

            #region CompositeForeignKeyByString
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("ContainingBlogId1", "ContainingBlogId2");
            }
            #endregion
        }
    }

    public class RequiredForeignKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a relationship as required, even though the foreign key is nullable:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredForeignKey{GetType().Name}.db");

            #region RequiredForeignKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region RequiredForeignKeyConfig2
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region RequiredForeignKeyConfigByProperty
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .Property(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class ShadowForeignKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a relationship with a shadow foreign key:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            var script = context1.Database.GenerateCreateScript();
            //Debug.Assert(schema == script);
        }

        public class Blog
        {
            public string Id { get; set; } = null!;
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; init; } = null!;
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ShadowForeignKey{GetType().Name}.db");

            #region ShadowForeignKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("MyBlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ShadowForeignKeyConfigByProperty
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .Property<string>("MyBlogId")
                    .IsRequired();

                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("MyBlogId");
            }
            #endregion
        }
    }

    public class ForeignKeyConstraintName
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a relationship as with a named foreign key constraint:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyConstraintName{GetType().Name}.db");

            #region ForeignKeyConstraintNameConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .HasConstraintName("My_BlogId_Constraint");
            }
            #endregion
        }
    }

    public class AlternateKeyConfigurationByLambda
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure an alternate key using HasPrincipalKey with a lambda expression:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = AlternateKeyConfigurationByLambda{GetType().Name}.db");

            #region AlternateKeyConfigurationByLambda
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId);
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .HasPrincipalKey(e => e.AlternateId);
            }
        }
    }

    public class AlternateKeyConfigurationByString
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure an alternate key using HasPrincipalKey with a string:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        public class Blog
        {
            public int Id { get; set; }
            private int AlternateId { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogAlternateId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = AlternateKeyConfigurationByString{GetType().Name}.db");

            #region AlternateKeyConfigurationByString
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey("AlternateId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogAlternateId")
                    .HasPrincipalKey("AlternateId");
            }
        }
    }

    public class CompositeAlternateKeyConfigurationByLambda
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a composite alternate key using HasPrincipalKey with a lambda expression:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId1 { get; set; }
            public int AlternateId2 { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogAlternateId1 { get; set; }
            public int? BlogAlternateId2 { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = CompositeAlternateKeyConfigurationByLambda{GetType().Name}.db");

            #region CompositeAlternateKeyConfigurationByLambda
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => new { e.AlternateId1, e.AlternateId2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => new { e.BlogAlternateId1, e.BlogAlternateId2 })
                    .HasPrincipalKey(e => new { e.AlternateId1, e.AlternateId2 });
            }
        }
    }

    public class CompositeAlternateKeyConfigurationByString
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a composite alternate key using HasPrincipalKey with a string:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId1 { get; set; }
            public int AlternateId2 { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = CompositeAlternateKeyConfigurationByString{GetType().Name}.db");

            #region CompositeAlternateKeyConfigurationByString
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey("AlternateId1", "AlternateId2");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogAlternateId1", "BlogAlternateId2")
                    .HasPrincipalKey("AlternateId1", "AlternateId2");
            }
        }
    }

    public class ForeignKeyInKeylessType
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure a foreign key on a keyless type to form a relationship:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        #region ForeignKeyInKeylessType
        public class Tag
        {
            public string Text { get; set; } = null!;
            public int PostId { get; set; }
            public Post Post { get; set; } = null!;
        }

        public class Post
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Blogs => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ForeignKeyInKeylessType{GetType().Name}.db");

            #region ForeignKeyInKeylessTypeConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Tag>()
                    .HasNoKey();

                modelBuilder.Entity<Post>()
                    .HasMany<Tag>()
                    .WithOne(e => e.Post);
            }
            #endregion
        }
    }

    public class ManyToManyForeignKeyConstraintNames
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Many-to-many with foreign key constraint names:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyForeignKeyConstraintNames
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyForeignKeyConstraintNames{GetType().Name}.db");

            #region ManyToManyForeignKeyConstraintNamesConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        l => l.HasOne(typeof(Tag)).WithMany().HasConstraintName("TagForeignKey_Constraint"),
                        r => r.HasOne(typeof(Post)).WithMany().HasConstraintName("PostForeignKey_Constraint"));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)).HasConstraintName("TagForeignKey_Constraint"),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)).HasConstraintName("PostForeignKey_Constraint"),
                        j => j.HasKey("PostsId", "TagsId"));
            }
        }
    }

    public class NavigationConfiguration
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Configure navigations to use property access instead of field access:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; init; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = NavigationConfiguration{GetType().Name}.db");

            #region NavigationConfiguration
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .Navigation(e => e.Posts)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                modelBuilder.Entity<Post>()
                    .Navigation(e => e.Blog)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            }
            #endregion
        }

        public class BlogContext1 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();

            #region ThrowForShadow
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.ConfigureWarnings(b => b.Throw(CoreEventId.ShadowPropertyCreated));
            #endregion
        }
    }
}
