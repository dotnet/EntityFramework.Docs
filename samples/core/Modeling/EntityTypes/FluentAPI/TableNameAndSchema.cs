using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.FluentAPI.TableNameAndSchema;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region TableNameAndSchema
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .ToTable("blogs", schema: "blogging");
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}