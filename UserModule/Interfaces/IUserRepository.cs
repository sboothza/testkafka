using DbCommon;
using UserModuleCommon;

namespace UserModule.Interfaces;

public interface IUserRepository : IRepository<User>
{
    IEnumerable<User> GetAll();
    User Get(Guid id);
    void Add(Guid id, string username, string name);
    void Delete(Guid id);
    void Update(Guid id, string username, string name);
    string CreateSql();
}