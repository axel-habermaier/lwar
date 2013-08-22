using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class WhiteSpacesParserTests : ParserTestsHelper<None>
	{
		public WhiteSpacesParserTests()
			: base(new WhiteSpacesParser())
		{
		}

		[Test]
		public void None()
		{
			Success("", new None());
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