using Microsoft.EntityFrameworkCore;

namespace EFModeling.KeylessEntityTypes.FluentAPI.Keyless;

internal class BlogsContext : DbContext
{
    public DbSet<BlogPostsCount> BlogPostCounts { get; set; }

    #region Keyless
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPostsCount>()
            .HasNoKey();
    }
    #endregion
}

public class BlogPostsCount
{
    public string BlogName { get; set; }
    public int PostCount { get; set; }
}