using System.Text.Json;
using System.Text.Json.Nodes;
using Confluent.Kafka;
namespace kafkacommon;

public class PublishingSubscriber : ISubscriber, IPublisher, IDisposable
{
	private readonly IConsumer<Ignore, string> _consumer;
	private readonly IProducer<Null, string> _producer;
	private bool _disposedValue;
	private readonly Thread _backgroundThread;
	private readonly int _pollMilliseconds;
	public event EventHandler<MessageReceivedEventArgs> MessageReceived;
	private CancellationTokenSource _cts;

	protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
	{
		EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
		if (handler != null)
		{
			handler(this, e);
		}
	}

	public PublishingSubscriber(string consumerGroup, string topic, string servers, int pollMilliseconds)
	{
		_pollMilliseconds = pollMilliseconds;
		var conf = new ConsumerConfig
		{
			GroupId = consumerGroup, BootstrapServers = servers, AutoOffsetReset = AutoOffsetReset.Earliest
		};

		_consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
		_consumer.Subscribe(topic);
		_backgroundThread = new Thread(new ThreadStart(ThreadRun));
		_cts = new CancellationTokenSource();
		var config = new ProducerConfig
		{
			BootstrapServers = servers
		};
		_producer = new ProducerBuilder<Null, string>(config).Build();
	}

	public async Task PublishAsync<T>(string topic, T message)
	{
		string jsonString = JsonSerializer.Serialize(message);

		try
		{
			var dr = await _producer.ProduceAsync(topic, new Message<Null, string>
			{
				Value = jsonString
			});
			Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
		}
		catch (ProduceException<Null, string> e)
		{
			//Console.WriteLine($"Delivery failed: {e.Error.Reason}");
			throw new PublishException("Delivery failed", e);
		}
	}
	
	public void Publish<T>(string topic, T message)
	{
		string jsonString = JsonSerializer.Serialize(message);

		try
		{
			var dr = _producer.ProduceAsync(topic, new Message<Null, string>
			{
				Value = jsonString
			}).Result;
			Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
		}
		catch (ProduceException<Null, string> e)
		{
			//Console.WriteLine($"Delivery failed: {e.Error.Reason}");
			throw new PublishException("Delivery failed", e);
		}
	}

	public void Start()
	{
		_backgroundThread.Start();
	}

	private void ThreadRun()
	{

		while (!_disposedValue)
		{
			try
			{
				var cr = _consumer.Consume(_cts.Token);
				if (cr is not null)
				{
					//Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
					JsonNode envelope = JsonNode.Parse(cr.Message.Value);
					var identifier = "";
					if (envelope["Identifier"] != null)
						identifier = envelope["Identifier"].ToString();
					else
						identifier = envelope["identifier"].ToString();

					var t = EnvelopeMapper.TypeFromIdentifier(identifier);
					var msg = JsonSerializer.Deserialize(cr.Message.Value, t);
					if (msg is not null)
						OnMessageReceived(new MessageReceivedEventArgs
						{
							Message = msg
						});

				}

			}
			catch (ConsumeException e)
			{
				throw new SubscriberException("Error consuming event", e);
				//Console.WriteLine($"Error occurred: {e.Error.Reason}");
			}
			catch (OperationCanceledException ce)
			{
				Console.WriteLine("Cancelled...");
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;
			_cts.Cancel();
			_backgroundThread?.Join();

			if (disposing)
			{
				_consumer.Dispose();
				_producer.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}