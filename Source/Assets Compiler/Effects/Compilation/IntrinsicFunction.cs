using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Represents an intrinsic function.
	/// </summary>
	internal enum IntrinsicFunction
	{
		/// <summary>
		///   Represents the texture or cubemap sampling function.
		/// </summary>
		Sample,

		/// <summary>
		///   Represents the texture or cubemap sampling function with manual mipmap level selection.
		/// </summary>
		SampleLevel
	}
}