using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;
	using System.Security;
	using Input;
	using Math;

	/// <summary>
	///   The window of the application.
	/// </summary>
	public sealed class Window : DisposableObject
	{
		/// <summary>
		///   The window parameters that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.WindowParams _params;

		/// <summary>
		///   The native window instance.
		/// </summary>
		private readonly IntPtr _window;

		/// <summary>
		///   A value indicating whether the mouse is currently captured by the window.
		/// </summary>
		private bool _mouseCaptured;

		/// <summary>
		///   The size of the rendering area of the window.
		/// </summary>
		private Size _size;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		internal Window()
		{
			Log.Info("Initializing window...");

			_params = new NativeMethods.WindowParams
			{
				Width = 640,
				Height = 480,
				Title = "Pegasus",
				Closing = () => Closing(),
				Closed = () => Closed(),
				Resized = (w, h) =>
					{
						_size = new Size(w, h);
						Resized(_size);
					},
				LostFocus = () => LostFocus(),
				GainedFocus = () => GainedFocus(),
				CharacterEntered = c => CharacterEntered((char)c),
				KeyPressed = (k, s) =>
					{
						Assert.InRange(k);
						KeyPressed(new KeyEventArgs(k, s));
					},
				KeyReleased = (k, s) =>
					{
						Assert.InRange(k);
						KeyReleased(new KeyEventArgs(k, s));
					},
				MouseWheel = d => MouseWheel(d),
				MousePressed = (m, x, y) =>
					{
						Assert.InRange(m);
						MousePressed(new MouseEventArgs(m, x, y));
					},
				MouseReleased = (m, x, y) =>
					{
						Assert.InRange(m);
						MouseReleased(new MouseEventArgs(m, x, y));
					},
				MouseMoved = (x, y) => MouseMoved(x, y),
				MouseEntered = () => MouseEntered(),
				MouseLeft = () => MouseLeft(),
			};

			_window = NativeMethods.OpenWindow(ref _params);
		}

		/// <summary>
		///   Gets the native window instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _window; }
		}

		/// <summary>
		///   Sets the window's title.
		/// </summary>
		public string Title
		{
			set
			{
				Assert.ArgumentNotNull(value, () => value);
				Assert.NotDisposed(this);

				NativeMethods.SetWindowTitle(_window, value);
			}
		}

		/// <summary>
		///   Gets or sets the size of the rendering area of the window.
		/// </summary>
		public Size Size
		{
			get
			{
				Assert.NotDisposed(this);

				int width, height;
				NativeMethods.GetWindowSize(_window, out width, out height);

				if (width == 0 || height == 0)
					return _size;

				return new Size(width, height);
			}
			set
			{
				Assert.NotDisposed(this);
				NativeMethods.SetWindowSize(_window, value.Width, value.Height);
				_size = new Size(value.Width, value.Height);
			}
		}

		/// <summary>
		///   Gets the width of the window's rendering area.
		/// </summary>
		public int Width
		{
			get { return Size.Width; }
		}

		/// <summary>
		///   Gets the height of the window's rendering area.
		/// </summary>
		public int Height
		{
			get { return Size.Height; }
		}

		/// <summary>
		///   Gets or sets a value indicating whether the mouse is currently captured by the window.
		/// </summary>
		public bool MouseCaptured
		{
			get { return _mouseCaptured; }
			set
			{
				_mouseCaptured = value;
				if (_mouseCaptured)
					NativeMethods.CaptureMouse(_window);
				else
					NativeMethods.ReleaseMouse(_window);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.CloseWindow(_window);
		}

		/// <summary>
		///   Changes the window's icon
		/// </summary>
		/// <param name="width">The icon's width, in pixels.</param>
		/// <param name="height">The icon's height, in pixels.</param>
		/// <param name="pixels">An array of pixels of the appropriate size; format must be RGBA 32 bits.</param>
		public void SetIcon(uint width, uint height, byte[] pixels)
		{
			Assert.NotDisposed(this);
			throw new NotImplementedException();
		}

		/// <summary>
		///   Shows or hides the mouse cursor.
		/// </summary>
		/// <param name="show">True to show the cursor, false to hide it.</param>
		public void ShowCursor(bool show)
		{
			Assert.NotDisposed(this);
			throw new NotImplementedException();
		}

		/// <summary>
		///   Processes all pending window events.
		/// </summary>
		public void ProcessEvents()
		{
			Assert.NotDisposed(this);
			NativeMethods.ProcessWindowEvents(_window);
		}

		/// <summary>
		///   Raised when the user requested the window to be closed. The window is not actually closed
		///   until Dispose() is called.
		/// </summary>
		public event Action Closing = () => { };

		/// <summary>
		///   Raised when the window is about to be closed.
		/// </summary>
		public event Action Closed = () => { };

		/// <summary>
		///   Raised when the window was resized.
		/// </summary>
		public event Action<Size> Resized = s => { };

		/// <summary>
		///   Raised when the window lost focus.
		/// </summary>
		public event Action LostFocus = () => { };

		/// <summary>
		///   Raised when the window gained focus.
		/// </summary>
		public event Action GainedFocus = () => { };

		/// <summary>
		///   Raised when text was entered.
		/// </summary>
		public event Action<char> CharacterEntered = c => { };

		/// <summary>
		///   Raised when a key was pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed = e => { };

		/// <summary>
		///   Raised when a key was released.
		/// </summary>
		public event Action<KeyEventArgs> KeyReleased = e => { };

		/// <summary>
		///   Raised when the mouse wheel was moved.
		/// </summary>
		public event Action<int> MouseWheel = d => { };

		/// <summary>
		///   Raised when a mouse button was pressed.
		/// </summary>
		public event Action<MouseEventArgs> MousePressed = e => { };

		/// <summary>
		///   Raised when a mouse button was released.
		/// </summary>
		public event Action<MouseEventArgs> MouseReleased = e => { };

		/// <summary>
		///   Raised when the mouse was moved inside the window.
		/// </summary>
		public event Action<int, int> MouseMoved = (x, y) => { };

		/// <summary>
		///   Raised when the mouse entered the window.
		/// </summary>
		public event Action MouseEntered = () => { };

		/// <summary>
		///   Raised when the mouse left the window.
		/// </summary>
		public event Action MouseLeft = () => { };

		/// <summary>
		///   Provides access to the native window-related types and functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			public delegate void CharacterEnteredCallback(ushort character);

			public delegate void ClosedCallback();

			public delegate void ClosingCallback();

			public delegate void GainedFocusCallback();

			public delegate void KeyPressedCallback(Key key, int scanCode);

			public delegate void KeyReleasedCallback(Key key, int scanCode);

			public delegate void LostFocusCallback();

			public delegate void MouseEnteredCallback();

			public delegate void MouseLeftCallback();

			public delegate void MouseMovedCallback(int x, int y);

			public delegate void MousePressedCallback(MouseButton button, int x, int y);

			public delegate void MouseReleasedCallback(MouseButton button, int x, int y);

			public delegate void MouseWheelCallback(int delta);

			public delegate void ResizedCallback(int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgOpenWindow")]
			public static extern IntPtr OpenWindow(ref WindowParams windowParams);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCloseWindow")]
			public static extern void CloseWindow(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgProcessWindowEvents")]
			public static extern void ProcessWindowEvents(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetWindowSize")]
			public static extern void GetWindowSize(IntPtr window, out int width, out int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowSize")]
			public static extern void SetWindowSize(IntPtr window, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowTitle")]
			public static extern void SetWindowTitle(IntPtr window, string title);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCaptureMouse")]
			public static extern void CaptureMouse(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgReleaseMouse")]
			public static extern void ReleaseMouse(IntPtr window);

			[StructLayout(LayoutKind.Sequential)]
			public struct WindowParams
			{
				public int Width;
				public int Height;
				public string Title;

				public ClosingCallback Closing;
				public ClosedCallback Closed;
				public ResizedCallback Resized;
				public LostFocusCallback LostFocus;
				public GainedFocusCallback GainedFocus;
				public CharacterEnteredCallback CharacterEntered;
				public KeyPressedCallback KeyPressed;
				public KeyReleasedCallback KeyReleased;
				public MouseWheelCallback MouseWheel;
				public MousePressedCallback MousePressed;
				public MouseReleasedCallback MouseReleased;
				public MouseMovedCallback MouseMoved;
				public MouseEnteredCallback MouseEntered;
				public MouseLeftCallback MouseLeft;
			}
		}
	}
}