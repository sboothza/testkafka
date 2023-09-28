import socket

from confluent_kafka import Producer

from hard_serializer import HardSerializer
from messages import TestMessage, TestMessage2, TestMessage3


class Publisher:
    producer: Producer
    serializer: HardSerializer

    def __init__(self, servers, serializer: HardSerializer):
        conf = {'bootstrap.servers': servers,
                'client.id': socket.gethostname()}
        self.producer = Producer(conf)
        self.serializer = serializer

    def publish(self, topic: str, obj):
        json_str = self.serializer.serialize(obj, False)
        self.producer.produce(topic, key="key", value=json_str)
        self.producer.flush()


def main():
    # send
    serializer = HardSerializer()
    publisher = Publisher("localhost:9092", serializer)
    publisher.publish("test", TestMessage("test", 1))
    publisher.publish("test", TestMessage2("test", 1, "test2"))
    publisher.publish("test", TestMessage3("test", 1, "test3"))


if __name__ == '__main__':
    main()
