using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.FluentAPI.IndexInclude;

internal class MyContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    #region IndexInclude
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.Url)
            .IncludeProperties(
                p => new { p.Title, p.PublishedOn });
    }
    #endregion
}

public class Post
{
    public int PostId { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
}