using System.Collections.Generic;

namespace ServerCore.PortalAPI.Core.Domain.Models.Crypto
{
    public class TaTumSolTransactionResp
    {
        public string Jsonrpc { get; set; }
        public TaTumSolTransactionRespResult Result { get; set; }
        public int Id { get; set; }
    }
    public class TaTumSolTransactionRespResult
    {
        public int BlockTime { get; set; }
        public TaTumSolTransactionRespMeta Meta { get; set; }
        public long Slot { get; set; }
        public TaTumSolTransactionRespTransaction Transaction { get; set; }
    }
    public class TaTumSolTransactionRespMeta
    {
        public int ComputeUnitsConsumed { get; set; }
        public object Err { get; set; }
        public int Fee { get; set; }
        public List<object> InnerInstructions { get; set; }
        public TaTumSolTransactionRespLoadedAddresses LoadedAddresses { get; set; }
        public List<string> LogMessages { get; set; }
        public List<long> PostBalances { get; set; }
        public List<object> PostTokenBalances { get; set; }
        public List<long> PreBalances { get; set; }
        public List<object> PreTokenBalances { get; set; }
        public List<object> Rewards { get; set; }
        public TaTumSolTransactionRespStatus Status { get; set; }
    }
    public class TaTumSolTransactionRespLoadedAddresses
    {
        public List<object> Readonly { get; set; }
        public List<object> Writable { get; set; }
    }

    public class TaTumSolTransactionRespStatus
    {
        public object Ok { get; set; }
    }

    public class TaTumSolTransactionRespTransaction
    {
        public TaTumSolTransactionRespMessage Message { get; set; }
        public List<string> Signatures { get; set; }
    }

    public class TaTumSolTransactionRespMessage
    {
        public List<string> AccountKeys { get; set; }
        public TaTumSolTransactionRespHeader Header { get; set; }
        public List<TaTumSolTransactionRespInstruction> Instructions { get; set; }
        public string RecentBlockhash { get; set; }
    }

    public class TaTumSolTransactionRespHeader
    {
        public int NumReadonlySignedAccounts { get; set; }
        public int NumReadonlyUnsignedAccounts { get; set; }
        public int NumRequiredSignatures { get; set; }
    }

    public class TaTumSolTransactionRespInstruction
    {
        public List<int> Accounts { get; set; }
        public string Data { get; set; }
        public int ProgramIdIndex { get; set; }
        public object StackHeight { get; set; }
    }
}
