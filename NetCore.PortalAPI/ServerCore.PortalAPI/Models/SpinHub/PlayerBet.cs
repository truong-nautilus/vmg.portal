namespace ServerCore.PortalAPI.Models.SpinHub
{
	public class PlayerBet
	{
		public string TxId { get; set; }
		public string UserName { get; set; }
		public string Currency { get; set;}
		public long Coin { get; set;}
		public int RTPType { get; set;}
	}
}
