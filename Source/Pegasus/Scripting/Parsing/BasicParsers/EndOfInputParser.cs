namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///   Parses the end of the input.
	/// </summary>
	public class EndOfInputParser : Parser<None>
	{
		/// <summary>
		///   Checks whether the current character in the input stream matches the given one.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<None> Parse(InputStream inputStream)
		{
			if (inputStream.EndOfInput)
				return Success(new None());

			return Expected("end of input");
		}
	}
}