using System;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.BusinessLogic;

public class BloggingContext : DbContext
{
    private readonly Action<BloggingContext, ModelBuilder> _modelCustomizer;

    #region Constructors
    public BloggingContext()
    {
    }

    public BloggingContext(DbContextOptions<BloggingContext> options, Action<BloggingContext, ModelBuilder> modelCustomizer = null)
        : base(options)
    {
        _modelCustomizer = modelCustomizer;
    }
    #endregion

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<UrlResource> UrlResources => Set<UrlResource>();

    #region OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlResource>().HasNoKey()
            .ToView("AllResources");

        if (_modelCustomizer is not null)
        {
            _modelCustomizer(this, modelBuilder);
        }
    }
}
