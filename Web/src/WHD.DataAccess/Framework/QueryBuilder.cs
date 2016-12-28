using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.DataAccess.Framework
{
    public class QueryBuilder
    {
        private readonly string _connectionString;
        private readonly string _query;

        private CommandType _commandType = System.Data.CommandType.Text;
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        public QueryBuilder(string connectionString, string query)
        {
            _connectionString = connectionString;
            _query = query;
        }

        public QueryBuilder AddParameter(string name, SqlDbType type, object value)
        {
            var parameter = new SqlParameter(name, type);
            parameter.Value = value == null ? DBNull.Value : value;
            _parameters.Add(parameter);
            return this;
        }

        public QueryBuilder CommandType(CommandType type)
        {
            _commandType = type;
            return this;
        }

        public async Task<QueryResult> ExecuteAsync()
        {
            var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch
            {
                connection.Dispose();
                throw;
            }

            var command = new SqlCommand(_query, connection);
            command.CommandType = _commandType;
            foreach (var parameter in _parameters)
                command.Parameters.Add(parameter);

            SqlDataReader reader;
            try
            {
                reader = await command.ExecuteReaderAsync();
            }
            catch
            {
                command.Dispose();
                connection.Dispose();
                throw;
            }

            return new QueryResult(connection, command, reader);
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(_query, connection))
                {
                    command.CommandType = _commandType;
                    foreach (var parameter in _parameters)
                        command.Parameters.Add(parameter);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<object> ExecuteScalarAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(_query, connection))
                {
                    command.CommandType = _commandType;
                    foreach (var parameter in _parameters)
                        command.Parameters.Add(parameter);

                    return await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>()
        {
            return (T)await ExecuteScalarAsync();
        }
    }
}
