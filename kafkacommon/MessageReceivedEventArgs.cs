using KafkaCommon.Interfaces;

namespace KafkaCommon;

public class MessageReceivedEventArgs : EventArgs
{
    public IEnvelope? Message { get; set; }
}