using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.FluentAPI.TableName;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region TableName
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .ToTable("blogs");
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}