using System.Data.Common;
using DbCommon;

namespace UserModuleCommon;

public class User : Entity
{
    public string? Username { get; set; }
    public string? Name { get; set; }
    public List<Permission> Permissions { get; protected set; } = new();

    public override void Load(DbDataReader reader)
    {
        base.Load(reader);
        Username = reader.GetString("username");
        Name = reader.GetString("name");
    }
}