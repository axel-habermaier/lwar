using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Indicates the usage pattern of a buffer.
	/// </summary>
	public enum ResourceUsage
	{
		/// <summary>
		///   Indicates that the default usage pattern should be used.
		/// </summary>
		Default = 1900,

		/// <summary>
		///   Indicates that the buffer is static, i.e., its contents do not or only
		///   rarely change after its initial creation.
		/// </summary>
		Static = 1901,

		/// <summary>
		///   Indicates that the buffer is dynamic, i.e., there are frequent changes made
		///   to its contents.
		/// </summary>
		Dynamic = 1902,

		Staging = 1903
	}
}