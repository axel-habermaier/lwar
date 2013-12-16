namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Diagnostics;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents an operating system window that hosts UI elements.
	/// </summary>
	public class Window : ContentControl, IDisposable
	{
		/// <summary>
		///     The application the window belongs to.
		/// </summary>
		private Application _application;

		/// <summary>
		///     Gets the swap chain that is used to render the window's contents.
		/// </summary>
		internal SwapChain SwapChain { get; private set; }

		/// <summary>
		///     The native operating system window that is used to display the UI.
		/// </summary>
		private NativeWindow _window;

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
		///     Initializes a new instance.
		/// </summary>
		public Window()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal Window(string title, Vector2i position, Size size, WindowMode mode)
		{
			Assert.ArgumentNotNull(title);

			Title = title;
			Position = position;
			Size = size;
			Mode = mode;
		}

		/// <summary>
		///     Gets the native operating system window that is used to display the UI.
		/// </summary>
		internal NativeWindow NativeWindow
		{
			get
			{
				CheckNativeWindowOpen();
				return _window;
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether the mouse is currently captured by the window.
		/// </summary>
		public bool MouseCaptured
		{
			get
			{
				CheckNativeWindowOpen();
				return _window.MouseCaptured;
			}
			set
			{
				CheckNativeWindowOpen();
				_window.MouseCaptured = value;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the window currently has the focus.
		/// </summary>
		public bool Focused
		{
			get
			{
				CheckNativeWindowOpen();
				return _window.Focused;
			}
		}

		/// <summary>
		///     Raised when the user requested the window to be closed. The window is not actually closed
		///     until Dispose() or Close() is called.
		/// </summary>
		public event Action Closing
		{
			add
			{
				CheckNativeWindowOpen();
				_window.Closing += value;
			}
			remove
			{
				CheckNativeWindowOpen();
				_window.Closing -= value;
			}
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

				if (_window != null)
					_window.Title = value;
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

				if (_window != null)
					_window.Size = value;
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

				if (_window != null)
					_window.Position = value;
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

				if (_window != null)
					_window.Mode = value;
			}
		}

		/// <summary>
		///     Opens the window.
		/// </summary>
		/// <param name="application">The application the window belongs to.</param>
		internal void Open(Application application)
		{
			Assert.ArgumentNotNull(application);
			Assert.That(!IsOpen, "The window is already open.");

			_application = application;

			_window = new NativeWindow(Title, Position, Size, Mode);
			SwapChain = new SwapChain(application.GraphicsDevice, _window, false, _window.Size);
			_output = new RenderOutput(application.GraphicsDevice)
			{
				RenderTarget = SwapChain.BackBuffer,
				Camera = new Camera2D(application.GraphicsDevice)
			};

			IsOpen = true;
			Template = DefaultTemplate;

			UpdateLayout();
		}

		/// <summary>
		///     Processes all pending window events.
		/// </summary>
		public void ProcessEvents()
		{
			_window.ProcessEvents();

			_size = _window.Size;
			_position = _window.Position;
			_mode = _window.Mode;
		}

		/// <summary>
		///     Closes the window.
		/// </summary>
		public void Close()
		{
			_application.RemoveWindow(this);

			_output.Camera.SafeDispose();
			_output.SafeDispose();
			SwapChain.SafeDispose();
			_window.SafeDispose();

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

		/// <summary>
		///     Updates the layout of the window and its children.
		/// </summary>
		internal void UpdateLayout()
		{
			var size = Size;
			Width = size.Width;
			Height = size.Height;

			var availableSize = new SizeD(size.Width, size.Height);
			Measure(availableSize);
			Arrange(new RectangleD(0, 0, availableSize));
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			return availableSize;
		}

		/// <summary>
		///   Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///   element. If this value is smaller than the given size, the UI element's alignment properties position it
		///   appropriately.
		/// </summary>
		/// <param name="finalSize">
		///   The final area allocated by the UI element's parent that the UI element should use to arrange
		///   itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			return finalSize;
		}

		/// <summary>
		///     Draws the window and its children.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		internal new void Draw(SpriteBatch spriteBatch)
		{
			Assert.ArgumentNotNull(spriteBatch);

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
			SwapChain.Present();
		}

		/// <summary>
		///     In debug builds, checks whether the native window is open.
		/// </summary>
		[Conditional("DEBUG")]
		private void CheckNativeWindowOpen()
		{
			Assert.NotNull(_window, "The window has not yet been opened.");
		}
	}
}