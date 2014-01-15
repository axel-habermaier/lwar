namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses a given character.
	/// </summary>
	public class CharacterParser : Parser<char>
	{
		/// <summary>
		///     The character that should be parsed.
		/// </summary>
		private readonly char _character;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="character">The character that should be parsed.</param>
		public CharacterParser(char character)
		{
			_character = character;
		}

		/// <summary>
		///     Checks whether the current character in the input stream matches the given one.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<char> Parse(InputStream inputStream)
		{
			if (inputStream.Peek() == _character)
			{
				inputStream.Skip(1);
				return Success(_character);
			}

			return Expected(string.Format("'{0}'", _character.ToString()));
		}
	}
}