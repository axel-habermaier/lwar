namespace Tests.Parsing.BasicParsers
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Int64ParserTests : ParserTestsHelper<long>
	{
		public Int64ParserTests()
			: base(new Int64Parser())
		{
		}

		[Test]
		public void Invalid_DecimalPoint()
		{
			Expected(".2", TypeRegistry.GetDescription<long>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<long>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<long>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<long>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<long>());
		}

		[Test]
		public void Overflow()
		{
			Message("1111111111111111111111111", String.Format(NumberParser<long>.OverflowMessage, TypeRegistry.GetDescription<long>()));
		}

		[Test]
		public void Underflow()
		{
			Message("-111111111111111111111111", String.Format(NumberParser<long>.OverflowMessage, TypeRegistry.GetDescription<long>()));
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