using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.Semantics
{
	/// <summary>
	///   Indicates that a shader argument or return value represents a color.
	/// </summary>
	public class ColorAttribute : SemanticsAttribute
	{
		/// <summary>
		///   Initializes a new instance with index 0.
		/// </summary>
		public ColorAttribute()
		{
		}

		/// <summary>
		///   Initializes a new instance with the given index.
		/// </summary>
		/// <param name="index">The index of the color.</param>
		public ColorAttribute(int index)
		{
			Index = index;
		}

		/// <summary>
		///   Gets the index of the color that is used to distinguish between multiple color inputs or outputs.
		/// </summary>
		public int Index { get; private set; }
	}
}