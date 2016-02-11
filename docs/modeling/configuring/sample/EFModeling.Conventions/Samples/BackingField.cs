using Microsoft.Data.Entity;
using System.Collections.Generic;

namespace EFModeling.Conventions.Samples.BackingField
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.Url)
                .HasAnnotation("BackingField", "_blogUrl");
        }
    }

    public class Blog
    {
        private string _url;

        public int BlogId { get; set; }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
