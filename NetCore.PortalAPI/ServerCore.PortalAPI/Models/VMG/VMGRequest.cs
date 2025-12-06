namespace ServerCore.PortalAPI.Models.VMG
{
    public class VMGRequest
    {
        public int AgencyId { get; set; }
        public string Method { get; set; }
        public string EncryptedData { get; set; }
        public string Checksum { get; set; }
    }
}
