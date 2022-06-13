
using Microsoft.EntityFrameworkCore;

namespace SqlServer.Faq;

public class TriggersContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region TriggerConfiguration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .ToTable(tb => tb.HasTrigger("SomeTrigger"));
    }
    #endregion
}
