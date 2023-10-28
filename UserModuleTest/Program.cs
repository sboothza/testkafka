using KafkaCommon;
using KafkaCommon.Implementations;
using KafkaCommon.Interfaces;
using UserModuleCommon;

namespace Application
{
	class Program
	{
		private static void HandleUserResponse(ISubscriber pubsub, IEnvelope envelope)
		{
			var userResponse = (UserResponse)envelope;
			Console.WriteLine($"User response id:{userResponse.CorrelationId}");
			if (userResponse.Users is { Length: > 0 })
				foreach (var user in userResponse.Users)
					Console.WriteLine(user);
		}

		static void HandleBasicResponse(ISubscriber pubsub, IEnvelope envelope)
		{
			var basicResponse = envelope as BasicResponse;
			Console.WriteLine(basicResponse);
		}
		
		static void Main(string[] args)
		{
			var servers = "localhost:9092";
			EnvelopeMapper.Scan();

			using (IPublishingSubscriber pubsub = new PublishingSubscriber("dotnet-cli", "test", servers))
			{
				pubsub.Bind<UserResponse>(HandleUserResponse);
				pubsub.Bind<BasicResponse>(HandleBasicResponse);
				
				pubsub.Start();

				var quit = false;
				while (!quit)
				{
					Console.Write("> ");
					var command = Console.ReadLine();
					var words = command?.Split(' ');
					if (words != null)
						switch (words[0])
						{
							case "help":
								Console.WriteLine("Help");
								Console.WriteLine("getall - get all users");
								Console.WriteLine("get <id> - get user for id");
								Console.WriteLine("add <id> <username> <name> - add user");
								Console.WriteLine("update <id> <username> <name> - update user");
								Console.WriteLine("delete <id> - delete user");
								Console.WriteLine("quit - exit");
								break;

							case "quit":
								quit = true;
								break;

							case "getall":
								var userRequestAll = new UserRequest
								{
									Id = Guid.Empty,
									CorrelationId = Guid.NewGuid()
								};
								Console.WriteLine($"getall request id:{userRequestAll.CorrelationId}");
								pubsub.Publish("test", userRequestAll);
								break;

							case "get":
								var userRequest = new UserRequest
								{
									Id = Guid.Parse(words[1]),
									CorrelationId = Guid.NewGuid()
								};
								Console.WriteLine($"get request id:{userRequest.CorrelationId}");
								pubsub.Publish("test", userRequest);
								break;

							case "add":
								var addRequest = new UserAddRequest
								{
									Id = Guid.Parse(words[1]), Username = words[2], Name = words[3],
									CorrelationId = Guid.NewGuid()
								};
								Console.WriteLine($"add request id:{addRequest.CorrelationId}");
								pubsub.Publish("test", addRequest);
								break;

							case "update":
								var updateRequest = new UserUpdateRequest
								{
									Id = Guid.Parse(words[1]), Username = words[2], Name = words[3],
									CorrelationId = Guid.NewGuid()
								};
								Console.WriteLine($"udpate request id:{updateRequest.CorrelationId}");
								pubsub.Publish("test", updateRequest);
								break;

							case "delete":
								var deleteRequest = new UserDeleteRequest
								{
									Id = Guid.Parse(words[1]),
									CorrelationId = Guid.NewGuid()
								};
								Console.WriteLine($"delete request id:{deleteRequest.CorrelationId}");
								pubsub.Publish("test", deleteRequest);
								break;
						}
				}

			}

		}
	}
}