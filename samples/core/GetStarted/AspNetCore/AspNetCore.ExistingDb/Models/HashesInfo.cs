namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public class HashesInfo
	{
		//TODO: implement smart loading, immediate results and delayed loading in the background
		public int Count { get; set; } = 0;
		public int KeyLength { get; set; } = 0;
		public string Alphabet { get; set; }
	}
}