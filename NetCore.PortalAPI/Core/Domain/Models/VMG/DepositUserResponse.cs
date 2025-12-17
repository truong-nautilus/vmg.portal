namespace ServerCore.PortalAPI.Core.Domain.Models.VMG
{
    public class DepositUserResponse
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public long TxID { get; set; }
    }
}
