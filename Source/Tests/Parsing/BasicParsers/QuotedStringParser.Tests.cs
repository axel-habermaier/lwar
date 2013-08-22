using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class QuotedStringParserTests : ParserTestsHelper<string>
	{
		public QuotedStringParserTests()
			: base(new QuotedStringParser())
		{
		}

		[Test]
		public void Valid_Empty()
		{
			Success("\"\"", "");
		}

		[Test]
		public void Valid_OneLetter()
		{
			Success("\"a\"", "a");
		}

		[Test]
		public void Valid_ThreeLetters()
		{
			Success("\"ab1\"", "ab1");
		}

		[Test]
		public void Valid_OnlyEscapedQuote()
		{
			Success("\"\\\"\"", "\"");
		}

		[Test]
		public void Valid_OnlyEscapedQuote_Beginning()
		{
			Success("\"\\\"abc\"", "\"abc");
		}

		[Test]
		public void Valid_OnlyEscapedQuote_End()
		{
			Success("\"abc\\\"\"", "abc\"");
		}

		[Test]
		public void Valid_OnlyEscapedQuote_Middle()
		{
			Success("\"ab\\\"cd\"", "ab\"cd");
		}

		[Test]
		public void Invalid_NoOpeningQuote()
		{
			Expected("ab", "opening quote '\"'");
		}

		[Test]
		public void Invalid_NoClosingQuote()
		{
			Message("\"ab", "missing closing quote '\"'");
		}
	}
}