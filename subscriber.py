import json
import sys
import time
from threading import Thread

from confluent_kafka import Consumer, KafkaError, KafkaException

from envelope_mapper import EnvelopeMapper
from hard_serializer import HardSerializer


class Subscriber(Thread):

    def __init__(self, serializer: HardSerializer, topics: list[str], group: str, servers, callback):
        self.serializer = serializer
        self.callback = callback
        self.running=True
        conf = {'bootstrap.servers': servers,
                'group.id': group,
                'auto.offset.reset': 'smallest'}
        self.consumer = Consumer(conf)
        self.consumer.subscribe(topics)
        Thread.__init__(self)
        self.start()


    def run(self)->None:
        try:
            while self.running:
                msg = self.consumer.poll(timeout=0.5)
                if msg is None: continue

                if msg.error():
                    if msg.error().code() == KafkaError._PARTITION_EOF:
                        # End of partition event
                        sys.stderr.write('%% %s [%d] reached end at offset %d\n' %
                                         (msg.topic(), msg.partition(), msg.offset()))
                    elif msg.error():
                        raise KafkaException(msg.error())
                else:
                    raw_data = json.loads(msg.value())
                    identifier = raw_data["Identifier"]
                    type = EnvelopeMapper.type_from_identifier(identifier)
                    obj = self.serializer.map_to_object(raw_data, type)
                    self.callback(obj)
        finally:
            self.consumer.close()

    def shutdown(self):
        self.running = False


def output_obj(obj):
    print(obj)


def main():
    serializer = HardSerializer()
    subscriber = Subscriber(serializer, ["test"], "test_group", "localhost:9092", output_obj)
    time.sleep(10)
    print("done")
    subscriber.shutdown()
    subscriber.join(1000)



if __name__ == '__main__':
    main()
