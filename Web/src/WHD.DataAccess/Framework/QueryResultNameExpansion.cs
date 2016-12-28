using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility;

namespace WHD.DataAccess.Framework
{
    public partial class QueryResult
    {
        private int GetColumn(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public short GetByte(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, byte defaultValue = 0)
        {
            return GetByte(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public short GetInt16(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, short defaultValue = 0)
        {
            return GetInt16(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public int GetInt32(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, int defaultValue = 0)
        {
            return GetInt32(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public long GetInt64(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, long defaultValue = 0)
        {
            return GetInt64(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public float GetFloat(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, float defaultValue = 0)
        {
            return GetFloat(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public double GetDouble(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, double defaultValue = 0)
        {
            return GetDouble(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public string GetString(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, string defaultValue = null)
        {
            return GetString(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public DateTime? GetUtcDateTime(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, DateTime? defaultValue = null)
        {
            return GetUtcDateTime(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public bool GetBoolean(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, bool defaultValue = false)
        {
            return GetBoolean(GetColumn(column), nullValueBehaviour, defaultValue);
        }

        public EnumType GetEnum<EnumType>(string column, SqlDbType fieldType, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue) where EnumType : struct
        {
            return GetEnum<EnumType>(GetColumn(column), fieldType, nullValueBehaviour);
        }

        public byte? GetNullByte(string column)
        {
            return GetNullByte(GetColumn(column));
        }

        public short? GetNullInt16(string column)
        {
            return GetNullInt16(GetColumn(column));
        }

        public int? GetNullInt32(string column)
        {
            return GetNullInt32(GetColumn(column));
        }

        public long? GetNullInt64(string column)
        {
            return GetNullInt64(GetColumn(column));
        }

        public float? GetNullFloat(string column)
        {
            return GetNullFloat(GetColumn(column));
        }

        public double? GetNullDouble(string column)
        {
            return GetNullDouble(GetColumn(column));
        }
    }
}
