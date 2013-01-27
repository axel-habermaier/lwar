using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	/// <summary>
	///   Applies the referenced parser whenever it should parse any input and returns the referenced parser's result. This
	///   parser kind can be used to break cyclic parser dependencies or to optimize the construction of larger parsers that
	///   have certain configurable parts.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class ReferenceParser<TResult, TUserState> : Parser<TResult, TUserState>
	{
		/// <summary>
		///   Gets or sets the parser that is applied whenever this parser instance should parse input.
		/// </summary>
		public Parser<TResult, TUserState> ReferencedParser { get; set; }

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream<TUserState> inputStream)
		{
			Assert.That(ReferencedParser != null, "Referenced parser has not been set.");
			return ReferencedParser.Parse(inputStream);
		}
	}
}