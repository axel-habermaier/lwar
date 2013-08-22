using System;

namespace Tests.Parsing.BasicParsers
{
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Int8ParserTests : ParserTestsHelper<sbyte>
	{
		public Int8ParserTests()
			: base(new Int8Parser())
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
			Expected(".2", TypeRegistry.GetDescription<sbyte>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<sbyte>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<sbyte>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<sbyte>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<sbyte>());
		}

		[Test]
		public void Underflow()
		{
			Message("-11111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<sbyte>()));
		}

		[Test]
		public void Overflow()
		{
			Message("11111", String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<sbyte>()));
		}
	}
}