namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses a string that satisfies the given predicates. The parser fails if not at least one character can be parsed.
	/// </summary>
	public class StringParser : Parser<string>
	{
		/// <summary>
		///     A description describing the expected input in the case of a parser error.
		/// </summary>
		private readonly string _description;

		/// <summary>
		///     The predicate that the first character of the parsed string must satisfy.
		/// </summary>
		private readonly Func<char, bool> _firstCharacter;

		/// <summary>
		///     The predicate that all but the first character of the parsed string must satisfy.
		/// </summary>
		private readonly Func<char, bool> _otherCharacters;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="firstCharacter">The predicate that the first character of the parsed string must satisfy.</param>
		/// <param name="otherCharacters">The predicate that all but the first character of the parsed string must satisfy.</param>
		/// <param name="description">A description describing the expected input in the case of a parser error.</param>
		public StringParser(Func<char, bool> firstCharacter, Func<char, bool> otherCharacters, string description)
		{
			_firstCharacter = firstCharacter;
			_otherCharacters = otherCharacters;
			_description = description;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="characters">The predicate that all characters of the parsed string must satisfy.</param>
		/// <param name="description">A description describing the expected input in the case of a parser error.</param>
		public StringParser(Func<char, bool> characters, string description)
		{
			_firstCharacter = characters;
			_otherCharacters = characters;
			_description = description;
		}

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<string> Parse(InputStream inputStream)
		{
			var position = inputStream.State.Position;

			if (!_firstCharacter(inputStream.Peek()))
				return Expected(_description);

			inputStream.Skip(1);
			while (!inputStream.EndOfInput && _otherCharacters(inputStream.Peek()))
				inputStream.Skip(1);

			return Success(inputStream.Substring(position, inputStream.State.Position - position));
		}
	}
}