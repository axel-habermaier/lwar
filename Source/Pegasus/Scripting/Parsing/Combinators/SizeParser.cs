using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	using Math;

	/// <summary>
	///   Parses a Size value.
	/// </summary>
	public class SizeParser : CombinedParser<Size>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public SizeParser()
		{
			Parser = Pipe(Int32, ~WhiteSpaces + String(";") + ~WhiteSpaces, Int32, (width, _, height) => new Size(width, height));
		}
	}
}