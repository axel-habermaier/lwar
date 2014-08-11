namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Parses an enumeration literal.
	/// </summary>
	/// <typeparam name="TResult">The type of the enumeration literal that is parsed.</typeparam>
	public class EnumerationLiteralParser<TResult> : Parser<TResult>
	{
		/// <summary>
		///     The type of the enumeration that is parsed.
		/// </summary>
		private readonly Type _enumeration;

		/// <summary>
		///     Indicates whether case is ignored.
		/// </summary>
		private readonly bool _ignoreCase;

		/// <summary>
		///     Parses an enumeration literal and returns the result as a string.
		/// </summary>
		private readonly Parser<string> _literalParser =
			new StringParser(c => Char.IsLetter(c) || c == '_', c => Char.IsLetterOrDigit(c) || c == '_',
							 string.Format("'{0}' enumeration literal", typeof(TResult).Name));

		/// <summary>
		///     Initializes a new instance, parsing the enumeration provided as the TResult parameter.
		/// </summary>
		/// <param name="ignoreCase">Indicates whether case should be ignored.</param>
		public EnumerationLiteralParser(bool ignoreCase)
		{
			_ignoreCase = ignoreCase;
			_enumeration = typeof(TResult);
		}

		/// <summary>
		///     Initializes a new instance, parsing the enumeration provided as the constructor's type parameter. The parsed literal
		///     is cast to an instance of object and returned, therefore TResult must be object in order to use this constructor.
		/// </summary>
		/// <param name="enumeration">The type of the enumeration that should be parsed.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored.</param>
		public EnumerationLiteralParser(Type enumeration, bool ignoreCase)
		{
			Assert.ArgumentNotNull(enumeration);
			Assert.That(typeof(TResult) == typeof(object), "TResult must be of type object in order to use this constructor.");

			_ignoreCase = ignoreCase;
			_enumeration = enumeration;
		}

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			var reply = _literalParser.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return Expected(string.Format("valid '{0}' literal", _enumeration.Name));

			try
			{
				var literal = Enum.Parse(_enumeration, reply.Result, _ignoreCase);
				return Success((TResult)literal);
			}
			catch (Exception)
			{
				return Expected(string.Format("valid '{0}' literal", _enumeration.Name));
			}
		}
	}
}