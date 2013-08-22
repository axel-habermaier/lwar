using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class CharacterParserTests : ParserTestsHelper<char>
	{
		public CharacterParserTests()
			: base(new CharacterParser('1'))
		{
		}

		[Test]
		public void ValidCharacter()
		{
			Success("1", '1');
		}

		[Test]
		public void InvalidCharacter()
		{
			Expected("2", "'1'");
		}

		[Test]
		public void EndOfInput()
		{
			Expected("", "'1'");
		}
	}
}