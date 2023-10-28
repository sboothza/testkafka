using System.Data.Common;

namespace DbCommon;

public abstract class Entity
{
    public Guid Id { get; set; }

    protected Entity()
    {
    }

    public virtual void Load(DbDataReader reader)
    {
        Id = reader.GetGuid("id")!.Value;
    }
}