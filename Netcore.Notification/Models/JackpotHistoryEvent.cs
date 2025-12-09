namespace NetCore.Notification.Models
{
    public class JackpotHistoryEvent
    {
        public long spinID { get; set; }
        public string username { get; set; }
        public long betValue { get; set; }
        public int roomID { get; set; }
        public long prizeValue { get; set; }
        public string createdTime { get; set; }
        public string gameName { get; set; }
    }
}
