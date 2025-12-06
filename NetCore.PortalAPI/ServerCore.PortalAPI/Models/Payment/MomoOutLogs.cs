using System;

namespace ServerCore.PortalAPI.Models.Payment
{
    public class MomoOutLogs
    {
        public long Id { get; set; }
        public string requestId { get; set; }
        public string bankAccount { get; set; }
        public string amount { get; set; }
        public string urlCallback { get; set; }
        public string signature { get; set; }
        public long accountId { get; set; }
        public string accountName { get; set; }
        public DateTime updatedTime { get; set; }
        public DateTime updatedInt { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime createdInt { get; set; }
        public int status { get; set; }
    }
}
