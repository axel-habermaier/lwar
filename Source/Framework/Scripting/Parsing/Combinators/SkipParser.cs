

using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Skips over the input parsed by the given parser and ignores the parser's result.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public abstract class SkipParser<TUserState> : Parser<None, TUserState>
	{
	}


	/// <summary>
	///   Skips over the input parsed by the given parser and ignores the parser's result.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result that is skipped.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class SkipParser<TResult, TUserState> : SkipParser<TUserState>
	{
		/// <summary>
		///   The parser that is skipped.
		/// </summary>
		private readonly Parser<TResult, TUserState> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is skipped.</param>
		public SkipParser(Parser<TResult, TUserState> parser)
		{
			Assert.ArgumentNotNull(parser, () => parser);
			_parser = parser;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream<TUserState> inputStream)
		{
			var result = _parser.Parse(inputStream);
			if (result.Status != ReplyStatus.Success)
				return ForwardError(result);

			return Success(new None());
		}
	}
}