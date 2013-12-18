namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Provides a render output for arbitrary 2D or 3D drawing. The final image is then drawn into the UI.
	/// </summary>
	public class RenderOutputPanel : Decorator
	{
		/// <summary>
		///     Gets or sets the render output that is used to draw the 2D or 3D elements.
		/// </summary>
		public RenderOutput RenderOutput { get; set; }

		/// <summary>
		///     Gets or sets the color buffer of the render output's render target that should be drawn into the UI.
		/// </summary>
		public Texture2D OutputTexture { get; set; }

		/// <summary>
		///     Gets a value indicating whether the area of the render output is non-zero.
		/// </summary>
		private bool HasVisibleArea
		{
			get { return !MathUtils.Equals(ActualWidth, 0) && !MathUtils.Equals(ActualHeight, 0); }
		}

		/// <summary>
		///     Raised when the render output has to be (re-)initialized, i.e., when the panel is attached to a logical tree or when its
		///     size changes.
		/// </summary>
		public event Action<Size> InitializeRenderOutput;

		/// <summary>
		///     Raised when the render output has to be disposed, i.e., when the panel is detached from a logical tree.
		/// </summary>
		public event Action DisposeRenderOutput;

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
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			return base.MeasureCore(availableSize);
		}

		/// <summary>
		///     Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///     element. If this value is smaller than the given size, the UI element's alignment properties position it
		///     appropriately.
		/// </summary>
		/// <param name="finalSize">
		///     The final area allocated by the UI element's parent that the UI element should use to arrange
		///     itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			return base.ArrangeCore(finalSize);
		}

		/// <summary>
		///     Initializes the render output and texture properties.
		/// </summary>
		private void Initialize()
		{
			if (!IsAttachedToRoot || InitializeRenderOutput == null)
				return;

			// No need to initialize the render output if its area is 0
			if (!HasVisibleArea)
				return;

			var size = new Size((int)Math.Round(ActualWidth), (int)Math.Round(ActualHeight));
			InitializeRenderOutput(size);

			Assert.NotNull(RenderOutput, "The render output has not been initialized.");
			Assert.NotNull(OutputTexture, "The output texture has not been initialized.");
			Assert.That(OutputTexture.Format == SurfaceFormat.Rgba8, "Invalid output texture format.");
		}

		/// <summary>
		///     Dispose the render output and the texture instances.
		/// </summary>
		private void Dispose()
		{
			if (DisposeRenderOutput != null)
				DisposeRenderOutput();

			Assert.NullOrDisposed(RenderOutput);
			Assert.NullOrDisposed(OutputTexture);
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			if (!HasVisibleArea)
				return;

			Assert.NotNull(OutputTexture, "The output texture has not been initialized.");

			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			// Take the different coordinates origins for OpenGL and Direct3D into account when rendering 
			// the render target's color buffer... annoying
#if Direct3D11
			var textureArea = new RectangleF(0, 1, 1, -1);
#elif OpenGL3
			var textureArea = new RectangleF(0, 0, 1, 1);
#endif

			var quad = new Quad(new RectangleF(x, y, width, height), Color.White, textureArea);
			spriteBatch.Draw(ref quad, OutputTexture);
		}
	}
}