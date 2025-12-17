using System;

namespace ServerCore.PortalAPI.Core.Domain.Models.Crypto
{
    public class TaTumPriceResp
    {
        public string BasePair { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
        public int Id { get; set; }
    }
}
