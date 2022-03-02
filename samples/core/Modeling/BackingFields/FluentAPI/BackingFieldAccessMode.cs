using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.BackingFields.FluentAPI.BackingFieldAccessMode;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region BackingFieldAccessMode
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Url)
            .HasField("_validatedUrl")
            .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }

    public string Url { get; private set; }

    public void SetUrl(string url)
    {
        using (var client = new HttpClient())
        {
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
        }

        Url = url;
    }
}