import uuid

from envelopes import UserRequest, UserResponse
from hard_serializer import HardSerializer
from pubsub import PubSub


def output_obj(obj):
    print(obj)


def main():
    serializer = HardSerializer()
    pubsub = PubSub(serializer, ["test"], "python-cli", "localhost:9092", output_obj)

    done = False
    print("cli")
    while not done:
        print("> ", end="")
        line = input()
        words = line.split(" ")
        if words[0] == "quit":
            done = True
        elif words[0] == "help":
            print("Help")
            print("getall - get all users")
            print("get <id> - get user for id")
            print("update <id> <username> <name> - update user")
            print("delete <id> - delete user")
            print("quit - exit")
        elif words[0] == "getall":
            req = UserRequest()
            pubsub.publish("test", req)
        elif words[0] == "get":
            req = UserRequest(uuid.UUID(words[1]))
            pubsub.publish("test", req)
        else:
            print("unknown command")


    print("done")
    pubsub.shutdown()
    pubsub.join(1000)



if __name__ == '__main__':
    main()
