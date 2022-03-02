using System.Collections.Generic;

namespace EFQuerying.RelatedData;

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public List<Post> AuthoredPosts { get; set; }
    public List<Blog> OwnedBlogs { get; set; }

    public int? PhotoId { get; set; }
    public PersonPhoto Photo { get; set; }
}