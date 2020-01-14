using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace EFModeling.FluentAPI.BackingField
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region BackingField
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.Url)
                .HasField("_validatedUrl");
        }
        #endregion
    }

    public class Blog
    {
        private string _validatedUrl;

        public int BlogId { get; set; }

        public string Url
        {
            get { return _validatedUrl; }
        }

        public void SetUrl(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }

            _validatedUrl = url;
        }
    }
}
