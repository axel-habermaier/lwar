namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Logging;
	using Math;
	using Memory;

	/// <summary>
	///     Represents the graphics device.
	/// </summary>
	public sealed class GraphicsDevice : DisposableObject
	{
		/// <summary>
		///     The maximum number of frames the GPU can be behind the CPU.
		/// </summary>
		public const int FrameLag = 3;

		/// <summary>
		///     The timestamp queries that mark the beginning of a frame.
		/// </summary>
		private readonly TimestampQuery[] _beginQueries = new TimestampQuery[FrameLag];

		/// <summary>
		///     The native graphics device instance.
		/// </summary>
		private readonly IntPtr _device;

		/// <summary>
		///     The timestamp disjoint queries that are used to check whether the timestamps are valid and that allow the
		///     correct interpretation of the timestamp values.
		/// </summary>
		private readonly TimestampDisjointQuery[] _disjointQueries = new TimestampDisjointQuery[FrameLag];

		/// <summary>
		///     The timestamp queries that mark the end of a frame.
		/// </summary>
		private readonly TimestampQuery[] _endQueries = new TimestampQuery[FrameLag];

		/// <summary>
		///     The synced queries that are used to synchronize the GPU and the CPU.
		/// </summary>
		private readonly SyncedQuery[] _syncedQueries = new SyncedQuery[3];

		/// <summary>
		///     The current primitive type of the input assembler stage.
		/// </summary>
		private PrimitiveType _primitiveType;

		/// <summary>
		///     The current scissor rectangle of the rasterizer stage of the device.
		/// </summary>
		private Rectangle _scissor;

		/// <summary>
		///     The index of the synced query that is currently used to synchronize the GPU and the CPU.
		/// </summary>
		private int _syncedIndex;

		/// <summary>
		///     The current viewport of the rasterizer stage of the device.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		internal GraphicsDevice()
		{
			Log.Info("Initializing graphics device...");
			_device = NativeMethods.CreateGraphicsDevice();

			RasterizerState.InitializeDefaultInstances(this);
			SamplerState.InitializeDefaultInstances(this);
			DepthStencilState.InitializeDefaultInstances(this);
			BlendState.InitializeDefaultInstances(this);
			Texture2D.InitializeDefaultInstances(this);

			for (var i = 0; i < FrameLag; ++i)
			{
				_syncedQueries[i] = new SyncedQuery(this);
				_beginQueries[i] = new TimestampQuery(this);
				_endQueries[i] = new TimestampQuery(this);
				_disjointQueries[i] = new TimestampDisjointQuery(this);

				_syncedQueries[i].SetName("Synced Query {0}", i);
				_beginQueries[i].SetName("GPU Profiling Begin Query {0}", i);
				_endQueries[i].SetName("GPU Profiling End Query {0}", i);
				_disjointQueries[i].SetName("GPU Profiling Disjoint Query {0}", i);

				_beginQueries[i].Query();
				_endQueries[i].Query();
				_disjointQueries[i].Begin();
				_disjointQueries[i].End();
				_syncedQueries[i].MarkSyncPoint();
			}

			_syncedQueries[0].MarkSyncPoint();
		}

		/// <summary>
		///     Gets the native graphics device instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get
			{
				Assert.NotDisposed(this);
				return _device;
			}
		}

		/// <summary>
		///     Gets or sets the current viewport of the rasterizer stage of the device.
		/// </summary>
		internal Rectangle Viewport
		{
			get
			{
				Assert.NotDisposed(this);
				return _viewport;
			}
			set
			{
				Assert.NotDisposed(this);

				_viewport = value;
				NativeMethods.SetViewport(_device, value);
			}
		}

		/// <summary>
		///     Gets or sets the current scissor area of the rasterizer stage of the device.
		/// </summary>
		internal Rectangle ScissorArea
		{
			get
			{
				Assert.NotDisposed(this);
				return _scissor;
			}
			set
			{
				Assert.NotDisposed(this);

				_scissor = value;
				NativeMethods.SetScissorArea(_device, value);
			}
		}

		/// <summary>
		///     Gets or sets the current primitive type of the input assembler stage.
		/// </summary>
		internal PrimitiveType PrimitiveType
		{
			get
			{
				Assert.NotDisposed(this);
				return _primitiveType;
			}
			set
			{
				Assert.NotDisposed(this);

				_primitiveType = value;
				NativeMethods.SetPrimitiveType(_device, _primitiveType);
			}
		}

		/// <summary>
		///     Gets the GPU frame time in seconds for the last frame.
		/// </summary>
		public double FrameTime { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_syncedQueries.SafeDisposeAll();
			_beginQueries.SafeDisposeAll();
			_endQueries.SafeDisposeAll();
			_disjointQueries.SafeDisposeAll();

			RasterizerState.DisposeDefaultInstances();
			SamplerState.DisposeDefaultInstances();
			DepthStencilState.DisposeDefaultInstances();
			BlendState.DisposeDefaultInstances();
			Texture2D.DisposeDefaultInstances();

			NativeMethods.DestroyGraphicsDevice(_device);
		}

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		internal void Draw(int primitiveCount, int offset = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.Draw(_device, primitiveCount, offset);
		}

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		internal void DrawIndexed(int indexCount, int indexOffset = 0, int vertexOffset = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.DrawIndexed(_device, indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///     Gets the statistics and resets all values to zero. This method should be called once per frame.
		/// </summary>
		internal GraphicsDeviceStatistics GetStatistics()
		{
			GraphicsDeviceStatistics statistics;
			NativeMethods.GetStatistics(_device, out statistics);
			return statistics;
		}

		/// <summary>
		///     Marks the beginning of a frame, properly synchronizing the GPU and the CPU.
		/// </summary>
		internal void BeginFrame()
		{
			// Make sure the GPU is not more than FrameLag frames behind, so let's wait for the completion of the synced
			// query issued FrameLag frames ago
			_syncedQueries[_syncedIndex].WaitForCompletion();

			// Get the GPU frame time for the frame that we just synced
			// However, timestamps might be invalid if the GPU changed its clock rate, for instance
			var result = _disjointQueries[_syncedIndex].Result;
			if (result.Valid)
			{
				var begin = _beginQueries[_syncedIndex].Timestamp;
				var end = _endQueries[_syncedIndex].Timestamp;
				FrameTime = (end - begin) / (double)result.Frequency * 1000.0;
			}

			// Issue timing queries for the current frame
			_disjointQueries[_syncedIndex].Begin();
			_beginQueries[_syncedIndex].Query();
		}

		/// <summary>
		///     Marks the beginning of a frame, properly synchronizing the GPU and the CPU and updating the GPU frame time.
		/// </summary>
		internal void EndFrame()
		{
			// Issue timing queries to get frame end time
			_endQueries[_syncedIndex].Query();
			_disjointQueries[_syncedIndex].End();

			// We've completed the frame, so issue the synced query for the current frame and update the synced index
			_syncedQueries[_syncedIndex].MarkSyncPoint();
			_syncedIndex = (_syncedIndex + 1) % FrameLag;
		}

		/// <summary>
		///     Provides access to the native graphics device functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateGraphicsDevice")]
			public static extern IntPtr CreateGraphicsDevice();

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyGraphicsDevice")]
			public static extern void DestroyGraphicsDevice(IntPtr device);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetViewport")]
			public static extern void SetViewport(IntPtr device, Rectangle viewport);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetScissorArea")]
			public static extern void SetScissorArea(IntPtr device, Rectangle scissorArea);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetPrimitiveType")]
			public static extern void SetPrimitiveType(IntPtr device, PrimitiveType primitiveType);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDraw")]
			public static extern void Draw(IntPtr device, int primitiveCount, int offset);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDrawIndexed")]
			public static extern void DrawIndexed(IntPtr device, int indexCount, int indexOffset, int vertexOffset);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetStatistics")]
			public static extern void GetStatistics(IntPtr device, out GraphicsDeviceStatistics statistics);
		}
	}
}