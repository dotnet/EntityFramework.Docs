using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public partial class Blog
    {
        public Blog()
        {
            Post = new HashSet<Post>();
        }

		[Required]
		[Key]
        public int BlogId { get; set; }
		[Required]
        public string Url { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
