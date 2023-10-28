using System.Text.Json;
using System.Text.Json.Nodes;
using Confluent.Kafka;
using KafkaCommon.Interfaces;

namespace KafkaCommon.Implementations;

public class Subscriber : ISubscriber
{
    private readonly IConsumer<Ignore, string> _consumer;
    private bool _disposedValue;
    private readonly Thread? _backgroundThread;
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    private readonly CancellationTokenSource _cts;

    protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
    {
        MessageReceived?.Invoke(this, args);
    }

    public Subscriber(string consumerGroup, string topic, string servers)
    {
        var conf = new ConsumerConfig
        {
            GroupId = consumerGroup, BootstrapServers = servers, AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        _consumer.Subscribe(topic);
        _backgroundThread = new Thread(new ThreadStart(ThreadRun));
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        _backgroundThread?.Start();
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    private void ThreadRun()
    {
        while (!_disposedValue)
        {
            try
            {
                var cr = _consumer.Consume(_cts.Token);
                if (cr is null)
                    continue;

                //Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                var envelope = JsonNode.Parse(cr.Message.Value);
                string identifier;
                if (envelope is null)
                    throw new InvalidCastException("Message is not envelope");

                if (envelope["Identifier"] != null)
                    identifier = envelope["Identifier"]!.ToString();
                else
                    throw new SubscriberException("Message is not envelope");

                var t = EnvelopeMapper.TypeFromIdentifier(identifier);
                if (t is null)
                    throw new SubscriberException("Type not found");

                if (JsonSerializer.Deserialize(cr.Message.Value, t) is IEnvelope msg)
                    OnMessageReceived(new MessageReceivedEventArgs { Message = msg });
            }
            catch (ConsumeException e)
            {
                throw new SubscriberException("Error consuming event", e);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancelled...");
            }
            catch (Exception ex)
            {
                throw new SubscriberException("Error consuming event", ex);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) 
            return;
        _disposedValue = true;
        _cts.Cancel();
        _backgroundThread?.Join();

        if (disposing)
            _consumer.Dispose();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}