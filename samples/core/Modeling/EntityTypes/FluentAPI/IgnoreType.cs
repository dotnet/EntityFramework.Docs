using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityTypes.FluentAPI.IgnoreType;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region IgnoreType
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BlogMetadata>();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public BlogMetadata Metadata { get; set; }
}

public class BlogMetadata
{
    public DateTime LoadedFromDatabase { get; set; }
}