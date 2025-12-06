namespace ServerCore.PortalAPI.Models.Payment
{
    public class MomoDepositRequest
    {
        public string api_key { get; set; }
        public string request_id { get; set; }
        public string amount { get; set; }
        public string url_callback { get; set; }
        public int custom_content { get; set; }
    }
}
