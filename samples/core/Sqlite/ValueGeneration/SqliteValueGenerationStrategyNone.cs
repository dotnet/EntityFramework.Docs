using Microsoft.EntityFrameworkCore;

namespace EFCore.Sqlite.ValueGeneration;

internal class SqliteValueGenerationStrategyNoneContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    #region SqliteValueGenerationStrategyNone
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .Property(p => p.Id)
            .HasAnnotation("Sqlite:ValueGenerationStrategy", "None");
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