using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Relationships.FluentAPI.ManyToManyShared;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region SharedConfiguration
        modelBuilder
            .Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Posts)
            .UsingEntity(j => j.ToTable("PostTags"));
        #endregion

        #region Metadata
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var skipNavigation in entityType.GetSkipNavigations())
            {
                Console.WriteLine(entityType.DisplayName() + "." + skipNavigation.Name);
            }
        }
        #endregion

        #region Seeding
        modelBuilder
            .Entity<Post>()
            .HasData(new Post { PostId = 1, Title = "First" });

        modelBuilder
            .Entity<Tag>()
            .HasData(new Tag { TagId = "ef" });

        modelBuilder
            .Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Posts)
            .UsingEntity(j => j.HasData(new { PostsPostId = 1, TagsTagId = "ef" }));
        #endregion

        #region Components
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Posts)
            .UsingEntity<Dictionary<string, object>>(
                "PostTag",
                j => j
                    .HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("TagsId")
                    .HasConstraintName("FK_PostTag_Tags_TagId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Post>()
                    .WithMany()
                    .HasForeignKey("PostsId")
                    .HasConstraintName("FK_PostTag_Posts_PostId")
                    .OnDelete(DeleteBehavior.ClientCascade));
        #endregion
    }
}

#region ManyToManyShared
public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public ICollection<Tag> Tags { get; set; }
}

public class Tag
{
    public string TagId { get; set; }

    public ICollection<Post> Posts { get; set; }
}
#endregion
