#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships;

public class OneToMany
{
    public class Required
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequired
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Required{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredFromPrincipal
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

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class Optional
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptional
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Optional{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadow
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadow{GetType().Name}.db");

            #region OneToManyRequiredShadowConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadow
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalShadowFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredNoNavigationToPrincipal
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigationToPrincipal{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalNoNavigationToPrincipal
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigationToPrincipal{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFkAndNoNavigationToPrincipal
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property and no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadowNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigationToPrincipal{GetType().Name}.db");

            #region OneToManyRequiredShadowNoNavigationToPrincipalConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne()
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne()
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany(e => e.Posts)
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFkAndNoNavigationToPrincipal
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property and no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadowNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowNoNavigationToPrincipal{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalShadowFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne()
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany(e => e.Posts)
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredNoNavigationToDependents
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigationToDependents{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalNoNavigationToDependents
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigationToDependents{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFkAndNoNavigationToDependents
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadowNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigationToDependents{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany()
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFkAndNoNavigationToDependents
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadowNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalShadowFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany()
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredNoNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigations{GetType().Name}.db");

            #region OneToManyRequiredNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalNoNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigations{GetType().Name}.db");

            #region OneToManyOptionalNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany()
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFkAndNoNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadowNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigations{GetType().Name}.db");

            #region OneToManyRequiredShadowNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne()
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne()
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany()
                    .HasForeignKey("BlogId")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFkAndNoNavigations
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadowNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");

            #region OneToManyOptionalShadowNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalShadowFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany<Post>()
                    .WithOne()
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne<Blog>()
                    .WithMany()
                    .HasForeignKey("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithAlternateKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the Post.BlogId foreign key
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredWithAlternateKey{GetType().Name}.db");

            #region OneToManyRequiredWithAlternateKeyConfig
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
            #region OneToManyRequiredFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithAlternateKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the Post.BlogId foreign key
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalWithAlternateKey{GetType().Name}.db");

            #region OneToManyOptionalWithAlternateKeyConfig
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
            #region OneToManyOptionalFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFkWithAlternateKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property and alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadowWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the shadow foreign key
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowWithAlternateKey{GetType().Name}.db");

            #region OneToManyRequiredShadowWithAlternateKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey("BlogAlternateId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey("BlogAlternateId")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFkWithAlternateKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property and alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadowWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the shadow foreign key
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowWithAlternateKey{GetType().Name}.db");

            #region OneToManyOptionalShadowWithAlternateKeyConfig
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
            #region OneToManyOptionalShadowFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey("BlogAlternateId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => e.AlternateId)
                    .HasForeignKey("BlogAlternateId")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithCompositeKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId1 { get; set; } // Required foreign key property part 1
            public int BlogId2 { get; set; } // Required foreign key property part 2
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredWithCompositeKey{GetType().Name}.db");

            #region OneToManyRequiredWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    nestedBuilder =>
                    {
                        nestedBuilder.HasKey(e => new { e.Id1, e.Id2 });

                        nestedBuilder.HasMany(e => e.Posts)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                            .HasForeignKey(e => new { e.BlogId1, e.BlogId2 })
                            .IsRequired();
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                    .HasForeignKey(e => new { e.BlogId1, e.BlogId2 })
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithCompositeKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId1 { get; set; } // Optional foreign key property part 1
            public int? BlogId2 { get; set; } // Optional foreign key property part 2
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalWithCompositeKey{GetType().Name}.db");

            #region OneToManyOptionalWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasMany(e => e.Posts)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                            .HasForeignKey(e => new { e.BlogId1, e.BlogId2 })
                            .IsRequired(false);
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                    .HasForeignKey(e => new { e.BlogId1, e.BlogId2 })
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithShadowFkWithCompositeKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many with shadow foreign key property and composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyRequiredShadowWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowWithCompositeKey{GetType().Name}.db");

            #region OneToManyRequiredShadowWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyRequiredShadowFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasMany(e => e.Posts)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                            .HasForeignKey("BlogId1", "BlogId2")
                            .IsRequired();
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyRequiredShadowFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                    .HasForeignKey("BlogId1", "BlogId2")
                    .IsRequired();
            }
            #endregion
        }
    }

    public class OptionalWithShadowFkWithCompositeKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Optional one-to-many with shadow foreign key property and composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToManyOptionalShadowWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowWithCompositeKey{GetType().Name}.db");

            #region OneToManyOptionalShadowWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToManyOptionalShadowFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasMany(e => e.Posts)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                            .HasForeignKey("BlogId1", "BlogId2")
                            .IsRequired(false);
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToManyOptionalShadowFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasPrincipalKey(e => new { e.Id1, e.Id2 })
                    .HasForeignKey("BlogId1", "BlogId2")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class SelfReferencing
    {
        public static void BuildModels()
        {
            using var context0 = new EmployeeContext0();
            Console.WriteLine("Self-referencing one-to-many: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new EmployeeContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));
        }

        #region SelfReferencing
        public class Employee
        {
            public int Id { get; set; }

            public int? ManagerId { get; set; } // Optional foreign key property
            public Employee? Manager { get; set; } // Optional reference navigation to principal
            public ICollection<Employee> Reports { get; } = new List<Employee>(); // Collection navigation containing dependents
        }
        #endregion

        public class EmployeeContext0 : DbContext
        {
            public DbSet<Employee> Blogs => Set<Employee>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = SelfReferencing{GetType().Name}.db");
        }

        public class EmployeeContext1 : EmployeeContext0
        {
            #region SelfReferencingConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Employee>()
                    .HasOne(e => e.Manager)
                    .WithMany(e => e.Reports)
                    .HasForeignKey(e => e.ManagerId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredWithoutCascadeDelete
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-many without cascade delete: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region RequiredWithoutCascadeDelete
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
        }

        // Dependent (child)
        public class Post
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Post> Posts => Set<Post>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Required{GetType().Name}.db");

            #region RequiredWithoutCascadeDeleteConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region RequiredWithoutCascadeDeleteFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Posts)
                    .WithOne(e => e.Blog)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region RequiredWithoutCascadeDeleteFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasOne(e => e.Blog)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.BlogId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            }
            #endregion
        }
    }
}
