namespace ServerCore.PortalAPI.Models.Crypto
{
    public class TatumDepositReq
    {
        public string Currency { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public int BlockNumber { get; set; }
        public string CounterAddress { get; set; }
        public string TxId { get; set; }
        public string Chain { get; set; }
        public string SubscriptionType { get; set; }
        public string ContractAddress { get; set; }
    }
}
