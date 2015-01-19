namespace Pegasus.Platform.Graphics.Interface
{
	using System;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents the target of a rendering operation.
	/// </summary>
	internal interface IRenderTarget : IDisposable
	{
		/// <summary>
		///     Gets or sets the size of the render target.
		/// </summary>
		Size Size { get; set; }

		/// <summary>
		///     Gets a value indicating whether the render target is the back buffer of a swap chain.
		/// </summary>
		bool IsBackBuffer { get; }

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		void ClearColor(Color color);

		/// <summary>
		///     Clears the depth stencil buffer of the render target.
		/// </summary>
		/// <param name="clearDepth">Indicates whether the depth buffer should be cleared.</param>
		/// <param name="clearStencil">Indicates whether the stencil buffer should be cleared.</param>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		void ClearDepthStencil(bool clearDepth, bool clearStencil, float depth, byte stencil);

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		void Bind();

		/// <summary>
		///     Sets the debug name of the render target.
		/// </summary>
		/// <param name="name">The debug name of the render target.</param>
		void SetName(string name);
	}
}