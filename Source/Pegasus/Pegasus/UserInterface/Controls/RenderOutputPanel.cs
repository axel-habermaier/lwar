namespace Pegasus.UserInterface.Controls
{
	using System;
	using System.Reflection;
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
		///     The camera that is used to draw to the content's of the render output panel.
		/// </summary>
		public static readonly DependencyProperty<Camera> CameraProperty = new DependencyProperty<Camera>();

		/// <summary>
		///     The name of the draw method that is invoked on the data context when the contents of the texture should be updated.
		/// </summary>
		public static readonly DependencyProperty<string> DrawMethodProperty = new DependencyProperty<string>();

		/// <summary>
		///     The depth stencil texture of the render output.
		/// </summary>
		private Texture2D _depthStencil;

		/// <summary>
		///     The draw method that is invoked on the data context when the contents of the texture should be updated.
		/// </summary>
		private DrawCallback _drawMethod;

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
			DrawMethodProperty.Changed += (obj, args) => GetDrawMethodDelegate(obj);
			DataContextProperty.Changed += (obj, args) => GetDrawMethodDelegate(obj);
			ResolutionSourceProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).DisposeGraphicsResources();
			ResolutionProperty.Changed += (obj, args) => ((RenderOutputPanel)obj).DisposeGraphicsResources();
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
		///     Gets or sets the camera that is used to draw to the content's of the render output panel.
		/// </summary>
		public Camera Camera
		{
			get { return GetValue(CameraProperty); }
			set { SetValue(CameraProperty, value); }
		}

		/// <summary>
		///     Gets or sets the name of the draw method that is invoked on the data context when the contents of the texture should be
		///     updated.
		/// </summary>
		public string DrawMethod
		{
			get { return GetValue(DrawMethodProperty); }
			set { SetValue(DrawMethodProperty, value); }
		}

		/// <summary>
		///     Gets a value indicating whether the area of the render output is non-zero.
		/// </summary>
		private bool HasVisibleArea
		{
			get { return !MathUtils.Equals(ActualWidth, 0) && !MathUtils.Equals(ActualHeight, 0); }
		}

		/// <summary>
		///     Gets the delegate of the draw method.
		/// </summary>
		private static void GetDrawMethodDelegate(object obj)
		{
			var renderOutputPanel = obj as RenderOutputPanel;
			if (renderOutputPanel == null)
				return;

			var dataContext = renderOutputPanel.DataContext;
			var drawMethod = renderOutputPanel.DrawMethod;

			if (dataContext == null || String.IsNullOrWhiteSpace(drawMethod))
				return;

			var method = dataContext.GetType().GetTypeInfo().GetDeclaredMethod(drawMethod);
			var validSignature = method != null && method.ReturnType == typeof(void) && method.GetParameters().Length == 1 &&
								 method.GetParameters()[0].ParameterType == typeof(RenderOutput);

			if (validSignature)
				renderOutputPanel._drawMethod = (DrawCallback)method.CreateDelegate(typeof(DrawCallback), dataContext);
			else
				Log.Debug("Unable to find method 'void {0}.{1}({2})'.", dataContext.GetType().FullName, drawMethod, typeof(RenderOutput).FullName);
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
		protected override void OnSizeChanged(Size oldSize, Size newSize)
		{
			base.OnSizeChanged(oldSize, newSize);
			DisposeGraphicsResources();
		}

		/// <summary>
		///     Invoked when the UI element is now (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnAttachedToRoot()
		{
			Cvars.ResolutionChanged += OnResolutionChanged;
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
				DisposeGraphicsResources();
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
			// No need to initialize the graphics resources if the panel's area is 0
			if (!HasVisibleArea)
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

			// Initialize the render output panel's graphics properties
			var viewport = new Rectangle(0, 0, size);
			_renderOutput = new RenderOutput(Application.Current.RenderContext)
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

			_renderOutput = null;
			_outputTexture = null;
			_depthStencil = null;
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			Log.DebugIf(Camera == null, "No camera has been set for the render output panel.");
			Log.DebugIf(_drawMethod == null, "No draw callback has been set for the render output panel.");

			if (!HasVisibleArea || _drawMethod == null)
				return;

			if (_renderOutput == null)
				InitializeGraphicsResources();

			// Take the different coordinate origins for OpenGL and Direct3D into account when rendering 
			// the render target's color buffer... annoying
			Rectangle textureArea;
			switch (_renderOutput.GraphicsDevice.GraphicsApi)
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

			// Update the contents of the texture
			_drawMethod(_renderOutput);

			// Draw the contents into the UI
			var quad = new Quad(VisualArea, Colors.White, textureArea);
			spriteBatch.Draw(ref quad, _outputTexture);
		}

		/// <summary>
		///     The type of the draw method that is invoked on the data context.
		/// </summary>
		/// <param name="renderOutput">The render output that should be drawn to.</param>
		private delegate void DrawCallback(RenderOutput renderOutput);
	}
}