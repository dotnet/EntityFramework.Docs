using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.ShadowAndIndexerProperties.IndexerProperty;

#region IndexerProperty
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Blog>().IndexerProperty<DateTime>("LastUpdated");
}

public class Blog
{
    readonly Dictionary<string, object> _data = [];
    public int BlogId { get; set; }

    public object this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }
}
#endregion
