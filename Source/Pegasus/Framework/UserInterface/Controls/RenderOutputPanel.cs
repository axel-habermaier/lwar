namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Provides a render output for arbitrary 2D or 3D drawing. The final image is then drawn into the UI.
	/// </summary>
	public sealed class RenderOutputPanel : Decorator
	{
		/// <summary>
		///     A value indicating whether the render output panel has a depth stencil buffer.
		/// </summary>
		public static readonly DependencyProperty<bool> HasDepthStencilProperty = new DependencyProperty<bool>();

		/// <summary>
		///     The surface format of the render output panel's depth stencil buffer.
		/// </summary>
		public static readonly DependencyProperty<SurfaceFormat> DepthStencilFormatProperty =
			new DependencyProperty<SurfaceFormat>(SurfaceFormat.Depth24Stencil8);

		/// <summary>
		///     The surface format of the render output panel's color buffer.
		/// </summary>
		public static readonly DependencyProperty<SurfaceFormat> ColorBufferFormatProperty =
			new DependencyProperty<SurfaceFormat>(SurfaceFormat.Rgba8);

		/// <summary>
		///     The camera that is used to draw to the content's of the render output panel.
		/// </summary>
		public static readonly DependencyProperty<Camera> CameraProperty = new DependencyProperty<Camera>();

		/// <summary>
		///     The callback that is invoked when the contents of the texture should be updated.
		/// </summary>
		public static readonly DependencyProperty<Action<RenderOutput>> DrawCallbackProperty = new DependencyProperty<Action<RenderOutput>>();

		/// <summary>
		///     The depth stencil texture of the render output.
		/// </summary>
		private Texture2D _depthStencil;

		/// <summary>
		///     The color buffer of the render output.
		/// </summary>
		private Texture2D _outputTexture;

		/// <summary>
		///     The render output that is used to draw into the output texture.
		/// </summary>
		private RenderOutput _renderOutput;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static RenderOutputPanel()
		{
			HasDepthStencilProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).DisposeGraphicsResources();
			DepthStencilFormatProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).DisposeGraphicsResources();
			ColorBufferFormatProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).DisposeGraphicsResources();
			CameraProperty.Changed += OnCameraChanged;
		}

		/// <summary>
		///     Gets or sets value indicating whether the render output panel has a depth stencil buffer.
		/// </summary>
		public bool HasDepthStencil
		{
			get { return GetValue(HasDepthStencilProperty); }
			set { SetValue(HasDepthStencilProperty, value); }
		}

		/// <summary>
		///     Gets or sets the surface format of the render output panel's depth stencil buffer.
		/// </summary>
		public SurfaceFormat DepthStencilFormat
		{
			get { return GetValue(DepthStencilFormatProperty); }
			set { SetValue(DepthStencilFormatProperty, value); }
		}

		/// <summary>
		///     Gets or sets the surface format of the render output panel's color buffer.
		/// </summary>
		public SurfaceFormat ColorBufferFormat
		{
			get { return GetValue(ColorBufferFormatProperty); }
			set { SetValue(ColorBufferFormatProperty, value); }
		}

		/// <summary>
		///     Gets or sets the camera that is used to draw to the content's of the render output panel.
		/// </summary>
		public Camera Camera
		{
			get { return GetValue(CameraProperty); }
			set { SetValue(CameraProperty, value); }
		}

		/// <summary>
		///     Gets or sets the callback that is invoked when the contents of the texture should be updated.
		/// </summary>
		public Action<RenderOutput> DrawCallback
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
		///     Changes the camera of the render output.
		/// </summary>
		private static void OnCameraChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<Camera> args)
		{
			var renderOutputPanel = (RenderOutputPanel)obj;
			if (renderOutputPanel._renderOutput == null)
				return;

			renderOutputPanel._renderOutput.Camera = args.NewValue;

			if (args.NewValue != null)
				args.NewValue.Viewport = renderOutputPanel._renderOutput.Viewport;
		}

		/// <summary>
		///     Invoked when the size of the UI element has changed.
		/// </summary>
		/// <param name="oldSize">The old size of the UI element.</param>
		/// <param name="newSize">The new size of the UI element.</param>
		protected override void OnSizeChanged(SizeD oldSize, SizeD newSize)
		{
			base.OnSizeChanged(oldSize, newSize);
			DisposeGraphicsResources();
		}

		/// <summary>
		///     Invoked when the UI element is no longer (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnDetachedFromRoot()
		{
			DisposeGraphicsResources();
		}

		/// <summary>
		///     Initializes all graphics resources.
		/// </summary>
		private void InitializeGraphicsResources()
		{
			// No need to initialize the graphics resources if the panel's area is 0
			if (!HasVisibleArea)
				return;

			var size = new Size((int)Math.Round(ActualWidth), (int)Math.Round(ActualHeight));

			// Initialize the color buffer of the render target
			_outputTexture = new Texture2D(Application.Current.GraphicsDevice, size, ColorBufferFormat, TextureFlags.RenderTarget);
			_outputTexture.SetName("RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			if (HasDepthStencil)
			{
				_depthStencil = new Texture2D(Application.Current.GraphicsDevice, size, DepthStencilFormat, TextureFlags.DepthStencil);
				_depthStencil.SetName("RenderOutputPanel.DepthStencil");
			}

			// Initialize the render output panel's graphics properties
			var viewport = new Rectangle(0, 0, size);
			_renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			{
				RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, _depthStencil, _outputTexture),
				Viewport = viewport,
				Camera = Camera
			};

			if (_renderOutput.Camera != null)
				_renderOutput.Camera.Viewport = viewport;
		}

		/// <summary>
		///     Disposes all graphics resources.
		/// </summary>
		private void DisposeGraphicsResources()
		{
			_depthStencil.SafeDispose();
			_outputTexture.SafeDispose();

			if (_renderOutput != null)
				_renderOutput.RenderTarget.SafeDispose();

			_renderOutput.SafeDispose();

			_renderOutput = null;
			_outputTexture = null;
			_depthStencil = null;
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			Log.DebugIf(Camera == null, "No camera has been set for the render output panel.");
			Log.DebugIf(DrawCallback == null, "No draw callback has been set for the render output panel.");

			if (!HasVisibleArea || DrawCallback == null)
				return;

			if (_renderOutput == null)
				InitializeGraphicsResources();

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
			DrawCallback(_renderOutput);

			// Draw the contents into the UI
			var quad = new Quad(new RectangleF(x, y, width, height), Color.White, textureArea);
			spriteBatch.Draw(ref quad, _outputTexture);
		}
	}
}