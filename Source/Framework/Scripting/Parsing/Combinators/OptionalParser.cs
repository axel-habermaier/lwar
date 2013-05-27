using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Applies the given parser and returns the result if it succeeds. If it does not succeed, the given default element is
	///   returned and the optional parser is successful anyway.
	/// </summary>
	/// <typeparam name="TParserResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class OptionalParser<TParserResult, TUserState> : Parser<TParserResult, TUserState>
	{
		/// <summary>
		///   The default element that is returned when the parser fails.
		/// </summary>
		private readonly TParserResult _default;

		/// <summary>
		///   The parser that is applied.
		/// </summary>
		private readonly Parser<TParserResult, TUserState> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="defaultValue">The default value that should be returned when the given parser fails.</param>
		public OptionalParser(Parser<TParserResult, TUserState> parser, TParserResult defaultValue)
		{
			Assert.ArgumentNotNull(parser);

			_parser = parser;
			_default = defaultValue;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TParserResult> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;
			var reply = _parser.Parse(inputStream);

			if (reply.Status == ReplyStatus.Success)
				return reply;

			if (reply.Status != ReplyStatus.Success && state == inputStream.State)
				return Success(_default);

			return ForwardError(reply);
		}
	}
}