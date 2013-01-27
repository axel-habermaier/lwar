using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   A base implementation for number parsers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the number that is parsed.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public abstract class NumberParser<TNumber, TUserState> : Parser<TNumber, TUserState>
	{
		/// <summary>
		///   The message that is printed in case of a value overflow.
		/// </summary>
		internal const string OverflowMessage = "This number is outside the allowable range for a {0}.";

		/// <summary>
		///   Indicates whether the number may contain a decimal point.
		/// </summary>
		private readonly bool _allowDecimal;

		/// <summary>
		///   Indicates whether the number may be prefixed with '-'.
		/// </summary>
		private readonly bool _allowNegative;

		/// <summary>
		///   A textual description of the type of the number that is parsed.
		/// </summary>
		private readonly string _description;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="allowNegative">Indicates whether the number may be prefixed with '-'.</param>
		/// <param name="allowDecimal">Indicates whether the number may contain a decimal point.</param>
		/// <param name="description">A textual description of the type of the number that is parsed.</param>
		protected NumberParser(bool allowNegative, bool allowDecimal, string description)
		{
			Assert.ArgumentNotNull(description, () => description);

			_allowDecimal = allowDecimal;
			_allowNegative = allowNegative;
			_description = description;
		}

		/// <summary>
		///   Parses the number.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TNumber> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;
			var count = 0;

			// Parse the optional sign
			var sign = inputStream.Peek();
			var negative = sign == '-';
			var positive = sign == '+';

			if (negative && !_allowNegative)
			{
				inputStream.State = state;
				return Expected(_description);
			}

			if (negative || positive)
				inputStream.Skip(1);

			// Parse the integer part, if any -- it may be missing if a fractional part follows
			count = inputStream.Skip(Char.IsDigit);
			if (count == 0 && (!_allowDecimal || inputStream.Peek() != '.'))
			{
				inputStream.State = state;
				return Expected(_description);
			}

			// If a fractional part is allowed, parse it, but there must be at least one digit following
			if (_allowDecimal && inputStream.Peek() == '.')
			{
				inputStream.Skip(1);
				count = inputStream.Skip(Char.IsDigit);

				if (count == 0)
				{
					inputStream.State = state;
					return Expected(_description);
				}
			}

			// Convert everything from the first digit (or sign or decimal point) to the last digit followed by a non-digit character
			try
			{
				return ConvertToNumber(inputStream.Substring(state.Position, inputStream.State.Position - state.Position));
			}
			catch (FormatException)
			{
				inputStream.State = state;
				return Expected(_description);
			}
			catch (OverflowException)
			{
				inputStream.State = state;
				return Message(string.Format(OverflowMessage, _description));
			}
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected abstract Reply<TNumber> ConvertToNumber(string number);
	}
}