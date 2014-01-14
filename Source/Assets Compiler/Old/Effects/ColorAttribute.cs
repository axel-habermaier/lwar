namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Indicates that a shader argument or return value represents a color.
	/// </summary>
	public class ColorAttribute : SemanticsAttribute
	{
		/// <summary>
		///     Initializes a new instance with index 0.
		/// </summary>
		public ColorAttribute()
		{
		}

		/// <summary>
		///     Initializes a new instance with the given index.
		/// </summary>
		/// <param name="index">The index of the color.</param>
		public ColorAttribute(int index)
			: base(index)
		{
		}

		/// <summary>
		///     Gets the corresponding data semantics literal.
		/// </summary>
		internal override DataSemantics Semantics
		{
			get
			{
				if (Index < 0 || Index > MaximumIndex)
					return DataSemantics.Color0;

				return DataSemantics.Color0 + Index;
			}
		}
	}
}