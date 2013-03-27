using System;

namespace Pegasus.AssetsCompiler.Effects
{
	/// <summary>
	///   Indicates how often a value that is part of a constant buffer changes.
	/// </summary>
	public enum ChangeFrequency
	{
		/// <summary>
		///   Indicates that the change frequency of the value is unknown.
		/// </summary>
		Unknown = 0,

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