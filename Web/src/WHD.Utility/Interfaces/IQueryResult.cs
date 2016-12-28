using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.Utility.Interfaces
{
    public interface IQueryResult
    {
        short GetByte(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, byte defaultValue = 0);

        short GetInt16(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, short defaultValue = 0);

        int GetInt32(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, int defaultValue = 0);

        long GetInt64(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, long defaultValue = 0);

        float GetFloat(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, float defaultValue = 0);

        double GetDouble(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, double defaultValue = 0);

        string GetString(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, string defaultValue = null);

        DateTime? GetUtcDateTime(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, DateTime? defaultValue = null);

        bool GetBoolean(int i, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, bool defaultValue = false);

        EnumType GetEnum<EnumType>(int i, SqlDbType fieldType, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue) where EnumType : struct;

        byte? GetNullByte(int i);

        short? GetNullInt16(int i);

        int? GetNullInt32(int i);

        long? GetNullInt64(int i);

        float? GetNullFloat(int i);

        double? GetNullDouble(int i);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////// column name expansion ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        short GetByte(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, byte defaultValue = 0);

        short GetInt16(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, short defaultValue = 0);

        int GetInt32(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, int defaultValue = 0);

        long GetInt64(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, long defaultValue = 0);

        float GetFloat(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, float defaultValue = 0);

        double GetDouble(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, double defaultValue = 0);

        string GetString(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, string defaultValue = null);

        DateTime? GetUtcDateTime(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, DateTime? defaultValue = null);

        bool GetBoolean(string column, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue, bool defaultValue = false);

        EnumType GetEnum<EnumType>(string column, SqlDbType fieldType, NullValueBehaviour nullValueBehaviour = NullValueBehaviour.ReturnDefaultValue) where EnumType : struct;

        byte? GetNullByte(string column);

        short? GetNullInt16(string column);

        int? GetNullInt32(string column);

        long? GetNullInt64(string column);

        float? GetNullFloat(string column);

        double? GetNullDouble(string column);
    }
}
