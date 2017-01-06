namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
	public class HashesInfo
	{
		//TODO: implement smart loading, immediate results and delayed loading in the background
		public int Count { get; set; }
		public int KeyLength { get; set; }
		public string Alphabet { get; set; }
		public bool IsCalculating { get; set; }
	}
}