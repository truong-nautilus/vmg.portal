namespace ServerCore.PortalAPI.Models.Crypto
{
    public class CurrencyProfile
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public string Logo { get; set; }
        public bool IsActive { get; set; }
    }
}
