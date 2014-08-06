namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Controls;
	using Math;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     Represents the state of the mouse.
	/// </summary>
	public class Mouse : DisposableObject
	{
		/// <summary>
		///     Stores whether a button is currently being double-clicked.
		/// </summary>
		private readonly bool[] _doubleClicked = new bool[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The mouse button states.
		/// </summary>
		private readonly InputState[] _states = new InputState[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The window that generates the mouse events.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that generates the mouse events.</param>
		internal Mouse(Window window)
		{
			Assert.ArgumentNotNull(window);

			_window = window;
			_window.NativeWindow.MousePressed += OnButtonPressed;
			_window.NativeWindow.MouseReleased += OnButtonReleased;
		}

		/// <summary>
		///     Gets the position of the mouse.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				int x, y;
				NativeMethods.GetMousePosition(_window.NativeWindow.NativePtr, out x, out y);

				return new Vector2i(x, y);
			}
		}

		/// <summary>
		///     Raised when the mouse has been moved.
		/// </summary>
		public event Action<Vector2i> Moved
		{
			add { _window.NativeWindow.MouseMoved += value; }
			remove { _window.NativeWindow.MouseMoved -= value; }
		}

		/// <summary>
		///     Raised when the mouse wheel is scrolled.
		/// </summary>
		public event Action<int> Wheel
		{
			add { _window.NativeWindow.MouseWheel += value; }
			remove { _window.NativeWindow.MouseWheel -= value; }
		}

		/// <summary>
		///     Invoked when a button has been pressed.
		/// </summary>
		/// <param name="button">Identifies the mouse button that has been pressed.</param>
		/// <param name="doubleClick">Indicates whether the event represents a double click.</param>
		/// <param name="position">The position of the mouse at the time of the button press.</param>
		private void OnButtonPressed(MouseButton button, bool doubleClick, Vector2i position)
		{
			Assert.ArgumentInRange(button);

			_states[(int)button].KeyPressed();
			_doubleClicked[(int)button] |= doubleClick;

			var args = MouseEventArgs.Create(button, doubleClick, position, _states[(int)button]);
		}

		/// <summary>
		///     Invoked when a button has been released.
		/// </summary>
		/// <param name="button">Identifies the mouse button that has been pressed.</param>
		/// <param name="position">The position of the mouse at the time of the button press.</param>
		private void OnButtonReleased(MouseButton button, Vector2i position)
		{
			Assert.ArgumentInRange(button);
			_states[(int)button].KeyReleased();

			var args = MouseEventArgs.Create(button, false, position, _states[(int)button]);
		}

		/// <summary>
		///     Updates the mouse state.
		/// </summary>
		internal void Update()
		{
			for (var i = 0; i < _states.Length; ++i)
			{
				_states[i].Update();
				_doubleClicked[i] = false;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the button is currently being pressed down.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool IsPressed(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _states[(int)button].IsPressed;
		}

		/// <summary>
		///     Gets a value indicating whether the button is currently being double-clicked.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool IsDoubleClicked(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _doubleClicked[(int)button];
		}

		/// <summary>
		///     Gets a value indicating whether the button was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool WentDown(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _states[(int)button].WentDown;
		}

		/// <summary>
		///     Gets a value indicating whether the button was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool WentUp(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _states[(int)button].WentUp;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_window.NativeWindow.MousePressed -= OnButtonPressed;
			_window.NativeWindow.MouseReleased -= OnButtonReleased;
		}

		/// <summary>
		///     Provides access to the native mouse functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetMousePosition")]
			public static extern IntPtr GetMousePosition(IntPtr window, out int x, out int y);
		}
	}
}