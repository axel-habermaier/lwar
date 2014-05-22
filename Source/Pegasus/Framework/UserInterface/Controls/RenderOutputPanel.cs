namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Rendering;

	/// <summary>
	///     Provides a render output for arbitrary 2D or 3D drawing. The final image is then drawn into the UI.
	/// </summary>
	public sealed class RenderOutputPanel : Decorator
	{
		/// <summary>
		///     The size callback of the render output panel.
		/// </summary>
		public static readonly DependencyProperty<Action<Size>> SizeCallbackProperty = new DependencyProperty<Action<Size>>();

		/// <summary>
		///     The texture that contains the render output panel's contents.
		/// </summary>
		public static readonly DependencyProperty<Texture2D> Texture2DProperty = new DependencyProperty<Texture2D>();

		/// <summary>
		///     The callback that is invoked when the contents of the texture should be updated.
		/// </summary>
		public static readonly DependencyProperty<Action> DrawCallbackProperty = new DependencyProperty<Action>();

		/// <summary>
		///     Gets or sets the size callback of the render output panel.
		/// </summary>
		public Action<Size> SizeCallback
		{
			get { return GetValue(SizeCallbackProperty); }
			set { SetValue(SizeCallbackProperty, value); }
		}

		/// <summary>
		///     Gets or sets the texture that contains the render output panel's contents.
		/// </summary>
		public Texture2D Texture2D
		{
			get { return GetValue(Texture2DProperty); }
			set { SetValue(Texture2DProperty, value); }
		}

		/// <summary>
		///     Gets or sets the callback that is invoked when the contents of the texture should be updated.
		/// </summary>
		public Action DrawCallback
		{
			get { return GetValue(DrawCallbackProperty); }
			set { SetValue(DrawCallbackProperty, value); }
		}

		/// <summary>
		///     Gets a value indicating whether the area of the render output is non-zero.
		/// </summary>
		private bool HasVisibleArea
		{
			get { return !MathUtils.Equals(ActualWidth, 0) && !MathUtils.Equals(ActualHeight, 0); }
		}

		/// <summary>
		///     Invoked when the size of the UI element has changed.
		/// </summary>
		/// <param name="oldSize">The old size of the UI element.</param>
		/// <param name="newSize">The new size of the UI element.</param>
		protected override void OnSizeChanged(SizeD oldSize, SizeD newSize)
		{
			base.OnSizeChanged(oldSize, newSize);

			if (SizeCallback != null)
				SizeCallback(new Size((int)Math.Round(newSize.Width), (int)Math.Round(newSize.Height)));
		}

		///// <summary>
		/////     Initializes the render output and texture properties.
		///// </summary>
		//private void Initialize()
		//{
		//	if (!IsAttachedToRoot)
		//		return;

		//	// No need to initialize the render output if its area is 0
		//	if (!HasVisibleArea)
		//		return;

		//	var size = new Size((int)Math.Round(ActualWidth), (int)Math.Round(ActualHeight));
		//	InitializeCore(size, out _renderOutput, out _outputTexture);

		//	Assert.NotNull(_renderOutput, "The render output has not been initialized.");
		//	Assert.NotNull(_outputTexture, "The output texture has not been initialized.");
		//	Assert.That(_outputTexture.Format == SurfaceFormat.Rgba8, "Invalid output texture format.");
		//}

		///// <summary>
		/////     Initializes the render output panel's graphics objects.
		///// </summary>
		///// <param name="panelSize">The size of the render output panel.</param>
		///// <param name="renderOutput">The render output that has been initialized for this render output panel.</param>
		///// <param name="outputTexture">The output texture that contains the contents of this render output panel.</param>
		//protected virtual void InitializeCore(Size panelSize, out RenderOutput renderOutput, out Texture2D outputTexture)
		//{
		//	// Initialize the color buffer of the render target
		//	var colorBuffer = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
		//	colorBuffer.SetName("RenderOutputPanel.ColorBuffer");

		//	// Initialize the depth stencil buffer of the render target
		//	var depthStencil = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
		//	depthStencil.SetName("RenderOutputPanel.DepthStencil");

		//	// Initialize the render output panel's graphics properties
		//	outputTexture = colorBuffer;
		//	renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
		//	{
		//		RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, depthStencil, colorBuffer),
		//		Viewport = new Rectangle(0, 0, panelSize)
		//	};
		//}

		///// <summary>
		/////     Dispose the render output and the texture instances.
		///// </summary>
		//private void Dispose()
		//{
		//	DisposeCore(_renderOutput, _outputTexture);

		//	Assert.NullOrDisposed(_renderOutput);
		//	Assert.NullOrDisposed(_outputTexture);

		//	_renderOutput = null;
		//	_outputTexture = null;
		//}

		///// <summary>
		/////     Disposes the render output panel's graphics objects.
		///// </summary>
		///// <param name="renderOutput">The render output that should be disposed.</param>
		///// <param name="outputTexture">The output texture that should be disposed.</param>
		//protected virtual void DisposeCore(RenderOutput renderOutput, Texture2D outputTexture)
		//{
		//	if (renderOutput != null)
		//		renderOutput.RenderTarget.SafeDispose();

		//	outputTexture.SafeDispose();
		//	renderOutput.SafeDispose();
		//}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			Log.DebugIf(Texture2D == null, "No texture has been set for the render output panel.");
			Log.DebugIf(DrawCallback == null, "No draw callback has been set for the render output panel.");

			if (!HasVisibleArea || Texture2D == null || DrawCallback == null)
				return;

			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			// Take the different coordinate origins for OpenGL and Direct3D into account when rendering 
			// the render target's color buffer... annoying
#if Direct3D11
			var textureArea = new RectangleF(0, 1, 1, -1);
#elif OpenGL3
			var textureArea = new RectangleF(0, 0, 1, 1);
#endif

			// Update the contents of the texture
			DrawCallback();

			// Draw the contents into the UI
			var quad = new Quad(new RectangleF(x, y, width, height), Color.White, textureArea);
			spriteBatch.Draw(ref quad, Texture2D);
		}
	}
}