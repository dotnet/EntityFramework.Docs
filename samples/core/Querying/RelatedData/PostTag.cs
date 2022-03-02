namespace EFQuerying.RelatedData;

public class PostTag
{
    public int PostTagId { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }

    public string TagId { get; set; }
    public Tag Tag { get; set; }
}