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
			var float32 = Parsers.Float32;
			var ws = ~Parsers.WhiteSpaces;
			Parser = Pipe(float32, ws + String("x") + ws, float32, (width, _, height) => new Size(width, height));
		}
	}
}