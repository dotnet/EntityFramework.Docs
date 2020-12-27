using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchCommon
{
    public class Person
    {
        public int PersonId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        [InverseProperty("Author")]
        public virtual ICollection<Post> AuthoredPosts { get; } = new HashSet<Post>();
    }

}
