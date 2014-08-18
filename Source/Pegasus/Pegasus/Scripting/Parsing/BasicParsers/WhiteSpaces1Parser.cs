namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses any number of white space characters, but at least one.
	/// </summary>
	public class WhiteSpaces1Parser : Parser<None>
	{
		/// <summary>
		///     Skips any number of white space characters, but at least one.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream inputStream)
		{
			// Parse a whitespace
			if (!Char.IsWhiteSpace(inputStream.Peek()))
				return Expected("whitespace");

			// Skip all remaining whitespaces
			inputStream.SkipWhiteSpaces();
			return Success(new None());
		}
	}
}