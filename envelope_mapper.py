from messages import TestMessage, TestMessage2, TestMessage3


class EnvelopeMapper:
    @staticmethod
    def type_from_identifier(identifier:str):
        if identifier == "TestMessage":
            return TestMessage
        elif identifier == "TestMessage2":
            return TestMessage2
        elif identifier == "TestMessage3":
            return TestMessage3
        else:
            raise Exception("Unknown type")
