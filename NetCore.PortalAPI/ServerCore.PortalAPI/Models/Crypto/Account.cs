using System;

namespace ServerCore.PortalAPI.Models.Crypto
{
    public class Account
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountUserName { get; set; }
        public int AvatarID { get; set; }
        public long Balance { get; set; }
        public int Status { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDay { get; set; }
        public string PhoneNumber { get; set; }
        public int PendingMessage { get; set; }
        public int PendingGiftcode { get; set; }
        public int TotalWin { get; set; }
        public int TotalLose { get; set; }
        public int TotalDraw { get; set; }
        public bool IsUpdateAccountName { get; set; }
        public int? AuthenType { get; set; }
        public long RankID { get; set; }
        public long RankDepositID { get; set; }
        public long VP { get; set; }
        public long VPDeposit { get; set; }
        public string RankName { get; set; }
        public string RankNameDeposit { get; set; }
        public string PhoneSafeNo { get; set; }
        public string SignalID { get; set; }
        public long SafeBalance { get; set; }

        public int IsFB { get; set; }
        public int IsFBReward { get; set; }

        public int IsUpdatedFB { get; set; }

        public long TelegramID { get; set; }

        public string FullName { get; set; }

        public bool IsPassTwo { get; set; }
        public string Email { get; set; }
        public string AreaCode { get; set; }

    }
}
