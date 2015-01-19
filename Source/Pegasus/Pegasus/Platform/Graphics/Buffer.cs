﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Base class for graphics buffers.
	/// </summary>
	public abstract class Buffer : GraphicsObject
	{
		/// <summary>
		///     Indicates whether the buffer is currently mapped.
		/// </summary>
		private bool _isMapped;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="type">The type of the buffer.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		/// <param name="data">The data that should be copied into the buffer.</param>
		/// <param name="sizeInBytes">The size of the buffer in bytes.</param>
		protected Buffer(GraphicsDevice graphicsDevice, BufferType type, ResourceUsage usage, IntPtr data, int sizeInBytes)
			: base(graphicsDevice)
		{
			Assert.ArgumentInRange(type);
			Assert.ArgumentInRange(usage);
			Assert.ArgumentSatisfies(sizeInBytes > 0, "A buffer must have a size greater than 0.");

			var description = new BufferDescription
			{
				Data = data,
				SizeInBytes = sizeInBytes,
				Type = type,
				Usage = usage
			};

			BufferObject = graphicsDevice.CreateBuffer(ref description);
		}

		/// <summary>
		///     Gets the underlying buffer object.
		/// </summary>
		internal IBuffer BufferObject { get; private set; }

		/// <summary>
		///     Gets the size of the buffer in bytes.
		/// </summary>
		public int SizeInBytes
		{
			get
			{
				Assert.NotDisposed(this);
				return BufferObject.SizeInBytes;
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			BufferObject.SafeDispose();
		}

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected override void OnRenamed(string name)
		{
			BufferObject.SetName(name);
		}

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		public unsafe BufferData Map(MapMode mapMode)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentInRange(mapMode);
			Assert.That(!_isMapped, "Buffer is already mapped.");

			_isMapped = true;
			return new BufferData(this, BufferObject.Map(mapMode));
		}

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		/// <param name="offsetInBytes">A zero-based index denoting the first byte of the buffer that should be mapped.</param>
		/// <param name="byteCount">The number of bytes that should be mapped.</param>
		public unsafe BufferData MapRange(MapMode mapMode, int offsetInBytes, int byteCount)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentInRange(mapMode);
			Assert.That(offsetInBytes < SizeInBytes, "Invalid offset.");
			Assert.InRange(byteCount, 1, SizeInBytes - 1);
			Assert.That(offsetInBytes + byteCount <= SizeInBytes, "Buffer overflow.");
			Assert.That(!_isMapped, "Buffer is already mapped.");

			_isMapped = true;
			return new BufferData(this, BufferObject.MapRange(mapMode, offsetInBytes, byteCount));
		}

		/// <summary>
		///     Unmaps the buffer.
		/// </summary>
		internal void Unmap()
		{
			Assert.NotDisposed(this);
			Assert.That(_isMapped, "Buffer is not mapped.");

			_isMapped = false;
			BufferObject.Unmap();
		}
	}
}