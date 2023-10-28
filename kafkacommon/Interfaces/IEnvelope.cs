namespace KafkaCommon.Interfaces;

public interface IEnvelope
{
    Guid CorrelationId { get; }
    string Identifier { get; }
}