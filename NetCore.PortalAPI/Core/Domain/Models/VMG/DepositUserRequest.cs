namespace ServerCore.PortalAPI.Core.Domain.Models.VMG
{
    public class DepositUserRequest
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string TransId { get; set; }
        public string TimeString { get; set; }
    }
}
