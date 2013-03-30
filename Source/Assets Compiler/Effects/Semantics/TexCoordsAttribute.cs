using System;

namespace Pegasus.AssetsCompiler.Effects.Semantics
{
	using Framework.Platform.Graphics;

	/// <summary>
	///   Indicates that a shader argument or return value represents a texture coordinate.
	/// </summary>
	public class TexCoordsAttribute : SemanticsAttribute
	{
		/// <summary>
		///   Initializes a new instance with index 0.
		/// </summary>
		public TexCoordsAttribute()
		{
		}

		/// <summary>
		///   Initializes a new instance with the given index.
		/// </summary>
		/// <param name="index">The index of the texture coordinates.</param>
		public TexCoordsAttribute(int index)
			: base(index)
		{
		}

		/// <summary>
		///   Gets the corresponding data semantics literal.
		/// </summary>
		internal override DataSemantics Semantics
		{
			get
			{
				if (Index < 0 || Index > MaximumIndex)
					return DataSemantics.TexCoords0;

				return DataSemantics.TexCoords0 + Index;
			}
		}
	}
}