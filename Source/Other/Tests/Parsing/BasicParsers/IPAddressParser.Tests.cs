namespace Tests.Parsing.BasicParsers
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Platform.Network;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using IP = System.Net.IPAddress;

	[TestFixture]
	public class IPAddressParserTest : ParserTestsHelper<IPAddress>
	{
		public IPAddressParserTest()
			: base(new IPAddressParser())
		{
		}

		private void CheckIPv4Valid(string ip, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).Should().Be(IP.Parse(ip));
		}

		private void CheckIPv4Valid(string ip, IP expected, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).Should().Be(expected);
		}

		private void CheckIPv6Valid(string ip, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).Should().Be(IP.Parse(ip));
		}

		private void CheckIPv6Valid(string ip, IP expected, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).Should().Be(expected);
		}

		private void CheckMappedIPv4Valid(string ip, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).MapToIPv6().Should().Be(IP.Parse(ip));
		}

		private void CheckMappedIPv4Valid(string ip, IP expected, bool parseAll = true)
		{
			IP.Parse(Success(ip, parseAll).ToString()).MapToIPv6().Should().Be(expected);
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
			CheckIPv4Invalid("3");
			CheckIPv4Invalid("2.2");
		}

		[Test]
		public void Invalid_IPv6()
		{
			CheckIPv6Invalid("");
			CheckIPv6Invalid("111");
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
			CheckIPv4Valid("127.0.0.1 ", IP.Parse("127.0.0.1"), false);
			CheckIPv4Valid("1.0.0.1");
			CheckIPv4Valid("127.1.1.1]", IP.Parse("127.1.1.1"), false);
			CheckIPv4Valid("172.14.2.3:", IP.Parse("172.14.2.3"), false);
			CheckIPv4Valid("127.0.0.1:32", IP.Parse("127.0.0.1"), false);
		}

		[Test]
		public void Valid_IPv6()
		{
			CheckIPv6Valid("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
			CheckIPv6Valid("2001:db8:85a3:0:0:8A2e:370:7334");
			CheckIPv6Valid("2001:db8:85a3::8a2E:370:7334");
			CheckIPv6Valid("0:0:0:0:0:0:0:1");
			CheckIPv6Valid("0:0:0:0:0:0:0:0");
			CheckIPv6Valid("::1");
			CheckIPv6Valid("::");
			CheckMappedIPv4Valid("::ffff:192.0.2.128");
			CheckMappedIPv4Valid("::ffff:192.0.2.128]", IP.Parse("::ffff:192.0.2.128"), false);
			CheckMappedIPv4Valid("::ffff:192.0.2.128]:22", IP.Parse("::ffff:192.0.2.128"), false);
			CheckIPv4Valid("::x:192.0.2.128", IP.Parse("::"), false);
			CheckIPv6Valid("::1]:546", IP.Parse("::1"), false);
		}
	}
}