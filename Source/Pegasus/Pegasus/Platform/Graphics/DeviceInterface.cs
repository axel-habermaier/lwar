namespace Pegasus.Platform.Graphics
{
	using System;
	using Rendering;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Represents the native device interface.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal unsafe interface IDeviceInterface
	{
		#region Drawing

		void Draw(int vertexCount, int vertexOffset);
		void DrawIndexed(int indexCount, int indexOffset, int vertexOffset);
		void DrawInstanced(int instanceCount, int vertexCount, int vertexOffset, int instanceOffset);
		void DrawIndexedInstanced(int instanceCount, int indexCount, int indexOffset, int vertexOffset, int instanceOffset);

		#endregion

		#region Buffer

		Buffer InitializeBuffer(BufferDescription* description);
		void FreeBuffer(Buffer buffer);

		void SetBufferName(Buffer buffer, string name);

		void* MapBuffer(Buffer buffer, MapMode mode);
		void* MapBufferRange(Buffer buffer, MapMode mode, int offset, int byteCount);
		void UnmapBuffer(Buffer buffer);

		void BindBuffer(Buffer buffer, int slot);
		void CopyBuffer(Buffer buffer, void* data);

		#endregion

		#region Pipeline States

		void SetPrimitiveType(PrimitiveType primitiveType);
		void SetViewport(int left, int top, int width, int height);
		void SetScissorArea(int left, int top, int width, int height);

		BlendState InitializeBlendState(BlendDescription* description);
		void FreeBlendState(BlendState state);

		void SetBlendStateName(BlendState state, string name);
		void BindBlendState(BlendState state);

		DepthStencilState InitializeDepthStencilState(DepthStencilDescription* description);
		void FreeDepthStencilState(DepthStencilState state);

		void SetDepthStencilStateName(DepthStencilState state, string name);
		void BindDepthStencilState(DepthStencilState state);

		RasterizerState InitializeRasterizerState(RasterizerDescription* description);
		void FreeRasterizerState(RasterizerState state);

		void SetRasterizerStateName(RasterizerState state, string name);
		void BindRasterizerState(RasterizerState state);

		SamplerState InitializeSamplerState(SamplerDescription* description);
		void FreeSamplerState(SamplerState state);

		void SetSamplerStateName(SamplerState state, string name);
		void BindSamplerState(SamplerState state, int slot);

		#endregion

		#region Query 

		Query InitializeQuery(QueryType queryType);
		void FreeQuery(Query query);

		void BeginQuery(Query query);
		void EndQuery(Query query);

		bool IsQueryCompleted(Query query);
		void GetQueryData(Query query, void* data);

		void SetQueryName(Query query, string name);

		#endregion

		#region Texture

		Texture InitializeTexture(TextureDescription* description, Surface* surfaces);
		void FreeTexture(Texture texture);

		void SetTextureName(Texture texture, string name);

		void BindTexture(Texture texture, int slot);
		void UnbindTexture(Texture texture, int slot);

		void GenerateMipmaps(Texture texture);

		#endregion

		#region RenderTarget

		RenderTarget InitializeRenderTarget(Texture[] colorBuffers, int colorBufferCount, Texture depthStencil, int width, int height);
		void FreeRenderTarget(RenderTarget renderTarget);

		void ClearColor(RenderTarget renderTarget, Color* color);
		void ClearDepthStencil(RenderTarget renderTarget, bool clearDepth, bool clearStencil, float depth, byte stencil);

		void BindRenderTarget(RenderTarget renderTarget);
		void SetRenderTargetName(RenderTarget renderTarget, string name);

		void GetRenderTargetSize(RenderTarget renderTarget, int* width, int* height);

		#endregion

		#region Shader

		Shader InitializeShader(ShaderType shaderType, byte* shaderCode, int sizeInBytes);
		void FreeShader(Shader shader);

		void SetShaderName(Shader shader, string name);

		ShaderProgram InitializeShaderProgram(Shader vertexShader, Shader fragmentShader);
		void FreeShaderProgram(ShaderProgram shaderProgram);

		void BindShaderProgram(ShaderProgram shaderProgram);

		#endregion

		#region SwapChain

		SwapChain InitializeSwapChain(NativeWindow window);
		void FreeSwapChain(SwapChain swapChain);

		void PresentSwapChain(SwapChain swapChain);
		void ResizeSwapChain(SwapChain swapChain, int width, int height);

		RenderTarget GetBackBuffer(SwapChain swapChain);

		#endregion

		#region VertexLayout

		VertexLayout InitializeVertexLayout(VertexLayoutDescription* description);
		void FreeVertexLayout(VertexLayout vertexLayout);

		void BindVertexLayout(VertexLayout vertexLayout);

		#endregion

		#region Other

		GraphicsApi GraphicsApi { get; set; }
		void PrintDeviceInfo();

		#endregion
	}
}