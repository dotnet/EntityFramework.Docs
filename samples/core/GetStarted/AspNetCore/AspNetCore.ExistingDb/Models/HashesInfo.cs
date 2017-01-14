using System.ComponentModel.DataAnnotations;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public enum KindEnum
	{
		MD5 = 0,
		SHA256 = 1
	}

	public class HashesInfo
	{
		//TODO: implement smart loading, immediate results and delayed loading in the background
		public int Count { get; set; }
		public int KeyLength { get; set; }
		public string Alphabet { get; set; }
		public bool IsCalculating { get; set; }

		[Required]
		[HashLength]
		public string Search { get; set; }
		[Required]
		public KindEnum Kind { get; set; }
	}
}