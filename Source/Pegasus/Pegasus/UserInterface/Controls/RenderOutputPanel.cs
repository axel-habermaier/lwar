namespace Pegasus.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;
	using Scripting;

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
		///     Indicates how the resolution of the render output panel is determined.
		/// </summary>
		public static readonly DependencyProperty<ResolutionSource> ResolutionSourceProperty =
			new DependencyProperty<ResolutionSource>(defaultValue: ResolutionSource.Layout, affectsRender: true);

		/// <summary>
		///     The explicit resolution of the render output panel.
		/// </summary>
		public static readonly DependencyProperty<Size> ResolutionProperty = new DependencyProperty<Size>(affectsRender: true);

		/// <summary>
		///     The render output that must be used to draw the panel's contents.
		/// </summary>
		public static readonly DependencyProperty<RenderOutput> RenderOutputProperty =new DependencyProperty<RenderOutput>();

		/// <summary>
		///     The depth stencil texture of the render output.
		/// </summary>
		private Texture2D _depthStencil;

		/// <summary>
		///     The color buffer of the render output.
		/// </summary>
		private Texture2D _outputTexture;

		/// <summary>
		/// The render target of the render output panel.
		/// </summary>
		private RenderTarget _renderTarget;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static RenderOutputPanel()
		{
			HasDepthStencilProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
			DepthStencilFormatProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
			ColorBufferFormatProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
			ResolutionSourceProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
			ResolutionProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
			RenderOutputProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).InitializeGraphicsResources();
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
		///     Gets or sets a value indicating how the resolution of the render output panel is determined.
		/// </summary>
		public ResolutionSource ResolutionSource
		{
			get { return GetValue(ResolutionSourceProperty); }
			set { SetValue(ResolutionSourceProperty, value); }
		}

		/// <summary>
		///     Gets or sets the explicit resolution of the render output panel.
		/// </summary>
		public Size Resolution
		{
			get { return GetValue(ResolutionProperty); }
			set { SetValue(ResolutionProperty, value); }
		}

		/// <summary>
		///     Gets or sets the render output that must be used to draw the panel's contents.
		/// </summary>
		public RenderOutput RenderOutput
		{
			get { return GetValue(RenderOutputProperty); }
			set { SetValue(RenderOutputProperty, value); }
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
		protected override void OnSizeChanged(Size oldSize, Size newSize)
		{
			base.OnSizeChanged(oldSize, newSize);
			InitializeGraphicsResources();
		}

		/// <summary>
		///     Invoked when the UI element is now (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnAttachedToRoot()
		{
			Cvars.ResolutionChanged += OnResolutionChanged;
			InitializeGraphicsResources();
		}

		/// <summary>
		///     Invoked when the UI element is no longer (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnDetachedFromRoot()
		{
			Cvars.ResolutionChanged -= OnResolutionChanged;
			DisposeGraphicsResources();
		}

		/// <summary>
		///     Disposes all graphics resources when the resolution has changed and the render output panel uses the app resolution.
		/// </summary>
		/// <param name="previousResolution">The previous app resolution.</param>
		private void OnResolutionChanged(Size previousResolution)
		{
			if (ResolutionSource == ResolutionSource.Application)
				InitializeGraphicsResources();
		}

		/// <summary>
		///     We assume that a render output panel always has non-transparent content
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2 position)
		{
			return true;
		}

		/// <summary>
		///     Initializes all graphics resources.
		/// </summary>
		private void InitializeGraphicsResources()
		{
			DisposeGraphicsResources();

			// No need to initialize the graphics resources if the panel's area is 0 or there is no render output set
			if (!HasVisibleArea || RenderOutput == null)
				return;

			Size size;
			switch (ResolutionSource)
			{
				case ResolutionSource.Application:
					if (!Cvars.ResolutionCvar.HasExplicitValue)
					{
						size = ParentWindow.Size;
						Log.Warn("No resolution has been set explicitly; using {0}.", TypeRegistry.ToString(size));
					}
					else
						size = Cvars.Resolution;
					break;
				case ResolutionSource.Explicit:
					size = Resolution;
					break;
				default:
					size = new Size(ActualWidth, ActualHeight);
					break;
			}

			// Initialize the color buffer of the render target
			_outputTexture = new Texture2D(Application.Current.GraphicsDevice, size, ColorBufferFormat, TextureFlags.RenderTarget);
			_outputTexture.SetName("RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			if (HasDepthStencil)
			{
				_depthStencil = new Texture2D(Application.Current.GraphicsDevice, size, DepthStencilFormat, TextureFlags.DepthStencil);
				_depthStencil.SetName("RenderOutputPanel.DepthStencil");
			}

			// Initialize the render target and output 
			_renderTarget = new RenderTarget(Application.Current.GraphicsDevice, _depthStencil, _outputTexture);

			RenderOutput.RenderTarget = _renderTarget;
			RenderOutput.Viewport = new Rectangle(0, 0, size);
		}

		/// <summary>
		///     Disposes all graphics resources.
		/// </summary>
		private void DisposeGraphicsResources()
		{
			_depthStencil.SafeDispose();
			_outputTexture.SafeDispose();
			_renderTarget.SafeDispose();

			_renderTarget = null;
			_outputTexture = null;
			_depthStencil = null;

			if (RenderOutput != null)
				RenderOutput.RenderTarget = null;
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			if (!HasVisibleArea || RenderOutput == null)
				return;

			// Take the different coordinate origins for OpenGL and Direct3D into account when rendering 
			// the render target's color buffer... annoying
			Rectangle textureArea;
			switch (RenderOutput.GraphicsDevice.GraphicsApi)
			{
				case GraphicsApi.Direct3D11:
					textureArea = new Rectangle(0, 0, 1, 1);
					break;
				case GraphicsApi.OpenGL3:
					textureArea = new Rectangle(0, 1, 1, -1);
					break;
				default:
					throw new InvalidOperationException("Unsupported graphics API.");
			}

			// Draw the contents of the render output into the UI
			var quad = new Quad(VisualArea, Colors.White, textureArea);
			spriteBatch.Draw(ref quad, _outputTexture);
		}
	}
}