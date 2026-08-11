namespace MigrationBundle;

public class Blog
{
    public int Id { get; set; }

    public required string Url { get; set; }

    public bool IsActive { get; set; }
}
