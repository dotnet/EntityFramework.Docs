using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	[Table("hashes")]//does not work for MySQL, works for SqlServer
	public partial class Hashes
	{
		[Key]
		[Required]
		//[MinLength(20)]
		//[MaxLength(20)]
		//[StringLength(maximumLength: 20, MinimumLength = 20)]
		[Column("key", TypeName = "varchar(20)")]//does not work for MySQL, works for SqlServer
		public string Key { get; set; }

		[Required]
		//[MinLength(32)]
		//[MaxLength(32)]
		//[StringLength(maximumLength:32, MinimumLength = 32)]
		[Column("hashMD5", TypeName = "char(32)")]//does not work for MySQL, works for SqlServer
		public string HashMD5 { get; set; }

		[Required]
		//[MinLength(64)]
		//[MaxLength(64)]
		//[StringLength(maximumLength: 64, MinimumLength = 64)]
		[Column("hashSHA256", TypeName = "char(64)")]//does not work for MySQL, works for SqlServer
		public string HashSHA256 { get; set; }
	}
}