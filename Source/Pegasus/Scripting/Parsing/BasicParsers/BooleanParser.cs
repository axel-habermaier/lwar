using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses a Boolean value (either 'true'/'false', '1'/'0', or 'on'/'off').
	/// </summary>
	public class BooleanParser : Parser<bool>
	{
		/// <summary>
		///   The error message that is displayed if the parser failed.
		/// </summary>
		internal const string ErrorMessage = "Boolean value (either 'true'/'false', '1'/'0', or 'on'/'off')";

		/// <summary>
		///   Parses a Boolean value.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<bool> Parse(InputStream inputStream)
		{
			var state = inputStream.State;

			if (IsKeyword(inputStream, "true") || IsKeyword(inputStream, "1") || IsKeyword(inputStream, "on"))
				return Success(true);

			inputStream.State = state;
			if (IsKeyword(inputStream, "false") || IsKeyword(inputStream, "0") || IsKeyword(inputStream, "off"))
				return Success(false);

			inputStream.State = state;
			return Expected(ErrorMessage);
		}

		/// <summary>
		///   Checks whether the next input in the stream is equal to the given keyword.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="keyword">The keyword that should be the next input.</param>
		private bool IsKeyword(InputStream inputStream, string keyword)
		{
			foreach (var character in keyword)
			{
				if (inputStream.EndOfInput || Char.ToLower(inputStream.Peek()) != character)
					return false;

				inputStream.Skip(1);
			}

			return true;
		}
	}
}