using System;

namespace Pegasus.Framework.Scripting.Parsing
{
	/// <summary>
	///   An abstract base class for the implementation of combined parsers.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public abstract class CombinedParser<TResult, TUserState> : Parser<TResult, TUserState>
	{
		/// <summary>
		///   Gets or sets the combined parser.
		/// </summary>
		protected Parser<TResult, TUserState> Parser { get; set; }

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override sealed Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			Assert.NotNull(Parser, "Parser has not been initialized.");
			return Parser.Parse(inputStream);
		}
	}
}