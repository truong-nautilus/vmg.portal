namespace ServerCore.PortalAPI.Models.VMG
{
    public class BetHistoryRequest
    {
        public string TimeString { get; set; }
        public long FromDate { get; set; }
        public long ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
