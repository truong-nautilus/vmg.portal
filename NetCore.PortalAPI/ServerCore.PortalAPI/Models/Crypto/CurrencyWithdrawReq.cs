namespace ServerCore.PortalAPI.Models.Crypto
{
    public class CurrencyWithdrawReq
    {
        public int CurrencyId { get; set; }
        public int ChainId { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string RechargePassword { get; set; }
    }
}
