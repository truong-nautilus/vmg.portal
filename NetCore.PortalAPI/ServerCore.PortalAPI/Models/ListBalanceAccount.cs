namespace ServerCore.PortalAPI.Models
{
    public class ListBalanceAccount
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
        public int DecimalDigits { get; set; }
    }
}
