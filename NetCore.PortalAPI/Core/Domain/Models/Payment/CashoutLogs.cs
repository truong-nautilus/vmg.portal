using System;

namespace ServerCore.PortalAPI.Core.Domain.Models.Payment
{
    public class CashoutLogs
    {
        public long id { get; set; }
        public string orgRequestId { get; set; }
        public long accountId { get; set; }
        public string accountName { get; set; }
        public string nickName { get; set; }
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string bankAccount { get; set; }
        public string bankAccountName { get; set; }
        public long amount { get; set; }
        public string description { get; set; }
        public long transIdPartner { get; set; }
        public long realAmount { get; set; }
        public string transCode { get; set; }
        public int status { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime lastModified { get; set; }
        public int lastModifiedInt { get; set; }
        public bool isFinished { get; set; }
    }
}
