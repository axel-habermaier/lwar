using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Skips over the input parsed by the given parser and ignores the parser's result.
	/// </summary>
	public abstract class SkipParser : Parser<None>
	{
	}

	/// <summary>
	///   Skips over the input parsed by the given parser and ignores the parser's result.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result that is skipped.</typeparam>
	public class SkipParser<TResult> : SkipParser
	{
		/// <summary>
		///   The parser that is skipped.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is skipped.</param>
		public SkipParser(Parser<TResult>parser)
		{
			Assert.ArgumentNotNull(parser);
			_parser = parser;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream inputStream)
		{
			var result = _parser.Parse(inputStream);
			if (result.Status != ReplyStatus.Success)
				return ForwardError(result);

			return Success(new None());
		}
	}
}