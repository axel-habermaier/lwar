namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Provides a render output for arbitrary 2D or 3D drawing. The final image is then drawn into the UI.
	/// </summary>
	public class RenderOutputPanel : Decorator
	{
		/// <summary>
		///     The color buffer of the render output's render target that should be drawn into the UI.
		/// </summary>
		private Texture2D _outputTexture;

		/// <summary>
		///     The render output that is used to draw the 2D or 3D elements.
		/// </summary>
		private RenderOutput _renderOutput;

		/// <summary>
		///     Gets a value indicating whether the area of the render output is non-zero.
		/// </summary>
		private bool HasVisibleArea
		{
			get { return !MathUtils.Equals(ActualWidth, 0) && !MathUtils.Equals(ActualHeight, 0); }
		}

		/// <summary>
		///     Invoked when the UI element is now (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnAttachedToRoot()
		{
			base.OnAttached();
			Initialize();
		}

		/// <summary>
		///     Invoked when the UI element is no longer (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnDetachedFromRoot()
		{
			base.OnDetached();
			Dispose();
		}

		/// <summary>
		///     Invoked when the size of the UI element has changed.
		/// </summary>
		/// <param name="oldSize">The old size of the UI element.</param>
		/// <param name="newSize">The new size of the UI element.</param>
		protected override void OnSizeChanged(SizeD oldSize, SizeD newSize)
		{
			base.OnSizeChanged(oldSize, newSize);

			Dispose();
			Initialize();
		}

		/// <summary>
		///     Initializes the render output and texture properties.
		/// </summary>
		private void Initialize()
		{
			if (!IsAttachedToRoot)
				return;

			// No need to initialize the render output if its area is 0
			if (!HasVisibleArea)
				return;

			var size = new Size((int)Math.Round(ActualWidth), (int)Math.Round(ActualHeight));
			InitializeCore(size, out _renderOutput, out _outputTexture);

			Assert.NotNull(_renderOutput, "The render output has not been initialized.");
			Assert.NotNull(_outputTexture, "The output texture has not been initialized.");
			Assert.That(_outputTexture.Format == SurfaceFormat.Rgba8, "Invalid output texture format.");
		}

		/// <summary>
		///     Initializes the render output panel's graphics objects.
		/// </summary>
		/// <param name="panelSize">The size of the render output panel.</param>
		/// <param name="renderOutput">The render output that has been initialized for this render output panel.</param>
		/// <param name="outputTexture">The output texture that contains the contents of this render output panel.</param>
		protected virtual void InitializeCore(Size panelSize, out RenderOutput renderOutput, out Texture2D outputTexture)
		{
			// Initialize the color buffer of the render target
			var colorBuffer = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			colorBuffer.SetName("RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			var depthStencil = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
			depthStencil.SetName("RenderOutputPanel.DepthStencil");

			// Initialize the render output panel's graphics properties
			outputTexture = colorBuffer;
			renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			{
				RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, depthStencil, colorBuffer),
				Viewport = new Rectangle(0, 0, panelSize)
			};
		}

		/// <summary>
		///     Dispose the render output and the texture instances.
		/// </summary>
		private void Dispose()
		{
			DisposeCore(_renderOutput, _outputTexture);

			Assert.NullOrDisposed(_renderOutput);
			Assert.NullOrDisposed(_outputTexture);

			_renderOutput = null;
			_outputTexture = null;
		}

		/// <summary>
		///     Disposes the render output panel's graphics objects.
		/// </summary>
		/// <param name="renderOutput">The render output that should be disposed.</param>
		/// <param name="outputTexture">The output texture that should be disposed.</param>
		protected virtual void DisposeCore(RenderOutput renderOutput, Texture2D outputTexture)
		{
			if (renderOutput != null)
				renderOutput.RenderTarget.SafeDispose();

			outputTexture.SafeDispose();
			renderOutput.SafeDispose();
		}

		/// <summary>
		///     Draws the contents of the panel into the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the content should be drawn into.</param>
		//protected abstract void Draw(RenderOutput renderOutput);

		protected override sealed void OnDraw(SpriteBatch spriteBatch)
		{
			if (!HasVisibleArea)
				return;

			Assert.NotNull(_outputTexture, "The output texture has not been initialized.");

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

			// Draw the contents of the render output panel into the output texture
			//Draw(_renderOutput);

			// Draw the contents into the UI
			var quad = new Quad(new RectangleF(x, y, width, height), Color.White, textureArea);
			spriteBatch.Draw(ref quad, _outputTexture);
		}
	}
}