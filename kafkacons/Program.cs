using KafkaCommon;
using KafkaCommon.Implementations;
using KafkaCommon.Interfaces;

namespace Application
{
	class Program
	{
		static void Main(string[] args)
		{
			EnvelopeMapper.Scan();
			using (ISubscriber subscriber = new Subscriber("test-consumer-group", "test", "localhost:9092"))
			{
				subscriber.MessageReceived += (sender, e) =>
										{
											Console.WriteLine(e.Message?.ToString());
										};
				subscriber.Start();

				//Thread.Sleep(10000);
				Console.ReadLine();
			}


		}
	}
}