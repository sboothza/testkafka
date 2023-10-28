using System.Data.Common;
using Microsoft.Extensions.Options;

namespace DbCommon;

public abstract class Repository<T> : IRepository<T>, IDisposable where T : Entity, new()
{
    private readonly DbConnection? _connection;
    protected readonly DbProviderFactory? _factory;

    protected Repository(string providerName, string connectionString)
    {
        _factory = DbProviderFactories.GetFactory(providerName);
        if (_factory is null)
            throw new DbException("Factory not found");
        _connection = _factory?.CreateConnection();
        if (_connection is null)
            throw new DbException("Could not create connection");
        _connection.ConnectionString = connectionString;
        try
        {
            _connection.Open();
        }
        catch (Exception ex)
        {
            throw new DbException("Could not open connection", ex);
        }
    }

    protected Repository(IOptions<DatabaseConfig> options) : this(options.Value.ProviderName!,
        options.Value.ConnectionString!)
    {
    }

    public List<T> GetMultiple(string sql, DbParameter[] parameters)
    {
        var command = _connection?.CreateCommand();
        if (command is null)
            throw new DbException("Could not create command");

        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);
        var list = new List<T>();
        try
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = new T();
                    item.Load(reader);
                    list.Add(item);
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DbException("Could not execute fetch", ex);
        }
    }

    public T? GetOne(string sql, DbParameter[] parameters)
    {
        var command = _connection?.CreateCommand();
        if (command is null)
            throw new DbException("Could not create command");

        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);

        try
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var item = new T();
                    item.Load(reader);
                    return item;
                }
            }
        }
        catch (Exception ex)
        {
            throw new DbException("Could not execute fetch", ex);
        }

        return null;
    }

    public void Modify(string sql, DbParameter[] parameters)
    {
        var command = _connection?.CreateCommand();
        if (command is null)
            throw new DbException("Could not create command");

        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DbException("Could not execute query", ex);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}