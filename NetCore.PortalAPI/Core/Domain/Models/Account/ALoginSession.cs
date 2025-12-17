using System;

namespace ServerCore.PortalAPI.Core.Domain.Models.Account
{
    public class ALoginSession
    {
        public long ID { get; set; }
        public long AccountID { get; set; }
        public string IPAddress { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Browser { get; set; } // Thông tin trình duyệt
        public string Device { get; set; } // Thông tin thiết bị
        public string SessionID { get; set; }
    }
}
