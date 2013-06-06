using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Applies the two parsers in sequence. The second parser is only applied if the first parser
	///   succeeds. The results of both parsers are passed to the given function, whose return value is the result
	///   of the pipe2 parser.
	/// </summary>
	/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
	/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
	/// <typeparam name="TResult">The type of the pipe2 parser's result.</typeparam>
	public class Pipe2Parser<TResultFirst, TResultSecond, TResult> : Parser<TResult>
	{
		/// <summary>
		///   The parser that is applied first.
		/// </summary>
		private readonly Parser<TResultFirst> _first;

		/// <summary>
		///   A function that is applied to the results of both parsers.
		/// </summary>
		private readonly Func<TResultFirst, TResultSecond, TResult> _function;

		/// <summary>
		///   The parser that is applied second.
		/// </summary>
		private readonly Parser<TResultSecond> _second;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="first">The parser that is applied first.</param>
		/// <param name="second">The parser that is applied second.</param>
		/// <param name="function"> A function that is applied to the results of both parsers.</param>
		public Pipe2Parser(Parser<TResultFirst> first, Parser<TResultSecond> second,
						   Func<TResultFirst, TResultSecond, TResult> function)
		{
			Assert.ArgumentNotNull(first);
			Assert.ArgumentNotNull(second);
			Assert.ArgumentNotNull(function);

			_first = first;
			_second = second;
			_function = function;
		}

		/// <summary>
		///   Applies the first parser, then, if successful, the second one and returns the result of the second parser.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			var firstResult = _first.Parse(inputStream);
			if (firstResult.Status != ReplyStatus.Success)
				return ForwardError(firstResult);

			var secondResult = _second.Parse(inputStream);
			if (secondResult.Status != ReplyStatus.Success)
				return ForwardError(secondResult);

			return Success(_function(firstResult.Result, secondResult.Result));
		}
	}
}