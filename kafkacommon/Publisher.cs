using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;

namespace kafkacommon
{
	public class PublishException : Exception
	{
		public PublishException()
		{
		}

		public PublishException(string message)
			: base(message)
		{
		}

		public PublishException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public interface IPublisher
	{
		Task Publish<T>(string topic, T message);
	}


	public class Publisher : IPublisher, IDisposable
	{
		private readonly IProducer<Null, string> _producer;
		private bool _disposedValue;

		public Publisher(string servers)
		{
			var config = new ProducerConfig { BootstrapServers = servers };
			_producer = new ProducerBuilder<Null, string>(config).Build();
		}


		public async Task Publish<T>(string topic, T message)
		{
			string jsonString = JsonSerializer.Serialize(message);

			try
			{
				var dr = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonString });
				Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
			}
			catch (ProduceException<Null, string> e)
			{
				//Console.WriteLine($"Delivery failed: {e.Error.Reason}");
				throw new PublishException("Delivery failed", e);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
					_producer.Dispose();

				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
