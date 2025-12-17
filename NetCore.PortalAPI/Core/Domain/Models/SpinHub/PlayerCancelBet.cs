namespace ServerCore.PortalAPI.Core.Domain.Models.SpinHub
{
	public class PlayerCancelBet
	{
		public string TxId { get; set; }
		public string UserName { get; set; }
		public string Currency { get; set; }
		public long Coin { get; set; }
	}
}
