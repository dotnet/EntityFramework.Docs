using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.ValueGeneratedOnAdd;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ValueGeneratedOnAdd
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Inserted)
            .ValueGeneratedOnAdd();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public DateTime Inserted { get; set; }
}