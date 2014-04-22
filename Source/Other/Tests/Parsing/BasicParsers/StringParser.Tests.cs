namespace Tests.Parsing.BasicParsers
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class StringParserTests : ParserTestsHelper<string>
	{
		private const string Description = "upper-lower";

		public StringParserTests()
			: base(new StringParser(Char.IsUpper, Char.IsLower, Description))
		{
		}

		[Test]
		public void EndOfInput()
		{
			Expected("", Description);
		}

		[Test]
		public void Invalid()
		{
			Expected("a", Description);
		}

		[Test]
		public void Valid_OneLetter()
		{
			Success("A", "A");
		}

		[Test]
		public void Valid_ThreeLetters()
		{
			Success("Abc", "Abc");
		}

		[Test]
		public void Valid_TwoLetters()
		{
			Success("Ab", "Ab");
		}
	}
}