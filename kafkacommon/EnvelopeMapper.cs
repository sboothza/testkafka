using KafkaCommon.Interfaces;

namespace KafkaCommon;

public static class EnvelopeMapper
{
    private static readonly Dictionary<string, Type> _typeMapping = new();

    public static void Scan()
    {
        var envelopeType = typeof(IEnvelope);
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var t in asm.GetTypes().Where(t => envelopeType.IsAssignableFrom(t) && !t.IsAbstract))
            {
                var obj = Activator.CreateInstance(t);
                if (obj is IEnvelope envelope)
                    _typeMapping[envelope.Identifier] = t;
            }
        }
    }

    public static Type TypeFromIdentifier(string identifier)
    {
        var t = _typeMapping[identifier];
        return t;
    }
}