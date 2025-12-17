using Newtonsoft.Json;

namespace ServerCore.PortalAPI.Core.Domain.Models.Crypto
{
    public class CoingeckoEthSolletPriceResp
    {
        [JsonProperty("wrapped-ethereum-sollet")]
        public Sollet Sollet { get; set; }
    }

    public class Sollet
    {
        public decimal Usd { get; set; }
    }
}
