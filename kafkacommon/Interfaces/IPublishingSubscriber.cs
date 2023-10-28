namespace KafkaCommon.Interfaces;

public interface IPublishingSubscriber : IPublisher, ISubscriber
{
    void Publish<T>(string topic, T message) where T : IEnvelope;
    void Bind<T>(Action<IPublishingSubscriber, IEnvelope> action);
    void BindElse(Action<IPublishingSubscriber, IEnvelope> action);
}