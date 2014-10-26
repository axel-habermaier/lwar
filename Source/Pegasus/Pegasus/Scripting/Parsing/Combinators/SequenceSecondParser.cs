namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using Utilities;

	/// <summary>
	///     Applies the two parsers in sequence. The second parser is only applied if the first parser
	///     succeeds. The result of the second parser is returned as the result of the combinator.
	/// </summary>
	/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
	/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
	public class SequenceSecondParser<TResultFirst, TResultSecond> : Parser<TResultSecond>
	{
		/// <summary>
		///     The parser that is applied first.
		/// </summary>
		private readonly Parser<TResultFirst> _first;

		/// <summary>
		///     The parser that is applied second.
		/// </summary>
		private readonly Parser<TResultSecond> _second;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="first">The parser that is applied first.</param>
		/// <param name="second">The parser that is applied second.</param>
		public SequenceSecondParser(Parser<TResultFirst> first, Parser<TResultSecond> second)
		{
			Assert.ArgumentNotNull(first);
			Assert.ArgumentNotNull(second);

			_first = first;
			_second = second;
		}

		/// <summary>
		///     Applies the first parser, then, if successful, the second one and returns the result of the second parser.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResultSecond> Parse(InputStream inputStream)
		{
			var firstResult = _first.Parse(inputStream);

			if (firstResult.Status != ReplyStatus.Success)
				return ForwardError(firstResult);

			return _second.Parse(inputStream);
		}
	}
}