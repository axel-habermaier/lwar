namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents an operating system window that hosts UI elements.
	/// </summary>
	public abstract class Window : ContentControl, IDisposable
	{
		/// <summary>
		///     The application the window belongs to.
		/// </summary>
		private Application _application;

		/// <summary>
		///     The swap chain that is used to render the window's contents.
		/// </summary>
		private SwapChain _swapChain;

		/// <summary>
		///     Gets the native operating system window that is used to display the UI.
		/// </summary>
		internal NativeWindow NativeWindow { get; private set; }

		/// <summary>
		///     The window's title.
		/// </summary>
		private string _title = String.Empty;

		/// <summary>
		///     The screen position of the window's left upper corner.
		/// </summary>
		private Vector2i _position = Vector2i.Zero;

		/// <summary>
		///     Gets a value indicating whether the window is open.
		/// </summary>
		public bool IsOpen { get; private set; }

		/// <summary>
		///     The size of the window's rendering area.
		/// </summary>
		private Size _size = new Size(1024, 768);

		/// <summary>
		///     The window mode.
		/// </summary>
		private WindowMode _mode = WindowMode.Normal;

		/// <summary>
		///     The output the window's contents are rendered to.
		/// </summary>
		private RenderOutput _output;

		/// <summary>
		///     Opens the window.
		/// </summary>
		/// <param name="application">The application the window belongs to.</param>
		internal void Open(Application application)
		{
			Assert.ArgumentNotNull(application);
			Assert.That(!IsOpen, "The window is already open.");

			_application = application;

			NativeWindow = new NativeWindow(Title, Position, Size, Mode);
			_swapChain = new SwapChain(application.GraphicsDevice, NativeWindow, false, NativeWindow.Size);
			_output = new RenderOutput(application.GraphicsDevice)
			{
				RenderTarget = _swapChain.BackBuffer,
				Camera = new Camera2D(application.GraphicsDevice)
			};

			IsOpen = true;
			Template = DefaultTemplate;
		}

		/// <summary>
		///     Processes all pending window events.
		/// </summary>
		public void ProcessEvents()
		{
			NativeWindow.ProcessEvents();

			_size = NativeWindow.Size;
			_position = NativeWindow.Position;
			_mode = NativeWindow.Mode;
		}

		/// <summary>
		///     Sets the window's title.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title == value)
					return;

				_title = value;

				if (NativeWindow != null)
					NativeWindow.Title = value;
			}
		}

		/// <summary>
		///     Gets or sets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get { return _size; }
			set
			{
				if (_size == value)
					return;

				_size = value;

				if (NativeWindow != null)
					NativeWindow.Size = value;
			}
		}

		/// <summary>
		///     Gets or sets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2i Position
		{
			get { return _position; }
			set
			{
				if (_position == value)
					return;

				_position = value;

				if (NativeWindow != null)
					NativeWindow.Position = value;
			}
		}

		/// <summary>
		///     Gets or sets the window state.
		/// </summary>
		public WindowMode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode == value)
					return;

				_mode = value;

				if (NativeWindow != null)
					NativeWindow.Mode = value;
			}
		}

		/// <summary>
		///     Closes the window.
		/// </summary>
		public void Close()
		{
			_application.RemoveWindow(this);

			_output.Camera.SafeDispose();
			_output.SafeDispose();
			_swapChain.SafeDispose();
			NativeWindow.SafeDispose();

			IsOpen = false;
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			Close();

#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

#if DEBUG
		/// <summary>
		///     Ensures that the instance has been disposed.
		/// </summary>
		~Window()
		{
			Log.Error("Finalizer runs for an instance of '{0}'.", GetType().FullName);
		}
#endif

		internal void UpdateLayout()
		{
			var size = Size;
			Width = size.Width;
			Height = size.Height;

			var availableSize = new SizeD(size.Width, size.Height);
			Measure(availableSize);
			Arrange(new RectangleD(0, 0, availableSize));
		}

		internal new void Draw(SpriteBatch spriteBatch)
		{
			if (Visibility != Visibility.Visible)
				return;

			spriteBatch.BlendState = BlendState.Premultiplied;
			spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;
			spriteBatch.SamplerState = SamplerState.PointClampNoMipmaps;

			var viewport = new Rectangle(0, 0, Size);
			_output.Camera.Viewport = viewport;
			_output.Viewport = viewport;
			_output.ClearColor(Background);
			_output.ClearDepth();

			OnDraw(spriteBatch);

			Assert.That(VisualChildrenCount == 1, "A window must have exactly one child element.");
			GetVisualChild(0).Draw(spriteBatch);

			spriteBatch.DrawBatch(_output);
			_swapChain.Present();
		}
	}
}