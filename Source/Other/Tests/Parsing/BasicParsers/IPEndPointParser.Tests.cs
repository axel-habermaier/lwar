namespace Tests.Parsing.BasicParsers
{
	using System;
	using NUnit.Framework;
	using Pegasus.Platform.Network;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class IPEndPointParserTest : ParserTestsHelper<IPEndPoint>
	{
		public IPEndPointParserTest()
			: base(new IPEndPointParser())
		{
		}

		private void CheckIPv4Valid(string ip, ushort port)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			Success(String.Format("{0}:{1}", ip, port), endPoint);
		}

		private void CheckIPv6Valid(string ip, ushort port)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			Success(String.Format("[{0}]:{1}", ip, port), endPoint);
		}

		private void CheckIPv4Valid(string ip)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), 0);
			Success(ip, endPoint);
		}

		private void CheckIPv6Valid(string ip)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), 0);
			Success(ip, endPoint);
		}

		private void CheckInvalid(string ip, ushort port, string error)
		{
			Expected(String.Format("[{0}]:{1}", ip, port), error);
		}

		private void CheckInvalid(string ip, params string[] errors)
		{
			Expected(ip, errors);
		}

		[Test]
		public void Invalid_IPv4()
		{
			CheckInvalid("832.129.321.2", 3, IPAddressParser.Description);
			CheckInvalid("832.2:2", 423, IPAddressParser.Description);
			CheckInvalid("832.129.321.2", 123, IPAddressParser.Description);
			CheckInvalid("832.129.321.2:", 43, IPAddressParser.Description);
			CheckInvalid("127.1.1.1:", TypeRegistry.GetDescription<ushort>());

			Message("127.1.1.1:5464532", String.Format(NumberParser<uint>.OverflowMessage, TypeRegistry.GetDescription<ushort>()));
		}

		[Test]
		public void Invalid_IPv6()
		{
			CheckInvalid("2001:0db885a3:0000:0000:8a2e:0370:7334", 489, IPAddressParser.Description);
			CheckInvalid("99999:db8:85a3:0:0:8a2e:370:7334", 8493, IPAddressParser.Description);
			CheckInvalid("2001:db8:85a3:::8a2e:370:7334", 567, IPAddressParser.Description);
			CheckInvalid("45:1", 143, IPAddressParser.Description);
			CheckInvalid(":", 1547, IPAddressParser.Description);
			CheckInvalid("::x:192.0.2.128", 183, "']'");

			Message("[::1]:5464532", String.Format(NumberParser<uint>.OverflowMessage, TypeRegistry.GetDescription<ushort>()));
			CheckInvalid("2001:0db885a3:0000:0000:8a2e:0370:7334:322", "'['", IPAddressParser.Description);
			CheckInvalid("[::1:546", "']'");
		}

		[Test]
		public void Valid_IPv4()
		{
			CheckIPv4Valid("172.14.2.3", 489);
			CheckIPv4Valid("127.0.0.1", 8493);
			CheckIPv4Valid("0.0.0.1", 1);
			CheckIPv4Valid("172.14.2.3");
			CheckIPv4Valid("127.0.0.1");
			CheckIPv4Valid("0.0.0.1");
		}

		[Test]
		public void Valid_IPv6()
		{
			CheckIPv6Valid("2001:0db8:85a3:0000:0000:8a2e:0370:7334", 489);
			CheckIPv6Valid("2001:db8:85a3:0:0:8a2e:370:7334", 8493);
			CheckIPv6Valid("2001:DB8:85a3::8a2e:370:7334", 1);
			CheckIPv6Valid("0:0:0:0:0:0:0:1", 1);
			CheckIPv6Valid("0:0:0:0:0:0:0:0", 17);
			CheckIPv6Valid("::1", 143);
			CheckIPv6Valid("::", 1547);
			CheckIPv6Valid("::ffff:192.0.2.128", 183);
			CheckIPv6Valid("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
			CheckIPv6Valid("2001:db8:85a3:0:0:8a2e:370:7334");
			CheckIPv6Valid("2001:db8:85a3::8a2e:370:7334");
			CheckIPv6Valid("0:0:0:0:0:0:0:1");
			CheckIPv6Valid("0:0:0:0:0:0:0:0");
			CheckIPv6Valid("::1");
			CheckIPv6Valid("::");
			CheckIPv6Valid("::ffff:192.0.2.128");

			Success("::1]:546", new IPEndPoint(IPAddress.Parse("::1"), 0), false);
		}
	}
}