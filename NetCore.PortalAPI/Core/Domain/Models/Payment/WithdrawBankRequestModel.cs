namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class WithdrawBankRequestModel
    {
        public long accountId { get; set; }
        public string accountName { get; set; }
        public string bankCode { get; set; }
        public string bankAccount { get; set; }
        public string bankAccountName { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
    }
}
