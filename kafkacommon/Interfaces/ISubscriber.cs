using KafkaCommon.Implementations;

namespace KafkaCommon.Interfaces;

public interface ISubscriber : IDisposable
{
    event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    void Start();
    void Stop();
}