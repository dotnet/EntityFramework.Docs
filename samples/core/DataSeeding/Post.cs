namespace EFModeling.Samples.DataSeeding
{
    public class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
