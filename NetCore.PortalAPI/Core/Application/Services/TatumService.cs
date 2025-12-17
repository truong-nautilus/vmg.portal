using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServerCore.PortalAPI.Core.Domain.Models.Crypto;
using ServerCore.Utilities.Utils;
using System.IO;
using System.Net;
using System.Text;

namespace ServerCore.PortalAPI.Core.Application.Services
{
    public class TatumService
    {
        private readonly string API_KEY;
        private readonly string CALLBACK_URL;
        private readonly string CALLBACK_SECRET_KEY;
        private readonly AppSettings _appSettings;

        public TatumService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            API_KEY = _appSettings.SPIDER_MAN_CRYPTO_TATUM_KEY;
            CALLBACK_URL = _appSettings.SPIDER_MAN_CRYPTO_TATUM_CALLBACK;
            CALLBACK_SECRET_KEY = _appSettings.SPIDER_MAN_CRYPTO_APIS_CALLBACK_SECRET_KEY;
        }

        public string CreateASubscriptionToken(string chainName, string address)
        {
            string url = "v4/subscription?type=mainnet";
            var obj = new
            {
                type = "INCOMING_FUNGIBLE_TX",
                attr = new
                {
                    address = address,
                    chain = chainName,
                    url = $"{CALLBACK_URL}?secretkey={CALLBACK_SECRET_KEY}"
                }
            };

            var resp = PostData<TaTumCreateASubscriptionResp>(url, obj);

            if (resp != null)
            {
                return resp.Id;
            }

            return null;
        }

        public string CreateASubscriptionNative(string chainName, string address)
        {
            string url = "v4/subscription?type=mainnet";
            var obj = new
            {
                type = "INCOMING_NATIVE_TX",
                attr = new
                {
                    address = address,
                    chain = chainName,
                    url = $"{CALLBACK_URL}?secretkey={CALLBACK_SECRET_KEY}"
                }
            };

            var resp = PostData<TaTumCreateASubscriptionResp>(url, obj);

            if (resp != null)
            {
                return resp.Id;
            }

            return null;
        }

        public TaTumSolTransactionResp SolTransaction(string tx)
        {
            var obj = new TatumGetSolTransactionReq()
            {
                JsonRpc = "2.0",
                Method = "getTransaction",
                Params = new string[]
                {
                    tx
                },
                Id = 1
            };

            var data = PostData<TaTumSolTransactionResp>($"v3/blockchain/node/solana-mainnet/{API_KEY}/https://02-dallas-054-01.rpc.tatum.io", obj);

            return data;
        }

        public TaTumPriceResp GetPrice(string tokenName)
        {
            string url = $"v3/tatum/rate/{tokenName.ToUpper()}?basePair=USD";
            var data = GetData<TaTumPriceResp>(url);

            return data;
        }

        public CoingeckoEthSolletPriceResp GetEthSolletPrice()
        {
            string url = $"https://api.coingecko.com/api/v3/simple/price?ids=wrapped-ethereum-sollet&vs_currencies=usd";
            var data = GetDataFullPatch<CoingeckoEthSolletPriceResp>(url);

            return data;
        }

        private T PostData<T>(string nameAction, object obj)
        {
            try
            {
                string url = $"https://api.tatum.io/{nameAction}";
                string json = JsonConvert.SerializeObject(obj);

                NLogManager.Info(url);
                NLogManager.Info(json);

                byte[] data = Encoding.UTF8.GetBytes(json);

                var request = CreateWebRequest(url, "POST");
                request.ContentLength = data.Length;
                request.Headers["x-api-key"] = API_KEY;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                var dataResp = JsonConvert.DeserializeObject<T>(result);

                NLogManager.Info(result);

                return dataResp;
            }
            catch (WebException ex)
            {
                NLogManager.Error(JsonConvert.SerializeObject(ex));
            }

            return default;
        }

        private T GetData<T>(string nameAction)
        {
            try
            {
                string url = $"https://api.tatum.io/{nameAction}";

                NLogManager.Info(url);


                var request = CreateWebRequest(url, "GET");
                request.Headers["x-api-key"] = API_KEY;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                var dataResp = JsonConvert.DeserializeObject<T>(result);

                NLogManager.Info(result);

                return dataResp;
            }
            catch (WebException ex)
            {
                NLogManager.Error(JsonConvert.SerializeObject(ex));
            }

            return default;
        }

        private T GetDataFullPatch<T>(string url)
        {
            try
            {

                NLogManager.Info(url);

                var request = CreateWebRequest(url, "GET");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                var dataResp = JsonConvert.DeserializeObject<T>(result);

                NLogManager.Info(result);

                return dataResp;
            }
            catch (WebException ex)
            {
                NLogManager.Error(JsonConvert.SerializeObject(ex));
            }

            return default;
        }

        private HttpWebRequest CreateWebRequest(string url, string method = "GET")
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = method;

            return request;
        }
    }

    class TaTumCreateASubscriptionResp
    {
        public string Id { get; set; }
    }

    class TatumGetSolTransactionReq
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public string[] Params { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
