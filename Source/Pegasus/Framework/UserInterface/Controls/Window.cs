namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Diagnostics;
	using Input;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents an operating system window that hosts UI elements.
	/// </summary>
	public class Window : Decorator, IDisposable
	{
		/// <summary>
		///     Gets the swap chain that is used to render the window's contents.
		/// </summary>
		internal SwapChain SwapChain { get; private set; }

		/// <summary>
		///     The native operating system window that is used to display the UI.
		/// </summary>
		private NativeWindow _window;

		/// <summary>
		///     Gets the keyboard state of this window.
		/// </summary>
		public Keyboard Keyboard { get; private set; }

		/// <summary>
		///     Gets the mouse state of this window.
		/// </summary>
		public Mouse Mouse { get; private set; }

		/// <summary>
		///     The output the window's contents are rendered to.
		/// </summary>
		private readonly RenderOutput _output;

		/// <summary>
		///     The sprite batch that is used for drawing the window's UI elements.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Window()
			: this(String.Empty, Vector2i.Zero, new Size(1024, 768), WindowMode.Normal)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		public Window(string title, Vector2i position, Size size, WindowMode mode)
		{
			Assert.ArgumentNotNull(title);

			_window = new NativeWindow(title, position, size, mode);
			Keyboard = new Keyboard(this) { FocusedElement = this };
			Mouse = new Mouse(this);

			var graphicsDevice = Application.Current.GraphicsDevice;
			SwapChain = new SwapChain(graphicsDevice, _window, false, _window.Size);
			_output = new RenderOutput(graphicsDevice)
			{
				RenderTarget = SwapChain.BackBuffer,
				Camera = new Camera2D(graphicsDevice)
			};

			_spriteBatch = new SpriteBatch(graphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};

			Application.Current.AddWindow(this);
		}

		/// <summary>
		///     Gets the native operating system window that is used to display the UI.
		/// </summary>
		internal NativeWindow NativeWindow
		{
			get
			{
				CheckWindowOpen();
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
				CheckWindowOpen();
				return _window.MouseCaptured;
			}
			set
			{
				CheckWindowOpen();
				_window.MouseCaptured = value;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the window currently has the operating system focus.
		/// </summary>
		public bool Focused
		{
			get
			{
				CheckWindowOpen();
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
				CheckWindowOpen();
				_window.Closing += value;
			}
			remove
			{
				CheckWindowOpen();
				_window.Closing -= value;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the window is open.
		/// </summary>
		public bool IsOpen
		{
			get { return _window != null; }
		}

		/// <summary>
		///     Sets the window's title.
		/// </summary>
		public string Title
		{
			set
			{
				CheckWindowOpen();
				_window.Title = value;
			}
		}

		/// <summary>
		///     Gets or sets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get
			{
				CheckWindowOpen();
				return _window.Size;
			}
			set
			{
				CheckWindowOpen();
				_window.Size = value;
			}
		}

		/// <summary>
		///     Gets or sets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				CheckWindowOpen();
				return _window.Position;
			}
			set
			{
				CheckWindowOpen();
				_window.Position = value;
			}
		}

		/// <summary>
		///     Gets or sets the window state.
		/// </summary>
		public WindowMode Mode
		{
			get
			{
				CheckWindowOpen();
				return _window.Mode;
			}
			set
			{
				CheckWindowOpen();
				_window.Mode = value;
			}
		}

		/// <summary>
		///     Processes all pending window events and handles the window's user input.
		/// </summary>
		internal virtual void HandleInput()
		{
			CheckWindowOpen();

			// Update the keyboard and mouse state first (this ensures that WentDown returns 
			// false for all keys and buttons, etc.)
			Keyboard.Update();
			Mouse.Update();

			// Process all pending operating system events
			_window.ProcessEvents();
		}

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected virtual void OnClosing()
		{
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			CheckWindowOpen();

			OnClosing();
			Application.Current.RemoveWindow(this);

			Mouse.SafeDispose();
			Keyboard.SafeDispose();

			_spriteBatch.SafeDispose();
			_output.Camera.SafeDispose();
			_output.SafeDispose();
			SwapChain.SafeDispose();
			_window.SafeDispose();

			_window = null;

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
		///     Draws the window and its children.
		/// </summary>
		internal void Draw()
		{
			if (Visibility != Visibility.Visible)
				return;

			var viewport = new Rectangle(0, 0, Size);
			_output.Camera.Viewport = viewport;
			_output.Viewport = viewport;

			Assert.That(Background.HasValue, "No background color has been set for the window.");
			_output.ClearColor(Background.Value);
			_output.ClearDepth();

			_spriteBatch.BlendState = BlendState.Premultiplied;
			_spriteBatch.Layer = 0;

			Assert.That(VisualChildrenCount == 1, "A window must have exactly one child element.");
			GetVisualChild(0).Draw(_spriteBatch);

			_spriteBatch.DrawBatch(_output);
		}

		/// <summary>
		///     Presents the contents of all windows' backbuffers.
		/// </summary>
		internal void Present()
		{
			SwapChain.Present();
		}

		protected override sealed void OnDraw(SpriteBatch spriteBatch)
		{
			throw new NotSupportedException("Call Draw() instead.");
		}

		/// <summary>
		///     In debug builds, checks whether the window is open.
		/// </summary>
		[Conditional("DEBUG")]
		private void CheckWindowOpen()
		{
			Assert.That(IsOpen, "The window has not yet been opened.");
		}
	}
}