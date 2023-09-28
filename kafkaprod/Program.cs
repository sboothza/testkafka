using System;

using Confluent.Kafka;

using kafkacommon;

namespace Application
{
	class Program
	{
		static void Main(string[] args)
		{
			EnvelopeMapper.Scan();
			using (var publisher = new Publisher("localhost:9092"))
			{
				publisher.Publish("test", new TestMessage { IntValue = 2, StringValue = "test" }).Wait();
				publisher.Publish("test", new TestMessage2 { IntValue = 2, StringValue = "test", SecondStringValue = "test2" }).Wait();
				publisher.Publish("test", new TestMessage3 { IntValue = 2, StringValue = "test", ThirdStringValue = "test3" }).Wait();
			}
		}
	}
}