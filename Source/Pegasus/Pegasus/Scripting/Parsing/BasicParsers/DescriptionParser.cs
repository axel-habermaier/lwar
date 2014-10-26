namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using Utilities;

	/// <summary>
	///     Applies the given parser and replaces the parser's error message by the given description in case of failure. The
	///     given description is returned as an expected error.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	public class DescriptionParser<TResult> : Parser<TResult>
	{
		/// <summary>
		///     The description that should be returned in the case of failure.
		/// </summary>
		private readonly string _description;

		/// <summary>
		///     The parser that is applied.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="description">The description that should be returned in the case of failure.</param>
		public DescriptionParser(Parser<TResult> parser, string description)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNullOrWhitespace(description);

			_parser = parser;
			_description = description;
		}

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			var reply = _parser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return Expected(_description);
			return reply;
		}
	}
}