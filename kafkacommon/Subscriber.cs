using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Confluent.Kafka;

namespace kafkacommon
{
	public class SubscriberException : Exception
	{
		public SubscriberException()
		{
		}

		public SubscriberException(string message)
			: base(message)
		{
		}

		public SubscriberException(string message, Exception inner)
			: base(message, inner)
		{
		}

	}

	public class MessageReceivedEventArgs : EventArgs
	{
		public object Message { get; set; }
	}

	public interface ISubscriber
	{
		event EventHandler<MessageReceivedEventArgs> MessageReceived;
		void Start();
	}

	public class Subscriber : ISubscriber, IDisposable
	{
		private readonly IConsumer<Ignore, string> _consumer;
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

		public Subscriber(string consumerGroup, string topic, string servers, int pollMilliseconds)
		{
			_pollMilliseconds = pollMilliseconds;
			var conf = new ConsumerConfig
			{
				GroupId = consumerGroup,
				BootstrapServers = servers,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			_consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
			_consumer.Subscribe(topic);
			_backgroundThread = new Thread(new ThreadStart(ThreadRun));
			_cts = new CancellationTokenSource();
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
						var identifier = envelope["Identifier"].ToString();

						var t = EnvelopeMapper.TypeFromIdentifier(identifier);
						var msg = JsonSerializer.Deserialize(cr.Message.Value, t);
						if (msg is not null)
							OnMessageReceived(new MessageReceivedEventArgs { Message = msg });

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

		public object Consume()
		{
			try
			{
				var cr = _consumer.Consume(_pollMilliseconds);
				if (cr is not null)
				{
					//Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
					JsonNode envelope = JsonNode.Parse(cr.Message.Value);
					var identifier = envelope["Identifier"].ToJsonString();

					var t = EnvelopeMapper.TypeFromIdentifier(identifier);
					var msg = JsonSerializer.Deserialize(cr.Message.Value, t);
					return msg;
				}
				return null;

			}
			catch (ConsumeException e)
			{
				throw new SubscriberException("Error consuming event", e);
				//Console.WriteLine($"Error occurred: {e.Error.Reason}");
			}
		}


		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;
				_cts.Cancel();
				if (_backgroundThread != null)
					_backgroundThread?.Join();

				if (disposing)
					_consumer.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
