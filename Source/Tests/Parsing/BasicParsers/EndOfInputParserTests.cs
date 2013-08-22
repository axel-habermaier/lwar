using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class EndOfInputParserTests : ParserTestsHelper<None>
	{
		public EndOfInputParserTests()
			: base(new EndOfInputParser())
		{
		}

		[Test]
		public void AtEndOfInput()
		{
			Success("", new None());
		}

		[Test]
		public void InvalidCharacter()
		{
			Expected("1", "end of input");
		}
	}
}