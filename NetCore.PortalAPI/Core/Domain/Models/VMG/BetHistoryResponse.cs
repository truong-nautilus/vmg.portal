using System;
using System.Collections.Generic;

namespace ServerCore.PortalAPI.Core.Domain.Models.VMG
{
    public class BetHistoryResponse
    {
        public int TotalPage { get; set; }
        public List<BetHistory> Lst { get; set; }
    }
    public class BetHistory
    {
        public string Type { get; set; }
        public long? BetID { get; set; }
        public string RelatedBetID { get; set; }
        public decimal BetAmount { get; set; }
        public decimal ValidBetAmount { get; set; }
        public decimal PrizeValue { get; set; }
        public int GameID { get; set; }
        public long SessionID { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UserName { get; set; }
        public int AgencyID { get; set; }
    }
}
