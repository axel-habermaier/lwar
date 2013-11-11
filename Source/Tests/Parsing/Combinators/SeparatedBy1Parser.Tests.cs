namespace Tests.Parsing.Combinators
{
	using System;
	using System.Collections.Generic;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;
	using Pegasus.Scripting.Parsing.Combinators;

	[TestFixture]
	public class SeparatedBy1ParserTests : ParserTestsHelper<List<int>>
	{
		public SeparatedBy1ParserTests()
			: base(new SeparatedBy1Parser<int, char>(new Int32Parser(), new CharacterParser('|')))
		{
		}

		[Test]
		public void Invalid_Empty()
		{
			Expected("", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_Letter()
		{
			Expected("a", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_One_EndsWithSeparator()
		{
			Expected("21|", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_SeparatorOnly()
		{
			Expected("|", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_Two_EndsWithSeparator()
		{
			Expected("21|33|", TypeRegistry.GetDescription<int>());
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
	}
}