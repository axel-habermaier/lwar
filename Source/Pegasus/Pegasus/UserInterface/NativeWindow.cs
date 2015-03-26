namespace Pegasus.UserInterface
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;
	using Controls;
	using Input;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a native operating system window.
	/// </summary>
	internal sealed unsafe class NativeWindow : DisposableObject
	{
		/// <summary>
		///     The native window interface.
		/// </summary>
		private readonly WindowInterface* _windowInterface;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">The mode that should be used to open window.</param>
		internal NativeWindow(string title, Vector2 position, Size size, WindowMode mode)
		{
			Assert.ArgumentNotNullOrWhitespace(title);
			Assert.ArgumentInRange(mode);

			_callbacks = new NativeMethods.WindowCallbacks
			{
				TextEntered = OnTextEntered,
				KeyPressed = OnKeyPressed,
				KeyReleased = OnKeyReleased,
				MouseWheel = OnMouseWheel,
				MousePressed = OnMousePressed,
				MouseReleased = OnMouseReleased,
				MouseMoved = OnMouseMoved
			};

			_windowInterface = NativeMethods.CreateWindowInterface(ref _callbacks);

			var x = position.IntegralX;
			var y = position.IntegralY;
			var width = size.IntegralWidth;
			var height = size.IntegralHeight;
			var titlePtr = Marshal.StringToHGlobalAnsi(title);

			_windowInterface->Open(titlePtr.ToPointer(), x, y, width, height, (int)mode);
			Marshal.FreeHGlobal(titlePtr);
		}

		/// <summary>
		///     Gets a value indicating whether the window currently has the focus.
		/// </summary>
		internal bool HasFocus
		{
			get
			{
				Assert.NotDisposed(this);
				return _windowInterface->HasFocus();
			}
		}

		/// <summary>
		///     Gets a value indicating whether the user requested to close the window.
		/// </summary>
		internal bool IsClosing
		{
			get
			{
				Assert.NotDisposed(this);
				return _windowInterface->IsClosing();
			}
		}

		/// <summary>
		///     Gets the native window instance.
		/// </summary>
		internal void* NativePtr
		{
			get
			{
				Assert.NotDisposed(this);
				return _windowInterface->_this;
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
				_windowInterface->GetSize(&w, &h);

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
				_windowInterface->GetPosition(&x, &y);

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
				return (WindowMode)_windowInterface->GetMode();
			}
		}

		/// <summary>
		///     Gets or sets the swap chain that belongs to the window.
		/// </summary>
		internal SwapChain SwapChain { get; set; }

		/// <summary>
		///     Invoked when a key was pressed.
		/// </summary>
		public event Action<Key, ScanCode> KeyPressed;

		/// <summary>
		///     Invoked when a key was released.
		/// </summary>
		public event Action<Key, ScanCode> KeyReleased;

		/// <summary>
		///     Invoked when the mouse was moved inside the window.
		/// </summary>
		public event Action<Vector2> MouseMoved;

		/// <summary>
		///     Invoked when a mouse button was pressed.
		/// </summary>
		public event Action<MouseButton, bool, Vector2> MousePressed;

		/// <summary>
		///     Invoked when a mouse button was released.
		/// </summary>
		public event Action<MouseButton, Vector2> MouseReleased;

		/// <summary>
		///     Invoked when the mouse wheel was moved.
		/// </summary>
		public event Action<int> MouseWheel;

		/// <summary>
		///     Invoked when text was entered.
		/// </summary>
		public event Action<string> TextEntered;

		/// <summary>
		///     Raises the character entered event.
		/// </summary>
		/// <param name="text">The text that has been entered.</param>
		private void OnTextEntered(byte* text)
		{
			var count = 0;
			while (text[count] != 0)
				++count;

			if (TextEntered != null)
				TextEntered(new string((sbyte*)text, 0, count, Encoding.UTF8));
		}

		/// <summary>
		///     Raises the key pressed event.
		/// </summary>
		/// <param name="key">The key that has been pressed.</param>
		/// <param name="scanCode">The scan code of the key.</param>
		private void OnKeyPressed(int key, int scanCode)
		{
			if (KeyPressed != null)
				KeyPressed((Key)key, (ScanCode)scanCode);
		}

		/// <summary>
		///     Raises the key released event.
		/// </summary>
		/// <param name="key">The key that has been released.</param>
		/// <param name="scanCode">The scan code of the key.</param>
		private void OnKeyReleased(int key, int scanCode)
		{
			if (KeyReleased != null)
				KeyReleased((Key)key, (ScanCode)scanCode);
		}

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
		///     Raises the mouse pressed event.
		/// </summary>
		/// <param name="button">The mouse button that has been pressed.</param>
		/// <param name="doubleClick">Indicates whether the event represents a double click.</param>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMousePressed(int button, bool doubleClick, int x, int y)
		{
			if (MousePressed != null)
				MousePressed((MouseButton)button, doubleClick, new Vector2(x, y));
		}

		/// <summary>
		///     Raises the mouse released event.
		/// </summary>
		/// <param name="button">The mouse button that has been released.</param>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMouseReleased(int button, int x, int y)
		{
			if (MouseReleased != null)
				MouseReleased((MouseButton)button, new Vector2(x, y));
		}

		/// <summary>
		///     Raises the mouse moved event.
		/// </summary>
		/// <param name="x">The X coordinate of the mouse.</param>
		/// <param name="y">The Y coordinate of the mouse.</param>
		private void OnMouseMoved(int x, int y)
		{
			if (MouseMoved != null)
				MouseMoved(new Vector2(x, y));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.FreeWindowInterface(_windowInterface);
		}

		/// <summary>
		///     Sets the window into fullscreen mode.
		/// </summary>
		public void ChangeToFullscreenMode()
		{
			Assert.NotDisposed(this);
			_windowInterface->ChangeToFullscreenMode();
		}

		/// <summary>
		///     Sets the window into windowed mode.
		/// </summary>
		public void ChangeToWindowedMode()
		{
			Assert.NotDisposed(this);
			_windowInterface->ChangeToWindowedMode();
		}

		/// <summary>
		///     The window callbacks that have been passed to the native code. Even though Resharper claims that these fields
		///     are useless, we must in fact keep references to the delegates in order to prevent the garbage collector
		///     from freeing them while they are still being used by native code.
		/// </summary>
		// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
		// ReSharper disable NotAccessedField.Local
		private readonly NativeMethods.WindowCallbacks _callbacks;
	}
}