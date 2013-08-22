using System;

namespace Tests.Parsing.Combinators
{
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class BetweenParserTests : ParserTestsHelper<int>
	{
		public BetweenParserTests()
			: base(
				new BetweenParser<int, char, char>(new Int32Parser(), new CharacterParser('('),
												   new CharacterParser(')')))
		{
		}

		[Test]
		public void Valid()
		{
			Success("(123)", 123);
		}

		[Test]
		public void Invalid_MissingLeft()
		{
			Expected("21)", "'('");
		}

		[Test]
		public void Invalid_LeftOtherCharacter()
		{
			Expected("a21)", "'('");
		}

		[Test]
		public void Invalid_MissingRight()
		{
			Expected("(21a", "')'");
		}

		[Test]
		public void Invalid_RightOtherCharacter()
		{
			Expected("(21a", "')'");
		}

		[Test]
		public void Invalid_MissingInt()
		{
			Expected("()", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_InvalidInt()
		{
			Expected("(a)", TypeRegistry.GetDescription<int>());
		}
	}
}