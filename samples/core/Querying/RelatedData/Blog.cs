using System.Collections.Generic;

namespace EFQuerying.RelatedData;

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int? Rating { get; set; }

    public List<Post> Posts { get; set; }

    public int OwnerId { get; set; }
    public Person Owner { get; set; }

    public int ThemeId { get; set; }
    public Theme Theme { get; set; }
}