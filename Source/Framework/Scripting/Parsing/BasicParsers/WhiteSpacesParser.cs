using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses any number of white space characters.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class WhiteSpacesParser<TUserState> : Parser<None, TUserState>
	{
		/// <summary>
		///   Skips any number of white space characters.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream<TUserState> inputStream)
		{
			inputStream.SkipWhiteSpaces();
			return Success(new None());
		}
	}
}