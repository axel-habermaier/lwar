﻿using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;
	using System.Security;
	using Input;
	using Logging;
	using Math;
	using Memory;

	/// <summary>
	///   The window of the application.
	/// </summary>
	public sealed class Window : DisposableObject
	{
		/// <summary>
		///   The minimal window size supported by the library.
		/// </summary>
		public static readonly Size MinimumSize = new Size(800, 600);

		/// <summary>
		///   The maximal window size supported by the library.
		/// </summary>
		public static readonly Size MaximumSize = new Size(4096, 2160);

		/// <summary>
		///   The window callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.Callbacks _callbacks;

		/// <summary>
		///   The native window instance.
		/// </summary>
		private readonly IntPtr _window;

		/// <summary>
		///   A value indicating whether the mouse is currently captured by the window.
		/// </summary>
		private bool _mouseCaptured;

		/// <summary>
		///   The current placement of the window.
		/// </summary>
		private NativeMethods.Placement _placement;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="state">Indicates the window state.</param>
		internal Window(string title, Vector2i position, Size size, WindowState state)
		{
			Log.Info(LogCategory.Platform, "Initializing window...");

			_callbacks = new NativeMethods.Callbacks
			{
				Closing = () => Closing(),
				Closed = () => Closed(),
				LostFocus = () => LostFocus(),
				GainedFocus = () => GainedFocus(),
				CharacterEntered = (c, s) =>
					{
						if (s != PlatformInfo.ConsoleKey)
							CharacterEntered((char)c);
					},
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

			_placement = new NativeMethods.Placement
			{
				State = state,
				X = position.X,
				Y = position.Y,
				Width = size.Width,
				Height = size.Height
			};

			_window = NativeMethods.OpenWindow(title, _placement, _callbacks);
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
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				NativeMethods.SetWindowTitle(_window, value);
			}
		}

		/// <summary>
		///   Gets or sets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get
			{
				Assert.NotDisposed(this);
				return new Size(_placement.Width, _placement.Height);
			}
			set
			{
				Assert.NotDisposed(this);
				NativeMethods.SetWindowSize(_window, value.Width, value.Height);
			}
		}

		/// <summary>
		///   Gets or sets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				Assert.NotDisposed(this);
				return new Vector2i(_placement.X, _placement.Y);
			}
			set
			{
				Assert.NotDisposed(this);
				NativeMethods.SetWindowPosition(_window, value.X, value.Y);
			}
		}

		/// <summary>
		///   Gets or sets the window state.
		/// </summary>
		public WindowState State
		{
			get
			{
				Assert.NotDisposed(this);
				return _placement.State;
			}
			set
			{
				Assert.NotDisposed(this);
				NativeMethods.SetWindowState(_window, value);
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
		///   Processes all pending window events.
		/// </summary>
		public void ProcessEvents()
		{
			Assert.NotDisposed(this);

			NativeMethods.ProcessWindowEvents(_window);
			NativeMethods.GetWindowPlacement(_window, out _placement);
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
			public delegate void CharacterEnteredCallback(ushort character, int scanCode);

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

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgOpenWindow")]
			public static extern IntPtr OpenWindow(string title, Placement placement, Callbacks callbacks);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCloseWindow")]
			public static extern void CloseWindow(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgProcessWindowEvents")]
			public static extern void ProcessWindowEvents(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetWindowPlacement")]
			public static extern void GetWindowPlacement(IntPtr window, out Placement placement);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowSize")]
			public static extern void SetWindowSize(IntPtr window, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowPosition")]
			public static extern void SetWindowPosition(IntPtr window, int x, int y);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowState")]
			public static extern void SetWindowState(IntPtr window, WindowState state);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetWindowTitle")]
			public static extern void SetWindowTitle(IntPtr window, string title);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCaptureMouse")]
			public static extern void CaptureMouse(IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgReleaseMouse")]
			public static extern void ReleaseMouse(IntPtr window);

			[StructLayout(LayoutKind.Sequential)]
			public struct Callbacks
			{
				public ClosingCallback Closing;
				public ClosedCallback Closed;
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

			[StructLayout(LayoutKind.Sequential)]
			public struct Placement
			{
				public WindowState State;
				public int X;
				public int Y;
				public int Width;
				public int Height;
			}
		}
	}
}