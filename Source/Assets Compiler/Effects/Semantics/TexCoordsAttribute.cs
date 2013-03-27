using System;

namespace Pegasus.AssetsCompiler.Effects.Semantics
{
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
		{
			Index = index;
		}

		/// <summary>
		///   Gets the index of the texture coordinates that is used to distinguish between multiple texture coordinates inputs or
		///   outputs.
		/// </summary>
		public int Index { get; private set; }
	}
}