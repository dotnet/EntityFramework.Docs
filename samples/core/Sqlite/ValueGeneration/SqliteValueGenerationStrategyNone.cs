using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Metadata;

namespace EFCore.Sqlite.ValueGeneration;

public class SqliteValueGenerationStrategyNoneContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region SqliteValueGenerationStrategyNone
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .Metadata.SetValueGenerationStrategy(SqliteValueGenerationStrategy.None);
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sample.db");
}

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
}