using System.Data.Common;

namespace DbCommon;

public class Entity
{
    public Guid Id { get; set; }

    public Entity()
    {
    }

    public virtual void Load(DbDataReader reader)
    {
        Id = reader.GetGuid("id");
    }
}