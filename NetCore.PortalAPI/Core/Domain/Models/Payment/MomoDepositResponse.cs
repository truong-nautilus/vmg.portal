namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class MomoDepositResponse
    {
        public int status { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public long amount { get; set; }
        public string redirectLink { get; set; }
        public string deepLink { get; set; }
    }
}
