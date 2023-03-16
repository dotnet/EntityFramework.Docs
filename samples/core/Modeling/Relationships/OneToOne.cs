#nullable enable

using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships;

public class OneToOne
{
    public class Required
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-one: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequired
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Required{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Optional one-to-one: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptional
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Optional{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class RequiredPkToPk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-one with primary key to primary key relationship: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredPkToPk
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredPkToPk{GetType().Name}.db");

            #region OneToOneRequiredPkToPkConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredPkToPkFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.Id)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredPkToPkFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.Id)
                    .IsRequired();
            }
            #endregion
        }
    }

    public class RequiredWithShadowFk
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();
            Console.WriteLine("Required one-to-one with shadow foreign key property: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadow
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadow{GetType().Name}.db");

            #region OneToOneRequiredShadowConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadow
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");

            #region OneToOneOptionalShadowConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependent
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Required one-to-one with no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigationToPrincipal{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Optional one-to-one with no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigationToPrincipal{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Required one-to-one with shadow foreign key property and no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadowNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigationToPrincipal{GetType().Name}.db");

            #region OneToOneRequiredShadowNoNavigationToPrincipalConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property and no navigation to principal: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadowNoNavigationToPrincipal
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowNoNavigationToPrincipal{GetType().Name}.db");

            #region OneToOneOptionalShadowNoNavigationToPrincipalConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipalNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependentNoNavigationToPrincipal
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Required one-to-one and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigationToDependents{GetType().Name}.db");

            #region OneToOneRequiredNoNavigationToDependentsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Optional one-to-one with no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigationToDependents{GetType().Name}.db");

            #region OneToOneOptionalNoNavigationToDependentsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Required one-to-one with shadow foreign key property and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadowNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigationToDependents{GetType().Name}.db");

            #region OneToOneRequiredShadowNoNavigationToDependentsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property and no navigation to dependents: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadowNoNavigationToDependents
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");

            #region OneToOneOptionalShadowNoNavigationToDependentsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipalNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependentNoNavigationToDependents
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Required one-to-one and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredNoNavigations{GetType().Name}.db");

            #region OneToOneRequiredNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Optional one-to-one with no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalNoNavigations{GetType().Name}.db");

            #region OneToOneOptionalNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Required one-to-one with shadow foreign key property and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadowNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowNoNavigations{GetType().Name}.db");

            #region OneToOneRequiredShadowNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property and no navigations: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadowNoNavigations
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadow{GetType().Name}.db");

            #region OneToOneOptionalShadowNoNavigationsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipalNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne<BlogHeader>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependentNoNavigations
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne<Blog>()
                    .WithOne()
                    .HasForeignKey<BlogHeader>("BlogId")
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
            Console.WriteLine("Required one-to-one with alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the BlogHeader.BlogId foreign key
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredWithAlternateKey{GetType().Name}.db");

            #region OneToOneRequiredWithAlternateKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId);
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Optional one-to-one with alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the BlogHeader.BlogId foreign key
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int? BlogId { get; set; } // Optional foreign key property
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalWithAlternateKey{GetType().Name}.db");

            #region OneToOneOptionalWithAlternateKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId);
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
            Console.WriteLine("Required one-to-one with shadow foreign key property and alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadowWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the shadow foreign key
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowWithAlternateKey{GetType().Name}.db");

            #region OneToOneRequiredShadowWithAlternateKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
                    .IsRequired();
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property and alternate key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadowWithAlternateKey
        // Principal (parent)
        public class Blog
        {
            public int Id { get; set; }
            public int AlternateId { get; set; } // Alternate key as target of the shadow foreign key
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowWithAlternateKey{GetType().Name}.db");

            #region OneToOneOptionalShadowWithAlternateKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipalWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
                    .IsRequired(false);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependentWithAlternateKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => e.AlternateId)
                    .HasForeignKey<BlogHeader>("BlogAlternateId")
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
            Console.WriteLine("Required one-to-one with composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
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
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredWithCompositeKey{GetType().Name}.db");

            #region OneToOneRequiredWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    nestedBuilder =>
                    {
                        nestedBuilder.HasKey(e => new { e.Id1, e.Id2 });

                        nestedBuilder.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                            .HasForeignKey<BlogHeader>(e => new { e.BlogId1, e.BlogId2 })
                            .IsRequired();
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                    .HasForeignKey<BlogHeader>(e => new { e.BlogId1, e.BlogId2 })
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
            Console.WriteLine("Optional one-to-one with composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
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
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalWithCompositeKey{GetType().Name}.db");

            #region OneToOneOptionalWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                            .HasForeignKey<BlogHeader>(e => new { e.BlogId1, e.BlogId2 })
                            .IsRequired(false);
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                    .HasForeignKey<BlogHeader>(e => new { e.BlogId1, e.BlogId2 })
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
            Console.WriteLine("Required one-to-one with shadow foreign key property and composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneRequiredShadowWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = RequiredShadowWithCompositeKey{GetType().Name}.db");

            #region OneToOneRequiredShadowWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 });
                    });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneRequiredShadowFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                            .HasForeignKey<BlogHeader>("BlogId1", "BlogId2")
                            .IsRequired();
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneRequiredShadowFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                    .HasForeignKey<BlogHeader>("BlogId1", "BlogId2")
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
            Console.WriteLine("Optional one-to-one with shadow foreign key property and composite key: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));

            using var context2 = new BlogContext2();
            Debug.Assert(modelDebugString == context2.Model.ToDebugString(indent: 2));
        }

        #region OneToOneOptionalShadowWithCompositeKey
        // Principal (parent)
        public class Blog
        {
            public int Id1 { get; set; } // Composite key part 1
            public int Id2 { get; set; } // Composite key part 2
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public Blog? Blog { get; set; } // Optional reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = OptionalShadowWithCompositeKey{GetType().Name}.db");

            #region OneToOneOptionalShadowWithCompositeKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 });
                    });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region OneToOneOptionalShadowFromPrincipalWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(
                    b =>
                    {
                        b.HasKey(e => new { e.Id1, e.Id2 });

                        b.HasOne(e => e.Header)
                            .WithOne(e => e.Blog)
                            .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                            .HasForeignKey<BlogHeader>("BlogId1", "BlogId2")
                            .IsRequired(false);
                    });
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region OneToOneOptionalShadowFromDependentWithCompositeKey
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasKey(e => new { e.Id1, e.Id2 });

                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasPrincipalKey<Blog>(e => new { e.Id1, e.Id2 })
                    .HasForeignKey<BlogHeader>("BlogId1", "BlogId2")
                    .IsRequired(false);
            }
            #endregion
        }
    }

    public class SelfReferencing
    {
        public static void BuildModels()
        {
            using var context0 = new PersonContext0();
            Console.WriteLine("Self-referencing one-to-one: ");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();

            using var context1 = new PersonContext1();
            Debug.Assert(modelDebugString == context1.Model.ToDebugString(indent: 2));
        }

        #region SelfReferencing
        public class Person
        {
            public int Id { get; set; }

            public int? HusbandId { get; set; } // Optional foreign key property
            public Person? Husband { get; set; } // Optional reference navigation to principal
            public Person? Wife { get; set; } // Reference navigation to dependent
        }
        #endregion

        public class PersonContext0 : DbContext
        {
            public DbSet<Person> People => Set<Person>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = SelfReferencing{GetType().Name}.db");
        }

        public class PersonContext1 : PersonContext0
        {
            #region SelfReferencingConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasOne(e => e.Husband)
                    .WithOne(e => e.Wife)
                    .HasForeignKey<Person>(e => e.HusbandId)
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
            Console.WriteLine("Required one-to-one without cascade delete: ");
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
            public BlogHeader? Header { get; set; } // Reference navigation to dependent
        }

        // Dependent (child)
        public class BlogHeader
        {
            public int Id { get; set; }
            public int BlogId { get; set; } // Required foreign key property
            public Blog Blog { get; set; } = null!; // Required reference navigation to principal
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<BlogHeader> BlogHeaders => Set<BlogHeader>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = Required{GetType().Name}.db");

            #region RequiredWithoutCascadeDeleteConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>()
                    .HasOne(e => e.Header)
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
                    .HasOne(e => e.Header)
                    .WithOne(e => e.Blog)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
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
                modelBuilder.Entity<BlogHeader>()
                    .HasOne(e => e.Blog)
                    .WithOne(e => e.Header)
                    .HasForeignKey<BlogHeader>(e => e.BlogId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            }
            #endregion
        }
    }
}
