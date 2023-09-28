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
			using (var subscriber = new Subscriber("test-consumer-group", "test", "localhost:9092", 100))
			{
				subscriber.MessageReceived += (sender, e) =>
										{
											Console.WriteLine(e.Message.ToString());
										};
				subscriber.Start();

				//Thread.Sleep(10000);
				Console.ReadLine();
			}


		}
	}
}