namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using Math;

	/// <summary>
	///     Parses a Vector2i value.
	/// </summary>
	public class Vector2IParser : CombinedParser<Vector2i>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Vector2IParser()
		{
			var int32 = Parsers.Int32;
			var ws = ~Parsers.WhiteSpaces;
			Parser = Pipe(int32, ws + String(";") + ws, int32, (x, _, y) => new Vector2i(x, y));
		}
	}
}