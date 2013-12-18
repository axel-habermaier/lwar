﻿namespace Pegasus.Framework.UserInterface.Controls
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
			if (RenderOutput != null)
			{
				RenderOutput.Camera.SafeDispose();
				RenderOutput.RenderTarget.SafeDispose();
			}

			OutputTexture.SafeDispose();
			RenderOutput.SafeDispose();

			OutputTexture = null;
			RenderOutput = null;
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

			spriteBatch.Draw(new Rectangle(x, y, width, height), OutputTexture);
		}
	}
}