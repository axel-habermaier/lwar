namespace Pegasus.Scripting.Parsing
{
	using System;
	using Utilities;

	/// <summary>
	///     An abstract base class for the implementation of combined parsers.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser result.</typeparam>
	public abstract class CombinedParser<TResult> : Parser<TResult>
	{
		/// <summary>
		///     Gets or sets the combined parser.
		/// </summary>
		protected Parser<TResult> Parser { get; set; }

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override sealed Reply<TResult> Parse(InputStream inputStream)
		{
			Assert.NotNull(Parser, "Parser has not been initialized.");
			return Parser.Parse(inputStream);
		}
	}
}