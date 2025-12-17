using Microsoft.Extensions.Options;
using NetCore.Utils.Interfaces;
using NetCore.PortalAPI.Core.Interfaces;
using ServerCore.PortalAPI.Core.Domain.Models;
using ServerCore.PortalAPI.Core.Domain.Models.Crypto;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace NetCore.PortalAPI.Infrastructure.Persistence.Repositories
{
    public class CryptoChargeRepository : ICryptoChargeRepository
    {
        private readonly AppSettings appSettings;
        private readonly IDBHelper _dbHepler;

        public CryptoChargeRepository(IDBHelper dbHepler, IOptions<AppSettings> options)
        {
            appSettings = options.Value;
            _dbHepler = dbHepler;
        }
        public List<CurrencyProfile> GetListCurrency()
        {
            try
            {
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                var res = _dbHepler.GetListSP<CurrencyProfile>("SP_CurrencyProfile_Select_All");
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }

            return new List<CurrencyProfile>();
        }
        public string GetAddress(long userId, int chainId)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = userId;
                param[1] = new SqlParameter("@_ChainId", SqlDbType.Int);
                param[1].Value = chainId;
                param[2] = new SqlParameter("@_Address", SqlDbType.NVarChar);
                param[2].Direction = ParameterDirection.Output;
                param[2].Size = 255;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_UserChainAddress_Select", param.ToArray());

                string address = Convert.ToString(param[2].Value);

                return address;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return "";
        }
        public string GetChainCreateName(int chainId)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@_ChainId", SqlDbType.Int);
                param[0].Value = chainId;
                param[1] = new SqlParameter("@_ChainCreateName", SqlDbType.NVarChar);
                param[1].Direction = ParameterDirection.Output;
                param[1].Size = 50;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_ChainProfile_Select_ChainCreateName", param.ToArray());

                string chainCreateName = Convert.ToString(param[1].Value);

                return chainCreateName;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return "";
        }
        public int CreateAddress(long userId, int chainId, string address, string privateKey)
        {
            int response = -99;

            try
            {
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = userId;
                param[1] = new SqlParameter("@_ChainId", SqlDbType.Int);
                param[1].Value = chainId;
                param[2] = new SqlParameter("@_Address", SqlDbType.NVarChar);
                param[2].Value = address;
                param[3] = new SqlParameter("@_PrivateKey", SqlDbType.NVarChar);
                param[3].Value = privateKey;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_UserChainAddress_Insert", param.ToArray());

                response = Convert.ToInt32(param[4].Value);

                return response;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return response;
        }
        public int UpdateTaTum(long userId, int chainId, string tatumtokenId, string tatumNativeId)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = userId;
                param[1] = new SqlParameter("@_ChainId", SqlDbType.Int);
                param[1].Value = chainId;
                param[2] = new SqlParameter("@_TaTumTokenId", SqlDbType.NVarChar);
                param[2].Value = tatumtokenId;
                param[3] = new SqlParameter("@_TaTumNativeId", SqlDbType.NVarChar);
                param[3].Value = tatumNativeId;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_UserChainAddress_Update_Tatum", param.ToArray());

                return 1;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return -99;
        }
        public int Deposit(string currrency, string chain, decimal amount, string senderAddress, string receiverAddress, string subscriptionType, long blockNumber, string txId, string contractAddress, out string currencySymbol, out decimal currencyPrice, out long rateUsdtToVnd, out long amountVnd)
        {
            currencySymbol = "";
            currencyPrice = 0;
            rateUsdtToVnd = 0;
            amountVnd = 0;

            try
            {
                SqlParameter[] param = new SqlParameter[14];
                param[0] = new SqlParameter("@_Currency", SqlDbType.NVarChar);
                param[0].Value = currrency;

                param[1] = new SqlParameter("@_Chain", SqlDbType.NVarChar);
                param[1].Value = chain;

                param[2] = new SqlParameter("@_Amount", SqlDbType.Decimal);
                param[2].Value = amount;

                param[3] = new SqlParameter("@_SenderAddress", SqlDbType.NVarChar);
                param[3].Value = senderAddress;

                param[4] = new SqlParameter("@_ReceiverAddress", SqlDbType.NVarChar);
                param[4].Value = receiverAddress;

                param[5] = new SqlParameter("@_SubscriptionType", SqlDbType.NVarChar);
                param[5].Value = subscriptionType;

                param[6] = new SqlParameter("@_BlockNumber", SqlDbType.NVarChar);
                param[6].Value = blockNumber;

                param[7] = new SqlParameter("@_TxId", SqlDbType.NVarChar);
                param[7].Value = txId;


                param[8] = new SqlParameter("@_ContractAddress", SqlDbType.NVarChar);
                param[8].Value = contractAddress;

                param[9] = new SqlParameter("@_CurrencySymbol", SqlDbType.NVarChar, 50);
                param[9].Direction = ParameterDirection.Output;

                param[10] = new SqlParameter("@_CurrnecyPrice", SqlDbType.Decimal);
                param[10].Precision = 18;
                param[10].Scale = 8;
                param[10].Direction = ParameterDirection.Output;

                param[11] = new SqlParameter("@_RateUsdtToVnd", SqlDbType.BigInt);
                param[11].Direction = ParameterDirection.Output;

                param[12] = new SqlParameter("@_AmountVnd", SqlDbType.BigInt);
                param[12].Direction = ParameterDirection.Output;

                param[13] = new SqlParameter("@_Response", SqlDbType.Int);
                param[13].Direction = ParameterDirection.Output;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_CurrencyTransaction_Deposit_Token", param.ToArray());

                currencySymbol = Convert.ToString(param[9].Value);
                currencyPrice = Convert.ToDecimal(param[10].Value);
                rateUsdtToVnd = Convert.ToInt64(param[11].Value);
                amountVnd = Convert.ToInt64(param[12].Value);

                return Convert.ToInt32(param[13].Value);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return -99;
        }
        public void GetUserIdByAddress(string address, out long userId)
        {
            userId = 0;
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_Address", address));

                param.Add(new SqlParameter("@_UserId", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                });
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_UserChainAddess_Get_UserId_By_Address", param.ToArray());

                userId = Convert.ToInt64(param[param.Count - 1].Value);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

        }
        public Account GetAccountInfo(long accountId, string username, string phone, int ServiceID)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
                param.Add(new SqlParameter("@_Phone", phone));
                param.Add(new SqlParameter("@_UserName", username));
                param.Add(new SqlParameter("@_ServiceID", ServiceID));
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                return _dbHepler.GetInstanceSP<Account>("SP_Account_GetAccountInfo", param.ToArray());
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }
            return null;
        }
        public List<CurrencyTransactionHistory> GetHistoryByUserId(long userId, int pageIndex, int pageSize, out int totalRecord)
        {
            totalRecord = 0;

            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_UserId", userId));
                param.Add(new SqlParameter("@_PageIndex", pageIndex));
                param.Add(new SqlParameter("@_PageSize", pageSize));
                param.Add(new SqlParameter("@_TotalRecord", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                });

                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                var list = _dbHepler.GetListSP<CurrencyTransactionHistory>("SP_CurrencyTransaction_History_User_Paging", param.ToArray());


                totalRecord = Convert.ToInt32(param[param.Count - 1].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return new List<CurrencyTransactionHistory>();
        }
        public void GetCoreConfig(string ParamType, string Code, out string Value)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = Code;
                param[2] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Direction = ParameterDirection.Output;
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_Param_Config_Get_Value", param.ToArray());
                Value = Convert.ToString(param[2].Value);
                return;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }
            Value = string.Empty;

        }
        public int Withdraw(long userId, int currencyId, int chainId, decimal amount, long rateToVnd, long amountVnd, string receiverAddress, int status, out long balance)
        {
            balance = 0;

            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_UserId", userId));
                param.Add(new SqlParameter("@_CurrencyId", currencyId));
                param.Add(new SqlParameter("@_ChainId", chainId));
                param.Add(new SqlParameter("@_Amount", amount));
                param.Add(new SqlParameter("@_RateToVnd", rateToVnd));
                param.Add(new SqlParameter("@_AmountVnd", amountVnd));
                param.Add(new SqlParameter("@_ReceiverAddress", receiverAddress));
                param.Add(new SqlParameter("@_Status", status));
                param.Add(new SqlParameter("@_Balance", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                });
                param.Add(new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                });
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_CurrencyTransaction_Withdraw_Token", param.ToArray());

                balance = Convert.ToInt64(param[param.Count - 2].Value);

                return Convert.ToInt32(param[param.Count - 1].Value);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return -99;
        }
        public List<ChainProfile> GetListChain(int currencyId)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_CurrencyId", currencyId));
                _dbHepler.SetConnectionString(appSettings.CryptoConnectionString);

                var list = _dbHepler.GetListSP<ChainProfile>("SP_ChainProfile_Select_By_CurrencyId", param.ToArray());
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            finally
            {
                if (_dbHepler != null)
                {
                    _dbHepler.Close();
                }
            }

            return new List<ChainProfile>();
        }
    }
}
