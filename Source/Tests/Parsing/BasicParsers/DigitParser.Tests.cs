using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class DigitParserTests : ParserTestsHelper<char>
	{
		public DigitParserTests()
			: base(new DigitParser())
		{
		}

		[Test]
		public void Digit()
		{
			Success("1", '1');
		}

		[Test]
		public void Letter()
		{
			Expected("a", "digit");
		}

		[Test]
		public void EndOfInput()
		{
			Expected("", "digit");
		}
	}
}