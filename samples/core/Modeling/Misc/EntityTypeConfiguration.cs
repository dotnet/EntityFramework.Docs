using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModeling.Misc.EntityTypeConfiguration;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region ApplyIEntityTypeConfiguration
        new BlogEntityTypeConfiguration().Configure(modelBuilder.Entity<Blog>());
        #endregion

        #region ApplyConfigurationsFromAssembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogEntityTypeConfiguration).Assembly);
        #endregion
    }
}

#region IEntityTypeConfiguration
public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder
            .Property(b => b.Url)
            .IsRequired();
    }
}
#endregion

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}