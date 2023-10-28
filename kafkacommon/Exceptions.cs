namespace KafkaCommon;

public class PublishException : Exception
{
    public PublishException()
    {
    }

    public PublishException(string message)
        : base(message)
    {
    }

    public PublishException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

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