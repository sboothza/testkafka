using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafkacommon
{
	public static class EnvelopeMapper
	{
		private static Dictionary<string, Type> _typeMapping = new Dictionary<string, Type>();

		public static void Scan()
		{
			var envelopeType = typeof(IEnvelope);
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var t in asm.GetTypes().Where(t=>envelopeType.IsAssignableFrom(t) && !t.IsAbstract))
				{
					var obj = (IEnvelope)Activator.CreateInstance(t);
					_typeMapping[obj.Identifier] = t;
				}
			}
		}

		public static Type TypeFromIdentifier(string identifier)
		{
			var t = _typeMapping[identifier];
			return t;
			//switch (identifier)
			//{
			//	case "TestMessage":
			//		return typeof(TestMessage);
			//	case "TestMessage2":
			//		return typeof(TestMessage2);
			//	case "TestMessage3":
			//		return typeof(TestMessage3);
			//	default:
			//		throw new InvalidCastException($"Unknown identifier - {identifier}");
			//}
		}
	}
}
