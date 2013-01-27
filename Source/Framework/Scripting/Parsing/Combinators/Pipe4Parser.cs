﻿using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Applies the four parsers in sequence. The second parser is only applied if the first parser
	///   succeeds, and the third parser is only applied if the second once succeeds, and so on. The results of all four
	///   parsers are passed to the given function, whose return value is the result of the pipe4 parser.
	/// </summary>
	/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
	/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
	/// <typeparam name="TResultThird">The type of the third parser's result.</typeparam>
	/// <typeparam name="TResultFourth">The type of the fourth parser's result.</typeparam>
	/// <typeparam name="TResult">The type of the pipe2 parser's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Pipe4Parser<TResultFirst, TResultSecond, TResultThird, TResultFourth, TResult, TUserState> :
		Parser<TResult, TUserState>
	{
		/// <summary>
		///   The parser that is applied first.
		/// </summary>
		private readonly Parser<TResultFirst, TUserState> _first;

		/// <summary>
		///   The parser that is applied fourth.
		/// </summary>
		private readonly Parser<TResultFourth, TUserState> _fourth;

		/// <summary>
		///   A function that is applied to the results of both parsers.
		/// </summary>
		private readonly Func<TResultFirst, TResultSecond, TResultThird, TResultFourth, TResult> _function;

		/// <summary>
		///   The parser that is applied second.
		/// </summary>
		private readonly Parser<TResultSecond, TUserState> _second;

		/// <summary>
		///   The parser that is applied third.
		/// </summary>
		private readonly Parser<TResultThird, TUserState> _third;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="first">The parser that is applied first.</param>
		/// <param name="second">The parser that is applied second.</param>
		/// <param name="third">The parser that is applied third.</param>
		/// <param name="fourth">The parser that is applied fourth.</param>
		/// <param name="function"> A function that is applied to the results of both parsers.</param>
		public Pipe4Parser(Parser<TResultFirst, TUserState> first, Parser<TResultSecond, TUserState> second,
						   Parser<TResultThird, TUserState> third, Parser<TResultFourth, TUserState> fourth,
						   Func<TResultFirst, TResultSecond, TResultThird, TResultFourth, TResult> function)
		{
			Assert.ArgumentNotNull(first, () => first);
			Assert.ArgumentNotNull(second, () => second);
			Assert.ArgumentNotNull(third, () => third);
			Assert.ArgumentNotNull(fourth, () => fourth);
			Assert.ArgumentNotNull(function, () => function);

			_first = first;
			_second = second;
			_third = third;
			_fourth = fourth;
			_function = function;
		}

		/// <summary>
		///   Applies the first parser, then, if successful, the second one and returns the result of the second parser.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			var firstResult = _first.Parse(inputStream);
			if (firstResult.Status != ReplyStatus.Success)
				return ForwardError(firstResult);

			var secondResult = _second.Parse(inputStream);
			if (secondResult.Status != ReplyStatus.Success)
				return ForwardError(secondResult);

			var thirdResult = _third.Parse(inputStream);
			if (thirdResult.Status != ReplyStatus.Success)
				return ForwardError(thirdResult);

			var fourthResult = _fourth.Parse(inputStream);
			if (fourthResult.Status != ReplyStatus.Success)
				return ForwardError(fourthResult);

			return Success(_function(firstResult.Result, secondResult.Result, thirdResult.Result, fourthResult.Result));
		}
	}
}