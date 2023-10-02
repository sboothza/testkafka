class EnvelopeMapper:
    identifiers = {}

    @classmethod
    def type_from_identifier(cls, identifier: str):
        if identifier in EnvelopeMapper.identifiers:
            return EnvelopeMapper.identifiers[identifier]
        raise Exception("Unknown type")

        # if identifier == "TestMessage":
        #     return TestMessage
        # elif identifier == "TestMessage2":
        #     return TestMessage2
        # elif identifier == "TestMessage3":
        #     return TestMessage3
        # elif identifier == "UserRequest":
        #     return UserRequest
        # elif identifier == "UserResponse":
        #     return UserResponse
        # elif identifier == "BasicResponse":
        #     return BasicResponse
        # else:
        #     raise Exception("Unknown type")

    @classmethod
    def register(cls, identifier: str, type: type):
        EnvelopeMapper.identifiers[identifier] = type
