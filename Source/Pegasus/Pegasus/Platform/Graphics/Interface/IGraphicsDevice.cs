namespace Pegasus.Platform.Graphics.Interface
{
	using System;
	using Math;
	using UserInterface;

	/// <summary>
	///     Represents a graphics device.
	/// </summary>
	internal interface IGraphicsDevice : IDisposable
	{
		/// <summary>
		///     Creates a buffer object.
		/// </summary>
		/// <param name="description">The description of the buffer.</param>
		IBuffer CreateBuffer(ref BufferDescription description);

		/// <summary>
		///     Creates a blend state.
		/// </summary>
		/// <param name="description">The description of the blend state.</param>
		IBlendState CreateBlendState(ref BlendDescription description);

		/// <summary>
		///     Creates a depth stencil state.
		/// </summary>
		/// <param name="description">The description of the depth stencil state.</param>
		IDepthStencilState CreateDepthStencilState(ref DepthStencilDescription description);

		/// <summary>
		///     Creates a rasterizer state.
		/// </summary>
		/// <param name="description">The description of the rasterizer state.</param>
		IRasterizerState CreateRasterizerState(ref RasterizerDescription description);

		/// <summary>
		///     Creates a sampler state.
		/// </summary>
		/// <param name="description">The description of the sampler state.</param>
		ISamplerState CreateSamplerState(ref SamplerDescription description);

		/// <summary>
		///     Creates a texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		ITexture CreateTexture(ref TextureDescription description, Surface[] surfaces);

		/// <summary>
		///     Creates a query.
		/// </summary>
		/// <param name="queryType">The type of the query.</param>
		IQuery CreateQuery(QueryType queryType);

		/// <summary>
		///     Creates a shader.
		/// </summary>
		/// <param name="shaderType">The type of the shader.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		IShader CreateShader(ShaderType shaderType, IntPtr byteCode, int byteCodeLength);

		/// <summary>
		///     Creates a shader program.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		IShaderProgram CreateShaderProgram(VertexShader vertexShader, FragmentShader fragmentShader);

		/// <summary>
		///     Creates a vertex layout.
		/// </summary>
		/// <param name="description">The description of the vertex layout.</param>
		IVertexLayout CreateVertexLayout(ref VertexLayoutDescription description);

		/// <summary>
		///     Creates a render target.
		/// </summary>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		IRenderTarget CreateRenderTarget(Texture2D depthStencil, Texture2D[] colorBuffers);

		/// <summary>
		///     Creates a swap chain.
		/// </summary>
		/// <param name="window">The window the swap chain should be created for..</param>
		ISwapChain CreateSwapChain(NativeWindow window);

		/// <summary>
		///     Prints information about the graphics device.
		/// </summary>
		void PrintInfo();

		/// <summary>
		///     Changes the primitive type that is used for drawing.
		/// </summary>
		/// <param name="primitiveType">The primitive type that should be used for drawing.</param>
		void ChangePrimitiveType(PrimitiveType primitiveType);

		/// <summary>
		///     Changes the viewport that is drawn to.
		/// </summary>
		/// <param name="viewport">The viewport that should be drawn to.</param>
		void ChangeViewport(ref Rectangle viewport);

		/// <summary>
		///     Changes the scissor area that is used when the scissor test is enabled.
		/// </summary>
		/// <param name="scissorArea">The scissor area that should be used by the scissor test.</param>
		void ChangeScissorArea(ref Rectangle scissorArea);

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="vertexCount">The number of vertices that should be drawn.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		void Draw(int vertexCount, int vertexOffset);

		/// <summary>
		///     Draws primitiveCount-many instanced primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="instanceCount">The number of instanced that should be drawn.</param>
		/// <param name="vertexCount">The number of vertices that should be drawn per instance.</param>
		/// <param name="vertexOffset">The offset into the vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		void DrawInstanced(int instanceCount, int vertexCount, int vertexOffset, int instanceOffset);

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		void DrawIndexed(int indexCount, int indexOffset, int vertexOffset);

		/// <summary>
		///     Draws indexCount-many instanced indices, starting at the given index offset into the currently bound index buffer.
		/// </summary>
		/// <param name="instanceCount">The number of instances to draw.</param>
		/// <param name="indexCount">The number of indices to draw per instance.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The offset applied to the non-instanced vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		void DrawIndexedInstanced(int instanceCount, int indexCount, int indexOffset, int vertexOffset, int instanceOffset);
	}
}