using Microsoft.EntityFrameworkCore;

namespace EFCore.Sqlite.ValueGeneration;

public class SqliteValueGeneratedNeverContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region SqliteValueGeneratedNever
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .ValueGeneratedNever();
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sample.db");
}
