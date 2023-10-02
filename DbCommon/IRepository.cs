using System.Data.Common;

namespace DbCommon;

public interface IRepository<T>
{
    List<T> GetMultiple(string sql, DbParameter[] parameters);
    T GetOne(string sql, DbParameter[] parameters);
    void Modify(string sql, DbParameter[] parameters);
}