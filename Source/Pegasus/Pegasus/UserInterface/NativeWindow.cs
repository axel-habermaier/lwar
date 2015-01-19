namespace Pegasus.UserInterface
{
	using System;
	using System.Runtime.InteropServices;
	using Controls;
	using Input;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Platform.SDL2;
	using Utilities;

	/// <summary>
	///     Represents a native operating system window.
	/// </summary>
	internal sealed class NativeWindow : DisposableObject
	{
		/// <summary>
		///     The window instances that have been allocated.
		/// </summary>
		private static readonly WindowData[] Windows = new WindowData[4];

		/// <summary>
		///     The index of the window.
		/// </summary>
		private readonly int _index;

		/// <summary>
		///     The native window instance.
		/// </summary>
		private readonly IntPtr _window;

		/// <summary>
		///     Invoked when a key was pressed.
		/// </summary>
		public Action<Key, ScanCode> KeyPressed;

		/// <summary>
		///     Invoked when a key was released.
		/// </summary>
		public Action<Key, ScanCode> KeyReleased;

		/// <summary>
		///     Invoked when the mouse was moved inside the window.
		/// </summary>
		public Action<Vector2> MouseMoved;

		/// <summary>
		///     Invoked when a mouse button was pressed.
		/// </summary>
		public Action<MouseButton, bool, Vector2> MousePressed;

		/// <summary>
		///     Invoked when a mouse button was released.
		/// </summary>
		public Action<MouseButton, Vector2> MouseReleased;

		/// <summary>
		///     Invoked when the mouse wheel was moved.
		/// </summary>
		public Action<int> MouseWheel;

		/// <summary>
		///     Invoked when text was entered.
		/// </summary>
		public Action<string> TextEntered;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="flags">The flags that should be used to open window.</param>
		internal NativeWindow(string title, Vector2 position, Size size, WindowFlags flags)
		{
			Assert.ArgumentNotNullOrWhitespace(title);

			flags |= WindowFlags.OpenGL;
			ConstrainWindowPlacement(ref position, ref size);

			_window = SDL_CreateWindow(title, position.IntegralX, position.IntegralY, size.IntegralWidth, size.IntegralHeight, flags);
			if (_window == IntPtr.Zero)
				Log.Die("Failed to create window: {0}.", NativeLibrary.GetError());

			Windows[_index = AllocateWindowIndex()] = new WindowData { SDLWindow = _window, NativeWindow = this };
			SDL_SetWindowMinimumSize(_window, Window.MinimumSize.IntegralWidth, Window.MinimumSize.IntegralHeight);
			SDL_SetWindowMaximumSize(_window, Window.MaximumSize.IntegralWidth, Window.MaximumSize.IntegralHeight);
		}

		/// <summary>
		///     Gets or sets a value indicating whether text input is currently enabled for the window.
		/// </summary>
		internal bool TextInputEnabled { get; set; }

		/// <summary>
		///     Gets a value indicating whether the window currently has the focus.
		/// </summary>
		internal bool Focused { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the user requested to close the window.
		/// </summary>
		internal bool IsClosing { get; private set; }

		/// <summary>
		///     Gets the native window instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get
			{
				Assert.NotDisposed(this);
				return _window;
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

				int w, h;
				SDL_GetWindowSize(_window, out w, out h);

				var width = MathUtils.Clamp(w, Window.MinimumSize.Width, Window.MaximumSize.Height);
				var height = MathUtils.Clamp(h, Window.MinimumSize.Height, Window.MaximumSize.Height);

				return new Size(width, height);
			}
		}

		/// <summary>
		///     Gets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2 Position
		{
			get
			{
				Assert.NotDisposed(this);

				int x, y;
				SDL_GetWindowPosition(_window, out x, out y);

				return new Vector2(x, y);
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

				var flags = SDL_GetWindowFlags(_window);

				if ((flags & WindowFlags.FullscreenDesktop) == WindowFlags.FullscreenDesktop)
					return WindowMode.Fullscreen;

				if ((flags & WindowFlags.Maximized) == WindowFlags.Maximized)
					return WindowMode.Maximized;

				if ((flags & WindowFlags.Minimized) == WindowFlags.Minimized)
					return WindowMode.Minimized;

				return WindowMode.Normal;
			}
		}

		/// <summary>
		///     Gets or sets the swap chain that belongs to the window.
		/// </summary>
		internal SwapChain SwapChain { get; set; }

		/// <summary>
		///     Gets the native platform handle of the window.
		/// </summary>
		internal IntPtr PlatformHandle
		{
			get
			{
				if (PlatformInfo.Platform != PlatformType.Windows)
					return IntPtr.Zero;

				var info = new SystemInfo { Version = VersionInfo.Required };
				if (!SDL_GetWindowWMInfo(_window, ref info))
					Log.Die("Failed to get the native window handle: {0}.", NativeLibrary.GetError());

				Assert.That(info.System == 1, "Expected a HWND.");
				return info.Handle;
			}
		}

		/// <summary>
		///     Handles the given platform event.
		/// </summary>
		internal unsafe void HandleEvent(ref Event e)
		{
			switch (e.EventType)
			{
				case EventType.Window:
					switch (e.Window.EventType)
					{
						case WindowEventType.Shown:
						case WindowEventType.Hidden:
						case WindowEventType.Exposed:
						case WindowEventType.Minimized:
						case WindowEventType.Maximized:
						case WindowEventType.Restored:
						case WindowEventType.Moved:
						case WindowEventType.Resized:
						case WindowEventType.Enter:
						case WindowEventType.Leave:
							// Don't care
							break;
						case WindowEventType.SizeChanged:
							if (SwapChain != null)
								SwapChain.Resize(new Size(e.Window.Data1, e.Window.Data2));
							break;
						case WindowEventType.FocusGained:
							Focused = true;
							break;
						case WindowEventType.FocusLost:
							Focused = false;
							break;
						case WindowEventType.Close:
							IsClosing = true;
							break;
						default:
							Log.Debug("Unsupported SDL window event.");
							break;
					}

					break;
				case EventType.KeyDown:
					if (KeyPressed != null)
						KeyPressed(e.Key.Key, e.Key.ScanCode);
					break;
				case EventType.KeyUp:
					if (KeyReleased != null)
						KeyReleased(e.Key.Key, e.Key.ScanCode);
					break;
				case EventType.TextInput:
					if (TextInputEnabled && TextEntered != null)
					{
						fixed (byte* text = e.Text.Text)
							TextEntered(StringMarshaler.ToManagedString(new IntPtr(text)));
					}
					break;
				case EventType.MouseMotion:
					if (MouseMoved != null && Mouse.RelativeMouseMode)
						MouseMoved(new Vector2(e.Motion.RelativeX, e.Motion.RelativeY));
					else if (MouseMoved != null)
						MouseMoved(new Vector2(e.Motion.X, e.Motion.Y));
					break;
				case EventType.MouseButtonDown:
					if (MousePressed != null)
						MousePressed(e.Button.Button, e.Button.Clicks == 2, new Vector2(e.Button.X, e.Button.Y));
					break;
				case EventType.MouseButtonUp:
					if (MousePressed != null)
						MouseReleased(e.Button.Button, new Vector2(e.Button.X, e.Button.Y));
					break;
				case EventType.MouseWheel:
					if (MouseWheel != null)
						MouseWheel(e.Wheel.Y);
					break;
				case EventType.TextEditing:
				case EventType.Quit:
					// Don't care
					break;
				default:
					Log.Debug("Unsupported SDL event.");
					break;
			}
		}

		/// <summary>
		///     Resets the closing state of all open windows.
		/// </summary>
		internal static void ResetClosingStates()
		{
			for (var i = 0; i < Windows.Length; ++i)
			{
				if (Windows[i].NativeWindow != null)
					Windows[i].NativeWindow.IsClosing = false;
			}
		}

		/// <summary>
		///     Allocates an unused window index for the window.
		/// </summary>
		private static int AllocateWindowIndex()
		{
			for (var i = 0; i < Windows.Length; ++i)
			{
				if (Windows[i].NativeWindow == null)
					return i;
			}

			Log.Die("Too many windows.");
			return -1;
		}

		/// <summary>
		///     Gets the window for the given window id.
		/// </summary>
		/// <param name="windowId">The id of the window that should be returned.</param>
		internal static NativeWindow Lookup(uint windowId)
		{
			var window = SDL_GetWindowFromID(windowId);
			if (window == IntPtr.Zero)
				return null;

			return Lookup(window);
		}

		/// <summary>
		///     Gets the window for the given SDL window.
		/// </summary>
		/// <param name="window">The SDL window the actual window should be returned for.</param>
		internal static NativeWindow Lookup(IntPtr window)
		{
			Assert.NotNull(window);

			for (var i = 0; i < Windows.Length; ++i)
			{
				if (Windows[i].SDLWindow == window)
					return Windows[i].NativeWindow;
			}

			Log.Die("Unable to find the NativeWindow instance for the given SDL2 window.");
			return null;
		}

		/// <summary>
		///     Constrains the placement and size of the window such that it is always visible.
		/// </summary>
		/// <param name="position">The position of the window.</param>
		/// <param name="size">The size of the window.</param>
		private static void ConstrainWindowPlacement(ref Vector2 position, ref Size size)
		{
			Assert.ArgumentInRange(position.X, -Window.MaximumSize.Width, Window.MaximumSize.Width);
			Assert.ArgumentInRange(position.Y, -Window.MaximumSize.Height, Window.MaximumSize.Height);
			Assert.ArgumentSatisfies(size.Width <= Window.MaximumSize.Width, "Invalid window width.");
			Assert.ArgumentSatisfies(size.Height <= Window.MaximumSize.Height, "Invalid window height.");

			var desktopArea = GetDesktopArea();

			// The window's size must not exceed the size of the desktop
			size.Width = MathUtils.Clamp(size.Width, Window.MinimumSize.Width, desktopArea.Width);
			size.Height = MathUtils.Clamp(size.Height, Window.MinimumSize.Height, desktopArea.Height);

			// Move the window's desired position such that at least MinOverlap pixels of the window are visible 
			// both vertically and horizontally
			position.X = MathUtils.Clamp(position.X, desktopArea.Left - size.Width + Window.MinimumOverlap,
				desktopArea.Left + desktopArea.Width - Window.MinimumOverlap);
			position.Y = MathUtils.Clamp(position.Y, desktopArea.Top - size.Height + Window.MinimumOverlap,
				desktopArea.Top + desktopArea.Height - Window.MinimumOverlap);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SDL_DestroyWindow(_window);

			Windows[_index].SDLWindow = IntPtr.Zero;
			Windows[_index].NativeWindow = null;
		}

		/// <summary>
		///     Sets the window into fullscreen mode.
		/// </summary>
		public void ChangeToFullscreenMode()
		{
			Assert.NotDisposed(this);
			Assert.That((SDL_GetWindowFlags(_window) & WindowFlags.FullscreenDesktop) != WindowFlags.FullscreenDesktop,
				"Window is already in fullscreen mode.");

			if (SDL_SetWindowFullscreen(_window, WindowFlags.FullscreenDesktop) != 0)
				Log.Die("Failed to switch to fullscreen mode: {0}.", NativeLibrary.GetError());

			SDL_SetWindowGrab(_window, true);
		}

		/// <summary>
		///     Sets the window into windowed mode.
		/// </summary>
		public void ChangeToWindowedMode()
		{
			Assert.NotDisposed(this);
			Assert.That((SDL_GetWindowFlags(_window) & WindowFlags.FullscreenDesktop) == WindowFlags.FullscreenDesktop,
				"Window is already in windowed mode.");

			if (SDL_SetWindowFullscreen(_window, 0) != 0)
				Log.Die("Failed to switch to windowed mode: {0}.", NativeLibrary.GetError());

			SDL_SetWindowGrab(_window, false);
		}

		/// <summary>
		///     Gets the area of the desktop.
		/// </summary>
		private static Rectangle GetDesktopArea()
		{
			int left = Int32.MaxValue, top = Int32.MaxValue, bottom = Int32.MinValue, right = Int32.MinValue;
			var num = SDL_GetNumVideoDisplays();

			if (num <= 0)
				Log.Die("Failed to determine the number of displays: {0}.", NativeLibrary.GetError());

			for (var i = 0; i < num; ++i)
			{
				Rect bounds;
				if (SDL_GetDisplayBounds(i, out bounds) != 0)
					Log.Die("Failed to retrieve display bounds of display {0}: {1}.", i, NativeLibrary.GetError());

				left = bounds.X < left ? bounds.X : left;
				right = bounds.X + bounds.Width > right ? bounds.X + bounds.Width : right;
				top = bounds.Y + top >= bounds.Y ? bounds.Y : top;
				bottom = bounds.Y + bounds.Height > bottom ? bounds.Y + bounds.Height : bottom;
			}

			return new Rectangle(left, top, right - left, bottom - top);
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_CreateWindow(
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string title, int x, int y, int w, int h,
			WindowFlags flags);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern WindowFlags SDL_GetWindowFlags(IntPtr window);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_SetWindowFullscreen(IntPtr window, WindowFlags flags);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_DestroyWindow(IntPtr window);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_SetWindowMaximumSize(IntPtr window, int maxWidth, int maxHeight);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_SetWindowMinimumSize(IntPtr window, int minWidth, int minHeight);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GetWindowPosition(IntPtr window, out int x, out int y);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GetWindowSize(IntPtr window, out int w, out int h);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_SetWindowGrab(IntPtr window, bool grabbed);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_GetWindowFromID(uint id);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool SDL_GetWindowWMInfo(IntPtr window, ref SystemInfo info);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GetNumVideoDisplays();

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GetDisplayBounds(int displayIndex, out Rect rect);

		/// <summary>
		///     Associates an SDL window with its NativeWindow instance.
		/// </summary>
		private struct WindowData
		{
			/// <summary>
			///     The NativeWindow instance.
			/// </summary>
			public NativeWindow NativeWindow;

			/// <summary>
			///     The SDL window instance.
			/// </summary>
			public IntPtr SDLWindow;
		}

		[StructLayout(LayoutKind.Sequential, Size = 64)]
		private struct SystemInfo
		{
			public VersionInfo Version;
			public readonly int System;
			public readonly IntPtr Handle;
		}
	}
}