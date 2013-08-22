using System;

namespace Tests.Parsing.Combinators
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class SequenceFirstParserTests : ParserTestsHelper<char>
	{
		public SequenceFirstParserTests()
			: base(new SequenceFirstParser<char, char>(new LetterParser(), new DigitParser()))
		{
		}

		[Test]
		public void Valid()
		{
			Success("f1", 'f');
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
		public void Invalid_EndOfInput()
		{
			Expected("", "letter");
		}

		[Test]
		public void Invalid_EndOfInputAfterFirst()
		{
			Expected("a", "digit");
		}
	}
}