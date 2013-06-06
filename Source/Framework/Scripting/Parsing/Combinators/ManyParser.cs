using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	using System.Collections.Generic;

	/// <summary>
	///   Applies the given parser zero or more times and returns the parser's results as a list in the order of occurrence.
	///   The many parser parses as many occurrences of the given parser as possible. At the end of the sequence the given
	///   parser must fail without consuming any input; otherwise, the many parser fails with the error returned by the given
	///   parser.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	public class ManyParser<TResult> : Parser<TResult[]>
	{
		/// <summary>
		///   The parser that is applied zero or more times.
		/// </summary>
		private readonly Parser<TResult> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser">The parser that is applied zero or more times.</param>
		public ManyParser(Parser<TResult>parser)
		{
			Assert.ArgumentNotNull(parser);
			_parser = parser;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult[]> Parse(InputStream inputStream)
		{
			var results = new List<TResult>();

			while (!inputStream.EndOfInput)
			{
				var state = inputStream.State;
				var reply = _parser.Parse(inputStream);

				if (reply.Status == ReplyStatus.Success)
					results.Add(reply.Result);
				else if (reply.Status == ReplyStatus.Error && state == inputStream.State)
					break;
				else
					return ForwardError(reply);
			}

			return Success(results.ToArray());
		}
	}
}