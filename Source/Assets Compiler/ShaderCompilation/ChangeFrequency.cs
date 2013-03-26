using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation
{
	/// <summary>
	///   Indicates how often a value that is part of a constant buffer changes.
	/// </summary>
	public enum ChangeFrequency
	{
		/// <summary>
		///   Indicates that the value usually changes once per frame.
		/// </summary>
		PerFrame,

		/// <summary>
		///   Indicates that the value usually changes for each draw call.
		/// </summary>
		PerDrawCall,
	}
}