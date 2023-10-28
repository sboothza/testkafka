using System.Data.Common;
using Application;
using KafkaCommon;
using KafkaCommon.Implementations;
using KafkaCommon.Interfaces;
using Npgsql;
using UserModule.Interfaces;
using UserModuleCommon;

namespace UserModuleService
{
    static class Program
    {
        private static IUserRepository? _repo;

        private static void HandleUserRequest(IPublishingSubscriber pubsub, IEnvelope msg)
        {
            var userRequest = (UserRequest)msg;
            if (userRequest.Id == Guid.Empty)
            {
                var users = _repo!.GetAll().Select(UserView.FromUser);
                var userResponse = new UserResponse
                {
                    Users = users.ToArray(),
                    CorrelationId = userRequest.CorrelationId
                };
                pubsub.Publish("test", userResponse);
            }
            else
            {
                var user = UserView.FromUser(_repo!.Get(userRequest.Id));
                var userResponse = new UserResponse
                {
                    Users = new[]
                    {
                        user
                    },
                    CorrelationId = userRequest.CorrelationId
                };
                pubsub.Publish("test", userResponse);
            }
        }

        private static void HandleUserAddRequest(IPublishingSubscriber pubsub, IEnvelope msg)
        {
            var userAddRequest = (UserAddRequest)msg;
            _repo!.Add(userAddRequest.Id, userAddRequest.Username!, userAddRequest.Name!);
            var userAddResponse = new BasicResponse
            {
                Success = true, Message = "Successfully added", Code = 1,
                CorrelationId = userAddRequest.CorrelationId
            };
            pubsub.Publish("test", userAddResponse);
        }

        private static void HandleUserUpdateRequest(IPublishingSubscriber pubsub, IEnvelope msg)
        {
            var userUpdateRequest = (UserUpdateRequest)msg;
            _repo!.Update(userUpdateRequest.Id, userUpdateRequest.Username!, userUpdateRequest.Name!);
            var userUpdateResponse = new BasicResponse
            {
                Success = true, Message = "Successfully updated", Code = 1,
                CorrelationId = userUpdateRequest.CorrelationId
            };
            pubsub.Publish("test", userUpdateResponse);
        }

        private static void HandleUserDeleteRequest(IPublishingSubscriber pubsub, IEnvelope msg)
        {
            var userDeleteRequest = (UserDeleteRequest)msg;
            _repo!.Delete(userDeleteRequest.Id);
            var userDeleteResponse = new BasicResponse
            {
                Success = true, Message = "Successfully deleted", Code = 1,
                CorrelationId = userDeleteRequest.CorrelationId
            };
            pubsub.Publish("test", userDeleteResponse);
        }

        static void Main(string[] args)
        {
            var servers = "localhost:9092";
            Admin.CreateTopic(servers, "test");
            EnvelopeMapper.Scan();
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            _repo = new UserRepository("Npgsql",
                "Server=localhost;Port=5432;Database=users;User Id=postgres;Password=admin;");

            using (IPublishingSubscriber pubsub = new PublishingSubscriber("dotnet-service", "test", servers))
            {
                pubsub.Bind<UserRequest>(HandleUserRequest);
                pubsub.Bind<UserAddRequest>(HandleUserAddRequest);
                pubsub.Bind<UserUpdateRequest>(HandleUserUpdateRequest);
                pubsub.Bind<UserDeleteRequest>(HandleUserDeleteRequest);
                pubsub.Start();
                Console.WriteLine("Press enter...");
                Console.ReadLine();
            }
        }
    }
}