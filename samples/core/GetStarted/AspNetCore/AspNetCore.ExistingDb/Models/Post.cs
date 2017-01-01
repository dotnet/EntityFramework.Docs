using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
    public partial class Post
    {
		[Required]
        public int PostId { get; set; }
		[Required]
		public int BlogId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
