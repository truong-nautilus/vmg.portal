namespace ServerCore.PortalAPI.Core.Domain.Models.VMG
{
    public class ChipValue
    {
        public int CurrencyType { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string ChipList { get; set; }
        public decimal MaxBetValue { get; set; }
        public decimal MinBetValue { get; set; }
    }
}
