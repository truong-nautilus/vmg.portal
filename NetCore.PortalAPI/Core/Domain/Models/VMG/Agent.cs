namespace ServerCore.PortalAPI.Core.Domain.Models.VMG
{
    public class Agency
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
        public string SHA256Key { get; set; }
        public string Prefix { get; set; }
    }
}
