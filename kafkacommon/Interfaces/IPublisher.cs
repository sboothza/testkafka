namespace KafkaCommon.Interfaces;

public interface IPublisher
{
    Task PublishAsync<T>(string topic, T message) where T : IEnvelope;
}