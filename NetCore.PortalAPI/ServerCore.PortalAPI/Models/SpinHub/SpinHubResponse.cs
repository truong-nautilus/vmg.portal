using Newtonsoft.Json;

namespace ServerCore.PortalAPI.Models.SpinHub
{
	public class SpinHubResponse
	{
		[JsonProperty("ErrorCode")]
		public int ErrorCode { get; set; }
		[JsonProperty("Result")]
		public bool Result { get; set; }
		[JsonProperty("Message")]
		public string Message { get; set; }
		[JsonProperty("Data")]
		public string Data { get; set; }
	}
}
