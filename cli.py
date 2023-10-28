import inspect
import uuid

from envelopes import (
    UserRequest,
    UserResponse,
    BasicResponse,
    UserAddRequest,
    UserUpdateRequest,
    UserDeleteRequest,
)
from hard_serializer import HardSerializer
from pubsub import PubSub


def handleUserResponse(pubsub, obj):
    print(obj)


def handleBasicResponse(pubsub, obj):
    print(obj)

def handleEverythingElse(pubsub, obj):
    print(obj)

def main():

    serializer = HardSerializer()

    pubsub = PubSub(serializer, ["test"], "python-cli", "localhost:9092")
    pubsub.bind(UserResponse, handleUserResponse)
    pubsub.bind(BasicResponse, handleBasicResponse)
    pubsub.bind(UserRequest, handleEverythingElse)

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
        elif words[0] == "add":
            req = UserAddRequest(id=uuid.uuid4(), username=words[1], name=words[2])
            pubsub.publish("test", req)
        elif words[0] == "update":
            req = UserUpdateRequest(id=uuid.UUID(words[1]), username=words[2], name=words[3])
            pubsub.publish("test", req)
        elif words[0] == "delete":
            req = UserDeleteRequest(id=uuid.UUID(words[1]))
            pubsub.publish("test", req)
        else:
            print("unknown command")

    print("done")
    pubsub.shutdown()
    pubsub.join(1000)


if __name__ == "__main__":
    main()
