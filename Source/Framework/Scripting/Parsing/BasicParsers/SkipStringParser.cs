using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a given string.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class SkipStringParser<TUserState> : Parser<string, TUserState>
	{
		/// <summary>
		///   The string that is parsed.
		/// </summary>
		private readonly string _string;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="str">The string that should be parsed.</param>
		public SkipStringParser(string str)
		{
			Assert.ArgumentNotNullOrWhitespace(str);
			_string = str;
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
				if (inputStream.EndOfInput || inputStream.Peek() != c)
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