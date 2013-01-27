using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a single digit character.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class DigitParser<TUserState> : Parser<char, TUserState>
	{
		/// <summary>
		///   Checks whether the current character in the input stream is a digit.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<char> Parse(InputStream<TUserState> inputStream)
		{
			var digit = inputStream.Peek();
			if (Char.IsDigit(digit))
			{
				inputStream.Skip(1);
				return Success(digit);
			}

			return Expected("digit");
		}
	}
}