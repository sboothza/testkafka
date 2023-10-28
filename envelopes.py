import uuid
from typing import List

from envelope_mapper import EnvelopeMapper


class Envelope(object):
    Identifier: str = ""
    CorrelationId: uuid.UUID = uuid.UUID(int=0)

    def __init__(self, correlation_id: uuid.UUID = uuid.uuid4()):
        self.CorrelationId = correlation_id


class UserRequest(Envelope):
    Identifier = "UserRequest"
    Id: uuid.UUID

    def __init__(
        self, id: uuid.UUID = uuid.UUID(int=0), correlation_id: uuid.UUID = uuid.uuid4()
    ):
        super().__init__(correlation_id)
        self.Id = id

    def __str__(self):
        return f"Identifier:{self.Identifier} id:{self.Id} corid:{self.CorrelationId}"


EnvelopeMapper.register(UserRequest.Identifier, UserRequest)


class UserView:
    Id: uuid.UUID
    Username: str
    Name: str

    def __init__(
        self, id: uuid.UUID = uuid.UUID(int=0), username: str = "", name: str = ""
    ):
        self.Id = id
        self.Username = username
        self.Name = name

    def __str__(self):
        return f"id:{self.Id} username:{self.Username} name:{self.Name}"

    @classmethod
    def empty(cls):
        return UserView()


class UserResponse(Envelope):
    Identifier = "UserResponse"
    Users: List[UserView]

    def __init__(self, correlation_id: uuid.UUID = uuid.uuid4()):
        super().__init__(correlation_id)
        self.Users: List[UserView]

    def __str__(self):
        s = f"Identifier:{self.Identifier} corid:{self.CorrelationId}"
        for u in self.Users:
            s += str(u)

        return s


EnvelopeMapper.register(UserResponse.Identifier, UserResponse)


class UserUpdateRequest(Envelope):
    Identifier: str = "UserUpdateRequest"
    Id: uuid.UUID
    Username: str
    Name: str

    def __init__(
        self,
        id: uuid.UUID = uuid.UUID(int=0),
        username: str = "",
        name: str = "",
        correlation_id: uuid.UUID = uuid.uuid4(),
    ):
        super().__init__(correlation_id)
        self.Id = id
        self.Username = username
        self.Name = name


EnvelopeMapper.register(UserUpdateRequest.Identifier, UserUpdateRequest)


class UserAddRequest(Envelope):
    Identifier: str = "UserAddRequest"
    Id: uuid.UUID
    Username: str
    Name: str

    def __init__(
        self,
        id: uuid.UUID = uuid.UUID(int=0),
        username: str = "",
        name: str = "",
        correlation_id: uuid.UUID = uuid.uuid4(),
    ):
        super().__init__(correlation_id)
        self.Id = id
        self.Username = username
        self.Name = name


EnvelopeMapper.register(UserAddRequest.Identifier, UserAddRequest)


class UserDeleteRequest(Envelope):
    Identifier: str = "UserDeleteRequest"
    Id: uuid.UUID

    def __init__(self, id: uuid.UUID = uuid.UUID(int=0)):
        super().__init__()
        self.Id = id


EnvelopeMapper.register(UserDeleteRequest.Identifier, UserDeleteRequest)


class BasicResponse(Envelope):
    Identifier: str = "BasicResponse"
    Success: bool
    Code: int
    Message: str

    def __init__(
        self,
        success: bool = True,
        code: int = 0,
        message: str = "",
        correlation_id: uuid.UUID = uuid.uuid4(),
    ):
        super().__init__(correlation_id)
        self.Success = success
        self.Code = code
        self.Message = message

    def __str__(self):
        return f"success:{self.Success} code:{self.Code} msg:{self.Message} corid:{self.CorrelationId}"


EnvelopeMapper.register(BasicResponse.Identifier, BasicResponse)
