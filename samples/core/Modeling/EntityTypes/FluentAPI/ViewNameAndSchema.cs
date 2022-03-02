using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.FluentAPI.ViewNameAndSchema;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region ViewNameAndSchema
        modelBuilder.Entity<Blog>()
            .ToView("blogsView", schema: "blogging");
        #endregion
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}