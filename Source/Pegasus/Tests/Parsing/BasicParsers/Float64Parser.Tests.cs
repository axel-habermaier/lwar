namespace Tests.Parsing.BasicParsers
{
	using System;
	using System.Globalization;
	using NUnit.Framework;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Parsing.BasicParsers;

	[TestFixture]
	public class Float64ParserTests : ParserTestsHelper<double>
	{
		public Float64ParserTests()
			: base(new Float64Parser())
		{
		}

		[Test]
		public void Invalid_OnlyNegativeSign()
		{
			Expected("-a", TypeRegistry.GetDescription<double>());
		}

		[Test]
		public void Invalid_OnlyNegativeSign_EndOfInput()
		{
			Expected("-", TypeRegistry.GetDescription<double>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign()
		{
			Expected("+a", TypeRegistry.GetDescription<double>());
		}

		[Test]
		public void Invalid_OnlyPositiveSign_EndOfInput()
		{
			Expected("+", TypeRegistry.GetDescription<double>());
		}

		[Test]
		public void Overflow()
		{
			Message(
				"111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
				String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<double>()));
		}

		[Test]
		public void Underflow()
		{
			Message(
				"-111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
				String.Format(NumberParser<int>.OverflowMessage, TypeRegistry.GetDescription<double>()));
		}

		[Test]
		public void Valid_Negative_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("-3.123", Double.Parse("-3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_FractionAndInteger_FollowedByLetter()
		{
			Success("-3.123a", Double.Parse("-3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Negative_FractionOnly_FollowedByEndOfInput()
		{
			Success("-.123", Double.Parse("-.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_FractionOnly_FollowedByLetter()
		{
			Success("-.123a", Double.Parse("-.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Negative_NoDecimal_FollowedByEndOfInput()
		{
			Success("-123", Double.Parse("-123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Negative_NoDecimal_FollowedByLetter()
		{
			Success("-123a", Double.Parse("-123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("3.123", Double.Parse("3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_FractionAndInteger_FollowedByLetter()
		{
			Success("3.123a", Double.Parse("3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_FractionOnly_FollowedByEndOfInput()
		{
			Success(".123", Double.Parse(".123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_FractionOnly_FollowedByLetter()
		{
			Success(".123a", Double.Parse(".123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_NoSign_NoDecimal_FollowedByEndOfInput()
		{
			Success("123", Double.Parse("123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_NoSign_NoDecimal_FollowedByLetter()
		{
			Success("123a", Double.Parse("123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_FractionAndInteger_FollowedByEndOfInput()
		{
			Success("+3.123", Double.Parse("+3.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_FractionAndInteger_FollowedByLetter()
		{
			Success("+3.123a", Double.Parse("+3.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_FractionOnly_FollowedByEndOfInput()
		{
			Success("+.123", Double.Parse("+.123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_FractionOnly_FollowedByLetter()
		{
			Success("+.123a", Double.Parse("+.123", CultureInfo.InvariantCulture), false);
		}

		[Test]
		public void Valid_Positive_NoDecimal_FollowedByEndOfInput()
		{
			Success("+123", Double.Parse("+123", CultureInfo.InvariantCulture));
		}

		[Test]
		public void Valid_Positive_NoDecimal_FollowedByLetter()
		{
			Success("+123a", Double.Parse("+123", CultureInfo.InvariantCulture), false);
		}
	}
}