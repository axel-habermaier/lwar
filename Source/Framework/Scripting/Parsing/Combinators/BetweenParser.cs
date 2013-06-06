using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Parses the single occurrence of the given parser, enclosed by a single occurrence of the given left and right
	///   parsers. The right parser is applied once the given parser returns.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TLeft">The type of the left separation parser's result.</typeparam>
	/// <typeparam name="TRight">The type of the right separation parser's result.</typeparam>
	public class BetweenParser<TResult, TLeft, TRight> : Parser<TResult>
	{
		/// <summary>
		///   The left separation parser.
		/// </summary>
		private readonly Parser<TLeft> _leftParser;

		/// <summary>
		///   The parser that is applied.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///   The right separation parser.
		/// </summary>
		private readonly Parser<TRight> _rightParser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="leftParser">The left separation parser.</param>
		/// <param name="rightParser">The right separation parser.</param>
		public BetweenParser(Parser<TResult>parser, Parser<TLeft> leftParser,
							 Parser<TRight> rightParser)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNull(leftParser);
			Assert.ArgumentNotNull(rightParser);

			_parser = parser;
			_leftParser = leftParser;
			_rightParser = rightParser;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			var leftReply = _leftParser.Parse(inputStream);
			if (leftReply.Status != ReplyStatus.Success)
				return ForwardError(leftReply);

			var reply = _parser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			var rightReply = _rightParser.Parse(inputStream);
			if (rightReply.Status != ReplyStatus.Success)
				return ForwardError(rightReply);

			return Success(reply.Result);
		}
	}
}