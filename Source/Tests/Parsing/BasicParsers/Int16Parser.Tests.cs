namespace Tests.Parsing.BasicParsers
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Int16ParserTests : ParserTestsHelper<short>
	{
		public Int16ParserTests()
			: base(new Int16Parser())
		{
		}

		[Test]
		public void Invalid_DecimalPoint()
		{
			Expected(".2", TypeRegistry.GetDescription<short>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<short>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<short>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<short>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<short>());
		}

		[Test]
		public void Overflow()
		{
			Message("1111111111111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<short>()));
		}

		[Test]
		public void Underflow()
		{
			Message("-1111111111111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<short>()));
		}

		[Test]
		public void Valid_Negative_FollowedByEndOfInput()
		{
			Success("-123", -123);
		}

		[Test]
		public void Valid_Negative_FollowedByLetter()
		{
			Success("-123a", -123, false);
		}

		[Test]
		public void Valid_NoSign_FollowedByEndOfInput()
		{
			Success("123", 123);
		}

		[Test]
		public void Valid_NoSign_FollowedByLetter()
		{
			Success("123a", 123, false);
		}

		[Test]
		public void Valid_Positive_FollowedByEndOfInput()
		{
			Success("+123", 123);
		}

		[Test]
		public void Valid_Positive_FollowedByLetter()
		{
			Success("+123a", 123, false);
		}
	}
}