namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using System.Collections.Generic;
	using Utilities;

	/// <summary>
	///     Parses as many occurrences (possibly zero) of the given parser as possible, where each occurrence is separated by the
	///     given separation parser. The sequence is expected to end without another occurrence of the separation parser.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TSeparate">The type of the separation parser's result.</typeparam>
	public class SeparatedByParser<TResult, TSeparate> : Parser<List<TResult>>
	{
		/// <summary>
		///     The parser that is applied several times.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///     The actual parsing is forwarded to this separated by one parser instance.
		/// </summary>
		private readonly SeparatedBy1Parser<TResult, TSeparate> _separatedBy;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied several times.</param>
		/// <param name="separationParser">The separation parser.</param>
		public SeparatedByParser(Parser<TResult> parser, Parser<TSeparate> separationParser)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNull(separationParser);

			_parser = parser;
			_separatedBy = new SeparatedBy1Parser<TResult, TSeparate>(parser, separationParser);
		}

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<List<TResult>> Parse(InputStream inputStream)
		{
			// Return an empty list if the parser fails without consuming input, 
			// otherwise return the result of the separated by one parser instance or the parser error.
			var state = inputStream.State;
			var reply = _parser.Parse(inputStream);

			if (reply.Status != ReplyStatus.Success && state.Position == inputStream.State.Position)
				return Success(new List<TResult>());

			if (reply.Status != ReplyStatus.Success && state.Position != inputStream.State.Position)
				return ForwardError(reply);

			inputStream.State = state;
			return _separatedBy.Parse(inputStream);
		}
	}
}