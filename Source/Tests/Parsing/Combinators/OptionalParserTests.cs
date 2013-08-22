using System;

namespace Tests.Parsing.Combinators
{
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class OptionalParserTests : ParserTestsHelper<int>
	{
		public OptionalParserTests()
			: base(new OptionalParser<int>(~new CharacterParser('!') + new Int32Parser(), -1))
		{
		}

		[Test]
		public void ReturnParsedElement()
		{
			Success("!123", 123);
		}

		[Test]
		public void ReturnDefaultElement()
		{
			Success("", -1);
			Success(" ", -1, false);
			Success("a", -1, false);
		}

		[Test]
		public void Invalid()
		{
			Expected("!", TypeRegistry.GetDescription<int>());
			Expected("!x", TypeRegistry.GetDescription<int>());
		}
	}
}