namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses a single letter character.
	/// </summary>
	public class LetterParser : Parser<char>
	{
		/// <summary>
		///     Checks whether the current character in the input stream is a letter.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<char> Parse(InputStream inputStream)
		{
			var letter = inputStream.Peek();
			if (Char.IsLetter(letter))
			{
				inputStream.Skip(1);
				return Success(letter);
			}

			return Expected("letter");
		}
	}
}