using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BenchCommon
{
    [Index(nameof(AuthorId), Name = "IX_Posts_AuthorId")]
    [Index(nameof(BlogId), Name = "IX_Posts_BlogId")]
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public int BlogId { get; set; }
        [ForeignKey(nameof(BlogId))]
        //[InverseProperty(nameof(Blog.Posts))]
        public Blog Blog { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public int AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        [InverseProperty(nameof(Person.AuthoredPosts))]
        public Person Author { get; set; }
    }

}
