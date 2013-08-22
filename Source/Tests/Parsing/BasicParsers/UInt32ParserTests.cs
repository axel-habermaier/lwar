using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class UInt32ParserTests : ParserTestsHelper<uint>
	{
		public UInt32ParserTests()
			: base(new UInt32Parser())
		{
		}

		[Test]
		public void Valid_NoSign_FollowedByEndOfInput()
		{
			Success("123", 123);
		}

		[Test]
		public void Valid_Positive_FollowedByEndOfInput()
		{
			Success("+123", 123);
		}

		[Test]
		public void Valid_Negative_FollowedByEndOfInput()
		{
			Expected("-123", TypeRegistry.GetDescription<uint>());
		}

		[Test]
		public void Valid_NoSign_FollowedByLetter()
		{
			Success("123a", 123, false);
		}

		[Test]
		public void Valid_Positive_FollowedByLetter()
		{
			Success("+123a", 123, false);
		}

		[Test]
		public void Invalid_DecimalPoint()
		{
			Expected(".2", TypeRegistry.GetDescription<uint>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<uint>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<uint>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<uint>());
		}

		[Test]
		public void Overflow()
		{
			Message("1111111111111", String.Format(NumberParser<uint>.OverflowMessage, TypeRegistry.GetDescription<uint>()));
		}
	}
}