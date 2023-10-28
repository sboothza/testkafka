import json
import sys
import time
from threading import Thread

from confluent_kafka import Consumer, KafkaError, KafkaException, Producer

from envelope_mapper import EnvelopeMapper
from hard_serializer import HardSerializer
from confluent_kafka.admin import AdminClient, NewTopic


class PubSub(Thread):
    def __init__(
        self, serializer: HardSerializer, topics: list[str], group: str, servers
    ):
        self.serializer = serializer
        self.running = True
        conf = {
            "bootstrap.servers": servers,
            "group.id": group,
            "auto.offset.reset": "smallest",
        }
        self.servers = servers
        self.consumer = Consumer(conf)
        self.producer = Producer(conf)
        self.consumer.subscribe(topics)
        self.bindings = {}
        Thread.__init__(self)
        self.start()

    def create_topic(self, topic_name, num_partitions):
        admin_client = AdminClient({"bootstrap.servers": self.servers})
        topic_list = []
        topic_list.append(NewTopic(topic_name, num_partitions, 1))
        admin_client.create_topics(topic_list)

    def bind(self, cls, callback):
        self.bindings[cls] = callback

    def publish(self, topic: str, obj):
        json_str = self.serializer.serialize(obj, False)
        self.producer.produce(topic, key="key", value=json_str)
        self.producer.flush()

    def run(self) -> None:
        try:
            while self.running:
                msg = self.consumer.poll(timeout=0.5)
                if msg is None:
                    continue

                if msg.error():
                    if msg.error().code() == KafkaError._PARTITION_EOF:
                        # End of partition event
                        sys.stderr.write(
                            "%% %s [%d] reached end at offset %d\n"
                            % (msg.topic(), msg.partition(), msg.offset())
                        )
                    elif msg.error():
                        raise KafkaException(msg.error())
                else:
                    raw_data = json.loads(msg.value())
                    if "Identifier" in raw_data:
                        identifier = raw_data["Identifier"]
                    elif "identifier" in raw_data:
                        identifier = raw_data["identifier"]
                    else:
                        continue

                    type = EnvelopeMapper.type_from_identifier(identifier)
                    try:
                        obj = self.serializer.map_to_object(raw_data, type)
                        if type in self.bindings:
                            callback = self.bindings[type]
                            callback(self, obj)
                        else:
                            print("binding not found")
                    except Exception as e:
                        print(e)
        finally:
            self.consumer.close()

    def shutdown(self):
        self.running = False


#
# def handleUserResponse(pubsub, obj):
#     print(obj)
#
# def handleBasicResponse(pubsub, obj):
#     print(obj)
#
# def main():
#     serializer = HardSerializer()
#     subscriber = PubSub(serializer, ["test"], "test_group", "localhost:9092")
#     pubsub.bind(UserResponse, handleUserResponse)
#     pubsub.bind(BasicResponse, handleBasicResponse)
#     time.sleep(10)
#     print("done")
#     subscriber.shutdown()
#     subscriber.join(1000)
#
# if __name__ == '__main__':
#     main()
