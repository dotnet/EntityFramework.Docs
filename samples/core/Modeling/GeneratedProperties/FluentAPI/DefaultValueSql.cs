using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.DefaultValueSql;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region DefaultValueSql
        modelBuilder.Entity<Blog>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getdate()");
        #endregion

        #region DefaultValueSqlNamed
        modelBuilder.Entity<Blog>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getdate()" , "DF_Blog_IsActive");
        #endregion
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public DateTime Created { get; set; }
}