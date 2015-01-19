namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates the usage pattern of a buffer.
	/// </summary>
	public enum ResourceUsage
	{
		/// <summary>
		///     Indicates that the default usage pattern should be used.
		/// </summary>
		Default,

		/// <summary>
		///     Indicates that the buffer is static, i.e., its contents do not or only rarely change after its initial creation.
		/// </summary>
		Static,

		/// <summary>
		///     Indicates that the buffer is dynamic, i.e., there are frequent changes made to its contents.
		/// </summary>
		Dynamic,

		/// <summary>
		///     Indicates that the resource supports data transfer from the GPU to the CPU.
		/// </summary>
		Staging
	}
}