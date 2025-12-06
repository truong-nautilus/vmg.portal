using System;

namespace ServerCore.PortalAPI.Models.Payment
{
    public class CardLogs
    {
        public long id { get; set; }
        public int partnerId { get; set; }
        public string orgRequestId { get; set; }
        public long accountId { get; set; }
        public string accountName { get; set; }
        public string nickName { get; set; }
        public string carrier { get; set; }
        public string pin { get; set; }
        public string serial { get; set; }
        public int amount { get; set; }
        public int realAmount { get; set; }
        public int virtualAmount { get; set; }
        public string transId { get; set; }
        public string description { get; set; }
        public DateTime createdTime { get; set; }
        public int createdInt { get; set; }
        public DateTime lastModified { get; set; }
        public int lastModifiedInt { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}
