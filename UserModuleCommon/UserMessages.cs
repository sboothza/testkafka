using KafkaCommon.Interfaces;

namespace UserModuleCommon;

public class UserRequest : IEnvelope
{
	public Guid Id { get; set; }
	public Guid CorrelationId { get; set; } 
	public string Identifier => "UserRequest";
}

public class UserView
{
	public Guid? Id { get; set; }
	public string? Username { get; set; }
	public string? Name { get; set; }
	public List<Permission> Permissions { get; protected set; } = new();

	public static UserView FromUser(User user)
	{
		var userView = new UserView { Id = user.Id, Username = user.Username!, Name = user.Name! };
		return userView;
	}

	public override string ToString()
	{
		return $"id:{Id} username:{Username} name:{Name}";
	}
}

public class UserResponse : IEnvelope
{
	public UserView[]? Users { get; set; }
	public Guid CorrelationId { get; set; } 
	public string Identifier => "UserResponse";
}

public class UserAddRequest : IEnvelope
{
	public Guid Id { get; set; }
	public Guid CorrelationId { get; set; } 
	public string? Username { get; set; }
	public string? Name { get; set; }
	public List<Permission> Permissions { get; protected set; } = new();
	public string Identifier => "UserAddRequest";
}

public class UserUpdateRequest : IEnvelope
{
	public Guid CorrelationId { get; set; } 
	public Guid Id { get; set; }
	public string? Username { get; set; }
	public string? Name { get; set; }
	public List<Permission> Permissions { get; protected set; } = new();
	public string Identifier => "UserUpdateRequest";
}

public class UserDeleteRequest : IEnvelope
{
	public Guid CorrelationId { get; set; } 
	public Guid Id { get; set; }
	public string Identifier => "UserDeleteRequest";
}

public class BasicResponse : IEnvelope
{
	public Guid CorrelationId { get; set; } 
	public bool Success { get; set; }
	public int Code { get; set; }
	public string? Message { get; set; }
	public string Identifier => "BasicResponse";

	public override string ToString()
	{
		return $"success:{Success} code:{Code} msg:{Message} id:{CorrelationId}";
	}
}

