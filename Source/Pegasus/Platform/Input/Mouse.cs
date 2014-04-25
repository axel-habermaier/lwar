namespace Pegasus.Platform.Input
{
	using System;
	using System.Runtime.InteropServices;
	using Math;
	using Memory;

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
		private readonly NativeWindow _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that generates the mouse events.</param>
		internal Mouse(NativeWindow window)
		{
			Assert.ArgumentNotNull(window);

			_window = window;
			_window.MousePressed += ButtonPressed;
			_window.MouseReleased += ButtonReleased;
		}

		/// <summary>
		///     Gets the position of the mouse.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				int x, y;
				NativeMethods.GetMousePosition(_window.NativePtr, out x, out y);

				return new Vector2i(x, y);
			}
		}

		/// <summary>
		///     Raised when the mouse has been moved.
		/// </summary>
		public event Action<int, int> Moved
		{
			add { _window.MouseMoved += value; }
			remove { _window.MouseMoved -= value; }
		}

		/// <summary>
		///     Raised when the mouse wheel is scrolled.
		/// </summary>
		public event Action<int> Wheel
		{
			add { _window.MouseWheel += value; }
			remove { _window.MouseWheel -= value; }
		}

		/// <summary>
		///     Invoked when a button has been pressed.
		/// </summary>
		/// <param name="button">Identifies the mouse button that has been pressed.</param>
		private void ButtonPressed(MouseEventArgs button)
		{
			_states[(int)button.Button].KeyPressed();
			_doubleClicked[(int)button.Button] |= button.DoubleClick;
		}

		/// <summary>
		///     Invoked when a button has been released.
		/// </summary>
		/// <param name="button">Identifies the mouse button that has been released.</param>
		private void ButtonReleased(MouseEventArgs button)
		{
			_states[(int)button.Button].KeyReleased();
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
			_window.MousePressed -= ButtonPressed;
			_window.MouseReleased -= ButtonReleased;
		}

		/// <summary>
		///     Provides access to the native mouse functions.
		/// </summary>
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetMousePosition")]
			public static extern IntPtr GetMousePosition(IntPtr window, out int x, out int y);
		}
	}
}