using System;

namespace Pegasus.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Attempts to apply the given parser, returning its reply on success. If the parser fails non-fatally, however, it
	///   resets the input stream state to where it was when the parser was applied. Therefore, the attempt parser implements a
	///   backtracking mechanism in the sense that the attemt parser fails without consuming input whenever the given parser
	///   fails with or without consuming input.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	public class AttemptParser<TResult> : Parser<TResult>
	{
		/// <summary>
		///   The parser that is attempted.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that should be attempted.</param>
		public AttemptParser(Parser<TResult> parser)
		{
			_parser = parser;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			var state = inputStream.State;
			var reply = _parser.Parse(inputStream);

			if (reply.Status == ReplyStatus.Error)
				inputStream.State = state;

			return reply;
		}
	}
}