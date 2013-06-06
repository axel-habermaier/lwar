using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a string literal enclosed in double quotes. Double quotes can be used inside the string literal if they are
	///   escaped by a backslash '\"'.
	/// </summary>
	public class QuotedStringParser : Parser<string>
	{
		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<string> Parse(InputStream inputStream)
		{
			// Parse the opening quote
			if (inputStream.Peek() != '"')
				return Expected("opening quote '\"'");

			// Parse the string literal, possibly containing escaped quotes
			inputStream.Skip(1);
			var position = inputStream.State.Position;

			var wasBackslash = false;
			while (!inputStream.EndOfInput)
			{
				var character = inputStream.Peek();

				if (character == '"' && !wasBackslash)
					break;

				wasBackslash = character == '\\';
				inputStream.Skip(1);
			}

			// Parse the closing quote
			if (inputStream.Peek() != '"')
				return Message("missing closing quote '\"'");

			// Extract the string literal and unescape all escaped quotes
			var result = inputStream.Substring(position, inputStream.State.Position - position);
			result = result.Replace("\\\"", "\"");

			// Skip the closing quote and return the result.
			inputStream.Skip(1);
			return Success(result);
		}
	}
}