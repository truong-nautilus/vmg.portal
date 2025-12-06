using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServerCore.DataAccess.DAO;
using ServerCore.PortalAPI.DataAccess.DAO;
using ServerCore.PortalAPI.Models;
using ServerCore.PortalAPI.Models.Account;
using ServerCore.PortalAPI.Models.VMG;
using ServerCore.PortalAPI.Services;
using ServerCore.Utilities.Models;
using ServerCore.Utilities.Security;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using UAParser;

namespace ServerCore.PortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VMGController : ControllerBase
    {
        private IVMGDAO _vmgDAO;
        private IAccountDAO _accountDAO;
        private IAuthenticateService _authenticateService;
        private AccountSession _accountSession;
        private string lng;
        public VMGController(IVMGDAO vmgDAO, IAuthenticateService authenticateService, IAccountDAO accountDAO, AccountSession accountSession)
        {
            _vmgDAO = vmgDAO;
            _authenticateService = authenticateService;
            lng = "en";
            _accountDAO = accountDAO;
            _accountSession = accountSession;
        }
        [HttpGet("Test")]
        public ActionResult GetEncyptedData()
        {
            return Ok("test");
        }
        [Authorize]
        [HttpGet("GetChip")]
        public ActionResult<ResponseBuilder> GetChip()
        {
            string lang = "en";
            try
            {
                var result = _vmgDAO.GetChipByCurrencyID(_accountSession.CurrencyID);

                if (result != null)
                {
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lang, result);
                }
                return new ResponseBuilder(ErrorCodes.FAIL, lang);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, lang);
        }

        [HttpGet("GetEncyptedData")]
        public ActionResult<ResponseBuilder> GetEncyptedData([FromQuery] int AgentId, [FromQuery] string KMData, [FromQuery] string timeString)
        {
            var agency = _vmgDAO.GetAgentById(AgentId);
            byte[] encryptedData = EncryptData(KMData, agency.EncryptKey);
            var encryptedDataString = Convert.ToBase64String(encryptedData);
            var checksum = ComputeChecksum(KMData, agency.SHA256Key, timeString, agency.SecretKey);
            return new ObjectResult(new
            {
                EncryptedData = HttpUtility.UrlEncode(encryptedDataString),
                Checksum = HttpUtility.UrlEncode(checksum)
            });
        }
        [HttpPost("Command")]
        public ActionResult<ResponseBuilder> VMGApi(VMGRequest request)
        {
            try
            {
                NLogManager.Info(String.Format("tham số đầu vào: agentId={0}, encryptedData={1}, checksum={2}", request.AgencyId, request.EncryptedData, request.Checksum));
                // giải mã data
                var agency = _vmgDAO.GetAgentById(request.AgencyId);
                var encryptedDataString = HttpUtility.UrlDecode(request.EncryptedData);
                var encryptedData = Convert.FromBase64String(encryptedDataString);
                var queryString = DecryptData(encryptedData, agency.EncryptKey);

                var dict = HttpUtility.ParseQueryString(queryString);
                string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                var queryStringObj = JsonConvert.DeserializeObject<dynamic>(json);
                NLogManager.Info(json);
                var time = (string)queryStringObj.TimeString;

                var checksum = ComputeChecksum(queryString, agency.SHA256Key, time, agency.SecretKey);
                var checksumEncode = HttpUtility.UrlEncode(checksum);
                if (request.Checksum != checksumEncode)
                {
                    return Ok(new ResponseBuilder(ErrorCodes.PARAM_INVALID, lng));
                }
                var account = new LoginAccount();
                string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                switch (request.Method)
                {
                    case "CreateMember":
                        var member = JsonConvert.DeserializeObject<Member>(json);
                        account.IpAddress = ipAddress;
                        account.UserName = $"{agency.Prefix}_{member.Username}";
                        account.NickName = member.Username;
                        account.MerchantId = agency.AgencyId;
                        account.PartnerUserID = member.UserId;
                        account.Password = Security.MD5Encrypt($"pw{account.UserName.ToLower()}{agency.AgencyId}@#");
                        AccountInfo accountInfo = null;
                        int responseStatus = _authenticateService.GetInfo2(account.MerchantId, account.PartnerUserID, out accountInfo);
                        if (responseStatus == 0)
                        {
                            return new ResponseBuilder(ErrorCodes.MEMBER_EXISTS, lng, new { AccountID = accountInfo.AccountID });
                        }
                        else
                        {
                            responseStatus = _authenticateService.Register(account, out accountInfo);
                            if (accountInfo != null)
                            {
                                return new ResponseBuilder(ErrorCodes.SUCCESS, lng, new { AccountID = accountInfo.AccountID });
                            }
                            return new ResponseBuilder(ErrorCodes.CREATE_MEMBER_FAIL, lng);
                        }

                    case "GetURLVMG":
                        var gameURL = JsonConvert.DeserializeObject<GameURL>(json);
                        var ip = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                        AccountInfo accountInf = null;
                        int response = _authenticateService.GetInfoByUserName(agency.AgencyId, $"{agency.Prefix}_{gameURL.UserName}", out accountInf);
                        if (response == 0)
                        {
                            account.IpAddress = ipAddress;
                            account.UserName = accountInf.UserName;
                            account.NickName = accountInf.NickName;
                            account.PlatformId = 4;
                            account.MerchantId = agency.AgencyId;
                            account.Password = Security.MD5Encrypt($"pw{accountInf.UserName}{agency.AgencyId}@#");
                            account.IpAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                            responseStatus = _authenticateService.Login(account, out accountInf);
                            if (responseStatus == 0)
                            {
                                if (accountInf != null)
                                {
                                    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                                    var parser = Parser.GetDefault();
                                    var clientInfo = parser.Parse(userAgent);
                                    var handler = new JwtSecurityTokenHandler();
                                    var jwtToken = handler.ReadJwtToken(accountInf.AccessToken);
                                    var expireDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "exp").Value);
                                    var createDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "iat").Value);
                                    var loginSession = new ALoginSession
                                    {
                                        AccountID = accountInf.AccountID,
                                        Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                                        Device = $"{clientInfo.Device.Family} - {clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                                        ExpireDate = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime,
                                        IPAddress = ipAddress,
                                        IsActive = true,
                                        Location = GetLocationFromIP(ipAddress),
                                        LoginDate = DateTimeOffset.FromUnixTimeSeconds(createDate).DateTime,
                                        SessionID = accountInf.SessionID
                                    };
                                    _authenticateService.CreateLoginSession(loginSession);
                                    var gameDb = _authenticateService.GetGameUrlByGameID(gameURL.GameId);
                                    var gameUrl = $"{gameDb.GameUrl}{accountInf.AccessToken}";
                                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, new { gameUrl = gameUrl });
                                }
                            }
                            return new ResponseBuilder(ErrorCodes.GET_GAMEURL_FAIL, lng);
                        }

                        return new ResponseBuilder(ErrorCodes.MEMBER_NOT_FOUND, lng);
                    case "GetBalance":
                        var getBalanceParam = JsonConvert.DeserializeObject<GetBalanceRequest>(json);
                        AccountInfo account_gb = null;
                        int response_gb = _authenticateService.GetInfoByUserName(agency.AgencyId, $"{agency.Prefix}_{getBalanceParam.UserName}", out account_gb);
                        if (response_gb == 0)
                        {
                            var lstBalanceAccount = _accountDAO.GetListBalanceAccount(account_gb.AccountID);
                            var balanceAccount = lstBalanceAccount.FirstOrDefault(x => x.WalletId == account_gb.CurrencyType);
                            return new ResponseBuilder(ErrorCodes.SUCCESS, lng, balanceAccount);
                        }
                        return new ResponseBuilder(ErrorCodes.MEMBER_NOT_FOUND, lng);
                    case "DepositUser":
                        var depositUserParam = JsonConvert.DeserializeObject<DepositUserRequest>(json);
                        AccountInfo account_du = null;
                        int response_du = _authenticateService.GetInfoByUserName(agency.AgencyId, $"{agency.Prefix}_{depositUserParam.UserName}", out account_du);
                        decimal balance_du = 0;
                        if (response_du == 0)
                        {
                            var res_du = _vmgDAO.DepositUser(account_du.AccountID, account_du.UserName, agency.AgencyId, depositUserParam.Amount, depositUserParam.TransId, out balance_du);
                            if (res_du > 0)
                                return new ResponseBuilder(ErrorCodes.SUCCESS, lng, new DepositUserResponse() { AccountId = account_du.AccountID, Amount = depositUserParam.Amount, Balance = balance_du, TxID = res_du });
                            else
                                return new ResponseBuilder(ErrorCodes.DEPOSIT_USER_FAIL, lng);
                        }
                        return new ResponseBuilder(ErrorCodes.MEMBER_NOT_FOUND, lng);
                    case "CashoutUser":
                        decimal balance_ca = 0;
                        var cashoutParam = JsonConvert.DeserializeObject<CashoutUserRequest>(json);
                        AccountInfo account_ca = null;
                        int response_ca = _authenticateService.GetInfoByUserName(agency.AgencyId, $"{agency.Prefix}_{cashoutParam.UserName}", out account_ca);
                        if (response_ca == 0)
                        {
                            var res_ca = _vmgDAO.CashoutUser(account_ca.AccountID, account_ca.UserName, agency.AgencyId, cashoutParam.Amount, cashoutParam.TransId, out balance_ca);
                            if (res_ca > 0)
                                return new ResponseBuilder(ErrorCodes.SUCCESS, lng, new CashoutUserResponse() { AccountId = account_ca.AccountID, Amount = cashoutParam.Amount, Balance = balance_ca, TxID = res_ca });
                            else
                                return new ResponseBuilder(ErrorCodes.CASHOUT_USER_FAIL, lng);
                        }
                        return new ResponseBuilder(ErrorCodes.MEMBER_NOT_FOUND, lng);
                    case "BetHistory":
                        //decimal balance_bh = 0;
                        var betHistoryParam = JsonConvert.DeserializeObject<BetHistoryRequest>(json);
                        //AccountInfo account_bh = null;
                        //int response_bh = _authenticateService.GetInfoByUserName(agency.AgencyId, $"{agency.Prefix}_{betHistoryParam.UserName}", out account_ca);
                        var totalPage = 0;
                        //if (response_bh == 0)
                        //{
                        var betHistories = _vmgDAO.GetAccountBetHistory(betHistoryParam.FromDate, betHistoryParam.ToDate, agency.AgencyId, out totalPage, betHistoryParam.PageNumber, betHistoryParam.PageSize);

                        var data = new BetHistoryResponse()
                        {
                            TotalPage = totalPage,
                            Lst = betHistories,
                        };
                        return new ResponseBuilder(ErrorCodes.SUCCESS, lng, data);
                    //}
                    //return new ResponseBuilder(ErrorCodes.MEMBER_NOT_FOUND, lng);
                    default:
                        return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, lng);
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SERVER_ERROR, lng);
            }
        }
        private byte[] EncryptData(string data, string encryptionKey)
        {
            using (AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider())
            {
                aesCryptoProvider.KeySize = 256;
                aesCryptoProvider.BlockSize = 128;
                aesCryptoProvider.Mode = CipherMode.CBC;
                aesCryptoProvider.Padding = PaddingMode.PKCS7;
                byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0, 16));
                aesCryptoProvider.Key = keyBytes;
                aesCryptoProvider.IV = ivBytes;
                ICryptoTransform encryptor =
                aesCryptoProvider.CreateEncryptor(aesCryptoProvider.Key, aesCryptoProvider.IV);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedDataBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return encryptedDataBytes;
            }
        }
        private string DecryptData(byte[] encryptedData, string encryptionKey)
        {
            using (AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider())
            {
                aesCryptoProvider.KeySize = 256;
                aesCryptoProvider.BlockSize = 128;
                aesCryptoProvider.Mode = CipherMode.CBC;
                aesCryptoProvider.Padding = PaddingMode.PKCS7;
                byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0, 16));
                aesCryptoProvider.Key = keyBytes;
                aesCryptoProvider.IV = ivBytes;
                ICryptoTransform decryptor = aesCryptoProvider.CreateDecryptor(aesCryptoProvider.Key, aesCryptoProvider.IV);
                byte[] decryptedDataBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string decryptedData = Encoding.UTF8.GetString(decryptedDataBytes);
                return decryptedData;
            }
        }
        private string ComputeChecksum(string queryString, string sha256Key, string time, string secretKey)
        {
            string dataToHash = queryString + sha256Key + time + secretKey;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private string GetLocationFromIP(string ipAddress)
        {
            try
            {
                HttpClient client = new HttpClient();
                // Tạo URI và log URL yêu cầu
                var uri = new Uri($"https://ipinfo.io/{ipAddress}/json");
                NLogManager.Info(uri.ToString());  // Log thông tin URL

                // Gửi yêu cầu HTTP GET và chờ phản hồi
                var response = client.GetStringAsync(uri).Result;

                // Phân tích cú pháp JSON để lấy thông tin địa lý
                dynamic locationData = JsonConvert.DeserializeObject(response);

                // Trả về thông tin vùng và quốc gia
                return $"{locationData.region}, {locationData.country}";
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi kết nối mạng hoặc lỗi khi gửi yêu cầu
                NLogManager.Error($"Lỗi yêu cầu HTTP: {e.Message}");
                return "";
            }
            catch (Exception e)
            {
                // Xử lý lỗi chung (ví dụ như lỗi phân tích cú pháp JSON)
                NLogManager.Error($"Lỗi: {e.Message}");
                return "";
            }
        }
    }
}
