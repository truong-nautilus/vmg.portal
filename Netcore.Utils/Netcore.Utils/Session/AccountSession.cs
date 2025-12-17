using Microsoft.AspNetCore.Http;
using ServerCore.Utilities.Utils;
using System;

namespace NetCore.Utils.Sessions
{
    public class AccountSession
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AccountSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext Current => _httpContextAccessor?.HttpContext;

        public long AccountID
        {
            get
            {
                long accountId = 0;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("AccountId");
                    if (claim != null) Int64.TryParse(claim.Value, out accountId);
                }
                return accountId;
            }
        }

        public string AccountName
        {
            get
            {
                try
                {
                    string val = "";
                    if (Current?.User?.Identity?.IsAuthenticated == true)
                    {
                        val = Current.User.FindFirst("UserName")?.Value ?? "";
                    }
                    return val;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public string NickName
        {
            get
            {
                try
                {
                    if (Current?.User?.Identity?.IsAuthenticated == true)
                    {
                        return Current.User.FindFirst("NickName")?.Value ?? string.Empty;
                    }
                    return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public int MerchantID
        {
            get
            {
                int merchantId = 0;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("MerchantId");
                    if (claim != null) int.TryParse(claim.Value, out merchantId);
                }
                return merchantId;
            }
        }

        public int PlatformID
        {
            get
            {
                int platformId = 0;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("PlatformId");
                    if (claim != null) int.TryParse(claim.Value, out platformId);
                }
                return platformId;
            }
        }

        public int SourceID
        {
            get
            {
                int platformId = 0;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("PlatformId"); // Note: Original code mapped SourceID to PlatformId claim
                    if (claim != null) int.TryParse(claim.Value, out platformId);
                }
                return platformId;
            }
        }

        public bool IsAgency
        {
            get
            {
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("IsAgency");
                    if (claim != null && int.TryParse(claim.Value, out int val))
                    {
                        return val == 1;
                    }
                }
                return false;
            }
        }

        public string IpAddress
        {
            get
            {
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    return Current.Connection?.RemoteIpAddress?.ToString() ?? "";
                }
                return "";
            }
        }

        public string Language
        {
            get
            {
                if (Current?.Request?.Headers != null && Current.Request.Headers.ContainsKey("Accept-Language"))
                {
                    string lang = Current.Request.Headers["Accept-Language"];
                    return lang?.ToLower() ?? "vi";
                }
                return "vi";
            }
        }
        public bool IsOTP
        {
            get
            {
                bool isOtp = false;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("IsOTP");
                    if (claim != null) bool.TryParse(claim.Value, out isOtp);
                }
                return isOtp;
            }
        }

        public int CurrencyID
        {
             get
            {
                int currencyID = 1;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("CurrencyID");
                    if (claim != null) int.TryParse(claim.Value, out currencyID);
                }
                return currencyID;
            }
        }

        public string RefCode
        {
            get
            {
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    return Current.User.FindFirst("RefCode")?.Value ?? "";
                }
                return "";
            }
        }
        public string PreFix
        {
            get
            {
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    return Current.User.FindFirst("PreFix")?.Value ?? "";
                }
                return "";
            }
        }
   
        public int LocationID
        {
            get
            {
                int locationID = 0;
                if (Current?.User?.Identity?.IsAuthenticated == true)
                {
                    var claim = Current.User.FindFirst("LocationID");
                    if (claim != null) int.TryParse(claim.Value, out locationID);
                }
                return locationID;
            }
        }
    }
}