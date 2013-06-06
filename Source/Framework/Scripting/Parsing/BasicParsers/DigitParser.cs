using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a single digit character.
	/// </summary>
	public class DigitParser : Parser<char>
	{
		/// <summary>
		///   Checks whether the current character in the input stream is a digit.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<char> Parse(InputStream inputStream)
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