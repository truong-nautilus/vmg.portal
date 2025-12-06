namespace ServerCore.PortalAPI.Models.Account
{
    public class EditPassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }
    }
}
