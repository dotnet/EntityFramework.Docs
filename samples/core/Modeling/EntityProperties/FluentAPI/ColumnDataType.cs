using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.DataType;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ColumnDataType
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(
            eb =>
            {
                eb.Property(b => b.Url).HasColumnType("varchar(200)");
                eb.Property(b => b.Rating).HasColumnType("decimal(5, 2)");
            });
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public decimal Rating { get; set; }
}