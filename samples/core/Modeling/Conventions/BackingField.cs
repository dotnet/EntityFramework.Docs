using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFModeling.Conventions.BackingField
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
    }

    #region Sample
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
    #endregion
}
