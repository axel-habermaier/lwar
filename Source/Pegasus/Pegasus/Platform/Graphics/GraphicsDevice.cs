namespace Pegasus.Platform.Graphics
{
	using System;
	using Direct3D11;
	using Interface;
	using Logging;
	using Math;
	using Memory;
	using OpenGL3;
	using UserInterface;
	using Utilities;

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
		///     The maximum number of constant buffers that can be bound simultaneously.
		/// </summary>
		public const int ConstantBufferSlotCount = 14;

		/// <summary>
		///     The maximum number of textures and samplers that can be bound simultaneously.
		/// </summary>
		public const int TextureSlotCount = 16;

		/// <summary>
		///     The maximum number of color buffers that can be bound simultaneously.
		/// </summary>
		public const int MaxColorBuffers = 4;

		/// <summary>
		///     The maximum number of mipmaps supported for each texture.
		/// </summary>
		public const int MaxMipmaps = 16;

		/// <summary>
		///     The maximum texture size in all directions.
		/// </summary>
		public const int MaxTextureSize = 8192;

		/// <summary>
		///     The maximum number of surfaces supported for each texture.
		/// </summary>
		public const int MaxSurfaceCount = MaxMipmaps * 6 * 3;

		/// <summary>
		///     The maximum number of vertex bindings that can be bound simultaneously.
		/// </summary>
		public const int MaxVertexBindings = 8;

		/// <summary>
		///     The timestamp queries that mark the beginning of a frame.
		/// </summary>
		private readonly TimestampQuery[] _beginQueries = new TimestampQuery[FrameLag];

		/// <summary>
		///     The underlying graphics device object.
		/// </summary>
		private readonly IGraphicsDevice _device;

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
		///     The index of the synced query that is currently used to synchronize the GPU and the CPU.
		/// </summary>
		private int _syncedIndex;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		internal GraphicsDevice(GraphicsApi graphicsApi)
		{
			Assert.ArgumentInRange(graphicsApi);
			Log.Info("Initializing graphics device...");

			GraphicsApi = graphicsApi;
			State = new DeviceState();

			switch (graphicsApi)
			{
				case GraphicsApi.Direct3D11:
					_device = new GraphicsDeviceD3D11();
					break;
				case GraphicsApi.OpenGL3:
					_device = new GraphicsDeviceGL3();
					break;
				default:
					throw new ArgumentOutOfRangeException("graphicsApi");
			}

			_device.PrintInfo();

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
		}

		/// <summary>
		///     The type of the underlying graphics API used by the graphics device.
		/// </summary>
		public GraphicsApi GraphicsApi { get; private set; }

		/// <summary>
		///     Gets the state of the graphics device.
		/// </summary>
		internal DeviceState State { get; private set; }

		/// <summary>
		///     Gets the GPU frame time in milliseconds for the last frame.
		/// </summary>
		internal double FrameTime { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_syncedQueries.SafeDisposeAll();
			_beginQueries.SafeDisposeAll();
			_endQueries.SafeDisposeAll();
			_disjointQueries.SafeDisposeAll();

			_device.SafeDispose();
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
			double frequency;
			if (_disjointQueries[_syncedIndex].TryGetFrequency(out frequency))
			{
				var begin = _beginQueries[_syncedIndex].Timestamp;
				var end = _endQueries[_syncedIndex].Timestamp;
				FrameTime = (end - begin) / frequency * 1000.0;
			}

			// Issue timing queries for the current frame
			_disjointQueries[_syncedIndex].Begin();
			_beginQueries[_syncedIndex].Query();

			// The graphics device can now be safely used for drawing
			State.CanDraw = true;
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

			// The graphics device can no longer safely be used for drawing
			State.CanDraw = false;
		}

		/// <summary>
		///     Gets the vertex count for the given primitive count and current primitive type.
		/// </summary>
		/// <param name="primitiveCount">The primitive count that should be converted.</param>
		private int GetVertexCount(int primitiveCount)
		{
			switch (State.PrimitiveType)
			{
				case PrimitiveType.TriangleList:
					return primitiveCount * 3;
				case PrimitiveType.TriangleStrip:
					return primitiveCount + 2;
				case PrimitiveType.LineList:
					return primitiveCount * 2;
				case PrimitiveType.LineStrip:
					return primitiveCount + 1;
				default:
					throw new InvalidOperationException("Unsupported primitive type.");
			}
		}

		/// <summary>
		///     Creates a buffer object.
		/// </summary>
		/// <param name="description">The description of the buffer.</param>
		internal IBuffer CreateBuffer(ref BufferDescription description)
		{
			return _device.CreateBuffer(ref description);
		}

		/// <summary>
		///     Creates a blend state.
		/// </summary>
		/// <param name="description">The description of the blend state.</param>
		internal IBlendState CreateBlendState(ref BlendDescription description)
		{
			return _device.CreateBlendState(ref description);
		}

		/// <summary>
		///     Creates a depth stencil state.
		/// </summary>
		/// <param name="description">The description of the depth stencil state.</param>
		internal IDepthStencilState CreateDepthStencilState(ref DepthStencilDescription description)
		{
			return _device.CreateDepthStencilState(ref description);
		}

		/// <summary>
		///     Creates a rasterizer state.
		/// </summary>
		/// <param name="description">The description of the rasterizer state.</param>
		internal IRasterizerState CreateRasterizerState(ref RasterizerDescription description)
		{
			return _device.CreateRasterizerState(ref description);
		}

		/// <summary>
		///     Creates a sampler state.
		/// </summary>
		/// <param name="description">The description of the sampler state.</param>
		internal ISamplerState CreateSamplerState(ref SamplerDescription description)
		{
			return _device.CreateSamplerState(ref description);
		}

		/// <summary>
		///     Creates a texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		internal ITexture CreateTexture(ref TextureDescription description, Surface[] surfaces)
		{
			return _device.CreateTexture(ref description, surfaces);
		}

		/// <summary>
		///     Creates a query.
		/// </summary>
		/// <param name="queryType">The type of the query.</param>
		internal IQuery CreateQuery(QueryType queryType)
		{
			return _device.CreateQuery(queryType);
		}

		/// <summary>
		///     Creates a shader.
		/// </summary>
		/// <param name="shaderType">The type of the shader.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		internal IShader CreateShader(ShaderType shaderType, IntPtr byteCode, int byteCodeLength)
		{
			return _device.CreateShader(shaderType, byteCode, byteCodeLength);
		}

		/// <summary>
		///     Creates a shader program.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		internal IShaderProgram CreateShaderProgram(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			return _device.CreateShaderProgram(vertexShader, fragmentShader);
		}

		/// <summary>
		///     Creates a vertex layout.
		/// </summary>
		/// <param name="description">The description of the vertex layout.</param>
		internal IVertexLayout CreateVertexLayout(ref VertexLayoutDescription description)
		{
			return _device.CreateVertexLayout(ref description);
		}

		/// <summary>
		///     Creates a render target.
		/// </summary>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		internal IRenderTarget CreateRenderTarget(Texture2D depthStencil, Texture2D[] colorBuffers)
		{
			return _device.CreateRenderTarget(depthStencil, colorBuffers);
		}

		/// <summary>
		///     Creates a swap chain.
		/// </summary>
		/// <param name="window">The window the swap chain should be created for..</param>
		internal ISwapChain CreateSwapChain(NativeWindow window)
		{
			return _device.CreateSwapChain(window);
		}

		/// <summary>
		///     Changes the primitive type that is used for drawing.
		/// </summary>
		/// <param name="primitiveType">The primitive type that should be used for drawing.</param>
		internal void ChangePrimitiveType(PrimitiveType primitiveType)
		{
			Assert.ArgumentInRange(primitiveType);

			// We cannot use DeviceState.Change here, as that creates garbage like crazy; apparently, 
			// EqualityComparer<T> has no optimized path for nullables of enum types
			if (State.PrimitiveType == primitiveType)
				return;

			State.PrimitiveType = primitiveType;
			_device.ChangePrimitiveType(primitiveType);
		}

		/// <summary>
		///     Changes the viewport that is drawn to.
		/// </summary>
		/// <param name="viewport">The viewport that should be drawn to.</param>
		internal void ChangeViewport(Rectangle viewport)
		{
			if (DeviceState.Change(ref State.Viewport, viewport))
				_device.ChangeViewport(ref viewport);
		}

		/// <summary>
		///     Changes the scissor area that is used when the scissor test is enabled.
		/// </summary>
		/// <param name="scissorArea">The scissor area that should be used by the scissor test.</param>
		internal void ChangeScissorArea(Rectangle scissorArea)
		{
			if (DeviceState.Change(ref State.ScissorArea, scissorArea))
				_device.ChangeScissorArea(ref scissorArea);
		}

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		internal void Draw(int primitiveCount, int vertexOffset)
		{
			Assert.NotDisposed(this);
			State.Validate();

			_device.Draw(GetVertexCount(primitiveCount), vertexOffset);
		}

		/// <summary>
		///     Draws primitiveCount-many instanced primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="instanceCount">The number of instanced that should be drawn.</param>
		/// <param name="primitiveCount">The number of primitives that should be drawn per instance.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		internal void DrawInstanced(int instanceCount, int primitiveCount, int vertexOffset, int instanceOffset)
		{
			Assert.NotDisposed(this);
			State.Validate();

			_device.DrawInstanced(instanceCount, GetVertexCount(primitiveCount), vertexOffset, instanceOffset);
		}

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		internal void DrawIndexed(int indexCount, int indexOffset, int vertexOffset)
		{
			Assert.NotDisposed(this);
			State.Validate();

			_device.DrawIndexed(indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///     Draws indexCount-many instanced indices, starting at the given index offset into the currently bound index buffer.
		/// </summary>
		/// <param name="instanceCount">The number of instances to draw.</param>
		/// <param name="indexCount">The number of indices to draw per instance.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The offset applied to the non-instanced vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		internal void DrawIndexedInstanced(int instanceCount, int indexCount, int indexOffset, int vertexOffset, int instanceOffset)
		{
			Assert.NotDisposed(this);
			State.Validate();

			_device.DrawIndexedInstanced(instanceCount, indexCount, indexOffset, vertexOffset, instanceOffset);
		}
	}
}