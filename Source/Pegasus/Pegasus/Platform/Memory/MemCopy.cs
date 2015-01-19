namespace Pegasus.Platform.Memory
{
	using System;
	using Utilities;

	/// <summary>
	///     Allows copying of unmanaged memory.
	/// </summary>
	internal static class MemCopy
	{
		/// <summary>
		///     Copies given number of bytes from the source to the destination.
		/// </summary>
		/// <param name="destination">The address of the first byte that should be written.</param>
		/// <param name="source">The address of the first byte that should be read.</param>
		/// <param name="byteCount">The number of bytes that should be copied.</param>
		internal static unsafe void Copy(IntPtr destination, IntPtr source, int byteCount)
		{
			Copy(destination.ToPointer(), source.ToPointer(), byteCount);
		}

		/// <summary>
		///     Copies given number of bytes from the source to the destination.
		/// </summary>
		/// <param name="destination">The address of the first byte that should be written.</param>
		/// <param name="source">The address of the first byte that should be read.</param>
		/// <param name="byteCount">The number of bytes that should be copied.</param>
		internal static unsafe void Copy(void* destination, void* source, int byteCount)
		{
			Assert.ArgumentNotNull(new IntPtr(destination));
			Assert.ArgumentNotNull(new IntPtr(source));
			Assert.ArgumentSatisfies(byteCount > 0, "At least 1 byte must be copied.");
			Assert.ArgumentSatisfies((source < destination && (byte*)source + byteCount <= destination) ||
									 (destination < source && (byte*)destination + byteCount <= source),
				"The memory regions overlap.");

			CopyBlock.Copy(destination, source, byteCount);
		}
	}
}