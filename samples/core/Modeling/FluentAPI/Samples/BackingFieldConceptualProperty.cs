using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace EFModeling.Configuring.FluentAPI.Samples.BackingFieldConceptualProperty
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region Sample
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property<string>("Url")
                .HasField("_validatedUrl");
        }
        #endregion
    }

    public class Blog
    {
        private string _validatedUrl;

        public int BlogId { get; set; }

        public string GetUrl()
        {
            return _validatedUrl; 
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
