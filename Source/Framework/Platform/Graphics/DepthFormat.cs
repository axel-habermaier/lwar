using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Describes the format of a depth stencil buffer.
	/// </summary>
	public enum DepthFormat
	{
		/// <summary>
		///   Indicates that no depth stencil buffer should be used.
		/// </summary>
		None = 1400,

		/// <summary>
		///   Indicates that a 16 bit depth buffer should be used without a stencil buffer.
		/// </summary>
		Depth16 = 1401,

		/// <summary>
		///   Indicates that a 24 bit depth buffer should be used without a stencil buffer.
		/// </summary>
		Depth24 = 1402,

		/// <summary>
		///   Indicates that a 24 bit depth buffer should be used in conjunction with a 8 bit stencil buffer.
		/// </summary>
		Depth24Stencil8 = 1403
	}
}