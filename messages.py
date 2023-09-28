class TestMessage:
    Identifier: str
    StringValue: str
    IntValue: int

    def __init__(self, string_value: str = "", int_value: int = 0):
        self.Identifier = "TestMessage"
        self.StringValue = string_value
        self.IntValue = int_value

    def __str__(self):
        return f"Int:{self.IntValue} Str:{self.StringValue}"


class TestMessage2(TestMessage):
    Identifier: str
    SecondStringValue: str

    def __init__(self, string_value: str = "", int_value: int = 0, second_string_value: str = ""):
        super().__init__(string_value, int_value)
        self.Identifier = "TestMessage2"
        self.SecondStringValue = second_string_value

    def __str__(self):
        return f"Int:{self.IntValue} Str:{self.StringValue} 2ndStr:{self.SecondStringValue}"


class TestMessage3(TestMessage):
    Identifier: str
    ThirdStringValue: str

    def __init__(self, string_value: str = "", int_value: int = 0, third_string_value: str = ""):
        super().__init__(string_value, int_value)
        self.Identifier = "TestMessage3"
        self.ThirdStringValue = third_string_value

    def __str__(self):
        return f"Int:{self.IntValue} Str:{self.StringValue} 3rdStr:{self.ThirdStringValue}"