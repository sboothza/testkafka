import uuid
from typing import ClassVar, List

from envelope_mapper import EnvelopeMapper


class Envelope:
    Identifier = ""

    def __init__(self):
        self.Identifier = ""


class UserRequest(Envelope):
    Id: uuid.UUID
    Identifier = "UserRequest"

    def __init__(self, id: uuid.UUID = uuid.UUID(int=0)):
        super().__init__()
        self.Id = id
        self.Identifier = "UserRequest"

    def __str__(self):
        return f"Identifier:{self.Identifier} id:{self.Id}"


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
    Users: List[UserView]
    Identifier: str = "UserResponse"

    def __init__(self):
        super().__init__()
        self.Users: List[UserView]
        self.Identifier = "UserResponse"

    def __str__(self):
        s = f"Identifier:{self.Identifier}"
        for u in self.Users:
            s += str(u)

        return s


EnvelopeMapper.register(UserResponse.Identifier, UserResponse)


class UserUpdateRequest(Envelope):
    Id: uuid.UUID
    Username: str
    Name: str
    Identifier: str = "UserUpdateRequest"

    def __init__(
        self, id: uuid.UUID = uuid.UUID(int=0), username: str = "", name: str = ""
    ):
        super().__init__()
        self.Id = id
        self.Username = username
        self.Name = name
        self.Identifier: str = "UserUpdateRequest"


EnvelopeMapper.register(UserUpdateRequest.Identifier, UserUpdateRequest)


class UserAddRequest(Envelope):
    Id: uuid.UUID
    Username: str
    Name: str
    Identifier: str = "UserAddRequest"

    def __init__(
        self, id: uuid.UUID = uuid.UUID(int=0), username: str = "", name: str = ""
    ):
        super().__init__()
        self.Id = id
        self.Username = username
        self.Name = name
        self.Identifier: str = "UserAddRequest"


EnvelopeMapper.register(UserAddRequest.Identifier, UserAddRequest)


class UserDeleteRequest(Envelope):
    Id: uuid.UUID
    Identifier: str = "UserDeleteRequest"

    def __init__(self, id: uuid.UUID = uuid.UUID(int=0)):
        super().__init__()
        self.Id = id
        self.Identifier: str = "UserDeleteRequest"


EnvelopeMapper.register(UserDeleteRequest.Identifier, UserDeleteRequest)


class BasicResponse(Envelope):
    Success: bool
    Code: int
    Message: str
    Identifier: str = "BasicResponse"

    def __init__(self, success: bool = True, code: int = 0, message: str = ""):
        super().__init__()
        self.Success = success
        self.Code = code
        self.Message = message
        self.Identifier: str = "BasicResponse"

    def __str__(self):
        return f"success:{self.Success} code:{self.Code} msg:{self.Message}"


EnvelopeMapper.register(BasicResponse.Identifier, BasicResponse)
