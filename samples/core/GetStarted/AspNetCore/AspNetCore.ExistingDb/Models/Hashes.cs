using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	[Table("hashes")]
	public partial class Hashes
	{
		[Key]
		[Column("key", TypeName = "varchar(20)")]
		public string Key { get; set; }

		[Column("hashMD5", TypeName = "char(32)")]
		public string HashMD5 { get; set; }
		
		[Column("hashSHA256", TypeName = "char(64)")]
		public string HashSHA256 { get; set; }
	}
}