using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a single letter character.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class LetterParser<TUserState> : Parser<char, TUserState>
	{
		/// <summary>
		///   Checks whether the current character in the input stream is a letter.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<char> Parse(InputStream<TUserState> inputStream)
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