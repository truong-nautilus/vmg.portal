namespace ServerCore.PortalAPI.Core.Domain.Models.Account
{
    public class AccountGeneral
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string UserFullname { get; set; }
        public string UserEmail { get; set; }
        public int LocationID { get; set; }
        public string Mobile { get; set; }
        public bool UseMK { get; set; }
        public int Avatar { get; set; }
    }
}
