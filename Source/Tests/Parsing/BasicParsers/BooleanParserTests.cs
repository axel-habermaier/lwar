using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class BooleanParserTests : ParserTestsHelper<bool>
	{
		public BooleanParserTests()
			: base(new BooleanParser())
		{
		}

		[Test]
		public void True()
		{
			Success("true", true, true);
			Success("1", true, true);
			Success("on", true, true);
		}

		[Test]
		public void False()
		{
			Success("false", false, true);
			Success("0", false, true);
			Success("off", false, true);
		}

		[Test]
		public void Invalid()
		{
			Expected("t", BooleanParser.ErrorMessage);
			Expected("tru", BooleanParser.ErrorMessage);
			Expected("fals", BooleanParser.ErrorMessage);
		}
	}
}