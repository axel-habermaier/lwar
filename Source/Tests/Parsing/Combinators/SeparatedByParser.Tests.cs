using System;

namespace Tests.Parsing.Combinators
{
	using System.Collections.Generic;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class SeparatedByParserTests : ParserTestsHelper<List<int>>
	{
		public SeparatedByParserTests()
			: base(new SeparatedByParser<int, char>(new Int32Parser(), new CharacterParser('|')))
		{
		}

		[Test]
		public void Valid_One()
		{
			var result = Success("21");
			result.Should().Equal(new List<int> { 21 });
		}

		[Test]
		public void Valid_Two()
		{
			var result = Success("21|33");
			result.Should().Equal(new List<int> { 21, 33 });
		}

		[Test]
		public void Valid_Empty()
		{
			var result = Success("");
			result.Should().Equal(new List<int>());
		}

		[Test]
		public void Valid_Letter()
		{
			var result = Success("a", false);
			result.Should().Equal(new List<int>());
		}

		[Test]
		public void Invalid_SeparatorOnly()
		{
			var result = Success("|", false);
			result.Should().Equal(new List<int> { });
		}

		[Test]
		public void Invalid_One_EndsWithSeparator()
		{
			Expected("21|", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_Two_EndsWithSeparator()
		{
			Expected("21|33|", TypeRegistry.GetDescription<int>());
		}
	}
}