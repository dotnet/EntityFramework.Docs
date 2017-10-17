using Microsoft.EntityFrameworkCore;

namespace EFModeling.Conventions.Samples.InheritanceModelBuilder
{
    #region Context
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RssBlog>().HasBaseType<Blog>();
        }
    }
    #endregion

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }

    public class RssBlog : Blog
    {
        public string RssUrl { get; set; }
    }
}
