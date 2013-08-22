using System;

namespace Tests.Parsing.Combinators
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class ApplyParserTests : ParserTestsHelper<string>
	{
		public ApplyParserTests()
			: base(new ApplyParser<char, string>(new LetterParser(), c => c.ToString()))
		{
		}

		[Test]
		public void Valid()
		{
			Success("f", "f");
		}

		[Test]
		public void Invalid()
		{
			Expected("3", "letter");
		}
	}
}