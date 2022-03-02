using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.DefaultValueSql;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region DefaultValueSql
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getdate()");
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public DateTime Created { get; set; }
}