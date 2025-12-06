using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ServerCore.PortalAPI.Services
{
    public class AdminDbService : IAdminDbService
    {
        private readonly string _connectionString;

        public AdminDbService(IConfiguration configuration)
        {
            // Try to get connection string from different possible keys
            _connectionString = configuration.GetConnectionString("BillingDatabaseAPIConnectionString");
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                // Fallback to searching in AppSettings section if not found in ConnectionStrings
                _connectionString = configuration["AppSettings:BillingDatabaseAPIConnectionString"];
            }
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Database connection string not found. Checked 'BillingDatabaseAPIConnectionString'");
            }
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<(IEnumerable<T> List, int TotalCount)> GetPaginatedListAsync<T>(string baseSql, string checkSql, object param, int pageNumber, int pageSize, string orderBy)
        {
            using var connection = CreateConnection();
            
            // Count query
            string countSql = $"SELECT COUNT(*) FROM ({checkSql}) as CountTable";
            int totalCount = await connection.ExecuteScalarAsync<int>(countSql, param);

            // Pagination query
            int offset = (pageNumber - 1) * pageSize;
            string paginatedSql = $"{baseSql} ORDER BY {orderBy} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            var dynamicParams = new DynamicParameters(param);
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", pageSize);

            var list = await connection.QueryAsync<T>(paginatedSql, dynamicParams);

            return (list, totalCount);
        }
    }
}
