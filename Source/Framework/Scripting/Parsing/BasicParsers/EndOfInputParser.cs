using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Parses the end of the input.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class EndOfInputParser<TUserState> : Parser<None, TUserState>
	{
		/// <summary>
		///   Checks whether the current character in the input stream matches the given one.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream<TUserState> inputStream)
		{
			if (inputStream.EndOfInput)
				return Success(new None());

			return Expected("end of input");
		}
	}
}