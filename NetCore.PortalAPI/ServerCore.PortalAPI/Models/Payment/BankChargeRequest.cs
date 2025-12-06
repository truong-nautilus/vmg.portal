namespace ServerCore.PortalAPI.Models.Payment
{
    public class BankChargeRequest
    {
        public long accountId { get; set; }
        public string accountName { get; set; }
        public string bankCode { get; set; }
        public int amount { get; set; }
    }
}
