﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFModeling.Inheritance.FluentAPI.TPCConfiguration;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region TPCConfiguration
        modelBuilder.Entity<Blog>().UseTpcMappingStrategy()
            .ToTable("Blogs");
        modelBuilder.Entity<RssBlog>()
            .ToTable("RssBlogs");
        #endregion

        #region Metadata
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            StoreObjectIdentifier? tableIdentifier = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);

            Console.WriteLine($"{entityType.DisplayName()}\t\t{tableIdentifier}");
            Console.WriteLine(" Property\tColumn");

            foreach (IMutableProperty property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(tableIdentifier.Value);
                Console.WriteLine($" {property.Name,-10}\t{columnName}");
            }

            Console.WriteLine();
        }
        #endregion
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}