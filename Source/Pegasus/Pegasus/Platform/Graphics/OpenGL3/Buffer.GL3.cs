namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;
	using Logging;

	/// <summary>
	///     Represents an OpenGL3-based buffer stored in GPU memory.
	/// </summary>
	internal unsafe class BufferGL3 : GraphicsObjectGL3, IBuffer
	{
		/// <summary>
		///     The OpenGL type of the buffer.
		/// </summary>
		private readonly uint _glType;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">The description of the buffer.</param>
		public BufferGL3(GraphicsDeviceGL3 graphicsDevice, ref BufferDescription description)
			: base(graphicsDevice)
		{
			SizeInBytes = description.SizeInBytes;
			Handle = GLContext.Allocate(_gl.GenBuffers, "Buffer");

			switch (description.Type)
			{
				case BufferType.ConstantBuffer:
					_glType = GL.UniformBuffer;
					break;
				case BufferType.IndexBuffer:
					_glType = GL.ElementArrayBuffer;
					break;
				case BufferType.VertexBuffer:
					_glType = GL.ArrayBuffer;
					break;
				default:
					throw new InvalidOperationException("Unknown buffer type.");
			}

			_gl.BindBuffer(_glType, Handle);
			_gl.BufferData(_glType, (void*)SizeInBytes, (void*)description.Data, description.Usage.Map());
		}

		/// <summary>
		///     Gets the OpenGL buffer handle.
		/// </summary>
		internal uint Handle { get; private set; }

		/// <summary>
		///     Gets the size of the buffer in bytes.
		/// </summary>
		public int SizeInBytes { get; private set; }

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		public void* Map(MapMode mapMode)
		{
			_gl.BindBuffer(_glType, Handle);
			var mappedBuffer = _gl.MapBuffer(_glType, mapMode.Map());

			if (mappedBuffer == null)
				Log.Die("Failed to map buffer.");

			return mappedBuffer;
		}

		/// <summary>
		///     Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///     returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		/// <param name="offsetInBytes">A zero-based index denoting the first byte of the buffer that should be mapped.</param>
		/// <param name="byteCount">The number of bytes that should be mapped.</param>
		public void* MapRange(MapMode mapMode, int offsetInBytes, int byteCount)
		{
			_gl.BindBuffer(_glType, Handle);
			var mappedBuffer = _gl.MapBufferRange(_glType, (void*)offsetInBytes, (void*)byteCount, mapMode.Map());

			if (mappedBuffer == null)
				Log.Die("Failed to map buffer.");

			return mappedBuffer;
		}

		/// <summary>
		///     Unmaps the buffer.
		/// </summary>
		public void Unmap()
		{
			_gl.BindBuffer(_glType, Handle);

			if (!_gl.UnmapBuffer(_glType))
				Log.Die("Failed to unmap buffer.");
		}

		/// <summary>
		///     Copies the given data to the constant buffer, overwriting all previous data.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		public void CopyConstantBufferData(void* data)
		{
			_gl.BindBuffer(GL.UniformBuffer, Handle);
			_gl.BufferSubData(GL.UniformBuffer, (void*)0, (void*)SizeInBytes, data);
		}

		/// <summary>
		///     Binds the constant buffer to the given slot.
		/// </summary>
		public void BindConstantBuffer(int slot)
		{
			_gl.BindBufferBase(GL.UniformBuffer, (uint)slot, Handle);
		}

		/// <summary>
		///     Sets the debug name of the buffer.
		/// </summary>
		/// <param name="name">The debug name of the buffer.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL3
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteBuffers, Handle);
		}
	}
}