using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	/// <summary>
	///   Applies the given parser and passes the parser's result to the given function. The value returned by the function
	///   is returned as the reply of the apply parser.
	/// </summary>
	/// <typeparam name="TParserResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TFuncResult">The type of the function's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class ApplyParser<TParserResult, TFuncResult, TUserState> : Parser<TFuncResult, TUserState>
	{
		/// <summary>
		///   The function that is applied to the parser's result.
		/// </summary>
		private readonly Func<TParserResult, TFuncResult> _function;

		/// <summary>
		///   The parser that is applied.
		/// </summary>
		private readonly Parser<TParserResult, TUserState> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="function">The function that is applied to the parser's result.</param>
		public ApplyParser(Parser<TParserResult, TUserState> parser, Func<TParserResult, TFuncResult> function)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNull(function);

			_parser = parser;
			_function = function;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TFuncResult> Parse(InputStream<TUserState> inputStream)
		{
			var reply = _parser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			return Success(_function(reply.Result));
		}
	}
}