namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;
	using Memory;

	/// <summary>
	///     Represents an Direct3D11-based buffer stored in GPU memory.
	/// </summary>
	internal unsafe class BufferD3D11 : GraphicsObjectD3D11, IBuffer
	{
		/// <summary>
		///     The underlying Direct3D11 buffer.
		/// </summary>
		internal D3D11Buffer Buffer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">The description of the buffer.</param>
		public BufferD3D11(GraphicsDeviceD3D11 graphicsDevice, ref BufferDescription description)
			: base(graphicsDevice)
		{
			D3D11BindFlags bindFlag;

			switch (description.Type)
			{
				case BufferType.ConstantBuffer:
					bindFlag = D3D11BindFlags.ConstantBuffer;
					break;
				case BufferType.IndexBuffer:
					bindFlag = D3D11BindFlags.IndexBuffer;
					break;
				case BufferType.VertexBuffer:
					bindFlag = D3D11BindFlags.VertexBuffer;
					break;
				default:
					throw new InvalidOperationException("Unsupported buffer type.");
			}

			var data = new D3D11SubResourceData
			{
				Data = description.Data,
				RowPitch = 0,
				SlicePitch = 0
			};

			var desc = new D3D11BufferDescription
			{
				BindFlags = bindFlag,
				CpuAccessFlags = description.Usage == ResourceUsage.Dynamic ? D3D11CpuAccessFlags.Write : D3D11CpuAccessFlags.None,
				SizeInBytes = description.SizeInBytes,
				OptionFlags = D3D11ResourceOptionFlags.None,
				StructureByteStride = 0,
				Usage = description.Usage.Map()
			};

			SizeInBytes = description.SizeInBytes;
			Device.CreateBuffer(ref desc, description.Data == IntPtr.Zero ? null : &data, out Buffer).CheckSuccess("Failed to create buffer.");
		}

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
			D3D11SubResourceData subResource;
			Context.Map(Buffer, 0, mapMode.Map(), D3D11MapFlags.None, &subResource).CheckSuccess("Failed to map buffer.");
			return (void*)subResource.Data;
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
			var ptr = Map(mapMode);
			return offsetInBytes + (byte*)ptr;
		}

		/// <summary>
		///     Unmaps the buffer.
		/// </summary>
		public void Unmap()
		{
			Context.Unmap(Buffer, 0);
		}

		/// <summary>
		///     Copies the given data to the constant buffer, overwriting all previous data.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		public void CopyConstantBufferData(void* data)
		{
			var bufferData = Map(MapMode.WriteDiscard);
			MemCopy.Copy(bufferData, data, SizeInBytes);
			Unmap();
		}

		/// <summary>
		///     Binds the constant buffer to the given slot.
		/// </summary>
		public void BindConstantBuffer(int slot)
		{
			var buffer = Buffer;
			Context.VSSetConstantBuffers(slot, 1, &buffer);
			Context.PSSetConstantBuffers(slot, 1, &buffer);
		}

		/// <summary>
		///     Sets the debug name of the buffer.
		/// </summary>
		/// <param name="name">The debug name of the buffer.</param>
		public void SetName(string name)
		{
			Buffer.SetDebugName(name);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Buffer.Release();
		}
	}
}