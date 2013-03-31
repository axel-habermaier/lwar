using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Framework.Platform.Graphics;

	/// <summary>
	///   Describes the semantics of a shader argument.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public abstract class SemanticsAttribute : Attribute
	{
		/// <summary>
		///   Gets the maximum allowed index; all semantic indices must lie between 0 and this value.
		/// </summary>
		internal const int MaximumIndex = 3;

		/// <summary>
		///   Initializes a new instance with the given index.
		/// </summary>
		/// <param name="index">The index of the texture coordinates.</param>
		protected SemanticsAttribute(int index)
		{
			Index = index;
		}

		/// <summary>
		///   Initializes a new instance with index 0.
		/// </summary>
		protected SemanticsAttribute()
		{
		}

		/// <summary>
		///   Gets the corresponding data semantics literal.
		/// </summary>
		internal abstract DataSemantics Semantics { get; }

		/// <summary>
		///   Gets the semantic index that is used to distinguish between multiple inputs or outputs with the same semantics.
		/// </summary>
		internal int Index { get; private set; }
	}
}