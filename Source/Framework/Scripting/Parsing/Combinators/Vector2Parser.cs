﻿using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	using Math;

	/// <summary>
	///   Parses a Vector2 value.
	/// </summary>
	public class Vector2Parser : CombinedParser<Vector2>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector2Parser()
		{
			Parser = Pipe(Float32, ~WhiteSpaces + String(";") + ~WhiteSpaces, Float32, (x, _, y) => new Vector2(x, y));
		}
	}
}