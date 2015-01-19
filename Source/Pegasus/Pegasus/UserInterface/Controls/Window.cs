namespace Pegasus.UserInterface.Controls
{
	using System;
	using System.Diagnostics;
	using Input;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents an operating system window that hosts UI elements.
	/// </summary>
	public class Window : Decorator, IDisposable
	{
		/// <summary>
		///     The minimum overlap of a window that must always be visible.
		/// </summary>
		internal const int MinimumOverlap = 100;

		/// <summary>
		///     The minimal window size supported by the library.
		/// </summary>
		public static readonly Size MinimumSize = new Size(800, 600);

		/// <summary>
		///     The maximal window size supported by the library.
		/// </summary>
		public static readonly Size MaximumSize = new Size(4096, 2160);

		/// <summary>
		///     Indicates whether the window is in fullscreen or windowed mode.
		/// </summary>
		public static readonly DependencyProperty<bool> FullscreenProperty = new DependencyProperty<bool>();

		/// <summary>
		///     Indicates the mode the window is currently in.
		/// </summary>
		public static readonly DependencyProperty<WindowMode> WindowModeProperty = new DependencyProperty<WindowMode>(isReadOnly: true);

		/// <summary>
		///     The screen position of the window's left upper corner.
		/// </summary>
		public static readonly DependencyProperty<Vector2> PositionProperty = new DependencyProperty<Vector2>(isReadOnly: true);

		/// <summary>
		///     The size of the window's rendering area.
		/// </summary>
		public static readonly DependencyProperty<Size> SizeProperty = new DependencyProperty<Size>(isReadOnly: true);

		/// <summary>
		///     The output the window's contents are rendered to.
		/// </summary>
		private readonly RenderOutput _output;

		/// <summary>
		///     The sprite batch that is used for drawing the window's UI elements.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///     The native operating system window that is used to display the UI.
		/// </summary>
		private NativeWindow _window;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Window()
		{
			FullscreenProperty.Changed += OnFullscreenChanged;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Window()
			: this(String.Empty, Vector2.Zero, new Size(1024, 768), WindowMode.Normal)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		public Window(string title, Vector2 position, Size size, WindowMode mode)
		{
			Assert.ArgumentNotNull(title);

			var flags = WindowFlags.Resizable;
			switch (mode)
			{
				case WindowMode.Fullscreen:
					flags |= WindowFlags.FullscreenDesktop | WindowFlags.InputGrabbed;
					break;
				case WindowMode.Maximized:
					flags |= WindowFlags.Maximized;
					break;
			}

			_window = new NativeWindow(title, position, size, flags);
			Keyboard = new Keyboard(this) { FocusedElement = this };
			Mouse = new Mouse(this);

			var renderContext = Application.Current.RenderContext;
			SwapChain = new SwapChain(renderContext.GraphicsDevice, _window);
			_output = new RenderOutput(renderContext)
			{
				RenderTarget = SwapChain.BackBuffer,
				Camera = new Camera2D(renderContext.GraphicsDevice)
			};

			_spriteBatch = new SpriteBatch
			{
				BlendState = renderContext.BlendStates.Premultiplied,
				DepthStencilState = renderContext.DepthStencilStates.DepthDisabled,
				SamplerState = renderContext.SamplerStates.PointClampNoMipmaps
			};

			UpdateDependencyProperties(mode, position, size);
			Application.Current.AddWindow(this);
		}

		/// <summary>
		///     Gets the swap chain that is used to render the window's contents.
		/// </summary>
		internal SwapChain SwapChain { get; private set; }

		/// <summary>
		///     Gets the keyboard state of this window.
		/// </summary>
		internal Keyboard Keyboard { get; private set; }

		/// <summary>
		///     Gets the mouse state of this window.
		/// </summary>
		internal Mouse Mouse { get; private set; }

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
		///     Gets a value indicating whether the window is open.
		/// </summary>
		public bool IsOpen
		{
			get { return _window != null; }
		}

		/// <summary>
		///     Gets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get { return GetValue(SizeProperty); }
		}

		/// <summary>
		///     Gets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2 Position
		{
			get { return GetValue(PositionProperty); }
		}

		/// <summary>
		///     Gets the window state.
		/// </summary>
		public WindowMode Mode
		{
			get { return GetValue(WindowModeProperty); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the window is in fullscreen or windowed mode.
		/// </summary>
		public bool Fullscreen
		{
			get { return GetValue(FullscreenProperty); }
			set { SetValue(FullscreenProperty, value); }
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
			SwapChain.SafeDispose();
			_window.SafeDispose();

			_window = null;

#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

		/// <summary>
		///     Raised when the user requested the window to be closed. The window is not actually closed
		///     until Dispose() or Close() is called.
		/// </summary>
		public event Action Closing;

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

			// Check if the window requested to be closed and raise the event, if necessary
			if (_window.IsClosing && Closing != null)
			{
				// Reset the flag so that we don't raise the event again if the close request is ignored
				Closing();
			}

			// Update the mode, position, and size dependency properties
			UpdateDependencyProperties(_window.Mode, _window.Position, _window.Size);
		}

		/// <summary>
		///     Updates the size, position, and mode dependency properties.
		/// </summary>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		private void UpdateDependencyProperties(WindowMode mode, Vector2 position, Size size)
		{
			// The mode must always be set first.
			SetReadOnlyValue(WindowModeProperty, mode);

			SetReadOnlyValue(SizeProperty, size);
			SetReadOnlyValue(PositionProperty, position);
		}

		/// <summary>
		///     Changes the window's mode to either fullscreen or windowed.
		/// </summary>
		private static void OnFullscreenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var window = obj as Window;
			if (window == null)
				return;

			if (args.NewValue && window.Mode == WindowMode.Fullscreen)
				return;

			if (!args.NewValue && window.Mode != WindowMode.Fullscreen)
				return;

			if (args.NewValue)
				window._window.ChangeToFullscreenMode();
			else
				window._window.ChangeToWindowedMode();
		}

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected virtual void OnClosing()
		{
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

			var availableSize = new Size(size.Width, size.Height);
			Measure(availableSize);
			Arrange(new Rectangle(0, 0, availableSize));
			UpdateVisualOffsets(Vector2.Zero);
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

			_spriteBatch.BlendState = _output.RenderContext.BlendStates.Premultiplied;
			_spriteBatch.Layer = 0;

			Assert.That(VisualChildrenCount == 1, "A window must have exactly one child element.");
			GetVisualChild(0).Draw(_spriteBatch);

			if (!Mouse.RelativeMouseMode)
				Mouse.DrawCursor(_spriteBatch);

			_spriteBatch.DrawBatch(_output);
		}

		/// <summary>
		///     Presents the contents of all windows' backbuffers.
		/// </summary>
		internal void Present()
		{
			SwapChain.Present();
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override sealed void DrawCore(SpriteBatch spriteBatch)
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