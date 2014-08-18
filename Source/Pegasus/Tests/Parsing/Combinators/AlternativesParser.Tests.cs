namespace Tests.Parsing.Combinators
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class AlternativesParserTests : ParserTestsHelper<object>
	{
		public AlternativesParserTests()
			: base(new AlternativesParser<object>(
					   new Int32Parser().Apply(i => (object)i),
					   (new LetterParser() + ~new LetterParser()).Apply(d => (object)d)))
		{
		}

		[Test]
		public void Invalid_BothFail_BothConsumeNoInput()
		{
			Expected("!", TypeRegistry.GetDescription<int>(), "letter");
		}

		[Test]
		public void Invalid_FirstFails_ConsumesInput()
		{
			Expected("+", TypeRegistry.GetDescription<int>(), "letter");
		}

		[Test]
		public void Invalid_SecondFails_ConsumesInput()
		{
			Expected("a1", "letter");
		}

		[Test]
		public void Valid_First()
		{
			Success("123", 123);
		}

		[Test]
		public void Valid_Second()
		{
			Success("ab", 'a');
		}
	}
}