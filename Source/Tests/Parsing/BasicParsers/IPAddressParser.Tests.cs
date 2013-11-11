namespace Tests.Parsing.BasicParsers
{
	using System;
	using System.Net;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class IPAddressParserTest : ParserTestsHelper<IPAddress>
	{
		public IPAddressParserTest()
			: base(new IPAddressParser())
		{
		}

		private void CheckIPv4Valid(string ip, bool parseAll = true)
		{
			Success(ip, IPAddress.Parse(ip), parseAll);
		}

		private void CheckIPv6Valid(string ip, bool parseAll = true)
		{
			Success(ip, IPAddress.Parse(ip), parseAll);
		}

		private void CheckIPv4Invalid(string ip)
		{
			Expected(ip, IPAddressParser.Description);
		}

		private void CheckIPv6Invalid(string ip)
		{
			Expected(ip, IPAddressParser.Description);
		}

		[Test]
		public void Invalid_IPv4()
		{
			CheckIPv4Invalid("");
			CheckIPv4Invalid(".111");
			CheckIPv4Invalid("832.129.321.2");
		}

		[Test]
		public void Invalid_IPv6()
		{
			CheckIPv6Invalid("");
			CheckIPv6Invalid("2001:0db885a3:0000:0000:8a2e:0370:7334");
			CheckIPv6Invalid("99999:db8:85a3:0:0:8a2e:370:7334");
			CheckIPv6Invalid("2001:db8:85a3:::8a2e:370:7334");
			CheckIPv6Invalid(":2001:db8:85a3:::8a2e:370:7334");
			CheckIPv6Invalid("45:1");
			CheckIPv6Invalid(":");

			Expected("[::1]:5464532", IPAddressParser.Description);
			Expected("[::1:546", IPAddressParser.Description);
		}

		[Test]
		public void Valid_IPv4()
		{
			CheckIPv4Valid("172.14.2.3");
			CheckIPv4Valid("127.0.0.1");
			Success("127.0.0.1 ", IPAddress.Parse("127.0.0.1"), false);
			CheckIPv4Valid("0.0.0.1");
			CheckIPv4Valid("3");
			Success("127.1.1.1]", IPAddress.Parse("127.1.1.1"), false);
			Success("172.14.2.3:", IPAddress.Parse("172.14.2.3"), false);
			Success("127.0.0.1:32", IPAddress.Parse("127.0.0.1"), false);
			Success("2.2:2", IPAddress.Parse("2.2"), false);
		}

		[Test]
		public void Valid_IPv6()
		{
			CheckIPv6Valid("111");
			CheckIPv6Valid("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
			CheckIPv6Valid("2001:db8:85a3:0:0:8A2e:370:7334");
			CheckIPv6Valid("2001:db8:85a3::8a2E:370:7334");
			CheckIPv6Valid("0:0:0:0:0:0:0:1");
			CheckIPv6Valid("0:0:0:0:0:0:0:0");
			CheckIPv6Valid("::1");
			CheckIPv6Valid("::");
			CheckIPv6Valid("::ffff:192.0.2.128");
			Success("::ffff:192.0.2.128]", IPAddress.Parse("::ffff:192.0.2.128"), false);
			Success("::ffff:192.0.2.128]:22", IPAddress.Parse("::ffff:192.0.2.128"), false);
			Success("::x:192.0.2.128", IPAddress.Parse("::"), false);
			Success("::1]:546", IPAddress.Parse("::1"), false);
		}
	}
}