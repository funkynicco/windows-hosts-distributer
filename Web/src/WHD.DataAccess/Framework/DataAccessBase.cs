using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.DataAccess.Framework
{
    /// <summary>
    /// Provides a base class for all DataAccess classes.
    /// </summary>
    public abstract class DataAccessBase
    {
        private readonly string _connectionString;

        protected DataAccessBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected QueryBuilder Query(string query)
        {
            return new QueryBuilder(_connectionString, query);
        }

        protected async Task<int> ExecuteAsync(string query)
        {
            int result = 0;

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = query;
                    result = await cmd.ExecuteNonQueryAsync();
                }
            }

            return result;
        }

        protected Task<int> ExecuteAsync(string query, params object[] args)
        {
            return ExecuteAsync(string.Format(query, args));
        }
    }
}
