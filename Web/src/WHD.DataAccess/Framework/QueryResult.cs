using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility;
using WHD.Utility.Interfaces;

namespace WHD.DataAccess.Framework
{
    public partial class QueryResult : IQueryResult, IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly SqlCommand _command;
        private readonly SqlDataReader _reader;

        public QueryResult(SqlConnection connection, SqlCommand command, SqlDataReader reader)
        {
            _connection = connection;
            _command = command;
            _reader = reader;
        }

        public void Dispose()
        {
            _reader.Dispose();
            _command.Dispose();

            switch (_connection.State)
            {
                case ConnectionState.Open:
                case ConnectionState.Fetching:
                case ConnectionState.Executing:
                    _connection.Close();
                    break;
            }
            _connection.Dispose();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Task<bool> ReadAsync()
        {
            return _reader.ReadAsync();
        }

        public Task<bool> NextResultAsync()
        {
            return _reader.NextResultAsync();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public short GetByte(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, byte defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetByte(i);
        }

        public short GetInt16(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, short defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetInt16(i);
        }

        public int GetInt32(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, int defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetInt32(i);
        }

        public long GetInt64(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, long defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetInt64(i);
        }

        public float GetFloat(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, float defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return (float)_reader.GetDouble(i);
        }

        public double GetDouble(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, double defaultValue = 0)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetDouble(i);
        }

        public string GetString(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, string defaultValue = null)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetString(i);
        }

        public DateTime? GetUtcDateTime(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, DateTime? defaultValue = null)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                _reader.IsDBNull(i))
                return defaultValue;

            DateTime date = _reader.GetDateTime(i);
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return date;
        }

        public bool GetBoolean(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, bool defaultValue = false)
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                   _reader.IsDBNull(i))
                return defaultValue;

            return _reader.GetBoolean(i);
        }

        public EnumType GetEnum<EnumType>(int i, SqlDbType fieldType, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue) where EnumType : struct
        {
            if (nullValueBehaviour == NullValueBehaviour.ReturnDefaultValue &&
                   _reader.IsDBNull(i))
                return default(EnumType);

            EnumType value = default(EnumType);
            switch (fieldType)
            {
                case SqlDbType.TinyInt:
                    value = (EnumType)(object)(short)GetByte(i, nullValueBehaviour);
                    break;
                case SqlDbType.SmallInt:
                    value = (EnumType)(object)GetInt16(i, nullValueBehaviour);
                    break;
                case SqlDbType.Int:
                    value = (EnumType)(object)GetInt32(i, nullValueBehaviour);
                    break;
                case SqlDbType.BigInt:
                    value = (EnumType)(object)GetInt64(i, nullValueBehaviour);
                    break;
            }

            return value;
        }

        public byte? GetNullByte(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return _reader.GetByte(i);
        }

        public short? GetNullInt16(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return _reader.GetInt16(i);
        }

        public int? GetNullInt32(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return _reader.GetInt32(i);
        }

        public long? GetNullInt64(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return _reader.GetInt64(i);
        }

        public float? GetNullFloat(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return (float)_reader.GetFloat(i);
        }

        public double? GetNullDouble(int i)
        {
            if (_reader.IsDBNull(i))
                return null;

            return _reader.GetDouble(i);
        }
    }
}
