using System.Data.Common;
using Application;
using kafkacommon;
using Npgsql;
using UserModule.Interfaces;
using UserModuleCommon;
namespace UserModuleService
{
	static class Program
	{
		private static IUserRepository repo;

		private static void HandleUserRequest(TopicManager topicManager, IEnvelope msg)
		{
			var userRequest = msg as UserRequest;
			if (userRequest.Id == Guid.Empty)
			{
				var users = repo.GetAll().Select(UserView.FromUser);
				var userResponse = new UserResponse
				{
					Users = users.ToArray(),
					CorrelationId = userRequest.CorrelationId
				};
				topicManager.Publish("test", userResponse);
			}
			else
			{
				var user = UserView.FromUser(repo.Get(userRequest.Id));
				var userResponse = new UserResponse
				{
					Users = new[]
					{
						user
					},
					CorrelationId = userRequest.CorrelationId
				};
				topicManager.Publish("test", userResponse);
			}
		}

		private static void HandleUserAddRequest(TopicManager topicManager, IEnvelope msg)
		{
			var userAddRequest = msg as UserAddRequest;
			repo.Add(userAddRequest.Id, userAddRequest.Username, userAddRequest.Name);
			var userAddResponse = new BasicResponse
			{
				Success = true, Message = "Successfully added", Code = 1,
				CorrelationId = userAddRequest.CorrelationId
			};
			topicManager.Publish("test", userAddResponse);
		}

		private static void HandleUserUpdateRequest(TopicManager topicManager, IEnvelope msg)
		{
			var userUpdateRequest = msg as UserUpdateRequest;
			repo.Update(userUpdateRequest.Id, userUpdateRequest.Username, userUpdateRequest.Name);
			var userUpdateResponse = new BasicResponse
			{
				Success = true, Message = "Successfully updated", Code = 1,
				CorrelationId = userUpdateRequest.CorrelationId
			};
			topicManager.Publish("test", userUpdateResponse);
		}

		private static void HandleUserDeleteRequest(TopicManager topicManager, IEnvelope msg)
		{
			var userDeleteRequest = msg as UserDeleteRequest;
			repo.Delete(userDeleteRequest.Id);
			var userDeleteResponse = new BasicResponse
			{
				Success = true, Message = "Successfully deleted", Code = 1,
				CorrelationId = userDeleteRequest.CorrelationId
			};
			topicManager.Publish("test", userDeleteResponse);
		}

		static void Main(string[] args)
		{
			EnvelopeMapper.Scan();
			DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
			repo = new UserRepository("Npgsql", "Server=localhost;Port=5432;Database=users;User Id=postgres;Password=admin;");
			var servers = "localhost:9092";


			using (var topicManager = new TopicManager("dotnet-service", "test", servers))
			{
				topicManager.Bind<UserRequest>(HandleUserRequest);
				topicManager.Bind<UserAddRequest>(HandleUserAddRequest);
				topicManager.Bind<UserUpdateRequest>(HandleUserUpdateRequest);
				topicManager.Bind<UserDeleteRequest>(HandleUserDeleteRequest);
				topicManager.Start();
				Console.WriteLine("Press enter...");
				Console.ReadLine();
			}
		}
	}
}