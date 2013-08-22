using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Int32ParserTests : ParserTestsHelper<int>
	{
		public Int32ParserTests()
			: base(new Int32Parser())
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
			Success("-123", -123);
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
		public void Valid_Negative_FollowedByLetter()
		{
			Success("-123a", -123, false);
		}

		[Test]
		public void Invalid_DecimalPoint()
		{
			Expected(".2", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<int>());
		}

		[Test]
		public void Underflow()
		{
			Message("-1111111111111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<int>()));
		}

		[Test]
		public void Overflow()
		{
			Message("1111111111111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<int>()));
		}
	}
}