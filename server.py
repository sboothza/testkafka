import typing
import uuid

from envelopes import (
    UserRequest,
    UserView,
    UserResponse,
    UserUpdateRequest,
    BasicResponse,
)
from hard_serializer import HardSerializer
from pubsub import PubSub

users = [UserView(uuid.uuid4(), "sbooth", "Stephen Booth"), UserView(uuid.uuid4(), "tbooth", "Tamrin Booth")]
subscriber = None

def handle_message(obj):
    if isinstance(obj, UserRequest):
        response = UserResponse()
        if obj.Id == uuid.UUID(int=0):
            response.Users = users
        else:
            response.Users = [user for user in users if user.Id == obj.Id]

        subscriber.publish("test", response)

    elif isinstance(obj, UserUpdateRequest):
        response = BasicResponse()
        users_found = [user for user in users if user.Id == obj.Id]
        if len(users_found) > 0:
            user = users_found[0]
            user.Username = obj.Username
            user.Name = obj.Name
            response.Success = True
            response.Message = "Updated ok"
            response.Code = 1

    else:
        print("unknown")




def main():
    global subscriber
    serializer = HardSerializer()

    # response = UserResponse()
    # # th = typing.get_type_hints(UserResponse)
    # response.users = users
    #
    # json = serializer.serialize(response)
    #
    # obj = serializer.de_serialize(json, UserResponse)
    # print(obj)

    subscriber = PubSub(
        serializer, ["test"], "python-service", "localhost:9092", handle_message
    )
    input()
    print("done")
    subscriber.shutdown()
    subscriber.join(1000)


if __name__ == "__main__":
    main()
