namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Provides rendering statistics.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct GraphicsDeviceStatistics
	{
		/// <summary>
		///   The number of draw calls that have been made.
		/// </summary>
		public int DrawCalls;

		/// <summary>
		///   The number of vertices that have been drawn.
		/// </summary>
		public int VertexCount;

		/// <summary>
		///   The number of render target binding changes that have been made.
		/// </summary>
		public int RenderTargetBindingCount;

		/// <summary>
		///   The number of texture binding changes that have been made.
		/// </summary>
		public int TextureBindingCount;

		/// <summary>
		///   The number of constant buffer binding changes that have been made.
		/// </summary>
		public int ConstantBufferBindingCount;

		/// <summary>
		///   The number of constant buffer updates that have been made.
		/// </summary>
		public int ConstantBufferUpdates;

		/// <summary>
		///   The number of buffer mapping operations that have been made.
		/// </summary>
		public int BufferMapCount;

		/// <summary>
		///   The number of shader binding changes that have been made.
		/// </summary>
		public int ShaderBindingCount;

		/// <summary>
		///   The number of input layout binding changes that have been made.
		/// </summary>
		public int InputLayoutBindingCount;

		/// <summary>
		///   The number of blend state binding changes that have been made.
		/// </summary>
		public int BlendStateBindingCount;

		/// <summary>
		///   The number of depth stencil state binding changes that have been made.
		/// </summary>
		public int DepthStencilStateBindingCount;

		/// <summary>
		///   The number of sampler staste binding changes that have been made.
		/// </summary>
		public int SamplerStateBindingCount;

		/// <summary>
		///   The number of rasterizer state binding changes that have been made.
		/// </summary>
		public int RasterizerStateBindingCount;
	}
}