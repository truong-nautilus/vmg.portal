namespace ServerCore.PortalAPI.Core.Domain.Models.Crypto
{
    public class OtpResponse
    {
        public string code { get; set; }
        public string des { get; set; }
    }
    public class OtpSuccess
    {
        public static string SUCESS = "100";
    }
}
