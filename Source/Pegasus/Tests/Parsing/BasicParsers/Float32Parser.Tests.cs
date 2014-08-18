namespace Tests.Parsing.BasicParsers
{
	using System;
	using System.Globalization;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Float32ParserTests : ParserTestsHelper<float>
	{
		public Float32ParserTests()
			: base(new Float32Parser())
		{
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<float>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<float>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<float>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<float>());
		}

		[Test]
		public void Overflow()
		{
			Message("111111111111111111111111111111111111111111111111",
					String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<float>()));
		}

		[Test]
		public void Underflow()
		{
			Message("-111111111111111111111111111111111111111111111111",
					String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<float>()));
		}

		[Test]
		public void Valid_Negative_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("-3.123", Single.Parse("-3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_FractionAndInteger_FollowedByLetter()
		{
			Success("-3.123a", Single.Parse("-3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Negative_FractionOnly_FollowedByEndOfInput()
		{
			Success("-.123", Single.Parse("-.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_FractionOnly_FollowedByLetter()
		{
			Success("-.123a", Single.Parse("-.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Negative_NoDecimal_FollowedByEndOfInput()
		{
			Success("-123", Single.Parse("-123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_NoDecimal_FollowedByLetter()
		{
			Success("-123a", Single.Parse("-123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("3.123", Single.Parse("3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_FractionAndInteger_FollowedByLetter()
		{
			Success("3.123a", Single.Parse("3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_FractionOnly_FollowedByEndOfInput()
		{
			Success(".123", Single.Parse(".123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_FractionOnly_FollowedByLetter()
		{
			Success(".123a", Single.Parse(".123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_NoDecimal_FollowedByEndOfInput()
		{
			Success("123", Single.Parse("123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_NoDecimal_FollowedByLetter()
		{
			Success("123a", Single.Parse("123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("+3.123", Single.Parse("+3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_FractionAndInteger_FollowedByLetter()
		{
			Success("+3.123a", Single.Parse("+3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_FractionOnly_FollowedByEndOfInput()
		{
			Success("+.123", Single.Parse("+.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_FractionOnly_FollowedByLetter()
		{
			Success("+.123a", Single.Parse("+.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_NoDecimal_FollowedByEndOfInput()
		{
			Success("+123", Single.Parse("+123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_NoDecimal_FollowedByLetter()
		{
			Success("+123a", Single.Parse("+123", CultureInfo.InvariantCulture), false);
		}
	}
}