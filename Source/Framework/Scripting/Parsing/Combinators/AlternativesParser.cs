using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   First, it applies the parser p1 to the input stream. If the first parser succeeds, the reply of the first parser is
	///   returned. If it fails with a non‐fatal error and without changing the parser state, the second parser is applied. If
	///   the second parser does not change the parser state, the error messages from both parsers are merged. The alternatives
	///   parser
	///   will always return with the reply of the first parser if the first parser changes the parser state, even if it
	///   eventually fails. Since a parser usually consumes input as soon as it can accept at least one atomic token from the
	///   input, this means that the alternatives parser by default implements backtracking with only a "one token look‐ahead".
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class AlternativesParser<TResult, TUserState> : Parser<TResult, TUserState>
	{
		/// <summary>
		///   The parser that is applied first.
		/// </summary>
		private readonly Parser<TResult, TUserState> _first;

		/// <summary>
		///   The parser that is applied second.
		/// </summary>
		private readonly Parser<TResult, TUserState> _second;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="first">The parser that is applied first.</param>
		/// <param name="second">The parser that is applied second.</param>
		public AlternativesParser(Parser<TResult, TUserState> first, Parser<TResult, TUserState> second)
		{
			Assert.ArgumentNotNull(first, () => first);
			Assert.ArgumentNotNull(second, () => second);

			_first = first;
			_second = second;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;
			var reply = _first.Parse(inputStream);

			if (reply.Status == ReplyStatus.Error && state == inputStream.State)
			{
				var errors = reply.Errors;
				reply = _second.Parse(inputStream);
				if (reply.Status != ReplyStatus.Success && state == inputStream.State)
					return MergeErrors(errors, reply.Errors);
			}

			return reply;
		}
	}
}