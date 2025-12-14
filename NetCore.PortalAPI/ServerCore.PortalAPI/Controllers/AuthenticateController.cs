using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.Google; // Add package if needed
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NetCore.Utils.Cache;
using Nethereum.Signer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerCore.PortalAPI.Models;
using ServerCore.PortalAPI.Models.Account;
using ServerCore.PortalAPI.Services;
using ServerCore.Utilities.Captcha;
using ServerCore.Utilities.Models;
using ServerCore.Utilities.Security;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UAParser;

namespace ServerCore.PortalAPI.Controllers
{
    [Route("Authen")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        //private IHttpContextAccessor _httpContextAccessor;
        private IHostingEnvironment _env;
        private IAuthenticateService _authenticateService;

        private static int _maxLengthUserName;
        private static int _maxLengthNickName;

        private static string _loginActionName;
        private static int _loginFailAllow;
        private static int _loginFailTime;
        private readonly AppSettings _appSettings;

        private readonly AccountSession _accountSession;
        private readonly Captcha _captcha;
        private readonly IDistributedCache _cache;
        private readonly CacheHandler _cacheHandler;

        public AuthenticateController(AccountSession accountSession, IHostingEnvironment env, IAuthenticateService authenticateService, IOptions<AppSettings> options, Captcha captchaUtil, IDistributedCache cache, CacheHandler cacheHandler)
        {
            _appSettings = options.Value;
            _env = env;
            _authenticateService = authenticateService;

            _maxLengthUserName = _appSettings.MaxLengthUserName;
            _maxLengthNickName = _appSettings.MaxLengthNickName;

            _loginFailAllow = _appSettings.LoginFailAllow;
            _loginFailTime = _appSettings.LoginFailTime;
            _loginActionName = _appSettings.LoginActionName;


            this._accountSession = accountSession;

            _captcha = captchaUtil;
            _cache = cache;
            _cacheHandler = cacheHandler;
        }

        [HttpGet("RefreshToken")]
        public ActionResult<ResponseBuilder> RefreshToken()
        {
            string lng = Utils.GetLanguage(Request.HttpContext);
            long accountID = _accountSession.AccountID;
            if (accountID <= 0)
            {
                return new ResponseBuilder((int)ErrorCodes.TOKEN_ERROR, lng);
            }
            string userName = _accountSession.AccountName;

            try
            {
                if (userName.Length > _maxLengthUserName)
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng);
                }

                AccountInfo accountInfo = null;
                int countCache = CacheCounter.CheckAccountActionFrequency(userName, _loginFailTime, _loginActionName);
                if (countCache >= _loginFailAllow)
                {
                    return new ResponseBuilder(ErrorCodes.TOKEN_ERROR, lng);
                }
                int responseStatus = _authenticateService.GetInfo(accountID, userName, out accountInfo);
                if (accountInfo == null || accountInfo.AccountID < 1)
                {
                    NLogManager.Error("Tài khoản không tồn tại");
                    return new ResponseBuilder((int)ErrorCodes.TOKEN_ERROR, lng, null);
                }
                accountInfo.AccessToken = _accountSession.Token;
                if (responseStatus == (int)ErrorCodes.SUCCESS)
                {
                    CacheCounter.AccountActionDelete(userName, _loginActionName);
                    return new ResponseBuilder((int)ErrorCodes.SUCCESS, lng, accountInfo);
                }
                else
                    return new ResponseBuilder((int)ErrorCodes.TOKEN_ERROR, lng, null);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder((int)ErrorCodes.TOKEN_ERROR, lng, null);
            }
        }

        [HttpGet("Test")]
        public ActionResult<ResponseBuilder> Test()
        {

            //{ "PlatformId":1,"MerchantId":1000,"IpAddress":"","UserName":"tktest01","Password":"eccbc87e4b5ce2fe28308fd9f2a7baf3","Uiid":"3aecc996-9d55-4a23-919a-73ce0c1270cf","CaptchaText":"","CaptchaToken":null,"NickName":null,"FacebookToken":null,"FacebookId":null,"VipCode":null,"CampaignSource":null,"AppleToken":null,"AppleId":null,"AppleEmail":null,"DeviceName":"Chrome 112.0.0.0"}
            LoginAccount abc = new LoginAccount();
            abc.UserName = "nhama345";
            abc.Password = "nhamaaa236";
            abc.IpAddress = "127.0.0.1";
            abc.Uiid = "3aecc996-9d55-4a23-919a-73ce0c1270cf";
            abc.PlatformId = 1;
            abc.CaptchaText = "Test";
            abc.MerchantId = 1;
            abc.CaptchaToken = "Test";
            LoginAccount abc2 = null;
            var ret = Register(abc);//, out abc2);
            return Ok(1);

        }

        [HttpPost("Login")]
        public ActionResult Login(LoginAccount loginAccount)
        {
            //var watch = Stopwatch.StartNew();
            string userName = loginAccount.UserName;
            string password = loginAccount.Password;
            string ipaddress = loginAccount.IpAddress;
            string uiid = loginAccount.Uiid;
            int platformId = loginAccount.PlatformId;
            int merchantId = loginAccount.MerchantId;

            string captcha = loginAccount.CaptchaText;
            string captchaToken = loginAccount.CaptchaToken;

            // string lng = Utils.GetLanguage(Request.HttpContext);
            string lng = "vi";
            NLogManager.Info("Login");
            try
            {
                NLogManager.Info(JsonConvert.SerializeObject("Login 1 " + loginAccount));

                //PolicyUtil.CheckNickName(userName);
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (userName.Length > _maxLengthUserName)
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (platformId < (int)PlatformDef.ANDROID_PLATFORM || platformId > (int)PlatformDef.WEB_PLATFORM)
                {
                    return Ok(new ResponseBuilder(ErrorCodes.PLATFORM_INVALID, lng));
                }

                //if (string.IsNullOrEmpty(loginAccount.Uiid))
                //{
                //    return Ok(new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, lng));
                //}

                // Lowercase userName khi đăng nhập;
                loginAccount.UserName = loginAccount.UserName.ToLower();

                AccountInfo accountInfo = null;

                //var elapsedMs = watch.ElapsedMilliseconds;
                // NLogManager.Info(string.Format("LOGIN 0: {0},{1}", userName, elapsedMs));
                NLogManager.Info("Login 2" + userName);
                int countCache = CacheCounter.CheckAccountActionFrequency(userName, _loginFailTime, _loginActionName);
                if (countCache >= _loginFailAllow)
                {
                    if (_captcha.VerifyCaptcha(captcha, captchaToken) < 0)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(ErrorCodes.CAPTCHA_INVALID, lng, accountInfo));
                    }
                }
                NLogManager.Info("Login 3 " + userName);
                LoginAccount account = loginAccount;
                account.IpAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);

                //watch.Stop();
                //long elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_CheckParam: {0},{1}", userName, elapsedMs));

                //watch.Restart();
                //(int, AccountInfo) res = await _authenticateService.LoginAsync(account);

                int response = _authenticateService.Login(account, out accountInfo);
                NLogManager.Info("Login 4 " + userName);
                //watch.Stop();
                //elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_LoginService: {0},{1}", userName, elapsedMs));

                if (response == (int)ErrorCodes.SUCCESS || response == (int)ErrorCodes.NEED_OTP_CODE)
                {
                    //watch.Restart();
                    CacheCounter.AccountActionDelete(userName, _loginActionName);
                    try
                    {
                        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                        var parser = Parser.GetDefault();
                        var clientInfo = parser.Parse(userAgent);
                        var ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(accountInfo.AccessToken);
                        var expireDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "exp").Value);
                        var createDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "iat").Value);
                        //var user = JsonConvert.DeserializeObject<dynamic>(jwtToken.Claims.);
                        var loginSession = new ALoginSession
                        {
                            AccountID = accountInfo.AccountID,
                            Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                            Device = $"{clientInfo.Device.Family} - {clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                            ExpireDate = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime,
                            IPAddress = ipAddress,
                            IsActive = true,
                            Location = GetLocationFromIP(ipAddress),
                            LoginDate = DateTimeOffset.FromUnixTimeSeconds(createDate).DateTime,
                            SessionID = accountInfo.SessionID
                        };
                        _authenticateService.CreateLoginSession(loginSession);
                    }
                    catch (Exception ex)
                    {
                        NLogManager.Error(ex);
                    }
                    object responseObj = new ResponseBuilder(response, lng, accountInfo);
                    //watch.Stop();
                    //elapsedMs = watch.ElapsedMilliseconds;
                    //NLogManager.Info(string.Format("LOGIN_BuildResponse: {0},{1}", userName, elapsedMs));
                    return Ok(responseObj);
                    //return Ok(new ResponseBuilder(response, lng, accountInfo));
                }
                else
                {
                    if (response < 0)
                    {
                        if (countCache >= _loginFailAllow)
                        {
                            accountInfo = new AccountInfo();
                            accountInfo.IsCaptcha = true;
                            return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng, accountInfo));
                        }
                        return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
                    }
                    if (countCache >= _loginFailAllow)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(response, lng, accountInfo));
                    }
                    return Ok(new ResponseBuilder(response, lng));
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
            }
        }


        [HttpPost("LoginUID")]
        public ActionResult LoginUID(LoginAccount loginAccount)
        {
            //var watch = Stopwatch.StartNew();
            string userName = loginAccount.UserName;
            string password = loginAccount.Password;
            string ipaddress = loginAccount.IpAddress;
            string uiid = loginAccount.Uiid;
            int platformId = loginAccount.PlatformId;
            int merchantId = loginAccount.MerchantId;

            string captcha = loginAccount.CaptchaText;
            string captchaToken = loginAccount.CaptchaToken;

            // string lng = Utils.GetLanguage(Request.HttpContext);
            string lng = "vi";
            NLogManager.Info("LoginUID");
            try
            {
                NLogManager.Info(JsonConvert.SerializeObject("LoginUID 1 " + loginAccount));

                //PolicyUtil.CheckNickName(userName);
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (userName.Length > _maxLengthUserName)
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (platformId < (int)PlatformDef.ANDROID_PLATFORM || platformId > (int)PlatformDef.WEB_PLATFORM)
                {
                    return Ok(new ResponseBuilder(ErrorCodes.PLATFORM_INVALID, lng));
                }

                if (string.IsNullOrEmpty(loginAccount.Uiid))
                {
                    //    return Ok(new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, lng));
                }

                // Lowercase userName khi đăng nhập;
                loginAccount.UserName = loginAccount.UserName.ToLower();

                AccountInfo accountInfo = null;

                //var elapsedMs = watch.ElapsedMilliseconds;
                // NLogManager.Info(string.Format("LOGIN 0: {0},{1}", userName, elapsedMs));
                NLogManager.Info("LoginUID 2: " + userName);
                int countCache = CacheCounter.CheckAccountActionFrequency(userName, _loginFailTime, _loginActionName);
                if (countCache >= _loginFailAllow)
                {
                    if (_captcha.VerifyCaptcha(captcha, captchaToken) < 0)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(ErrorCodes.CAPTCHA_INVALID, lng, accountInfo));
                    }
                }
                NLogManager.Info("LoginUID 3: " + userName);
                LoginAccount account = loginAccount;
                account.IpAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);

                //watch.Stop();
                //long elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_CheckParam: {0},{1}", userName, elapsedMs));

                //watch.Restart();
                //(int, AccountInfo) res = await _authenticateService.LoginAsync(account);

                int response = _authenticateService.Login(account, out accountInfo);
                NLogManager.Info("LoginUID 4: " + userName);
                //watch.Stop();
                //elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_LoginService: {0},{1}", userName, elapsedMs));

                if (response == (int)ErrorCodes.SUCCESS || response == (int)ErrorCodes.NEED_OTP_CODE)
                {
                    //watch.Restart();
                    CacheCounter.AccountActionDelete(userName, _loginActionName);
                    object responseObj = new ResponseBuilder(response, lng, accountInfo);
                    //watch.Stop();
                    //elapsedMs = watch.ElapsedMilliseconds;
                    //NLogManager.Info(string.Format("LOGIN_BuildResponse: {0},{1}", userName, elapsedMs));
                    return Ok(responseObj);
                    //return Ok(new ResponseBuilder(response, lng, accountInfo));
                }
                else
                {
                    if (response < 0)
                    {
                        if (countCache >= _loginFailAllow)
                        {
                            accountInfo = new AccountInfo();
                            accountInfo.IsCaptcha = true;
                            return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng, accountInfo));
                        }
                        return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
                    }
                    if (countCache >= _loginFailAllow)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(response, lng, accountInfo));
                    }
                    return Ok(new ResponseBuilder(response, lng));
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
            }
        }


        // for test
        [HttpPost("LoginAsync")]
        public async Task<ActionResult> LoginAsync(LoginAccount loginAccount)
        {
            //var watch = Stopwatch.StartNew();
            string userName = loginAccount.UserName;
            string password = loginAccount.Password;
            string ipaddress = loginAccount.IpAddress;
            string uiid = loginAccount.Uiid;
            int platformId = loginAccount.PlatformId;
            int merchantId = loginAccount.MerchantId;

            string captcha = loginAccount.CaptchaText;
            string captchaToken = loginAccount.CaptchaToken;

            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                NLogManager.Info(JsonConvert.SerializeObject(loginAccount));
                PolicyUtil.CheckNickName(userName);
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (userName.Length > _maxLengthUserName)
                {
                    NLogManager.Error("Tên tài khoản không hợp lệ");
                    return Ok(new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng));
                }

                if (platformId < (int)PlatformDef.ANDROID_PLATFORM || platformId > (int)PlatformDef.WEB_PLATFORM)
                {
                    return Ok(new ResponseBuilder(ErrorCodes.PLATFORM_INVALID, lng));
                }

                //if (string.IsNullOrEmpty(loginAccount.Uiid))
                //{
                //    return Ok(new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, lng));
                //}

                // Lowercase userName khi đăng nhập;
                loginAccount.UserName = loginAccount.UserName.ToLower();

                AccountInfo accountInfo = null;

                //var elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN 0: {0},{1}", userName, elapsedMs));

                int countCache = CacheCounter.CheckAccountActionFrequency(userName, _loginFailTime, _loginActionName);
                if (countCache >= _loginFailAllow)
                {
                    if (_captcha.VerifyCaptcha(captcha, captchaToken) < 0)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(ErrorCodes.CAPTCHA_INVALID, lng, accountInfo));
                    }
                }
                LoginAccount account = loginAccount;
                account.IpAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);

                //watch.Stop();
                //long elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_CheckParam: {0},{1}", userName, elapsedMs));

                //watch.Restart();

                (int, AccountInfo) res = await _authenticateService.LoginAsync(account);

                //int response = _authenticateService.Login(account, out accountInfo);
                int response = res.Item1;
                accountInfo = res.Item2;

                //watch.Stop();
                //elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("LOGIN_LoginService: {0},{1}", userName, elapsedMs));

                if (response == (int)ErrorCodes.SUCCESS || response == (int)ErrorCodes.NEED_OTP_CODE)
                {
                    //watch.Restart();
                    CacheCounter.AccountActionDelete(userName, _loginActionName);
                    object responseObj = new ResponseBuilder(response, lng, accountInfo);
                    //watch.Stop();
                    //elapsedMs = watch.ElapsedMilliseconds;
                    //NLogManager.Info(string.Format("LOGIN_BuildResponse: {0},{1}", userName, elapsedMs));
                    return Ok(responseObj);
                    //return Ok(new ResponseBuilder(response, lng, accountInfo));
                }
                else
                {
                    if (response < 0)
                    {
                        if (countCache >= _loginFailAllow)
                        {
                            accountInfo = new AccountInfo();
                            accountInfo.IsCaptcha = true;
                            return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng, accountInfo));
                        }
                        return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
                    }
                    if (countCache >= _loginFailAllow)
                    {
                        accountInfo = new AccountInfo();
                        accountInfo.IsCaptcha = true;
                        return Ok(new ResponseBuilder(response, lng, accountInfo));
                    }
                    return Ok(new ResponseBuilder(response, lng));
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return Ok(new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng));
            }
        }

        [HttpPost("LoginFacebook")]
        public async Task<ActionResult<ResponseBuilder>> LoginFacebook(LoginAccount loginAccount)
        {
            string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
            //var watch = Stopwatch.StartNew();
            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                if (loginAccount == null)
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                NLogManager.Info(JsonConvert.SerializeObject(loginAccount));
                //AccountInfo accountInfo = null;
                //int response = _authenticateService.LoginFBAsync(loginAccount, out accountInfo);
                loginAccount.IpAddress = ipAddress;
                AccountInfo accountInfo = await _authenticateService.LoginFBAsync(loginAccount);

                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("TOTALTIME FB: {0}", elapsedMs));

                if (accountInfo.ErrorCode == (int)ErrorCodes.SUCCESS)
                {
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                }

                if (accountInfo.ErrorCode == (int)ErrorCodes.NEED_OTP_CODE)
                    return new ResponseBuilder(ErrorCodes.NEED_OTP_CODE, lng, accountInfo);
                return new ResponseBuilder(accountInfo.ErrorCode, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng);
            }
        }
        [HttpGet("LoginGoogle")]
        public async Task<ActionResult<ResponseBuilder>> LoginGoogle()
        {
            string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
            //var watch = Stopwatch.StartNew();
            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
                return Challenge(properties, "Google"); // GoogleDefaults.AuthenticationScheme
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng);
            }
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var gcode = -1;
            if (result?.Principal != null)
            {
                _cacheHandler.MemSet(120, "GGResult", result);
                gcode = 0;
            }
            var redirectUrl = $"{_appSettings.ClientGoogleCallback}?gcode={gcode}";
            return Redirect(redirectUrl);
        }
        [HttpGet("ExchangeCode")]
        public async Task<ActionResult<ResponseBuilder>> ExchangeCode()
        {
            string lng = Utils.GetLanguage(Request.HttpContext).Substring(0, 2);
            try
            {
                // Nhận kết quả xác thực từ Google
                AuthenticateResult result = null;
                _cacheHandler.MemTryGet<AuthenticateResult>("GGResult", out result);
                if (result?.Principal != null)
                {
                    // Lấy thông tin từ Google
                    var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                    var name = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var account = new LoginAccount();
                    string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                    account.IpAddress = ipAddress;
                    account.UserName = "gm_" + name;
                    account.Email = email;
                    account.NickName = "cexsea" + name;
                    account.Password = Security.MD5Encrypt($"pw{name}@#");
                    AccountInfo accountInfo = null;
                    int responseStatus = _authenticateService.GetInfo(0, account.UserName, out accountInfo);
                    if (responseStatus == 0)
                    {
                        responseStatus = _authenticateService.Login(account, out accountInfo);
                    }
                    else
                    {
                        responseStatus = _authenticateService.Register(account, out accountInfo);
                    }
                    // Trả về token cho client (có thể lưu vào local storage hoặc cookie)
                    if (accountInfo != null)
                    {
                        try
                        {
                            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                            var parser = Parser.GetDefault();
                            var clientInfo = parser.Parse(userAgent);
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(accountInfo.AccessToken);
                            var expireDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "exp").Value);
                            var createDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "iat").Value);
                            var loginSession = new ALoginSession
                            {
                                AccountID = accountInfo.AccountID,
                                Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                                Device = $"{clientInfo.Device.Family} - {clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                                ExpireDate = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime,
                                IPAddress = ipAddress,
                                IsActive = true,
                                Location = GetLocationFromIP(ipAddress),
                                LoginDate = DateTimeOffset.FromUnixTimeSeconds(createDate).DateTime,
                                SessionID = accountInfo.SessionID
                            };
                            _authenticateService.CreateLoginSession(loginSession);
                        }
                        catch (Exception ex)
                        {
                            NLogManager.Error(ex);
                        }
                        return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                    }
                    return new ResponseBuilder(responseStatus, lng);
                }
                return new ResponseBuilder(ErrorCodes.REGISTER_ACCOUNT_ERROR, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.REGISTER_ACCOUNT_ERROR, lng);
            }

        }
        [HttpPost("Register")]
        public ActionResult<ResponseBuilder> Register(LoginAccount loginAccount)
        {
            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                if (loginAccount == null)
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                NLogManager.Info(JsonConvert.SerializeObject(loginAccount));

                if (string.IsNullOrEmpty(loginAccount.Uiid))
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                if (_captcha.VerifyCaptcha(loginAccount.CaptchaText, loginAccount.CaptchaToken) != (int)ErrorCodes.SUCCESS)
                    return new ResponseBuilder(ErrorCodes.VERIFY_CODE_INVALID, lng);

                string userName = loginAccount.UserName.ToLower();
                NLogManager.Info("Register 0" + JsonConvert.SerializeObject(loginAccount));
                if (!PolicyUtil.CheckUserName(userName))
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng);
                NLogManager.Info("Register 1" + JsonConvert.SerializeObject(loginAccount));
                if (!PolicyUtil.CheckPassword(loginAccount.Password))
                    return new ResponseBuilder(ErrorCodes.PASSWORD_INVALID, lng);

                //if (!PolicyUtil.CheckNickName(loginAccount.NickName))
                //    return new ResponseBuilder(ErrorCodes.NICKNAME_INVALID, lng);

                //if(loginAccount.NickName.ToLower().Equals(loginAccount.UserName.ToLower()))
                //    return new ResponseBuilder(ErrorCodes.ACCOUNT_CHARACTER_NAME_MUST_DEFFENT_ACCNAME, lng);

                string checkMobile = loginAccount.UserName;
                checkMobile = loginAccount.UserName.Replace("+", "");
                if (checkMobile.All(char.IsDigit))
                {
                    if (_authenticateService.CheckUserNameIsMobileOther(checkMobile))
                        return new ResponseBuilder(ErrorCodes.ACCOUNT_CURRENT_IS_MOBILE_ACTIVE_ACCOUNT_OTHER, lng);
                }

                // string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                LoginAccount account = loginAccount;
                account.IpAddress = ipAddress;
                account.UserName = userName;
                AccountInfo accountInfo = null;
                int response = _authenticateService.Register(account, out accountInfo);

                //if (response == -105)
                //    return new ResponseBuilder(ErrorCodes.ACCOUNT_EXIST, lng);
                //else if (response == -106)
                //    return new ResponseBuilder(ErrorCodes.NICKNAME_EXIST, lng);

                if (accountInfo != null)
                {
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                }

                return new ResponseBuilder(response, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.REGISTER_ACCOUNT_ERROR, lng);
            }
        }

        [HttpPost("CreateAccountTest")]
        public ActionResult<ResponseBuilder> CreateAccountTest(LoginAccount loginAccount)
        {
            NLogManager.Info("RegisterCheck:" + JsonConvert.SerializeObject(loginAccount));

            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                if (loginAccount == null)
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                loginAccount.UserName = loginAccount.UserName.ToLower();

                string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                LoginAccount account = loginAccount;
                account.IpAddress = ipAddress;
                AccountInfo accountInfo = null;
                int response = _authenticateService.Register(account, out accountInfo);

                if (accountInfo != null)
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);

                return new ResponseBuilder(response, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.REGISTER_ACCOUNT_ERROR, lng);
            }
        }

        [Authorize]
        [HttpGet("Validate")]
        public ActionResult<ResponseBuilder> Validate(string token)
        {
            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                //var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                //if(authHeader != null && !string.IsNullOrEmpty(authHeader.Parameter))
                AccountInfo accountInfo = null;
                int response = _authenticateService.Validate(token, out accountInfo);
                return new ResponseBuilder(response, lng, accountInfo);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.VALIDATE_ERROR, lng);
        }

        [HttpPost("LoginAppleId")]
        public async Task<ActionResult<ResponseBuilder>> LoginAppleId(LoginAccount loginAccount)
        {
            //var watch = Stopwatch.StartNew();
            string lng = Utils.GetLanguage(Request.HttpContext);
            try
            {
                if (loginAccount == null)
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                NLogManager.Info(JsonConvert.SerializeObject(loginAccount));
                //AccountInfo accountInfo = null;
                //int response = _authenticateService.LoginFBAsync(loginAccount, out accountInfo);
                AccountInfo accountInfo = await _authenticateService.LoginAppleIdAsync(loginAccount);

                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //NLogManager.Info(string.Format("TOTALTIME FB: {0}", elapsedMs));

                if (accountInfo.ErrorCode == (int)ErrorCodes.SUCCESS)
                {
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                }

                if (accountInfo.ErrorCode == (int)ErrorCodes.NEED_OTP_CODE)
                    return new ResponseBuilder(ErrorCodes.NEED_OTP_CODE, lng, accountInfo);
                return new ResponseBuilder(accountInfo.ErrorCode, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.LOGIN_ERROR, lng);
            }
        }

        private void CacheAccessToken(AccountInfo accountInfo, string uiid)
        {
            try
            {
                string accountCacheExist = _cache.GetString(accountInfo.AccountID.ToString());
                if (string.IsNullOrEmpty(accountCacheExist))
                {
                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(_appSettings.CacheExpireHour));
                    List<AccessTokenCache> list = new List<AccessTokenCache>();
                    list.Add(new AccessTokenCache(accountInfo.AccessToken, uiid, accountInfo.AccountID, _appSettings.CacheExpireHour));
                    _cache.SetString(accountInfo.AccountID.ToString(), JsonConvert.SerializeObject(list), option);
                }
                else
                {
                    List<AccessTokenCache> list = JsonConvert.DeserializeObject<List<AccessTokenCache>>(accountCacheExist);
                    bool isExist = false;
                    foreach (var it in list)
                    {
                        if (it.Uiid.Equals(uiid))
                        {
                            it.AccessToken = accountInfo.AccessToken;
                            it.CreatedTime = DateTime.Now.Ticks;
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                        list.Add(new AccessTokenCache(accountInfo.AccessToken, uiid, accountInfo.AccountID, _appSettings.CacheExpireHour));

                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(_appSettings.CacheExpireHour));
                    _cache.SetString(accountInfo.AccountID.ToString(), JsonConvert.SerializeObject(list), option);
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
        }


        public static bool CheckPassword(string password)
        {
            if (password.Length < 6)
                return false;

            return password.Select(c => (byte)c).All(code => !((code < 33) | (code > 122)));
        }

        public static bool CheckUserName(string userName)
        {
            try
            {
                NLogManager.Info("CheckUserName 0");
                // Độ dài từ 6-18
                if (userName.Length < 6 || userName.Length > 18)
                    return false;

                //Kí tự đầu tiên phải là chữ cái
                var fillterChar = "abcdefghijklmnopqrstuvxyzw0123456789._";
                if (fillterChar.IndexOf(userName[0]) < 0)
                    return false;
                NLogManager.Info("CheckUserName 2");
                //Kí tự '.' không được xuất hiện liền nhau
                if (userName.IndexOf("..") >= 0)
                    return false;

                // Ký tự '.' không được ở sau cùng
                if (userName.EndsWith("."))
                    return false;

                // Kiem tra userName co vi phạm các từ khóa hay không
                //var file = new System.IO.StreamReader(HttpContext.Current.Server.MapPath(RuleUserNameFile));
                //string ruleData = file.ReadToEnd();
                //file.Close();

                //if(ruleData.IndexOf(userNameLower) > 0)
                //    return false;

                string userNameLower = userName.ToLower();
                if (userNameLower.Contains("daili") || userNameLower.Contains("admin"))
                    return false;

                //Chuỗi hợp lệ   abcdefghijklmnopqrstuvxyzw012345678.
                fillterChar = "abcdefghijklmnopqrstuvxyzw0123456789._";
                return userName.All(t => fillterChar.IndexOf(t) >= 0);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return false;
            }
        }


        [HttpPost("FastRegister")]
        public ActionResult<ResponseBuilder> FastRegister(FastRegister model)
        {
            LoginAccount loginAccount = new LoginAccount();
            loginAccount.UserName = model.Username;
            loginAccount.Password = model.Password;
            loginAccount.NickName = model.NickName;
            loginAccount.Email = model.Email;
            loginAccount.IpAddress = "127.0.0.1";
            loginAccount.Uiid = model.Uiid;
            loginAccount.PlatformId = 1;
            loginAccount.CaptchaText = "";
            loginAccount.MerchantId = 1;
            loginAccount.CaptchaToken = "";

            string lng = "vi";
            try
            {
                if (loginAccount == null)
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                NLogManager.Info(JsonConvert.SerializeObject(loginAccount));

                if (string.IsNullOrEmpty(loginAccount.Uiid))
                    return new ResponseBuilder(ErrorCodes.DATA_IS_NULL, lng);

                string userName = loginAccount.UserName.ToLower();
                NLogManager.Info("Register 0" + JsonConvert.SerializeObject(loginAccount));
                if (!CheckUserName(userName))
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_NAME_INVALID, lng);
                NLogManager.Info("Register 1" + JsonConvert.SerializeObject(loginAccount));
                if (!CheckPassword(loginAccount.Password))
                    return new ResponseBuilder(ErrorCodes.PASSWORD_INVALID, lng);

                //if (!PolicyUtil.CheckNickName(loginAccount.NickName))
                //    return new ResponseBuilder(ErrorCodes.NICKNAME_INVALID, lng);

                //if(loginAccount.NickName.ToLower().Equals(loginAccount.UserName.ToLower()))
                //    return new ResponseBuilder(ErrorCodes.ACCOUNT_CHARACTER_NAME_MUST_DEFFENT_ACCNAME, lng);

                string checkMobile = loginAccount.UserName;
                checkMobile = loginAccount.UserName.Replace("+", "");
                if (checkMobile.All(char.IsDigit))
                {
                    if (_authenticateService.CheckUserNameIsMobileOther(checkMobile))
                        return new ResponseBuilder(ErrorCodes.ACCOUNT_CURRENT_IS_MOBILE_ACTIVE_ACCOUNT_OTHER, lng);
                }

                // string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                LoginAccount account = loginAccount;
                account.IpAddress = ipAddress;
                account.UserName = userName;
                AccountInfo accountInfo = null;
                int response = _authenticateService.Register(account, out accountInfo);

                //if (response == -105)
                //    return new ResponseBuilder(ErrorCodes.ACCOUNT_EXIST, lng);
                //else if (response == -106)
                //    return new ResponseBuilder(ErrorCodes.NICKNAME_EXIST, lng);

                if (accountInfo != null)
                {
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                }

                return new ResponseBuilder(response, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.REGISTER_ACCOUNT_ERROR, lng);
            }

        }
        [HttpGet("TelegramAuth")]
        public ActionResult<ResponseBuilder> TelegramAuth(string teleId, string username, string secretKey)
        {
            string lng = "vi";
            try
            {
                if (secretKey != _appSettings.SecretKeyTelegram)
                {
                    return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, lng);
                }
                var account = new LoginAccount();
                string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                account.IpAddress = ipAddress;
                account.UserName = string.IsNullOrEmpty(username) ? $"tl{teleId}" : username;
                account.NickName = string.IsNullOrEmpty(username) ? $"tlnk_{teleId}" : $"tlnk_{username}";
                account.Password = Security.MD5Encrypt($"pw{username}@#tl");
                AccountInfo accountInfo = null;
                int responseStatus = _authenticateService.GetInfoTele(teleId, out accountInfo);
                if (responseStatus == 0)
                {
                    responseStatus = _authenticateService.Login(account, out accountInfo);
                }
                else
                {
                    responseStatus = _authenticateService.Register(account, out accountInfo);
                    if (accountInfo != null)
                    {
                        _authenticateService.UpdateTeleChatID((int)accountInfo.AccountID, teleId);
                    }

                }
                // Trả về token cho client (có thể lưu vào local storage hoặc cookie)
                if (accountInfo != null)
                {
                    try
                    {
                        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                        var parser = Parser.GetDefault();
                        var clientInfo = parser.Parse(userAgent);
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(accountInfo.AccessToken);
                        var expireDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "exp").Value);
                        var createDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "iat").Value);
                        var loginSession = new ALoginSession
                        {
                            AccountID = accountInfo.AccountID,
                            Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                            Device = $"{clientInfo.Device.Family} - {clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                            ExpireDate = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime,
                            IPAddress = ipAddress,
                            IsActive = true,
                            Location = GetLocationFromIP(ipAddress),
                            LoginDate = DateTimeOffset.FromUnixTimeSeconds(createDate).DateTime,
                            SessionID = accountInfo.SessionID
                        };
                        _authenticateService.CreateLoginSession(loginSession);
                    }
                    catch (Exception ex)
                    {
                        NLogManager.Error(ex);
                    }
                    return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                }
                return new ResponseBuilder(responseStatus, lng);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, lng);
            }

        }
        [HttpPost("MetamaskAuth")]
        public ActionResult<ResponseBuilder> MetaMaskLogin([FromBody] JObject payload)
        {
            string lng = "vi";
            try
            {
                var message = payload["message"].ToString();
                var address = payload["address"].ToString();
                var signature = payload["signature"].ToString();
                NLogManager.Info($"MetaMaskLogin: message={message}, address={address}, signature={signature}");
                // Xác minh chữ ký
                var smessage = ExtractTimestampFromMessage(message);
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - smessage > 60)
                {
                    return new ResponseBuilder(ErrorCodes.SIGNATURE_EXPIRED, lng);
                }
                var signer = new EthereumMessageSigner();
                var addressRecovered = signer.EncodeUTF8AndEcRecover(message, signature);

                if (addressRecovered.Equals(address, StringComparison.OrdinalIgnoreCase))
                {
                    var account = new LoginAccount();
                    string ipAddress = IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true);
                    AccountInfo accountInfo = null;
                    account.WAddress = address;
                    int responseStatus = _authenticateService.GetInfoByWAddress(account.WAddress, out accountInfo);
                    account.IpAddress = ipAddress;
                    if (responseStatus == 0)
                    {
                        account.UserName = accountInfo.UserName;
                        account.NickName = $"nk_{account.UserName}";
                        account.Password = Security.MD5Encrypt($"pw{account.UserName}@#");
                        responseStatus = _authenticateService.Login(account, out accountInfo);
                    }
                    else
                    {
                        account.UserName = $"mtk{DateTimeOffset.Now.ToUnixTimeSeconds()}";
                        account.NickName = $"nk_{account.UserName}";
                        account.Password = Security.MD5Encrypt($"pw{account.UserName}@#");
                        responseStatus = _authenticateService.Register(account, out accountInfo);
                    }
                    // Trả về token cho client (có thể lưu vào local storage hoặc cookie)
                    if (accountInfo != null)
                    {
                        try
                        {
                            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                            var parser = Parser.GetDefault();
                            var clientInfo = parser.Parse(userAgent);
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(accountInfo.AccessToken);
                            var expireDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "exp").Value);
                            var createDate = Convert.ToInt64(jwtToken.Claims.FirstOrDefault(x => x.Type == "iat").Value);
                            var loginSession = new ALoginSession
                            {
                                AccountID = accountInfo.AccountID,
                                Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                                Device = $"{clientInfo.Device.Family} - {clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                                ExpireDate = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime,
                                IPAddress = ipAddress,
                                IsActive = true,
                                Location = GetLocationFromIP(ipAddress),
                                LoginDate = DateTimeOffset.FromUnixTimeSeconds(createDate).DateTime,
                                SessionID = accountInfo.SessionID
                            };
                            _authenticateService.CreateLoginSession(loginSession);
                        }
                        catch (Exception ex)
                        {
                            NLogManager.Error(ex);
                        }
                        return new ResponseBuilder(ErrorCodes.SUCCESS, lng, accountInfo);
                    }
                    return new ResponseBuilder(responseStatus, lng);
                }
                else
                {
                    return new ResponseBuilder(ErrorCodes.SIGNATURE_INCORRECT, lng);
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, lng);
        }
        private long ExtractTimestampFromMessage(string message)
        {
            var regex = new Regex(@"authenticate: (\d+)");
            var match = regex.Match(message);

            return long.Parse(match.Groups[1].Value);
            if (match.Success && match.Groups.Count > 1)
            {
            };
            return 0;
        }
        public string GetLocationFromIP(string ipAddress)
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
