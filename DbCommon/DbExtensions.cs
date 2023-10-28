using System.Data;
using System.Data.Common;

namespace DbCommon;

public class DbException : Exception
{
    public DbException()
    {
    }

    public DbException(string message)
        : base(message)
    {
    }

    public DbException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public static class DbExtensions
{
    public static DbParameter CreateParameter(this DbCommand command, string name, DbType @type)
    {
        var parm = command.CreateParameter();
        parm.DbType = @type;
        parm.ParameterName = name;
        return parm;
    }

    public static DbParameter CreateParameter(this DbCommand command, string name, DbType @type, object value, int size)
    {
        var parm = command.CreateParameter();
        parm.DbType = @type;
        parm.ParameterName = name;
        parm.Value = value;
        parm.Size = size;
        command.Parameters.Add(parm);
        return parm;
    }

    public static DbParameter CreateParameter(this DbCommand command, string name, DbType @type, object value)
    {
        var parm = command.CreateParameter();
        parm.DbType = @type;
        parm.ParameterName = name;
        parm.Value = value;
        command.Parameters.Add(parm);
        return parm;
    }
    
    public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType @type, object value)
    {
        var parameter = factory.CreateParameter();
        if (parameter != null)
        {
            parameter.DbType = @type;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        throw new DbException("Cannot create parameter");
    }

    public static T? ReturnNullValue<T>()
    {
        switch (typeof(T).Name)
        {
            case "Int32":
            case "Int16":
            case "Byte":
                return (T)Convert.ChangeType(0, typeof(T));
            default: return default;
        }
    }

    public static string? GetString(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<string>() : reader.GetString(col);
    }

    public static int? GetInt32(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<int>() : reader.GetInt32(col);
    }

    public static short? GetInt16(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<short>() : reader.GetInt16(col);
    }

    public static byte? GetByte(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<byte>() : reader.GetByte(col);
    }

    public static bool? GetBoolean(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<bool>() : reader.GetBoolean(col);
    }
    
    public static Guid? GetGuid(this DbDataReader reader, string name)
    {
        var col = reader.GetOrdinal(name);
        return reader.IsDBNull(col) ? ReturnNullValue<Guid>() : reader.GetGuid(col);
    }

    public static string? Quote(this object value, bool mustQuote = true)
    {
        return mustQuote ? $"'{value}'" : value.ToString();
    }

    public static string ToCamel(this string value)
    {
        return value.Length switch
        {
            0 => string.Empty,
            < 2 => value.ToUpper(),
            _ => $"{char.ToUpper(value[0])}{value[1..].ToLowerInvariant()}"
        };
    }

    public static void SqlEscape(this object[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            if (values[i] is DBNull)
                values[i] = "NULL";
            values[i] = values[i].ToString()?.Replace("\'", "\'\'")!;
        }
    }
}