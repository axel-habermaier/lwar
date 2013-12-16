namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Indicates that a shader argument or return value represents a normal.
	/// </summary>
	public class NormalAttribute : SemanticsAttribute
	{
		/// <summary>
		///     Gets the corresponding data semantics literal.
		/// </summary>
		internal override DataSemantics Semantics
		{
			get { return DataSemantics.Normal; }
		}
	}
}