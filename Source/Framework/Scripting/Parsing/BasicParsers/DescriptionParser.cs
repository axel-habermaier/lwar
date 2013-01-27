using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Applies the given parser and replaces the parser's error message by the given description in case of failure. The
	///   given description is returned as an expected error.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class DescriptionParser<TResult, TUserState> : Parser<TResult, TUserState>
	{
		/// <summary>
		///   The description that should be returned in the case of failure.
		/// </summary>
		private readonly string _description;

		/// <summary>
		///   The parser that is applied.
		/// </summary>
		private readonly Parser<TResult, TUserState> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="description">The description that should be returned in the case of failure.</param>
		public DescriptionParser(Parser<TResult, TUserState> parser, string description)
		{
			Assert.ArgumentNotNull(parser, () => parser);
			Assert.ArgumentNotNullOrWhitespace(description, () => description);

			_parser = parser;
			_description = description;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			var reply = _parser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return Expected(_description);
			return reply;
		}
	}
}