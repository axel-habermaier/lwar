namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;
	using Logging;
	using Math;
	using Memory;
	using Rendering;
	using UserInterface;

	/// <summary>
	///     Represents an OpenGL3-based graphics device.
	/// </summary>
	internal unsafe class GraphicsDeviceGL3 : DisposableObject, IGraphicsDevice
	{
		/// <summary>
		///     Provides access to the OpenGL entry points.
		/// </summary>
		// ReSharper disable once InconsistentNaming
		internal readonly EntryPoints _gl;

		/// <summary>
		///     Caches some of the state of the OpenGL state machine.
		/// </summary>
		internal GLState State;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public GraphicsDeviceGL3()
		{
			Context = new GLContext();
			State = new GLState();

			_gl = Context.EntryPoints;
			_gl.Viewport(0, 0, 0, 0);
			_gl.Scissor(0, 0, 0, 0);
		}

		/// <summary>
		///     Provides access to the OpenGL context.
		/// </summary>
		internal GLContext Context { get; private set; }

		/// <summary>
		///     Creates a buffer object.
		/// </summary>
		/// <param name="description">The description of the buffer.</param>
		public IBuffer CreateBuffer(ref BufferDescription description)
		{
			return new BufferGL3(this, ref description);
		}

		/// <summary>
		///     Creates a blend state.
		/// </summary>
		/// <param name="description">The description of the blend state.</param>
		public IBlendState CreateBlendState(ref BlendDescription description)
		{
			return new BlendStateGL3(this, ref description);
		}

		/// <summary>
		///     Creates a depth stencil state.
		/// </summary>
		/// <param name="description">The description of the depth stencil state.</param>
		public IDepthStencilState CreateDepthStencilState(ref DepthStencilDescription description)
		{
			return new DepthStencilStateGL3(this, ref description);
		}

		/// <summary>
		///     Creates a rasterizer state.
		/// </summary>
		/// <param name="description">The description of the rasterizer state.</param>
		public IRasterizerState CreateRasterizerState(ref RasterizerDescription description)
		{
			return new RasterizerStateGL3(this, ref description);
		}

		/// <summary>
		///     Creates a sampler state.
		/// </summary>
		/// <param name="description">The description of the sampler state.</param>
		public ISamplerState CreateSamplerState(ref SamplerDescription description)
		{
			return new SamplerStateGL3(this, ref description);
		}

		/// <summary>
		///     Creates a texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		public ITexture CreateTexture(ref TextureDescription description, Surface[] surfaces)
		{
			return new TextureGL3(this, ref description, surfaces);
		}

		/// <summary>
		///     Creates a query.
		/// </summary>
		/// <param name="queryType">The type of the query.</param>
		public IQuery CreateQuery(QueryType queryType)
		{
			switch (queryType)
			{
				case QueryType.Synced:
					return new SyncedQueryGL3(this);
				case QueryType.Timestamp:
					return new TimestampQueryGL3(this);
				case QueryType.TimestampDisjoint:
					return new TimestampDisjointQueryGL3(this);
				default:
					throw new ArgumentOutOfRangeException("queryType");
			}
		}

		/// <summary>
		///     Creates a shader.
		/// </summary>
		/// <param name="shaderType">The type of the shader.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		public IShader CreateShader(ShaderType shaderType, IntPtr byteCode, int byteCodeLength)
		{
			return new ShaderGL3(this, shaderType, byteCode, byteCodeLength);
		}

		/// <summary>
		///     Creates a shader program.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		public IShaderProgram CreateShaderProgram(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			return new ShaderProgramGL3(this, vertexShader, fragmentShader);
		}

		/// <summary>
		///     Creates a vertex layout.
		/// </summary>
		/// <param name="description">The description of the vertex layout.</param>
		public IVertexLayout CreateVertexLayout(ref VertexLayoutDescription description)
		{
			return new VertexLayoutGL3(this, ref description);
		}

		/// <summary>
		///     Creates a render target.
		/// </summary>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public IRenderTarget CreateRenderTarget(Texture2D depthStencil, Texture2D[] colorBuffers)
		{
			return new RenderTargetGL3(this, depthStencil, colorBuffers);
		}

		/// <summary>
		///     Creates a swap chain.
		/// </summary>
		/// <param name="window">The window the swap chain should be created for..</param>
		public ISwapChain CreateSwapChain(NativeWindow window)
		{
			return new SwapChainGL3(this, window);
		}

		/// <summary>
		///     Prints information about the graphics device.
		/// </summary>
		public void PrintInfo()
		{
			Func<uint, string> getString = info => Marshal.PtrToStringAnsi(new IntPtr(_gl.GetString(info)));

			Log.Info("OpenGL renderer: {0} ({1})", getString(GL.Renderer), getString(GL.Vendor));
			Log.Info("OpenGL version: {0}", getString(GL.Version));
			Log.Info("OpenGL GLSL version: {0}", getString(GL.ShadingLanguageVersion));
		}

		/// <summary>
		///     Changes the primitive type that is used for drawing.
		/// </summary>
		/// <param name="primitiveType">The primitive type that should be used for drawing.</param>
		public void ChangePrimitiveType(PrimitiveType primitiveType)
		{
			State.PrimitiveType = primitiveType.Map();
		}

		/// <summary>
		///     Changes the viewport that is drawn to.
		/// </summary>
		/// <param name="viewport">The viewport that should be drawn to.</param>
		public void ChangeViewport(ref Rectangle viewport)
		{
			State.Viewport = viewport;
			var viewportGL = new Rectangle(viewport.Left, FlipY(viewport.Top, viewport.Height), viewport.Size);

			if (DeviceState.Change(ref State.ViewportGL, viewportGL))
				_gl.Viewport(viewportGL.Position.IntegralX, viewportGL.Position.IntegralY, viewportGL.Size.IntegralWidth, viewportGL.Size.IntegralHeight);
		}

		/// <summary>
		///     Changes the scissor area that is used when the scissor test is enabled.
		/// </summary>
		/// <param name="scissorArea">The scissor area that should be used by the scissor test.</param>
		public void ChangeScissorArea(ref Rectangle scissorArea)
		{
			State.ScissorArea = scissorArea;
			var areaGL = new Rectangle(scissorArea.Left, FlipY(scissorArea.Top, scissorArea.Height), scissorArea.Size);

			if (DeviceState.Change(ref State.ScissorAreaGL, areaGL))
				_gl.Scissor(areaGL.Position.IntegralX, areaGL.Position.IntegralY, areaGL.Size.IntegralWidth, areaGL.Size.IntegralHeight);
		}

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="vertexCount">The number of vertices that should be drawn.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		public void Draw(int vertexCount, int vertexOffset)
		{
			_gl.BindVertexArray(State.VertexLayout.NativeObject);
			_gl.DrawArrays(State.PrimitiveType, vertexOffset, vertexCount);
			_gl.BindVertexArray(0);
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
			_gl.BindVertexArray(State.VertexLayout.NativeObject);
			_gl.DrawArraysInstancedBaseInstance(State.PrimitiveType, vertexOffset, vertexCount, instanceCount, (uint)instanceOffset);
			_gl.BindVertexArray(0);
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
			var offset = (void*)((indexOffset + State.VertexLayout.IndexOffset) * State.VertexLayout.IndexSizeInBytes);
			_gl.BindVertexArray(State.VertexLayout.NativeObject);
			_gl.DrawElementsBaseVertex(State.PrimitiveType, indexCount, State.VertexLayout.IndexType, offset, vertexOffset);
			_gl.BindVertexArray(0);
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
			var offset = (void*)((indexOffset + State.VertexLayout.IndexOffset) * State.VertexLayout.IndexSizeInBytes);
			_gl.BindVertexArray(State.VertexLayout.NativeObject);
			_gl.DrawElementsInstancedBaseVertexBaseInstance(State.PrimitiveType, indexCount, State.VertexLayout.IndexType, offset,
				instanceCount, vertexOffset, (uint)instanceOffset);
			_gl.BindVertexArray(0);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Context.SafeDispose();
		}

		/// <summary>
		///     Flips the Y value of the rectangle, as OpenGL coordinates are upside-down compared to Direct3D.
		/// </summary>
		/// <param name="top">The original top value of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		private float FlipY(float top, float height)
		{
			if (State.RenderTarget == null)
				return Single.MaxValue;

			return State.RenderTarget.Size.Height - height - top;
		}

		/// <summary>
		///     Enables or disable an OpenGL capability.
		/// </summary>
		/// <param name="capability">The capability that should be enabled or disabled.</param>
		/// <param name="current">The current state of the capability.</param>
		/// <param name="enabled">The desired state of the capability.</param>
		private void Enable(uint capability, ref bool? current, bool enabled)
		{
			if (current == enabled)
				return;

			current = enabled;

			if (enabled)
				_gl.Enable(capability);
			else
				_gl.Disable(capability);
		}

		/// <summary>
		///     Enables or disables the scissor test.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableScissor(bool enabled)
		{
			Enable(GL.ScissorTest, ref State.ScissorEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables blending.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableBlend(bool enabled)
		{
			Enable(GL.Blend, ref State.BlendEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables the depth test.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableDepthTest(bool enabled)
		{
			Enable(GL.DepthTest, ref State.DepthTestEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables face culling.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableCullFace(bool enabled)
		{
			Enable(GL.CullFace, ref State.CullFaceEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables depth clamping.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableDepthClamp(bool enabled)
		{
			Enable(GL.DepthClamp, ref State.DepthClampEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables multi sample anti aliasing.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableMultisample(bool enabled)
		{
			Enable(GL.Multisample, ref State.MultisampleEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables anti aliasing of lines.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableAntialiasedLine(bool enabled)
		{
			Enable(GL.LineSmooth, ref State.AntialiasedLineEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables the stencil test.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableStencilTest(bool enabled)
		{
			Enable(GL.StencilTest, ref State.StencilTestEnabled, enabled);
		}

		/// <summary>
		///     Enables or disables depth writes.
		/// </summary>
		/// <param name="enabled">The desired state value.</param>
		internal void EnableDepthWrites(bool enabled)
		{
			if (!DeviceState.Change(ref State.DepthWritesEnabled, enabled))
				return;

			_gl.DepthMask(enabled);
		}

		/// <summary>
		///     Changes the active texture slot.
		/// </summary>
		/// <param name="slot">The texture slot that should be activated.</param>
		internal void ChangeActiveTexture(int slot)
		{
			if (!DeviceState.Change(ref State.ActiveTexture, (uint)(GL.Texture0 + slot)))
				return;

			_gl.ActiveTexture((uint)(GL.Texture0 + slot));
		}

		/// <summary>
		///     Sets the face that should be culled.
		/// </summary>
		/// <param name="cullFace">The face that should be culled.</param>
		internal void SetCullFace(uint cullFace)
		{
			if (!DeviceState.Change(ref State.CullFace, cullFace))
				return;

			_gl.CullFace(cullFace);
		}

		/// <summary>
		///     Determines which side of a face should be culled.
		/// </summary>
		/// <param name="frontFace">Determines which side of a face should be culled.</param>
		internal void SetFrontFace(uint frontFace)
		{
			if (!DeviceState.Change(ref State.FrontFace, frontFace))
				return;

			_gl.FrontFace(frontFace);
		}

		/// <summary>
		///     Sets the polygon mode.
		/// </summary>
		/// <param name="mode">The polygon mode.</param>
		internal void SetPolygonMode(uint mode)
		{
			if (!DeviceState.Change(ref State.PolygonMode, mode))
				return;

			_gl.PolygonMode(GL.FrontAndBack, mode);
		}

		/// <summary>
		///     Sets the polygon offset.
		/// </summary>
		internal void SetPolygonOffset(float slopeScaledDepthBias, float depthBiasClamp)
		{
			if (!DeviceState.Change(ref State.SlopeScaledDepthBias, slopeScaledDepthBias) &
				!DeviceState.Change(ref State.DepthBiasClamp, depthBiasClamp))
				return;

			_gl.PolygonOffset(slopeScaledDepthBias, depthBiasClamp);
		}

		/// <summary>
		///     Sets the depth function.
		/// </summary>
		/// <param name="func">The depth function that should be used.</param>
		internal void SetDepthFunc(uint func)
		{
			if (!DeviceState.Change(ref State.DepthFunc, func))
				return;

			_gl.DepthFunc(func);
		}

		/// <summary>
		///     Sets the clear color.
		/// </summary>
		/// <param name="color">The clear color that should be used.</param>
		internal void SetClearColor(Color color)
		{
			if (!DeviceState.Change(ref State.ClearColor, color))
				return;

			_gl.ClearColor(color.Red, color.Green, color.Blue, color.Alpha);
		}

		/// <summary>
		///     Sets the depth clear value.
		/// </summary>
		/// <param name="depth">The depth clear value that should be used.</param>
		internal void SetClearDepth(float depth)
		{
			if (!DeviceState.Change(ref State.DepthClear, depth))
				return;

			_gl.ClearDepth(depth);
		}

		/// <summary>
		///     Sets the stencil clear value.
		/// </summary>
		/// <param name="stencil">The stencil clear value that should be used.</param>
		internal void SetClearStencil(int stencil)
		{
			if (!DeviceState.Change(ref State.StencilClear, stencil))
				return;

			_gl.ClearStencil(stencil);
		}

		/// <summary>
		///     Sets the blend equation.
		/// </summary>
		/// <param name="blendOperation">The blend operation that should be used on the RGB channels.</param>
		/// <param name="blendOperationAlpha">The blend operation that should be used on the alpha channel.</param>
		internal void SetBlendEquation(uint blendOperation, uint blendOperationAlpha)
		{
			if (!DeviceState.Change(ref State.BlendOperation, blendOperation) &
				!DeviceState.Change(ref State.BlendOperationAlpha, blendOperationAlpha))
				return;

			_gl.BlendEquationSeparate(blendOperation, blendOperationAlpha);
		}

		/// <summary>
		///     Sets the blend functions.
		/// </summary>
		/// <param name="sourceBlend">The source blend function on the RGB channels.</param>
		/// <param name="destinationBlend">The destination blend function on the RGB channels.</param>
		/// <param name="sourceBlendAlpha">The source blend function on the alpha channel.</param>
		/// <param name="destinationBlendAlpha">The destination blend function on the alpha channel.</param>
		internal void SetBlendFuncs(uint sourceBlend, uint destinationBlend, uint sourceBlendAlpha, uint destinationBlendAlpha)
		{
			if (!DeviceState.Change(ref State.SourceBlend, sourceBlend) &
				!DeviceState.Change(ref State.DestinationBlend, destinationBlend) &
				!DeviceState.Change(ref State.SourceBlendAlpha, sourceBlendAlpha) &
				!DeviceState.Change(ref State.DestinationBlendAlpha, destinationBlendAlpha))
			{
				return;
			}

			_gl.BlendFuncSeparate(sourceBlend, destinationBlend, sourceBlendAlpha, destinationBlendAlpha);
		}

		/// <summary>
		///     Sets the color write mask.
		/// </summary>
		/// <param name="red">Indicates whether the red channel should be written to.</param>
		/// <param name="green">Indicates whether the green channel should be written to.</param>
		/// <param name="blue">Indicates whether the blue channel should be written to.</param>
		/// <param name="alpha">Indicates whether the alpha channel should be written to.</param>
		internal void SetColorMask(bool red, bool green, bool blue, bool alpha)
		{
			if (!DeviceState.Change(ref State.ColorMaskRed, red) &
				!DeviceState.Change(ref State.ColorMaskGreen, green) &
				!DeviceState.Change(ref State.ColorMaskBlue, blue) &
				!DeviceState.Change(ref State.ColorMaskAlpha, alpha))
			{
				return;
			}

			_gl.ColorMask(red, green, blue, alpha);
		}
	}
}