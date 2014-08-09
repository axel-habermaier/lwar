namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using Controls;
	using Platform.Memory;

	/// <summary>
	///     Manages logical inputs that are triggered by input triggers. The logical input device listens for the corresponding
	///     input events on its associated window, guaranteeing that it only sees inputs that have not been handled by some other
	///     UI element within the window. Therefore, the key and mouse button input states within the logical device might
	///     differ from the actual input states of the window's keyboard and mouse.
	/// </summary>
	public class LogicalInputDevice : DisposableObject
	{
		/// <summary>
		///     Stores whether a button is currently being double-clicked.
		/// </summary>
		private readonly bool[] _doubleClicked = new bool[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The logical inputs that are currently registered on the device.
		/// </summary>
		private readonly List<LogicalInput> _inputs = new List<LogicalInput>(64);

		/// <summary>
		///     The key states.
		/// </summary>
		private readonly InputState[] _keyStates = new InputState[Enum.GetValues(typeof(Key)).Length];

		/// <summary>
		///     The mouse button states.
		/// </summary>
		private readonly InputState[] _mouseButtonStates = new InputState[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The window that is associated with this logical device.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that should be associated with this logical device.</param>
		public LogicalInputDevice(Window window)
		{
			Assert.ArgumentNotNull(window);

			_window = window;
			_window.KeyDown += OnKeyDown;
			_window.KeyDown += OnKeyUp;
			_window.MouseDown += OnMouseDown;
			_window.MouseUp += OnMouseUp;
		}

		/// <summary>
		///     Gets or sets a value indicating whether the logical input device provides text input.
		/// </summary>
		internal bool TextInputEnabled { get; set; }

		/// <summary>
		///     Gets the keyboard that is associated with this logical device.
		/// </summary>
		public Keyboard Keyboard
		{
			get { return _window.Keyboard; }
		}

		/// <summary>
		///     Gets the mouse that is associated with this logical device.
		/// </summary>
		public Mouse Mouse
		{
			get { return _window.Mouse; }
		}

		/// <summary>
		///     Raised when a key has been pressed.
		/// </summary>
		internal event Action<KeyEventArgs> KeyPressed;

		/// <summary>
		///     Raised when a key has been released.
		/// </summary>
		internal event Action<KeyEventArgs> KeyReleased;

		/// <summary>
		///     Updates the mouse button state of the mouse button the event has been raised for.
		/// </summary>
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			_mouseButtonStates[(int)e.Button].Pressed();
			_doubleClicked[(int)e.Button] |= e.DoubleClick;
		}

		/// <summary>
		///     Updates the mouse button state of the mouse button the event has been raised for.
		/// </summary>
		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			_mouseButtonStates[(int)e.Button].Released();
		}

		/// <summary>
		///     Updates the key state of the key the event has been raised for.
		/// </summary>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			_keyStates[(int)e.Key].Pressed();

			if (KeyPressed != null)
				KeyPressed(e);
		}

		/// <summary>
		///     Updates the key state of the key the event has been raised for.
		/// </summary>
		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			_keyStates[(int)e.Key].Released();

			if (KeyReleased != null)
				KeyReleased(e);
		}

		/// <summary>
		///     Registers a logical input on the logical input device. Subsequently, the logical input's IsTriggered
		///     property can be used to determine whether the logical input is currently triggered.
		/// </summary>
		/// <param name="input">The logical input that should be registered on the device.</param>
		public void Add(LogicalInput input)
		{
			Assert.ArgumentNotNull(input);
			Assert.ArgumentSatisfies(!input.IsRegistered, "The input is already registered on a device.");

			_inputs.Add(input);
			input.SetLogicalDevice(this);
		}

		/// <summary>
		///     Removes the logical input from the logical input device.
		/// </summary>
		/// <param name="input">The logical input that should be removed.</param>
		public void Remove(LogicalInput input)
		{
			Assert.ArgumentNotNull(input);
			Assert.ArgumentSatisfies(input.IsRegistered, "The input trigger is not registered.");

			if (_inputs.Remove(input))
				input.SetLogicalDevice(null);
		}

		/// <summary>
		///     Updates the device state.
		/// </summary>
		internal void Update()
		{
			// Update all logical inputs
			foreach (var input in _inputs)
				input.Update(this);

			// Update all key states
			for (var i = 0; i < _keyStates.Length; ++i)
				_keyStates[i].Update();

			// Update all mouse button states
			for (var i = 0; i < _mouseButtonStates.Length; ++i)
				_mouseButtonStates[i].Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_window.KeyDown -= OnKeyDown;
			_window.KeyDown -= OnKeyUp;
			_window.MouseDown -= OnMouseDown;
			_window.MouseUp -= OnMouseUp;
		}

		/// <summary>
		///     Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsPressed(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].IsPressed;
		}

		/// <summary>
		///     Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].WentDown;
		}

		/// <summary>
		///     Gets a value indicating whether the key was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].WentUp;
		}

		/// <summary>
		///     Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///     when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].IsRepeated;
		}

		/// <summary>
		///     Gets a value indicating whether the button is currently being pressed down.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool IsPressed(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _mouseButtonStates[(int)button].IsPressed;
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
			return _mouseButtonStates[(int)button].WentDown;
		}

		/// <summary>
		///     Gets a value indicating whether the button was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool WentUp(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _mouseButtonStates[(int)button].WentUp;
		}
	}
}