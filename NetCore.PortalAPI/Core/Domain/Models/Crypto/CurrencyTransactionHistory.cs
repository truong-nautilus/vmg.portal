using System;

namespace ServerCore.PortalAPI.Core.Domain.Models.Crypto
{
    public class CurrencyTransactionHistory
    {
        public long Id { get; set; }
        public int RequestType { get; set; }
        public string Currency { get; set; }
        public string Chain { get; set; }
        public decimal Amount { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public string SubscriptionType { get; set; }
        public long BlockNumber { get; set; }
        public string TxId { get; set; }
        public string ContractAddress { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyLogo { get; set; }
        public string CurrencySymbol { get; set; }
        public int ChainId { get; set; }
        public string ChainLogo { get; set; }
        public string ChainName { get; set; }
        public string ChainShortName { get; set; }
        public long UserId { get; set; }
        //public decimal VndPrice { get; set; }
        public DateTime CDate { get; set; }
        public int Status { get; set; }
        public bool IsWithdraw { get; set; }
        public long RateToVnd { get; set; }
        public long AmountVnd { get; set; }
    }
}
