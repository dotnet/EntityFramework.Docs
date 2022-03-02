using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.ValueGeneratedOnAddOrUpdate;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ValueGeneratedOnAddOrUpdate
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.LastUpdated)
            .ValueGeneratedOnAddOrUpdate();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public DateTime LastUpdated { get; set; }
}