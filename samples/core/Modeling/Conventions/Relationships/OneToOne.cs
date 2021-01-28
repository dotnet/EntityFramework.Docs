using Microsoft.EntityFrameworkCore;

namespace EFModeling.Conventions.Relationships.OneToOne
{
    internal class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
    }

    #region OneToOne
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public BlogImage BlogImage { get; set; }
    }

    public class BlogImage
    {
        public int BlogImageId { get; set; }
        public byte[] Image { get; set; }
        public string Caption { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
    #endregion
}
