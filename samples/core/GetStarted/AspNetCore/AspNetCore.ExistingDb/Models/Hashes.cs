using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public enum KindEnum
	{
		MD5 = 0,
		SHA256 = 1
	}

	[Table("hashes")]//does not work for MySQL, works for SqlServer
	public partial class Hashes
	{
		[Key]
		[Required]
		[Column("key", TypeName = "varchar(20)")]//does not work for MySQL, works for SqlServer
		public string Key { get; set; }

		[Required]
		[Column("hashMD5", TypeName = "char(32)")]//does not work for MySQL, works for SqlServer
		public string HashMD5 { get; set; }

		[Required]
		[Column("hashSHA256", TypeName = "char(64)")]//does not work for MySQL, works for SqlServer
		public string HashSHA256 { get; set; }


		[Required]
		[HashLength]
		[NotMapped]
		public string Search { get; set; }

		[Required]
		[NotMapped]
		public KindEnum Kind { get; set; }
	}

	public class HashesInfo
	{
		//TODO: implement smart loading, immediate results and delayed loading in the background
		public int Count { get; set; } = 0;
		public int KeyLength { get; set; }
		public string Alphabet { get; set; }
		public bool IsCalculating { get; set; }
	}
}