using System.Text.Json;
using Confluent.Kafka;
using KafkaCommon.Interfaces;

namespace KafkaCommon.Implementations;

public class PublishingSubscriber : Subscriber, IPublishingSubscriber
{
    private readonly IProducer<Null, string> _producer;
    private readonly Dictionary<Type, Action<IPublishingSubscriber, IEnvelope>> _bindings;
    private Action<IPublishingSubscriber, IEnvelope>? _bindingElse;
    
    public PublishingSubscriber(string consumerGroup, string topic, string servers) : base(consumerGroup, topic,
        servers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = servers
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
        _bindings = new Dictionary<Type, Action<IPublishingSubscriber, IEnvelope>>();
        MessageReceived += OnMessageReceived;
    }
    
    public void Bind<T>(Action<IPublishingSubscriber, IEnvelope> action)
    {
        _bindings[typeof(T)] = action;
    }
    
    public void BindElse(Action<IPublishingSubscriber, IEnvelope> action)
    {
        _bindingElse = action;
    }
    
    protected virtual void OnMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        var type = args.Message!.GetType();
        if (_bindings.TryGetValue(type, out var action))
        {
            action.Invoke(this, args.Message);
        }
        else
        {
            _bindingElse?.Invoke(this, args.Message);
        }
    }

    public async Task PublishAsync<T>(string topic, T message) where T : IEnvelope
    {
        var jsonString = JsonSerializer.Serialize(message);

        try
        {
            var dr = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonString });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            throw new PublishException("Delivery failed", e);
        }
    }

    public void Publish<T>(string topic, T message) where T : IEnvelope
    {
        PublishAsync(topic, message).Wait();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _producer.Dispose();
    }
}