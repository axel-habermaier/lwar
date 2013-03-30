﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Semantics
{
	using Framework.Platform.Graphics;

	/// <summary>
	///   Indicates that a shader argument or return value represents a position.
	/// </summary>
	public class PositionAttribute : SemanticsAttribute
	{
		/// <summary>
		///   Gets the corresponding data semantics literal.
		/// </summary>
		internal override DataSemantics Semantics
		{
			get { return DataSemantics.Position; }
		}
	}
}