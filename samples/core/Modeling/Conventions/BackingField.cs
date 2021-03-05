using Microsoft.EntityFrameworkCore;

namespace EFModeling.Conventions.BackingField
{
    internal class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region Sample
    public class Blog
    {
        private string _url;
        
        public int BlogId { get; set; }

        public string Url { get; set; }
    }
    #endregion
}
