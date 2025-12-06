namespace ServerCore.PortalAPI.Models.VMG
{
    public class CashoutUserRequest
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string TransId { get; set; }
        public string TimeString { get; set; }
    }
}
