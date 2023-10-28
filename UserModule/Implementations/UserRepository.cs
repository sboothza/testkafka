using System.Data;
using System.Data.Common;
using UserModuleCommon;
using DbCommon;
using Microsoft.Extensions.Options;
using UserModule.Interfaces;

namespace Application;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(string providerName, string connectionString) : base(providerName, connectionString)
    {
    }

    public UserRepository(IOptions<DatabaseConfig> options) : base(options)
    {
    }

    public string CreateSql() =>
        "CREATE TABLE users (id uuid NOT NULL, username varchar(50) NULL, name varchar(100) NULL, CONSTRAINT users_pk PRIMARY KEY (id));";

    public IEnumerable<User> GetAll()
    {
        return GetMultiple("select * from users;", Array.Empty<DbParameter>());
    }

    public User Get(Guid id)
    {
        return GetOne("select * from users where id = @id;", new[] { _factory!.CreateParameter("id", DbType.Guid, id) })!;
    }

    public void Add(Guid id, string username, string name)
    {
        var parameters = new List<DbParameter>();
        parameters.Add(_factory!.CreateParameter("id", DbType.Guid, id));
        parameters.Add(_factory!.CreateParameter("username", DbType.String, username));
        parameters.Add(_factory!.CreateParameter("name", DbType.String, name));
        Modify("insert into users (id, username, name) values (@id, @username, @name)", parameters.ToArray());
    }

    public void Delete(Guid id)
    {
        var parameters = new List<DbParameter>();
        parameters.Add(_factory!.CreateParameter("id", DbType.Guid, id));
        Modify("delete from users where id = @id", parameters.ToArray());
    }

    public void Update(Guid id, string username, string name)
    {
        var parameters = new List<DbParameter>();
        parameters.Add(_factory!.CreateParameter("username", DbType.String, username));
        parameters.Add(_factory!.CreateParameter("name", DbType.String, name));
        parameters.Add(_factory!.CreateParameter("id", DbType.Guid, id));
        Modify("update users set username = @username, name = @name where id = @id;", parameters.ToArray());
    }
}