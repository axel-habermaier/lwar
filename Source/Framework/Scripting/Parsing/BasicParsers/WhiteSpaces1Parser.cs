using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses any number of white space characters, but at least one.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class WhiteSpaces1Parser<TUserState> : Parser<None, TUserState>
	{
		/// <summary>
		///   Skips any number of white space characters, but at least one.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream<TUserState> inputStream)
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