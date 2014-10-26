namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using System.Collections.Generic;
	using Utilities;

	/// <summary>
	///     Parses as many occurrences (at least one) of the given parser as possible, where each occurrence is separated by the
	///     given separation parser. The sequence is expected to end without another occurrence of the separation parser.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TSeparate">The type of the separation parser's result.</typeparam>
	public class SeparatedBy1Parser<TResult, TSeparate> : Parser<List<TResult>>
	{
		/// <summary>
		///     The parser that is applied several times.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///     The separation parser.
		/// </summary>
		private readonly Parser<TSeparate> _separationParser;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied several times.</param>
		/// <param name="separationParser">The separation parser.</param>
		public SeparatedBy1Parser(Parser<TResult> parser, Parser<TSeparate> separationParser)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNull(separationParser);

			_parser = parser;
			_separationParser = separationParser;
		}

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<List<TResult>> Parse(InputStream inputStream)
		{
			var result = new List<TResult>();
			do
			{
				var reply = _parser.Parse(inputStream);
				if (reply.Status != ReplyStatus.Success)
					return ForwardError(reply);

				result.Add(reply.Result);

				// If the parser fails without consuming input, we have reached the end of the sequence
				var position = inputStream.State.Position;
				var separationReply = _separationParser.Parse(inputStream);
				if (separationReply.Status != ReplyStatus.Success && position != inputStream.State.Position)
					return ForwardError(separationReply);

				if (separationReply.Status != ReplyStatus.Success && position == inputStream.State.Position)
					break;
			} while (true);

			return Success(result);
		}
	}
}