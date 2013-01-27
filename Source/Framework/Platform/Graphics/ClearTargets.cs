using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Indicates the targets of a clear operation.
	/// </summary>
	[Flags]
	public enum ClearTargets
	{
		/// <summary>
		///   Indicates that the color buffers should be cleared.
		/// </summary>
		Color = 1,

		/// <summary>
		///   Indicates that the depth buffer should be cleared.
		/// </summary>
		Depth = 2,

		/// <summary>
		///   Indicates that the stencil buffer should be cleared.
		/// </summary>
		Stencil = 4
	}
}