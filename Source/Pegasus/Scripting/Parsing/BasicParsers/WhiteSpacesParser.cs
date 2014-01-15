namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses any number of white space characters.
	/// </summary>
	public class WhiteSpacesParser : Parser<None>
	{
		/// <summary>
		///     Skips any number of white space characters.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream inputStream)
		{
			inputStream.SkipWhiteSpaces();
			return Success(new None());
		}
	}
}