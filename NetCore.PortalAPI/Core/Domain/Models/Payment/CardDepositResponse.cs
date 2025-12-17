namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class CardDepositResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public int trans_code { get; set; }
    }
}
