namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using Math;

	/// <summary>
	///     Parses a Vector2 value.
	/// </summary>
	public class Vector2Parser : CombinedParser<Vector2>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Vector2Parser()
		{
			var float32 = Parsers.Float32;
			var ws = ~Parsers.WhiteSpaces;
			Parser = Pipe(float32, ws + String(";") + ws, float32, (x, _, y) => new Vector2(x, y));
		}
	}
}