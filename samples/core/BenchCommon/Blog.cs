using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchCommon
{
    public class Blog : IEquatable<Blog>
    {
        public const int baseID = 1;
        [Key]       // EF supposes IDENTITY(1,1) in relational db (e.g. MSSQL)
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]    // default
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]        // alt if app generates #
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime CreationTime { get; set; }
        public int Rating { get; set; }

        [InverseProperty("Blog")]
        public virtual ICollection<Post> Posts { get; } = new HashSet<Post>();

        public bool Equals(Blog other) => BlogId == other?.BlogId;

        public override bool Equals(object obj) => Equals(obj as Blog);

        public override int GetHashCode() => BlogId.GetHashCode();
    }

}
