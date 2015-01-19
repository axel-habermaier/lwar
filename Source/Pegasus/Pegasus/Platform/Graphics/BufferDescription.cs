namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the properties of a buffer.
	/// </summary>
	internal struct BufferDescription
	{
		/// <summary>
		///     The initial data of the buffer.
		/// </summary>
		public IntPtr Data;

		/// <summary>
		///     The size of the buffer in bytes.
		/// </summary>
		public int SizeInBytes;

		/// <summary>
		///     The type of the buffer.
		/// </summary>
		public BufferType Type;

		/// <summary>
		///     The usage pattern of the buffer.
		/// </summary>
		public ResourceUsage Usage;
	}
}