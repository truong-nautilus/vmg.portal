using PortalAPI.Models;
using NetCore.PortalAPI.Core.DTO;
using System;
using System.Collections.Generic;

namespace NetCore.PortalAPI.Core.Interfaces
{
    public interface IEventRepository
    {
        List<EventX2> GetEvent(long accountId);
        int CheckGiftCode(long accountId, string accountName, string nickName, string giftcode, int merchantId, string merchantKey, int sourceId, string ip, out long balance);
    }
}