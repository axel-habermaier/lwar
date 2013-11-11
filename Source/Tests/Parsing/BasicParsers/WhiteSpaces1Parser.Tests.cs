namespace Tests.Parsing.BasicParsers
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class WhiteSpaces1ParserTests : ParserTestsHelper<None>
	{
		public WhiteSpaces1ParserTests()
			: base(new WhiteSpaces1Parser())
		{
		}

		[Test]
		public void None()
		{
			Expected("", "whitespace");
		}

		[Test]
		public void One()
		{
			Success(" ", new None());
		}

		[Test]
		public void Two()
		{
			Success("  ", new None());
		}
	}
}