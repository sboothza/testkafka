using KafkaCommon;
using KafkaCommon.Implementations;
using kafkaprod;

namespace Application
{
	class Program
	{
		static void Main(string[] args)
		{
			var servers = "localhost:9092";
			EnvelopeMapper.Scan();
			Admin.CreateTopic(servers, "test");
			using (var publisher = new Publisher(servers))
			{
				publisher.PublishAsync("test", new TestMessage { IntValue = 2, StringValue = "test" }).Wait();
				publisher.PublishAsync("test", new TestMessage2 { IntValue = 2, StringValue = "test", SecondStringValue = "test2" }).Wait();
				publisher.PublishAsync("test", new TestMessage3 { IntValue = 2, StringValue = "test", ThirdStringValue = "test3" }).Wait();
			}
		}
	}
}