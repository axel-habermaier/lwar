namespace Pegasus.Platform.Graphics.Interface
{
	using System;
	using Math;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	internal interface ISwapChain : IDisposable
	{
		/// <summary>
		///     Gets the back buffer of the swap chain.
		/// </summary>
		IRenderTarget BackBuffer { get; }

		/// <summary>
		///     Presents the back buffer to the screen.
		/// </summary>
		void Present();

		/// <summary>
		///     Resizes the swap chain to the given size.
		/// </summary>
		/// <param name="size">The new size of the swap chain.</param>
		void Resize(Size size);
	}
}