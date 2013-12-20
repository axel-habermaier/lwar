namespace Tests.Parsing.Combinators
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class SequenceSecondParserTests : ParserTestsHelper<char>
	{
		public SequenceSecondParserTests()
			: base(new SequenceSecondParser<char, char>(new LetterParser(), new DigitParser()))
		{
		}

		[Test]
		public void Invalid_EndOfInput()
		{
			Expected("", "letter");
		}

		[Test]
		public void Invalid_EndOfInputAfterFirst()
		{
			Expected("a", "digit");
		}

		[Test]
		public void Invalid_FirstFails()
		{
			Expected("3a", "letter");
		}

		[Test]
		public void Invalid_SecondFails()
		{
			Expected("ab", "digit");
		}

		[Test]
		public void Valid()
		{
			Success("f1", '1');
		}
	}
}