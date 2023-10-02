namespace kafkacommon
{

	public interface IEnvelope
	{
		Guid CorrelationId { get; }
		string Identifier { get; }
	}

	public class TestMessage : IEnvelope
	{
		public Guid CorrelationId { get; }
		public string Identifier => "TestMessage";
		public string StringValue { get; set; }
		public int IntValue { get; set; }

		public override string ToString()
		{
			return $"Int:{IntValue} Str:{StringValue}";
		}
	}

	public class TestMessage2 : TestMessage, IEnvelope
	{
		public new string Identifier => "TestMessage2";
		public string SecondStringValue { get; set; }

		public override string ToString()
		{
			return $"Int:{IntValue} Str:{StringValue} 2ndStr:{SecondStringValue}";
		}
	}

	public class TestMessage3 : TestMessage, IEnvelope
	{
		public new string Identifier => "TestMessage3";
		public string ThirdStringValue { get; set; }

		public override string ToString()
		{
			return $"Int:{IntValue} Str:{StringValue} 3rdStr:{ThirdStringValue}";
		}
	}	
}
