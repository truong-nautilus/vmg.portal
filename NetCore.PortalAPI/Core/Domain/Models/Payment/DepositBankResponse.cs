namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class DepositBankResponse
    {
        public string status { get; set; }
        public string bank { get; set; }
        public string account { get; set; }
        public string bank_name { get; set; }
        public string content { get; set; }
        public string redirectLink { get; set; }
    }
}
