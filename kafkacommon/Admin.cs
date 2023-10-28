using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace KafkaCommon;

public static class Admin
{
    public static void CreateTopic(string servers, string topic)
    {
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = servers }).Build())
        {
            try
            {
                adminClient.CreateTopicsAsync(new[]
                    { new TopicSpecification { Name = topic, ReplicationFactor = 1, NumPartitions = 1 } }).Wait();
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Topic with this name already exists"))
                    throw;
            }
        }
    }
    
    public static void DeleteTopic(string servers, string topic)
    {
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = servers }).Build())
        {
            try
            {
                adminClient.DeleteTopicsAsync(new[] { topic }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}