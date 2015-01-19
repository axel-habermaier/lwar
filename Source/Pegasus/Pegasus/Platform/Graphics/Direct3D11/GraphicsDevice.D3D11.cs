namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;
	using Logging;
	using Math;
	using Memory;
	using UserInterface;

	/// <summary>
	///     Represents an Direct3D11-based graphics device.
	/// </summary>
	internal class GraphicsDeviceD3D11 : DisposableObject, IGraphicsDevice
	{
		/// <summary>
		///     The minimum feature level required by the application.
		/// </summary>
		private const D3D11FeatureLevel RequiredFeatureLevel = D3D11FeatureLevel.Level_9_3;

		/// <summary>
		///     The DXGI adapter that was used to create the device.
		/// </summary>
		private DXGIAdapter _adapter;

		/// <summary>
		///     The pixel shader that is currently bound.
		/// </summary>
		internal D3D11PixelShader BoundPixelShader;

		/// <summary>
		///     The vertex shader that is currently bound.
		/// </summary>
		internal D3D11VertexShader BoundVertexShader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public unsafe GraphicsDeviceD3D11()
		{
			var factoryGuid = new Guid("7b7166ec-21c7-44ae-b21a-c9ae321ae369");
			const uint sdkVersion = 7;

			DXGIFactory factory;
			CreateDXGIFactory(&factoryGuid, &factory).CheckSuccess("Failed to create a DXGI factory.");
			Factory = factory;

			DXGIAdapter adapter;
			Factory.EnumAdapters(0, &adapter).CheckSuccess("Failed to get a DXGI adapter.");
			_adapter = adapter;

			var flags = D3D11DeviceCreationFlags.SingleThreaded;
#if DEBUG
			flags |= D3D11DeviceCreationFlags.Debug;
#endif

			D3D11FeatureLevel featureLevel;
			D3D11Device device;
			D3D11DeviceContext context;
			D3D11CreateDevice(_adapter, D3D11DriverType.Unknown, null, flags, null, 0, sdkVersion, &device, &featureLevel, &context)
				.CheckSuccess("Failed to create a Direct3D 11 device.");

			Device = device;
			Context = context;

			if (featureLevel < RequiredFeatureLevel)
			{
				Log.Die("Incompatible graphics card: Only feature level {0} is supported, but feature level {1} is required.",
					FeatureLevelToString(featureLevel), FeatureLevelToString(RequiredFeatureLevel));
			}
		}

		/// <summary>
		///     Gets the DXGI factory that was used to create the device.
		/// </summary>
		internal DXGIFactory Factory { get; private set; }

		/// <summary>
		///     Gets the device context.
		/// </summary>
		internal D3D11DeviceContext Context { get; private set; }

		/// <summary>
		///     Gets the underlying Direct3D11 device.
		/// </summary>
		internal D3D11Device Device { get; private set; }

		/// <summary>
		///     Creates a buffer object.
		/// </summary>
		/// <param name="description">The description of the buffer.</param>
		public IBuffer CreateBuffer(ref BufferDescription description)
		{
			return new BufferD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a blend state.
		/// </summary>
		/// <param name="description">The description of the blend state.</param>
		public IBlendState CreateBlendState(ref BlendDescription description)
		{
			return new BlendStateD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a depth stencil state.
		/// </summary>
		/// <param name="description">The description of the depth stencil state.</param>
		public IDepthStencilState CreateDepthStencilState(ref DepthStencilDescription description)
		{
			return new DepthStencilStateD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a rasterizer state.
		/// </summary>
		/// <param name="description">The description of the rasterizer state.</param>
		public IRasterizerState CreateRasterizerState(ref RasterizerDescription description)
		{
			return new RasterizerStateD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a sampler state.
		/// </summary>
		/// <param name="description">The description of the sampler state.</param>
		public ISamplerState CreateSamplerState(ref SamplerDescription description)
		{
			return new SamplerStateD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		public ITexture CreateTexture(ref TextureDescription description, Surface[] surfaces)
		{
			return new TextureD3D11(this, ref description, surfaces);
		}

		/// <summary>
		///     Creates a query.
		/// </summary>
		/// <param name="queryType">The type of the query.</param>
		public IQuery CreateQuery(QueryType queryType)
		{
			return new QueryD3D11(this, queryType);
		}

		/// <summary>
		///     Creates a shader.
		/// </summary>
		/// <param name="shaderType">The type of the shader.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		public IShader CreateShader(ShaderType shaderType, IntPtr byteCode, int byteCodeLength)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					return new VertexShaderD3D11(this, byteCode, byteCodeLength);
				case ShaderType.FragmentShader:
					return new PixelShaderD3D11(this, byteCode, byteCodeLength);
				default:
					throw new ArgumentOutOfRangeException("shaderType");
			}
		}

		/// <summary>
		///     Creates a shader program.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		public IShaderProgram CreateShaderProgram(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			return new ShaderProgramD3D11(this, vertexShader, fragmentShader);
		}

		/// <summary>
		///     Creates a vertex layout.
		/// </summary>
		/// <param name="description">The description of the vertex layout.</param>
		public IVertexLayout CreateVertexLayout(ref VertexLayoutDescription description)
		{
			return new VertexLayoutD3D11(this, ref description);
		}

		/// <summary>
		///     Creates a render target.
		/// </summary>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public IRenderTarget CreateRenderTarget(Texture2D depthStencil, Texture2D[] colorBuffers)
		{
			return new RenderTargetD3D11(this, depthStencil, colorBuffers);
		}

		/// <summary>
		///     Creates a swap chain.
		/// </summary>
		/// <param name="window">The window the swap chain should be created for..</param>
		public ISwapChain CreateSwapChain(NativeWindow window)
		{
			return new SwapChainD3D11(this, window);
		}

		/// <summary>
		///     Prints information about the graphics device.
		/// </summary>
		public unsafe void PrintInfo()
		{
			DXGIAdapterDescription desc;
			_adapter.GetDesc(&desc).CheckSuccess("Failed to get the description of the adapter.");

			Log.Info("Direct3D renderer: {0}", new String((char*)&desc.Description0));
			Log.Info("Direct3D feature level: {0}", FeatureLevelToString(Device.GetFeatureLevel()));
		}

		/// <summary>
		///     Changes the primitive type that is used for drawing.
		/// </summary>
		/// <param name="primitiveType">The primitive type that should be used for drawing.</param>
		public void ChangePrimitiveType(PrimitiveType primitiveType)
		{
			Context.IASetPrimitiveTopology(primitiveType.Map());
		}

		/// <summary>
		///     Changes the viewport that is drawn to.
		/// </summary>
		/// <param name="viewport">The viewport that should be drawn to.</param>
		public unsafe void ChangeViewport(ref Rectangle viewport)
		{
			var vp = new D3D11Viewport
			{
				X = viewport.Left,
				Y = viewport.Top,
				Width = viewport.Width,
				Height = viewport.Height,
				MinDepth = 0,
				MaxDepth = 1
			};

			Context.RSSetViewports(1, &vp);
		}

		/// <summary>
		///     Changes the scissor area that is used when the scissor test is enabled.
		/// </summary>
		/// <param name="scissorArea">The scissor area that should be used by the scissor test.</param>
		public unsafe void ChangeScissorArea(ref Rectangle scissorArea)
		{
			var area = new D3D11Rectangle
			{
				Left = scissorArea.Position.IntegralX,
				Top = scissorArea.Position.IntegralY,
				Right = scissorArea.Position.IntegralX + scissorArea.Size.IntegralWidth,
				Bottom = scissorArea.Position.IntegralY + scissorArea.Size.IntegralHeight
			};

			Context.RSSetScissorRects(1, &area);
		}

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="vertexCount">The number of vertices that should be drawn.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		public void Draw(int vertexCount, int vertexOffset)
		{
			Context.Draw(vertexCount, vertexOffset);
		}

		/// <summary>
		///     Draws primitiveCount-many instanced primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="instanceCount">The number of instanced that should be drawn.</param>
		/// <param name="vertexCount">The number of vertices that should be drawn per instance.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		public void DrawInstanced(int instanceCount, int vertexCount, int vertexOffset, int instanceOffset)
		{
			Context.DrawInstanced(vertexCount, instanceCount, vertexOffset, instanceOffset);
		}

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		public void DrawIndexed(int indexCount, int indexOffset, int vertexOffset)
		{
			Context.DrawIndexed(indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///     Draws indexCount-many instanced indices, starting at the given index offset into the currently bound index buffer.
		/// </summary>
		/// <param name="instanceCount">The number of instances to draw.</param>
		/// <param name="indexCount">The number of indices to draw per instance.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The offset applied to the non-instanced vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		public void DrawIndexedInstanced(int instanceCount, int indexCount, int indexOffset, int vertexOffset, int instanceOffset)
		{
			Context.DrawIndexedInstanced(indexCount, instanceCount, indexOffset, vertexOffset, instanceOffset);
		}

		[DllImport("d3d11.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern unsafe D3D11Result D3D11CreateDevice(DXGIAdapter adapter, D3D11DriverType driverType, void* software,
																   D3D11DeviceCreationFlags flags, D3D11FeatureLevel* featureLevels,
																   int featureLevelCount, uint sdkVersion, D3D11Device* device,
																   D3D11FeatureLevel* featureLevel, D3D11DeviceContext* immediateContext);

		[DllImport("dxgi.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern unsafe D3D11Result CreateDXGIFactory(void* guid, DXGIFactory* factory);

		[DllImport("dxgidebug.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern unsafe D3D11Result DXGIGetDebugInterface(void* guid, DXGIDebug* factory);

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override unsafe void OnDisposing()
		{
			if (Context.IsInitialized)
			{
				Context.ClearState();
				Context.Flush();
				Context.Release();
			}

			Device.Release();
			_adapter.Release();

#if DEBUG
			var debugId = new Guid("119E7452-DE9E-40fe-8806-88F90C12B441");
			var debugAll = new Guid(0xe48ae283, 0xda80, 0x490b, 0x87, 0xe6, 0x43, 0xe9, 0xa9, 0xcf, 0xda, 0x8);

			DXGIDebug debug;
			DXGIGetDebugInterface(&debugId, &debug).CheckSuccess("Failed to get the Direct3D 11 debug interface.");

			debug.ReportLiveObjects(debugAll, D3D11ReportingLevel.Detail | D3D11ReportingLevel.Summary)
				 .CheckSuccess("Direct3D11 live object reporting failed.");

			debug.Release();
#endif

			Factory.Release();
		}

		/// <summary>
		///     Gets a string representation of the given feature level.
		/// </summary>
		/// <param name="featureLevel">The feature level that should be represented by the returned string.</param>
		private static string FeatureLevelToString(D3D11FeatureLevel featureLevel)
		{
			switch (featureLevel)
			{
				case D3D11FeatureLevel.Level_9_1:
					return "9.1";
				case D3D11FeatureLevel.Level_9_2:
					return "9.2";
				case D3D11FeatureLevel.Level_9_3:
					return "9.3";
				case D3D11FeatureLevel.Level_10_0:
					return "10.0";
				case D3D11FeatureLevel.Level_10_1:
					return "10.1";
				case D3D11FeatureLevel.Level_11_0:
					return "11.0";
				case D3D11FeatureLevel.Level_11_1:
					return "11.1";
				default:
					return "> 11.1";
			}
		}
	}
}