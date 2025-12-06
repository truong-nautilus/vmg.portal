using Newtonsoft.Json;

namespace ServerCore.PortalAPI.Models.SpinHub
{
	public class UserBalance
	{
		[JsonProperty("AgentId")]
		public int AgentId { get; set; }
		[JsonProperty("UserID")]
		public int UserID { get; set; }
		[JsonProperty("Nickname")]
		public string Nickname { get; set; }
		[JsonProperty("AccountBalance")]
		public long AccountBalance { get; set; }
	}
}
