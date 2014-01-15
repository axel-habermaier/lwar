namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;

	/// <summary>
	///     Applies the referenced parser whenever it should parse any input and returns the referenced parser's result. This
	///     parser kind can be used to break cyclic parser dependencies or to optimize the construction of larger parsers that
	///     have certain configurable parts.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser's result.</typeparam>
	public class ReferenceParser<TResult> : Parser<TResult>
	{
		/// <summary>
		///     Gets or sets the parser that is applied whenever this parser instance should parse input.
		/// </summary>
		public Parser<TResult> ReferencedParser { get; set; }

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<TResult> Parse(InputStream inputStream)
		{
			Assert.That(ReferencedParser != null, "Referenced parser has not been set.");
			return ReferencedParser.Parse(inputStream);
		}
	}
}