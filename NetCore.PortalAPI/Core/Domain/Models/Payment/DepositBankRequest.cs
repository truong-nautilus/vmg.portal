namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class DepositBankRequest
    {
        public int amount { get; set; }
        public int requestType { get; set; }
        public long merchantID { get; set; }
        public string bankCode { get; set; }
        public string requestID { get; set; }
        public string callBack { get; set; }
    }
}
