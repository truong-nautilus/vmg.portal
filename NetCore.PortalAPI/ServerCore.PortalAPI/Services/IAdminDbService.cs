using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Services
{
    public interface IAdminDbService
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        Task<(IEnumerable<T> List, int TotalCount)> GetPaginatedListAsync<T>(string baseSql, string checkSql, object param, int pageNumber, int pageSize, string orderBy);
    }
}
