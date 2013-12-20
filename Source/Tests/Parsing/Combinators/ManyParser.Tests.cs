namespace Tests.Parsing.Combinators
{
	using System;
	using System.Collections.Generic;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class ManyParserTest : ParserTestsHelper<char[]>
	{
		public ManyParserTest()
			: base(new ManyParser<char>(~new WhiteSpacesParser() + new LetterParser()))
		{
		}

		[Test]
		public void Invalid_First_ConsumesInput()
		{
			Expected(" 1", "letter");
		}

		[Test]
		public void Invalid_Second_ConsumesInput()
		{
			Expected("a  1", "letter");
		}

		[Test]
		public void Valid_Empty()
		{
			var result = Success("");
			result.Should().Equal(new List<char>());
		}

		[Test]
		public void Valid_One()
		{
			var result = Success("a");
			result.Should().Equal(new List<char> { 'a' });
		}

		[Test]
		public void Valid_Three()
		{
			var result = Success("abc");
			result.Should().Equal(new List<char> { 'a', 'b', 'c' });
		}

		[Test]
		public void Valid_Two()
		{
			var result = Success("ab");
			result.Should().Equal(new List<char> { 'a', 'b' });
		}
	}
}