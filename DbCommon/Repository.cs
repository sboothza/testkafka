using System.Data.Common;
using Microsoft.Extensions.Options;
namespace DbCommon;

public class Repository<T> : IRepository<T>, IDisposable where T : Entity, new()
{
    protected readonly DbConnection _connection;
    protected readonly DbProviderFactory _factory;

    public Repository(string providerName, string connectionString)
    {
        _factory = DbProviderFactories.GetFactory(providerName);
        _connection = _factory.CreateConnection();
        _connection.ConnectionString = connectionString;
        _connection.Open();
    }
    
    public Repository(IOptions<DatabaseConfig> options)
    {
        _factory = DbProviderFactories.GetFactory(options.Value.ProviderName);
        _connection = _factory.CreateConnection();
        _connection.ConnectionString = options.Value.ConnectionString;
        _connection.Open();
    }

    public List<T> GetMultiple(string sql, DbParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);
        var list = new List<T>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                T item = new T();
                item.Load(reader);
                list.Add(item);
            }
        }

        return list;
    }

    public T GetOne(string sql, DbParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                T item = new T();
                item.Load(reader);
                return item;
            }
        }

        return null;
    }

    public void Modify(string sql, DbParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Clear();
        command.Parameters.AddRange(parameters);
        command.ExecuteNonQuery();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}