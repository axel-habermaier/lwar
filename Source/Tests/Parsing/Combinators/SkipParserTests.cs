using System;

namespace Tests.Parsing.Combinators
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class SkipParserTest : ParserTestsHelper<None>
	{
		public SkipParserTest()
			: base(new SkipParser<char>(new LetterParser()))
		{
		}

		[Test]
		public void Valid()
		{
			Success("f", new None());
		}

		[Test]
		public void Invalid()
		{
			Expected("3", "letter");
		}
	}
}