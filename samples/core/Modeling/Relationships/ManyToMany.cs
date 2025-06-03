#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships;

public class ManyToMany
{
    public class BasicManyToMany
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Basic, bidirectional many-to-many:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        #region BasicManyToMany
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = BasicManyToMany{GetType().Name}.db");
        }

        public class BlogContext1 : BlogContext0
        {
            #region BasicManyToManyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts);
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region BasicManyToManyFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)),
                        j => j.HasKey("PostsId", "TagsId"));
            }
            #endregion
        }
    }

    public class UnidirectionalManyToMany
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Basic, unidirectional many-to-many:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region UnidirectionalManyToMany
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = UnidirectionalManyToMany{GetType().Name}.db");

            #region UnidirectionalManyToManyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region UnidirectionalManyToManyFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany()
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId").HasPrincipalKey(nameof(Post.Id)),
                        j => j.HasKey("PostId", "TagsId"));
            }
            #endregion
        }
    }

    public class ManyToManyNamedJoinTable
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Basic, bidirectional many-to-many with explicitly named join table:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyNamedJoinTable
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyNamedJoinTable{GetType().Name}.db");

            #region ManyToManyNamedJoinTableConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity("PostsToTagsJoinTable");
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyNamedJoinTableFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostsToTagsJoinTable",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)),
                        j => j.HasKey("PostsId", "TagsId"));
            }
            #endregion
        }
    }

    public class ManyToManyNamedForeignKeyColumns
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Basic, bidirectional many-to-many with explicitly named foreign key columns:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        #region ManyToManyNamedForeignKeyColumns
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyNamedForeignKeyColumns{GetType().Name}.db");

            #region ManyToManyNamedForeignKeyColumnsConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagForeignKey"),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostForeignKey"));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyNamedForeignKeyColumnsFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagForeignKey").HasPrincipalKey(nameof(Tag.Id)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostForeignKey").HasPrincipalKey(nameof(Post.Id)),
                        j => j.HasKey("PostForeignKey", "TagForeignKey"));
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region ManyToManyNamedForeignKeyColumnsAlternateConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        j =>
                        {
                            j.Property("PostsId").HasColumnName("PostForeignKey");
                            j.Property("TagsId").HasColumnName("TagForeignKey");
                        });
            }
            #endregion
        }
    }

    public class ManyToManyWithJoinClass
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with join table mapped to an entity type class:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        #region ManyToManyWithJoinClass
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithJoinClass{GetType().Name}.db");

            #region ManyToManyWithJoinClassConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithJoinClassFkConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId),
                        r => r.HasOne<Post>().WithMany().HasForeignKey(e => e.PostId));
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region ManyToManyWithJoinClassFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>().WithMany().HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.Id),
                        j => j.HasKey(e => new { e.PostId, e.TagId }));
            }
            #endregion
        }
    }

    public class ManyToManyWithNavsToJoinClass
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with navigations to the join entity type:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        #region ManyToManyWithNavsToJoinClass
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNavsToJoinClass{GetType().Name}.db");

            #region ManyToManyWithNavsToJoinClassConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithNavsToJoinClassWithNavConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne<Tag>().WithMany(e => e.PostTags),
                        r => r.HasOne<Post>().WithMany(e => e.PostTags));
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region ManyToManyWithNavsToJoinClassFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany(e => e.PostTags).HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>().WithMany(e => e.PostTags).HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.Id),
                        j => j.HasKey(e => new { e.PostId, e.TagId }));
            }
            #endregion
        }
    }

    public class ManyToManyWithNavsToAndFromJoinClass
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with navigations to and from the join entity type:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            using var context2 = new BlogContext2();
            Debug.Assert(schema == context2.Database.GenerateCreateScript());
        }

        #region ManyToManyWithNavsToAndFromJoinClass
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNavsToAndFromJoinClass{GetType().Name}.db");

            #region ManyToManyWithNavsToAndFromJoinClassConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithNavsToAndFromJoinClassWithNavConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags));
            }
            #endregion
        }

        public class BlogContext2 : BlogContext0
        {
            #region ManyToManyWithNavsToAndFromJoinClassFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.Id),
                        j => j.HasKey(e => new { e.PostId, e.TagId }));
            }
            #endregion
        }
    }

    public class ManyToManyWithNamedFksAndNavsToAndFromJoinClass
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with named foreign key properties navigations to and from the join entity type:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithNamedFksAndNavsToAndFromJoinClass
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostForeignKey { get; set; }
            public int TagForeignKey { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNamedFksAndNavsToAndFromJoinClass{GetType().Name}.db");

            #region ManyToManyWithNamedFksAndNavsToAndFromJoinClassConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasForeignKey(e => e.TagForeignKey),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasForeignKey(e => e.PostForeignKey));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithNavsToAndFromJoinClassFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasForeignKey(e => e.TagForeignKey).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasForeignKey(e => e.PostForeignKey).HasPrincipalKey(e => e.Id),
                        j => j.HasKey(e => new { e.PostForeignKey, e.TagForeignKey }));
            }
            #endregion
        }
    }

    public class ManyToManyAlternateKeys
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Basic, bidirectional many-to-many using alternate keys on either side of the relationship:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyAlternateKeys
        public class Post
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyAlternateKeys{GetType().Name}.db");

            #region ManyToManyAlternateKeysConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        l => l.HasOne(typeof(Tag)).WithMany().HasPrincipalKey(nameof(Tag.AlternateKey)),
                        r => r.HasOne(typeof(Post)).WithMany().HasPrincipalKey(nameof(Post.AlternateKey)));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyAlternateKeysFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsAlternateKey").HasPrincipalKey(nameof(Tag.AlternateKey)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsAlternateKey").HasPrincipalKey(nameof(Post.AlternateKey)),
                        j => j.HasKey("PostsAlternateKey", "TagsAlternateKey"));
            }
            #endregion
        }
    }

    public class ManyToManyWithNavsAndAlternateKeys
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with alternate keys and navigations to and from the join entity type:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithNavsAndAlternateKeys
        public class Post
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Post> Posts { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNavsAndAlternateKeys{GetType().Name}.db");

            #region ManyToManyWithNavsAndAlternateKeysConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasPrincipalKey(e => e.AlternateKey),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasPrincipalKey(e => e.AlternateKey));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithNavsAndAlternateKeysFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.AlternateKey),
                        r => r.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.AlternateKey),
                        j => j.HasKey(e => new { e.PostId, e.TagId }));
            }
            #endregion
        }
    }

    public class ManyToManyWithJoinClassHavingPrimaryKey
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with join table mapped to an entity type class, which has a separate primary key property:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithJoinClassHavingPrimaryKey
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }

        public class PostTag
        {
            public int Id { get; set; }
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithJoinClassHavingPrimaryKey{GetType().Name}.db");

            #region ManyToManyWithJoinClassHavingPrimaryKeyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithJoinClassHavingPrimaryKeyFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>().WithMany().HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.Id),
                        j => j.HasKey(e => e.Id));
            }
            #endregion
        }
    }

    public class ManyToManyWithPrimaryKeyInJoinEntity
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with separate primary key property in the join entity:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithPrimaryKeyInJoinEntity
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithPrimaryKeyInJoinEntity{GetType().Name}.db");

            #region ManyToManyWithPrimaryKeyInJoinEntityConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        j =>
                        {
                            j.IndexerProperty<int>("Id");
                            j.HasKey("Id");
                        });
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithPrimaryKeyInJoinEntityFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)),
                        j =>
                        {
                            j.IndexerProperty<int>("Id");
                            j.HasKey("Id");
                        });
            }
            #endregion
        }
    }

    public class ManyToManyWithPayloadAndNavsToJoinClass
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many with navigations to a join entity type with payload:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithPayloadAndNavsToJoinClass
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public DateTime CreatedOn { get; set; }
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithPayloadAndNavsToJoinClass{GetType().Name}.db");

            #region ManyToManyWithPayloadAndNavsToJoinClassConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithPayloadAndNavsToJoinClassFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany(e => e.PostTags).HasForeignKey(e => e.TagId).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>().WithMany(e => e.PostTags).HasForeignKey(e => e.PostId).HasPrincipalKey(e => e.Id),
                        j =>
                        {
                            j.HasKey(e => new { e.PostId, e.TagId });
                            j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        });
            }
            #endregion
        }
    }

    public class ManyToManyWithNoCascadeDelete
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Bidirectional many-to-many without cascade delete:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithNoCascadeDelete
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNoCascadeDelete{GetType().Name}.db");

            #region ManyToManyWithNoCascadeDeleteConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        l => l.HasOne(typeof(Tag)).WithMany().OnDelete(DeleteBehavior.Restrict),
                        r => r.HasOne(typeof(Post)).WithMany().OnDelete(DeleteBehavior.Restrict));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithNoCascadeDeleteFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)).OnDelete(DeleteBehavior.Restrict),
                        r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)).OnDelete(DeleteBehavior.Restrict),
                        j => j.HasKey("PostsId", "TagsId"));
            }
            #endregion
        }
    }

    public class SelfReferencingManyToMany
    {
        public static void BuildModels()
        {
            using var context0 = new PeopleContext0();

            Console.WriteLine("Bidirectional self-referencing many-to-many:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new PeopleContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region SelfReferencingManyToMany
        public class Person
        {
            public int Id { get; set; }
            public List<Person> Parents { get; } = [];
            public List<Person> Children { get; } = [];
        }
        #endregion

        public class PeopleContext0 : DbContext
        {
            public DbSet<Person> People => Set<Person>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNoCascadeDelete{GetType().Name}.db");
        }

        public class PeopleContext1 : PeopleContext0
        {
            #region SelfReferencingManyToManyFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasMany(e => e.Children)
                    .WithMany(e => e.Parents)
                    .UsingEntity(
                        "PersonPerson",
                        l => l.HasOne(typeof(Person)).WithMany().HasForeignKey("ChildrenId").HasPrincipalKey(nameof(Person.Id)),
                        r => r.HasOne(typeof(Person)).WithMany().HasForeignKey("ParentsId").HasPrincipalKey(nameof(Person.Id)),
                        j => j.HasKey("ChildrenId", "ParentsId"));
            }
            #endregion
        }
    }

    public class SelfReferencingUnidirectionalManyToMany
    {
        public static void BuildModels()
        {
            using var context0 = new PeopleContext0();

            Console.WriteLine("Unidirectional self-referencing many-to-many:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new PeopleContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());

            var ginny = new Person();
            var hermione = new Person();
            ginny.Friends.Add(hermione);
            hermione.Friends.Add(ginny);
        }

        #region SelfReferencingUnidirectionalManyToMany
        public class Person
        {
            public int Id { get; set; }
            public List<Person> Friends { get; } = [];
        }
        #endregion

        public class PeopleContext0 : DbContext
        {
            public DbSet<Person> People => Set<Person>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithNoCascadeDelete{GetType().Name}.db");

            #region SelfReferencingUnidirectionalManyToManyConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasMany(e => e.Friends)
                    .WithMany();
            }
            #endregion
        }

        public class PeopleContext1 : PeopleContext0
        {
            #region SelfReferencingUnidirectionalManyToManyFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasMany(e => e.Friends)
                    .WithMany()
                    .UsingEntity(
                        "PersonPerson",
                        l => l.HasOne(typeof(Person)).WithMany().HasForeignKey("FriendsId").HasPrincipalKey(nameof(Person.Id)),
                        r => r.HasOne(typeof(Person)).WithMany().HasForeignKey("PersonId").HasPrincipalKey(nameof(Person.Id)),
                        j => j.HasKey("FriendsId", "PersonId"));
            }
            #endregion
        }
    }

    public class ManyToManyWithCustomSharedTypeEntityType
    {
        public static void BuildModels()
        {
            using var context0 = new BlogContext0();

            Console.WriteLine("Many-to-many with a custom shared type entity type as a join entity type with payload:");
            Console.WriteLine(context0.Model.ToDebugString(indent: 2));
            Console.WriteLine();

            var schema = context0.Database.GenerateCreateScript();
            Console.WriteLine(schema);
            Console.WriteLine();

            using var context1 = new BlogContext1();
            Debug.Assert(schema == context1.Database.GenerateCreateScript());
        }

        #region ManyToManyWithCustomSharedTypeEntityType
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = [];
            public List<JoinType> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = [];
            public List<JoinType> PostTags { get; } = [];
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Author> Authors { get; } = [];
            public List<JoinType> BlogAuthors { get; } = [];
        }

        public class Author
        {
            public int Id { get; set; }
            public List<Blog> Blogs { get; } = [];
            public List<JoinType> BlogAuthors { get; } = [];
        }
        #endregion

        public class JoinType
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();
            public DbSet<Blog> Blogs => Set<Blog>();
            public DbSet<Author> Authors => Set<Author>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManyWithPayloadAndNavsToJoinClass{GetType().Name}.db");

            #region ManyToManyWithCustomSharedTypeEntityTypeConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<JoinType>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id1),
                        r => r.HasOne<Post>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id2),
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));

                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Authors)
                    .WithMany(e => e.Blogs)
                    .UsingEntity<JoinType>(
                        "BlogAuthor",
                        l => l.HasOne<Author>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id1),
                        r => r.HasOne<Blog>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id2),
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
            }
            #endregion
        }

        public class BlogContext1 : BlogContext0
        {
            #region ManyToManyWithCustomSharedTypeEntityTypeFullConfig
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<JoinType>(
                        "PostTag",
                        l => l.HasOne<Tag>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id1).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Post>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id2).HasPrincipalKey(e => e.Id),
                        j =>
                        {
                            j.HasKey(e => new { e.Id1, e.Id2 });
                            j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        });

                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Authors)
                    .WithMany(e => e.Blogs)
                    .UsingEntity<JoinType>(
                        "BlogAuthor",
                        l => l.HasOne<Author>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id1).HasPrincipalKey(e => e.Id),
                        r => r.HasOne<Blog>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id2).HasPrincipalKey(e => e.Id),
                        j =>
                        {
                            j.HasKey(e => new { e.Id1, e.Id2 });
                            j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        });
            }
            #endregion
        }
    }

    public class DirectJoinTableNoManyToMany
    {
        public static async Task BuildModels()
        {
            using var context0 = new BlogContext0();
            await context0.Database.EnsureDeletedAsync();
            await context0.Database.EnsureCreatedAsync();

            Console.WriteLine("Directly mapping the join table without creating a many-to-many relationship:");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();
        }

        #region DirectJoinTableMapping
        public class Post
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = [];
        }

        public class PostTag
        {
            public int PostsId { get; set; }
            public int TagsId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManySimple{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>().HasMany(e => e.PostTags).WithOne(e => e.Post).HasForeignKey(e => e.PostsId);
                modelBuilder.Entity<Tag>().HasMany(e => e.PostTags).WithOne(e => e.Tag).HasForeignKey(e => e.TagsId);
                modelBuilder.Entity<PostTag>().HasKey(e => new { e.PostsId, e.TagsId });
            }
        }
    }

    public class FullMappingWithJoinEntity
    {
        public static async Task BuildModels()
        {
            using var context0 = new BlogContext0();
            await context0.Database.EnsureDeletedAsync();
            await context0.Database.EnsureCreatedAsync();

            Console.WriteLine("Directly mapping the join table and including skip navigations for the many-to-many relationship:");
            var modelDebugString = context0.Model.ToDebugString(indent: 2);
            Console.WriteLine(modelDebugString);
            Console.WriteLine();
        }

        #region FullMappingWithJoinEntity
        public class Post
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = [];
            public List<Tag> Tags { get; } = [];
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = [];
            public List<Post> Posts { get; } = [];
        }

        public class PostTag
        {
            public int PostsId { get; set; }
            public int TagsId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
        #endregion

        public class BlogContext0 : DbContext
        {
            public DbSet<Post> Posts => Set<Post>();
            public DbSet<Tag> Tags => Set<Tag>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite($"DataSource = ManyToManySimple{GetType().Name}.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        l => l.HasOne(e => e.Tag).WithMany(e => e.PostTags),
                        r => r.HasOne(e => e.Post).WithMany(e => e.PostTags));
            }
        }
    }
}
