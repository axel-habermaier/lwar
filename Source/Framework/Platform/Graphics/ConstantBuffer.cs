using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   A constant buffer holds constant data for shader programs.
	/// </summary>
	/// <typeparam name="TData">The type of the data in the constant buffer.</typeparam>
	public sealed class ConstantBuffer<TData> : Buffer
		where TData : struct
	{
		/// <summary>
		///   The unmanaged size of the buffer in bytes.
		/// </summary>
		private static readonly int Size = Marshal.SizeOf(typeof(TData));

		/// <summary>
		///   The action that uploads the constant buffer's data to the GPU.
		/// </summary>
		private readonly Action<ConstantBuffer<TData>, TData> _update;

		/// <summary>
		///   The data stored in the constant buffer.
		/// </summary>
		public TData Data;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="update">An action that should be used to upload the constant buffer's data to the GPU.</param>
		public ConstantBuffer(GraphicsDevice graphicsDevice, Action<ConstantBuffer<TData>, TData> update)
			: this(graphicsDevice, update, new TData())
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="update">An action that should be used to upload the constant buffer's data to the GPU.</param>
		/// <param name="data">The initial contents of the constant buffer.</param>
		public ConstantBuffer(GraphicsDevice graphicsDevice, Action<ConstantBuffer<TData>, TData> update, TData data)
			: base(graphicsDevice, BufferType.ConstantBuffer, ResourceUsage.Dynamic, IntPtr.Zero, Size)
		{
			Assert.ArgumentNotNull(update, () => update);

			_update = update;
			Data = data;

			Update();
		}

		/// <summary>
		///   Binds the constant buffer to the given slot without uploading any possible changes of the buffer to the GPU.
		/// </summary>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			BindBuffer(slot);
		}

		/// <summary>
		///   Sends all changes to the contents of the constant buffer to the GPU.
		/// </summary>
		public void Update()
		{
			Assert.NotDisposed(this);
			_update(this, Data);
		}

		/// <summary>
		///   Copies the data to the buffer.
		/// </summary>
		/// <param name="data">The data that should be copied into the buffer.</param>
		public unsafe void Copy(void* data)
		{
			Assert.NotDisposed(this);
			CopyData(new IntPtr(data), Size);
		}
	}

	/// <summary>
	///   A constant buffer holds constant data for shader programs.
	/// </summary>
	public sealed class ConstantBuffer : Buffer
	{
		/// <summary>
		///   The unmanaged size of the buffer in bytes.
		/// </summary>
		private readonly int _size;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="size">The size of the constant buffer's contents in bytes.</param>
		public ConstantBuffer(GraphicsDevice graphicsDevice, int size)
			: base(graphicsDevice, BufferType.ConstantBuffer, ResourceUsage.Dynamic, IntPtr.Zero, size)
		{
			_size = size;
		}

		/// <summary>
		///   Binds the constant buffer to the given slot without uploading any possible changes of the buffer to the GPU.
		/// </summary>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			BindBuffer(slot);
		}

		/// <summary>
		///   Copies the given data to the buffer. The size of the data is determined by the parameter that has been passed to the
		///   constructor of this instance.
		/// </summary>
		/// <param name="data">The data that should be copied into the buffer.</param>
		public unsafe void CopyData(void* data)
		{
			Assert.NotDisposed(this);
			CopyData(new IntPtr(data), _size);
		}
	}
}