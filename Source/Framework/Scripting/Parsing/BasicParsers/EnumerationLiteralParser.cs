using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses an enumeration literal.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	/// <typeparam name="TResult">The type of the enumeration literal that is parsed.</typeparam>
	public class EnumerationLiteralParser<TResult, TUserState> : Parser<TResult, TUserState>
		where TResult : struct
	{
		/// <summary>
		///   Parses an enumeration literal and returns the result as a string.
		/// </summary>
		private readonly Parser<string, TUserState> _literalParser =
			new StringParser<TUserState>(c => Char.IsLetter(c) || c == '_',
										 c => Char.IsLetterOrDigit(c) || c == '_', string.Format("'{0}' enumeration literal", typeof(TResult).Name));

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			var reply = _literalParser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			TResult literal;
			if (!Enum.TryParse(reply.Result, out literal))
				return Expected(string.Format("valid '{0}' literal", typeof(TResult).Name));

			return Success(literal);
		}
	}
}