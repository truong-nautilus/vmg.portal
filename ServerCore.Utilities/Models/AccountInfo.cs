using Newtonsoft.Json;
using System;

namespace ServerCore.Utilities.Models
{
    [Serializable]
    public class AccountInfo
    {
        //[JsonIgnore]
        public long AccountID { get; set; }


        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalXu { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalCoin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Avatar { get; set; }

        public string NickName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Level { get; set; }

        [JsonIgnore]
        public bool IsOtp { get; set; }

        [JsonProperty("Token")]
        public string OtpToken { get; set; }
        public bool IsCaptcha { get; set; }
        //  OTP APP
        public string AccessToken { get; set; }
        public string AccessTokenFishing { get; set; }
        public bool IsMobileActived { get; set; }
        public bool IsAllowChat { get; set; }
        public bool IsAgency { get; set; }

        [JsonIgnore]
        public int ErrorCode { get; set; }

        [JsonIgnore]
        public string Mobile { get; set; }
        [JsonIgnore]
        public int LocationID { get; set; }
        //[JsonIgnore]
        public string PreFix { get; set; }
        public bool IsAccountLive { get; set; }
        //[JsonIgnore]
        //public string RefCode { get; set; }
        public decimal VND { get; set; }
        public decimal USDT { get; set; }
        public decimal USD { get; set; }
        public decimal IDR { get; set; }
        public decimal MYR { get; set; }
        public decimal SGD { get; set; }
        public decimal THB { get; set; }
        public decimal MMK { get; set; }
        public decimal AUD { get; set; }
        public decimal BND { get; set; }
        public decimal PHP { get; set; }
        public decimal EUR { get; set; }
        public decimal BRL { get; set; }
        public decimal HKD { get; set; }
        public decimal INR { get; set; }
        public decimal KVND { get; set; }
        public string RefreshToken { get; set; }
        public string SessionID { get; set; }
        public int CurrencyType { get; set; }

        public AccountInfo(AccountDb accountDb)
        {
            AccountID = accountDb.AccountID;
            UserName = accountDb.UserName;
            TotalCoin = accountDb.TotalCoin;
            TotalXu = accountDb.TotalXu;
            Avatar = accountDb.Avatar;
            NickName = accountDb.UserFullname;
            Level = accountDb.Level;
            IsOtp = accountDb.IsOtp == 1 ? true : false;
            IsMobileActived = accountDb.IsMobileActived;
            IsAgency = accountDb.IsAgency == 1 ? true : false;
            LocationID = accountDb.LocationID;
            PreFix = accountDb.PreFix;
            //           RefCode = accountDb.RefCode;
            IsAccountLive = accountDb.IsAccountLive;
            USDT = accountDb.USDT;
            USD = accountDb.USD;
            VND = accountDb.VND;
            IDR = accountDb.IDR;
            MYR = accountDb.MYR;
            SGD = accountDb.SGD;
            THB = accountDb.THB;
            MMK = accountDb.MMK;
            AUD = accountDb.AUD;
            BND = accountDb.BND;
            PHP = accountDb.PHP;
            EUR = accountDb.EUR;
            BRL = accountDb.BRL;
            HKD = accountDb.HKD;
            INR = accountDb.INR;
            KVND = accountDb.KVND;
            CurrencyType = accountDb.CurrencyType;
        }

        public AccountInfo()
        {
        }
    }
}
