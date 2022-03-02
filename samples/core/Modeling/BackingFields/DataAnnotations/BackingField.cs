using Microsoft.EntityFrameworkCore;

namespace EFModeling.BackingFields.DataAnnotations.BackingField;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

#region BackingField
public class Blog
{
    private string _validatedUrl;

    public int BlogId { get; set; }

    [BackingField(nameof(_validatedUrl))]
    public string Url
    {
        get { return _validatedUrl; }
    }

    public void SetUrl(string url)
    {
        // put your validation code here

        _validatedUrl = url;
    }
}
#endregion