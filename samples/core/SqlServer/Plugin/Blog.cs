using System.ComponentModel.DataAnnotations.Schema;

namespace SqlServer.Plugin;
public class Blog
{
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
    public int BlogId { get; set; }
    public string Url { get; set; }
}
