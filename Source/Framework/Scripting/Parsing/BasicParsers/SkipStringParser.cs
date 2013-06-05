using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a given string.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class SkipStringParser<TUserState> : Parser<string, TUserState>
	{/// <summary>
		///   Indicates whether case is ignored.
		/// </summary>
		private readonly bool _ignoreCase;
		/// <summary>
		///   The string that is parsed.
		/// </summary>
		private readonly string _string;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="str">The string that should be parsed.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored.</param>
		public SkipStringParser(string str, bool ignoreCase)
		{
			Assert.ArgumentNotNullOrWhitespace(str);

			_string = str;
			_ignoreCase = ignoreCase;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<string> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;

			foreach (char c in _string)
			{
				var streamChar = _ignoreCase ? Char.ToLower(inputStream.Peek()) : inputStream.Peek();
				var inputChar = _ignoreCase ? Char.ToLower(c) : c;
				if (inputStream.EndOfInput || streamChar != inputChar)
				{
					inputStream.State = state;
					return Expected(string.Format("string '{0}'", _string));
				}

				inputStream.Skip(1);
			}

			return Success(_string);
		}
	}
}