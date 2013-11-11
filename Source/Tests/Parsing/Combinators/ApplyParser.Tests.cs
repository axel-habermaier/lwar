namespace Tests.Parsing.Combinators
{
	using System;
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
		public void Invalid()
		{
			Expected("3", "letter");
		}

		[Test]
		public void Valid()
		{
			Success("f", "f");
		}
	}
}