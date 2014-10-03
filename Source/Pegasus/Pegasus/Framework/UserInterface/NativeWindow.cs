namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Controls;
	using Input;
	using Math;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///     Represents a native operating system window.
	/// </summary>
	internal sealed class NativeWindow : DisposableObject
	{
		/// <summary>
		///     Reacts to a character being entered as the result of a dead key press.
		/// </summary>
		/// <param name="character">Provides information about the character that has been entered.</param>
		/// <param name="cancel">
		///     If set to true, the dead character is removed such that the subsequently entered character is not
		///     influenced by the dead character.
		/// </param>
		public delegate void DeadCharacterEnteredHandler(CharacterEnteredEventArgs character, out bool cancel);

		/// <summary>
		///     The minimal window size supported by the library.
		/// </summary>
		public static readonly Size MinimumSize = new Size(800, 600);

		/// <summary>
		///     The maximal window size supported by the library.
		/// </summary>
		public static readonly Size MaximumSize = new Size(4096, 2160);

		/// <summary>
		///     The window callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///     the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.Callbacks _callbacks;

		/// <summary>
		///     The native window instance.
		/// </summary>
		private readonly IntPtr _window;

		/// <summary>
		///     A value indicating whether the mouse is currently captured by the window.
		/// </summary>
		private bool _mouseCaptured;

		/// <summary>
		///     The current placement of the window.
		/// </summary>
		private NativeMethods.Placement _placement;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal NativeWindow(string title, Vector2i position, Size size, WindowMode mode)
		{
			Log.Info("Initializing window...");

			_callbacks = new NativeMethods.Callbacks
			{
				CharacterEntered = OnCharacterEntered,
				DeadCharacterEntered = OnDeadCharacterEntered,
				KeyPressed = OnKeyPressed,
				KeyReleased = OnKeyReleased,
				MouseWheel = OnMouseWheel,
				MousePressed = OnMousePressed,
				MouseReleased = OnMouseReleased,
				MouseMoved = OnMouseMoved,
				MouseEntered = OnMouseEntered,
				MouseLeft = OnMouseLeft
			};

			_placement = new NativeMethods.Placement
			{
				Mode = mode,
				X = position.X,
				Y = position.Y,
				Width = size.Width,
				Height = size.Height
			};

			_window = NativeMethods.OpenWindow(title, _placement, _callbacks);
		}

		/// <summary>
		///     Gets a value indicating whether the window currently has the focus.
		/// </summary>
		public bool Focused { get; private set; }

		/// <summary>
		///     Gets the native window instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _window; }
		}

		/// <summary>
		///     Sets the window's title.
		/// </summary>
		public string Title
		{
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				NativeMethods.SetWindowTitle(_window, value);
			}
		}

		/// <summary>
		///     Gets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get
			{
				Assert.NotDisposed(this);
				return new Size(_placement.Width, _placement.Height);
			}
		}

		/// <summary>
		///     Gets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				Assert.NotDisposed(this);
				return new Vector2i(_placement.X, _placement.Y);
			}
		}

		/// <summary>
		///     Gets the window state.
		/// </summary>
		public WindowMode Mode
		{
			get
			{
				Assert.NotDisposed(this);
				Assert.InRange(_placement.Mode);

				return _placement.Mode;
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether the mouse is currently captured by the window.
		/// </summary>
		public bool MouseCaptured
		{
			get
			{
				Assert.NotDisposed(this);
				return _mouseCaptured;
			}
			set
			{
				Assert.NotDisposed(this);

				if (_mouseCaptured == value)
					return;

				_mouseCaptured = value;
				if (_mouseCaptured)
					NativeMethods.CaptureMouse(_window);
				else
					NativeMethods.ReleaseMouse(_window);
			}
		}

		/// <summary>
		///     Gets the resolution of the window's monitor.
		/// </summary>
		public Size MonitorResolution
		{
			get
			{
				Assert.NotDisposed(this);

				int width, height;
				NativeMethods.GetMonitorResolution(_window, out width, out height);
				return new Size(width, height);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.CloseWindow(_window);
		}

		/// <summary>
		///     Processes all pending window events.
		/// </summary>
		public void ProcessEvents()
		{
			Assert.NotDisposed(this);

			NativeMethods.ProcessWindowEvents(_window);
			NativeMethods.GetWindowPlacement(_window, out _placement);

			if (Closing != null && NativeMethods.IsClosing(_window))
				Closing();

			Focused = NativeMethods.IsFocused(_window);
		}

		/// <summary>
		///     Sets the window into fullscreen mode.
		/// </summary>
		public void ChangeToFullscreenMode()
		{
			Assert.NotDisposed(this);
			NativeMethods.ChangeToFullscreenMode(_window);
		}

		/// <summary>
		///     Sets the window into windowed mode.
		/// </summary>
		public void ChangeToWindowedMode()
		{
			Assert.NotDisposed(this);
			NativeMethods.ChangeToWindowedMode(_window);
		}

		/// <summary>
		///     Raised when the user requested the window to be closed. The window is not actually closed
		///     until Dispose() is called.
		/// </summary>
		public event Action Closing;

		/// <summary>
		///     Raised when a character was entered.
		/// </summary>
		public event Action<CharacterEnteredEventArgs> CharacterEntered;

		/// <summary>
		///     Raises the character entered event.
		/// </summary>
		/// <param name="character">The character that has been entered.</param>
		/// <param name="scanCode">The scan code of the key that generated the character.</param>
		private void OnCharacterEntered(ushort character, int scanCode)
		{
			if (CharacterEntered != null)
				CharacterEntered(new CharacterEnteredEventArgs((char)character, scanCode));
		}

		/// <summary>
		///     Raised when a character was entered as a result of a dead key press.
		/// </summary>
		public event DeadCharacterEnteredHandler DeadCharacterEntered;

		/// <summary>
		///     Raises the dead character entered event.
		/// </summary>
		/// <param name="character">The character that has been entered.</param>
		/// <param name="scanCode">The scan code of the key that generated the character.</param>
		/// <param name="cancel">If set to true, the dead key is canceled.</param>
		private void OnDeadCharacterEntered(ushort character, int scanCode, out bool cancel)
		{
			if (DeadCharacterEntered != null)
				DeadCharacterEntered(new CharacterEnteredEventArgs((char)character, scanCode), out cancel);
			else
				cancel = false;
		}

		/// <summary>
		///     Raised when a key was pressed.
		/// </summary>
		public event Action<Key, int> KeyPressed;

		/// <summary>
		///     Raises the key pressed event.
		/// </summary>
		/// <param name="key">The key that has been pressed.</param>
		/// <param name="scanCode">The scan code of the key.</param>
		private void OnKeyPressed(Key key, int scanCode)
		{
			if (KeyPressed != null)
				KeyPressed(key, scanCode);
		}

		/// <summary>
		///     Raised when a key was released.
		/// </summary>
		public event Action<Key, int> KeyReleased;

		/// <summary>
		///     Raises the key released event.
		/// </summary>
		/// <param name="key">The key that has been released.</param>
		/// <param name="scanCode">The scan code of the key.</param>
		private void OnKeyReleased(Key key, int scanCode)
		{
			if (KeyReleased != null)
				KeyReleased(key, scanCode);
		}

		/// <summary>
		///     Raised when the mouse wheel was moved.
		/// </summary>
		public event Action<int> MouseWheel;

		/// <summary>
		///     Raises the mouse wheel event.
		/// </summary>
		/// <param name="delta">The mouse wheel delta.</param>
		private void OnMouseWheel(int delta)
		{
			if (MouseWheel != null)
				MouseWheel(delta);
		}

		/// <summary>
		///     Raised when a mouse button was pressed.
		/// </summary>
		public event Action<MouseButton, bool, Vector2i> MousePressed;

		/// <summary>
		///     Raises the mouse pressed event.
		/// </summary>
		/// <param name="button">The mouse button that has been pressed.</param>
		/// <param name="doubleClick">Indicates whether the event represents a double click.</param>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMousePressed(MouseButton button, bool doubleClick, int x, int y)
		{
			if (MousePressed != null)
				MousePressed(button, doubleClick, new Vector2i(x, y));
		}

		/// <summary>
		///     Raised when a mouse button was released.
		/// </summary>
		public event Action<MouseButton, Vector2i> MouseReleased;

		/// <summary>
		///     Raises the mouse released event.
		/// </summary>
		/// <param name="button">The mouse button that has been released.</param>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMouseReleased(MouseButton button, int x, int y)
		{
			if (MouseReleased != null)
				MouseReleased(button, new Vector2i(x, y));
		}

		/// <summary>
		///     Raised when the mouse was moved inside the window.
		/// </summary>
		public event Action<Vector2i> MouseMoved;

		/// <summary>
		///     Raises the mouse moved event.
		/// </summary>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMouseMoved(int x, int y)
		{
			if (MouseMoved != null)
				MouseMoved(new Vector2i(x, y));
		}

		/// <summary>
		///     Raised when the mouse entered the window.
		/// </summary>
		public event Action MouseEntered;

		/// <summary>
		///     Raises the mouse entered event.
		/// </summary>
		private void OnMouseEntered()
		{
			if (MouseEntered != null)
				MouseEntered();
		}

		/// <summary>
		///     Raised when the mouse left the window.
		/// </summary>
		public event Action MouseLeft;

		/// <summary>
		///     Raises the mouse left event.
		/// </summary>
		private void OnMouseLeft()
		{
			if (MouseLeft != null)
				MouseLeft();
		}

		/// <summary>
		///     Provides access to the native window-related types and functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			public delegate void CharacterEnteredCallback(ushort character, int scanCode);

			public delegate void DeadCharacterEnteredCallback(ushort character, int scanCode, out bool cancel);

			public delegate void KeyPressedCallback(Key key, int scanCode);

			public delegate void KeyReleasedCallback(Key key, int scanCode);

			public delegate void MouseEnteredCallback();

			public delegate void MouseLeftCallback();

			public delegate void MouseMovedCallback(int x, int y);

			public delegate void MousePressedCallback(MouseButton button, bool doubleClick, int x, int y);

			public delegate void MouseReleasedCallback(MouseButton button, int x, int y);

			public delegate void MouseWheelCallback(int delta);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgOpenWindow")]
			public static extern IntPtr OpenWindow(string title, Placement placement, Callbacks callbacks);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCloseWindow")]
			public static extern void CloseWindow(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgProcessWindowEvents")]
			public static extern void ProcessWindowEvents(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIsWindowFocused")]
			public static extern bool IsFocused(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIsWindowClosing")]
			public static extern bool IsClosing(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetWindowPlacement")]
			public static extern void GetWindowPlacement(IntPtr window, out Placement placement);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgChangeToFullscreenMode")]
			public static extern void ChangeToFullscreenMode(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgChangeToWindowedMode")]
			public static extern void ChangeToWindowedMode(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowTitle")]
			public static extern void SetWindowTitle(IntPtr window, string title);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCaptureMouse")]
			public static extern void CaptureMouse(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgReleaseMouse")]
			public static extern void ReleaseMouse(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetMonitorResolution")]
			public static extern void GetMonitorResolution(IntPtr window, out int width, out int height);

			[StructLayout(LayoutKind.Sequential)]
			public struct Callbacks
			{
				public CharacterEnteredCallback CharacterEntered;
				public DeadCharacterEnteredCallback DeadCharacterEntered;
				public KeyPressedCallback KeyPressed;
				public KeyReleasedCallback KeyReleased;
				public MouseWheelCallback MouseWheel;
				public MousePressedCallback MousePressed;
				public MouseReleasedCallback MouseReleased;
				public MouseMovedCallback MouseMoved;
				public MouseEnteredCallback MouseEntered;
				public MouseLeftCallback MouseLeft;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct Placement
			{
				public WindowMode Mode;
				public int X;
				public int Y;
				public int Width;
				public int Height;
			}
		}
	}
}