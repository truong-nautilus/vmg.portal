namespace ServerCore.PortalAPI.Models
{
    public class WorldBank
    {
        public int BankID { get; set; }
        public int LocationID { get; set; }
        public string Prefix { get; set; }
        public string BankName { get; set; }
    }
    public class AccountWorldBank
    {
        public long AccountID { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
    }
}
