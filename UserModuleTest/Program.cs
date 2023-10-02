using System;
using Confluent.Kafka;
using kafkacommon;
using UserModuleCommon;

namespace Application
{
	class Program
	{
		static void HandleUserResponse(TopicManager topicManager, IEnvelope envelope)
		{
			var userResponse = envelope as UserResponse;
			Console.WriteLine($"User response id:{userResponse.CorrelationId}");
			if (userResponse.Users != null)
				foreach (var user in userResponse.Users)
					Console.WriteLine(user);
		}

		static void HandleBasicResponse(TopicManager topicManager, IEnvelope envelope)
		{
			var basicResponse = envelope as BasicResponse;
			Console.WriteLine(basicResponse);
		}
		
		static void Main(string[] args)
		{
			var servers = "localhost:9092";
			EnvelopeMapper.Scan();

			using (var topicManager = new TopicManager("dotnet-cli", "test", servers))
			{
				topicManager.Bind<UserResponse>(HandleUserResponse);
				topicManager.Bind<BasicResponse>(HandleBasicResponse);
				
				topicManager.Start();

				var quit = false;
				while (!quit)
				{
					Console.Write("> ");
					var command = Console.ReadLine();
					var words = command.Split(' ');
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
							topicManager.Publish("test", userRequestAll);
							break;

						case "get":
							var userRequest = new UserRequest
							{
								Id = Guid.Parse(words[1]),
								CorrelationId = Guid.NewGuid()
							};
							Console.WriteLine($"get request id:{userRequest.CorrelationId}");
							topicManager.Publish("test", userRequest);
							break;

						case "add":
							var addRequest = new UserAddRequest
							{
								Id = Guid.Parse(words[1]), Username = words[2], Name = words[3],
								CorrelationId = Guid.NewGuid()
							};
							Console.WriteLine($"add request id:{addRequest.CorrelationId}");
							topicManager.Publish("test", addRequest);
							break;

						case "update":
							var updateRequest = new UserUpdateRequest
							{
								Id = Guid.Parse(words[1]), Username = words[2], Name = words[3],
								CorrelationId = Guid.NewGuid()
							};
							Console.WriteLine($"udpate request id:{updateRequest.CorrelationId}");
							topicManager.Publish("test", updateRequest);
							break;

						case "delete":
							var deleteRequest = new UserDeleteRequest
							{
								Id = Guid.Parse(words[1]),
								CorrelationId = Guid.NewGuid()
							};
							Console.WriteLine($"delete request id:{deleteRequest.CorrelationId}");
							topicManager.Publish("test", deleteRequest);
							break;
					}
				}

			}

		}
	}
}