namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a buffer stored in GPU memory.
	/// </summary>
	internal interface IBuffer : IDisposable
	{
		/// <summary>
		///     Gets the size of the buffer in bytes.
		/// </summary>
		int SizeInBytes { get; }

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		unsafe void* Map(MapMode mapMode);

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		/// <param name="offsetInBytes">A zero-based index denoting the first byte of the buffer that should be mapped.</param>
		/// <param name="byteCount">The number of bytes that should be mapped.</param>
		unsafe void* MapRange(MapMode mapMode, int offsetInBytes, int byteCount);

		/// <summary>
		///     Unmaps the buffer.
		/// </summary>
		void Unmap();

		/// <summary>
		///     Copies the given data to the constant buffer, overwriting all previous data.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		unsafe void CopyConstantBufferData(void* data);

		/// <summary>
		///     Binds the constant buffer to the given slot.
		/// </summary>
		void BindConstantBuffer(int slot);

		/// <summary>
		///     Sets the debug name of the buffer.
		/// </summary>
		/// <param name="name">The debug name of the buffer.</param>
		void SetName(string name);
	}
}