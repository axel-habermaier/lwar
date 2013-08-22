using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class LetterParserTests : ParserTestsHelper<char>
	{
		public LetterParserTests()
			: base(new LetterParser())
		{
		}

		[Test]
		public void Letter()
		{
			Success("a", 'a');
		}

		[Test]
		public void Digit()
		{
			Expected("1", "letter");
		}

		[Test]
		public void EndOfInput()
		{
			Expected("", "letter");
		}
	}
}