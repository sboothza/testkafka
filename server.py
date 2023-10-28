import inspect
import typing
import uuid

from envelopes import (
    UserRequest,
    UserView,
    UserResponse,
    UserUpdateRequest,
    BasicResponse,
    Envelope,
    UserAddRequest,
    UserDeleteRequest,
)
from hard_serializer import HardSerializer
from pubsub import PubSub

# users = [
#     UserView(uuid.uuid4(), "sbooth", "Stephen Booth"),
#     UserView(uuid.uuid4(), "tbooth", "Tamrin Booth"),
# ]
users = []
primary_pubsub = None
primary_correlation_id = None


def handle_user_request(pubsub, msg):
    global users
    if len(users) > 0:
        response = UserResponse(correlation_id=msg.CorrelationId)
        if msg.Id == uuid.UUID(int=0):
            response.Users = users
        else:
            response.Users = [user for user in users if user.Id == msg.Id]

        pubsub.publish("test", response)


def handle_user_response(pubsub, msg):
    global users
    if msg.CorrelationId == primary_correlation_id:
        users = msg.Users


def handle_user_update_request(pubsub, msg):
    response = BasicResponse(correlation_id=msg.CorrelationId)
    users_found = [user for user in users if user.Id == msg.Id]
    if len(users_found) > 0:
        user = users_found[0]
        user.Username = msg.Username
        user.Name = msg.Name
        response.Success = True
        response.Message = "Updated ok"
        response.Code = 1
    else:
        response.Success = False
        response.Message = "Not found"
        response.Code = 0

    pubsub.publish("test", response)


def handle_user_add_request(pubsub, msg):
    response = BasicResponse(correlation_id=msg.CorrelationId)
    users_found = [user for user in users if user.Id == msg.Id]
    if len(users_found) > 0:
        response.Success = False
        response.Message = "Already there"
        response.Code = 0
    else:
        users.append(UserView(msg.Id, msg.Username, msg.Name))
        response.Success = True
        response.Message = "Added successfully"
        response.Code = 1

    pubsub.publish("test", response)


def handle_user_delete_request(pubsub, msg):
    response = BasicResponse(correlation_id=msg.CorrelationId)
    users_found = [user for user in users if user.Id == msg.Id]
    if len(users_found) > 0:
        users.remove(users_found[0])
        response.Success = True
        response.Message = "Deleted successfully"
        response.Code = 1

    pubsub.publish("test", response)


def main():
    global primary_pubsub
    global primary_correlation_id

    serializer = HardSerializer()

    primary_pubsub = PubSub(serializer, ["test"], "python-service", "localhost:9092")
    primary_pubsub.bind(UserRequest, handle_user_request)
    primary_pubsub.bind(UserResponse, handle_user_response)
    primary_pubsub.bind(UserUpdateRequest, handle_user_update_request)
    primary_pubsub.bind(UserAddRequest, handle_user_add_request)
    primary_pubsub.bind(UserDeleteRequest, handle_user_delete_request)

    # initialize list from primary source
    req = UserRequest()
    primary_correlation_id = req.CorrelationId
    primary_pubsub.publish("test", req)

    input()
    print("done")
    primary_pubsub.shutdown()
    primary_pubsub.join(1000)


if __name__ == "__main__":
    main()
