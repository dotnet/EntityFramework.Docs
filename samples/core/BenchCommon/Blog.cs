using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchCommon
{
    public class Blog : IEquatable<Blog>
    {
        [Key]
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime CreationTime { get; set; }
        public int Rating { get; set; }

        [InverseProperty("Blog")]
        public virtual ICollection<Post> Posts { get; } = new HashSet<Post>();

        public bool Equals(Blog other) => BlogId == other.BlogId;
    }

}
