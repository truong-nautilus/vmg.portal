using Microsoft.Extensions.Options;
using NetCore.Utils.Interfaces;
using ServerCore.PortalAPI.DataAccess.DAO;
using ServerCore.PortalAPI.Models.VMG;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ServerCore.PortalAPI.DataAccess.DAOImpl
{
    public class VMGDAOImpl : IVMGDAO
    {
        private readonly AppSettings _appSettings;
        private readonly IDBHelper _dbHepler;

        public VMGDAOImpl(IDBHelper dbHepler, IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            _dbHepler = dbHepler;
        }
        public Agency GetAgentById(int id)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@AgencyId", id);
                _dbHepler.SetConnectionString(_appSettings.BillingDatabaseAPIConnectionString);
                var agency = _dbHepler.GetListSP<Agency>("SP_Agency_GetById", pars);
                return agency[0];
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new Agency();
            }
        }
        public ChipValue GetChipByCurrencyID(int currencyID)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_CurrencyID", currencyID);
                _dbHepler.SetConnectionString(_appSettings.BillingDatabaseAPIConnectionString);
                var chipValue = _dbHepler.GetInstanceSP<ChipValue>("SP_VMG_GetChipByCurrencyID", pars);
                return chipValue;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return null;
            }
        }
        public int DepositUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance)
        {
            balance = 0;
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountID", accountID);
                pars[1] = new SqlParameter("@_UserName", username);
                pars[2] = new SqlParameter("@_MerchantID", merchantID);
                pars[3] = new SqlParameter("@_Amount", amount);
                pars[4] = new SqlParameter("@_TransID", transID);
                pars[5] = new SqlParameter("@_Balance", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 28, Scale = 8 };
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(_appSettings.BillingDatabaseAPIConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_VMG_Deposit_User", pars);
                var responseStatus = Convert.ToInt32(pars[6].Value);
                if (responseStatus > 0)
                {
                    balance = Convert.ToDecimal(pars[5].Value);
                }

                return responseStatus;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return -99;
            }
        }
        public int CashoutUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance)
        {
            balance = 0;
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountID", accountID);
                pars[1] = new SqlParameter("@_UserName", username);
                pars[2] = new SqlParameter("@_MerchantID", merchantID);
                pars[3] = new SqlParameter("@_Amount", amount);
                pars[4] = new SqlParameter("@_TransID", transID);
                pars[5] = new SqlParameter("@_Balance", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 28, Scale = 8 };
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(_appSettings.BillingDatabaseAPIConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_VMG_Cashout_User", pars);
                var responseStatus = Convert.ToInt32(pars[6].Value);
                if (responseStatus > 0)
                {
                    balance = Convert.ToDecimal(pars[5].Value);
                }

                return responseStatus;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return -99;
            }
        }
        public List<BetHistory> GetAccountBetHistory(long fromDate, long toDate, int merchantID, out int totalPage, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_MerchantID", merchantID);
                pars[1] = new SqlParameter("@_PageNumber", pageNumber);
                pars[2] = new SqlParameter("@_PageSize", pageSize);
                pars[3] = new SqlParameter("@_TotalPage", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_FromDate", fromDate);
                pars[5] = new SqlParameter("@_ToDate", toDate);
                _dbHepler.SetConnectionString(_appSettings.BillingDatabaseAPIConnectionString);
                var lst = _dbHepler.GetListSP<BetHistory>("SP_VMG_GetAccountBetHistory_V1", pars);

                totalPage = Convert.ToInt32(pars[3].Value);

                return lst;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                totalPage = 0;
                return new List<BetHistory>();
            }
        }
    }
}
