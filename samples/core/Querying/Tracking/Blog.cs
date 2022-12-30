using System.Collections.Generic;

namespace EFQuerying.Tracking;

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int? Rating { get; set; }
    public List<Post> Posts { get; set; }
}