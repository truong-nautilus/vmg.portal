using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.PortalAPI.Core.Interfaces
{
    public interface IGiftcodeRepository
    {
        int Giftcode(int accountId, string username, string giftcode, string clientIP, string mobile, ref int value);
    }
}
