namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using Math;

	/// <summary>
	///     Parses a Size value.
	/// </summary>
	public class SizeParser : CombinedParser<Size>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public SizeParser()
		{
			var int32 = Parsers.Int32;
			var ws = ~Parsers.WhiteSpaces;
			Parser = Pipe(int32, ws + String("x") + ws, int32, (width, _, height) => new Size(width, height));
		}
	}
}