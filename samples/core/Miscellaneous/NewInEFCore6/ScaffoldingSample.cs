using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable enable

public static class ScaffoldingSample
{
    public static async Task Reverse_engineer_from_database()
    {
        Console.WriteLine($">>>> Sample: {nameof(Reverse_engineer_from_database)}");
        Console.WriteLine();

        using var context = new BloggingWithNRTsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    // The code below was scaffolded from an existing database.
    // To replicate this, run this sample to create the database and then use the command line as documented in the text.

    #region Blog
    public partial class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }
    #endregion

    #region Post
    public partial class Post
    {
        public Post()
        {
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Contents { get; set; } = null!;
        public DateTime PostedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; } = null!;

        public virtual ICollection<Tag> Tags { get; set; }
    }
    #endregion

    #region Tag
    public partial class Tag
    {
        public Tag()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
    #endregion

    public partial class BloggingWithNRTsContext : DbContext
    {
        public BloggingWithNRTsContext()
        {
        }

        public BloggingWithNRTsContext(DbContextOptions<BloggingWithNRTsContext> options)
            : base(options)
        {
        }

        #region DbSets
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BloggingWithNRTs");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(
                entity =>
                    {
                        entity.HasIndex(e => e.BlogId, "IX_Posts_BlogId");

                        entity.HasOne(d => d.Blog)
                            .WithMany(p => p.Posts)
                            .HasForeignKey(d => d.BlogId)
                            .OnDelete(DeleteBehavior.ClientSetNull);

                        #region ManyToMany
                        entity.HasMany(d => d.Tags)
                            .WithMany(p => p.Posts)
                            .UsingEntity<Dictionary<string, object>>(
                                "PostTag",
                                l => l.HasOne<Tag>().WithMany().HasForeignKey("PostsId"),
                                r => r.HasOne<Post>().WithMany().HasForeignKey("TagsId"),
                                j =>
                                    {
                                        j.HasKey("PostsId", "TagsId");
                                        j.ToTable("PostTag");
                                        j.HasIndex(new[] { "TagsId" }, "IX_PostTag_TagsId");
                                    });
                        #endregion
                    });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
