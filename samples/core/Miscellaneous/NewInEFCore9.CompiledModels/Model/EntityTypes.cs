namespace NewInEfCore9;

public class Blog
{
    public int Id { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public Blog Blog { get; set; } = null!;
}
