using System;

namespace SqlServer.Columns;

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public DateTime PublishedOn { get; set; }
}