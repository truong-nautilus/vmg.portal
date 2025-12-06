using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServerCore.PortalAPI.Models.Crypto;
using ServerCore.Utilities.Utils;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ServerCore.PortalAPI.Services
{
    public class WalletService
    {
        private readonly string API_URL;
        protected string SEND_PUSH_TELEGRAM_URL;
        private AppSettings _appSettings;
        public WalletService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            API_URL = _appSettings.SPIDER_MAN_CRYPTO_WALLET_URL;
            SEND_PUSH_TELEGRAM_URL = _appSettings.SEND_PUSH_TELEGRAM_URL;
        }

        public WalletCreateResp Create(string chain)
        {
            var data = PostData<WalletResp<WalletCreateResp>>("wallet", new
            {
                chain = chain.ToUpper(),
            });

            return data.Data;
        }

        private T PostData<T>(string nameAction, object obj)
        {
            try
            {
                string url = $"{API_URL}/{nameAction}";
                string json = JsonConvert.SerializeObject(obj);

                NLogManager.Info(url);
                NLogManager.Info(json);

                byte[] data = Encoding.UTF8.GetBytes(json);

                var request = CreateWebRequest(url, "POST");
                request.ContentLength = data.Length;

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
                string url = $"{API_URL}/{nameAction}";

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
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = method;

            return request;
        }
        public OtpResponse SendTelePush(string Content, long TelegramID = 0)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = $"{SEND_PUSH_TELEGRAM_URL}/push-system-send";
            string urlParameter = JsonConvert.SerializeObject(new
            {
                Content = Content,
                Action = TelegramID
            });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.UTF8.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.Exception(e);
            }
            return model;
        }
    }

    class WalletResp<T>
    {
        public string Code { get; set; }
        public string Mess { get; set; }
        public T Data { get; set; }
    }
}
