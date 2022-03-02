using System.Collections.Generic;

namespace EFQuerying.ComplexQuery;

public class Tag
{
    public string TagId { get; set; }

    public List<PostTag> Posts { get; set; }
}