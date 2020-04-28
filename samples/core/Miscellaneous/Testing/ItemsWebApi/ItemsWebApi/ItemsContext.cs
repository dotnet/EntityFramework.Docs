using Microsoft.EntityFrameworkCore;

namespace Items
{
    public class ItemsContext : DbContext
    {
        public ItemsContext(DbContextOptions options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureItem
            modelBuilder.Entity<Item>(
                b =>
                    {
                        b.Property("_id");
                        b.HasKey("_id");
                        b.Property(e => e.Name);
                        b.HasMany(e => e.Tags).WithOne().IsRequired();
                    });
            #endregion

            #region ConfigureTag
            modelBuilder.Entity<Tag>(
                b =>
                    {
                        b.Property("_id");
                        b.HasKey("_id");
                        b.Property(e => e.Label);
                    });
            #endregion
        }
    }
}