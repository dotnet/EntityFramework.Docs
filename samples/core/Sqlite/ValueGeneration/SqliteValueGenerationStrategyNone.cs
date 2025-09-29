using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Metadata;

namespace EFCore.Sqlite.ValueGeneration;

public class SqliteValueGenerationStrategyNoneContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    #region SqliteValueGenerationStrategyNone
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .Property(p => p.Id)
            .Metadata.SetValueGenerationStrategy(SqliteValueGenerationStrategy.None);
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sample.db");
}

public class Post
{
    public int Id { get; set; }
    public string Content { get; set; }
}