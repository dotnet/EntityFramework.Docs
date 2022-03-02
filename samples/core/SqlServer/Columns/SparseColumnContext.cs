using Microsoft.EntityFrameworkCore;

namespace SqlServer.Columns;

public class SparseColumnContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<RareBlog> RareBlogs { get; set; }

    #region SparseColumn
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RareBlog>()
            .Property(b => b.RareProperty)
            .IsSparse();
    }
    #endregion
}