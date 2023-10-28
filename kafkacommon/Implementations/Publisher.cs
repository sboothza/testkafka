using System.Text.Json;
using Confluent.Kafka;
using KafkaCommon.Interfaces;

namespace KafkaCommon.Implementations;

public class Publisher : IPublisher, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private bool _disposedValue;

    public Publisher(string servers)
    {
        var config = new ProducerConfig { BootstrapServers = servers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
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

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (disposing)
            _producer.Dispose();

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}