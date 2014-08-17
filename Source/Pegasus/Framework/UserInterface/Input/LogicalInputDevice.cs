namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using Controls;
	using Platform.Memory;

	/// <summary>
	///     Manages logical inputs that are triggered by input triggers. The logical input device listens for the corresponding
	///     input events on its associated UI element, guaranteeing that it only sees inputs that have not already been handled by
	///     some other UI element within the element tree. Therefore, the key and mouse button input states within the logical
	///     device might differ from the actual input states of the window's keyboard and mouse.
	/// </summary>
	public class LogicalInputDevice : DisposableObject
	{
		/// <summary>
		///     Stores whether a button is currently being double-clicked.
		/// </summary>
		private readonly bool[] _doubleClicked = new bool[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The UI element that is associated with this logical device.
		/// </summary>
		private readonly UIElement _element;

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
		///     The keyboard providing the keyboard input.
		/// </summary>
		private Keyboard _keyboard;

		/// <summary>
		///     The mouse providing the mouse input.
		/// </summary>
		private Mouse _mouse;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="element">The UI element that should be associated with this logical device.</param>
		public LogicalInputDevice(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			_element = element;
			_element.KeyDown += OnKeyDown;
			_element.KeyUp += OnKeyUp;
			_element.MouseDown += OnMouseDown;
			_element.MouseUp += OnMouseUp;
			_element.MouseWheel += OnMouseWheel;
			_element.MouseMove += OnMouseMoved;
		}

		/// <summary>
		///     The keyboard providing the keyboard input.
		/// </summary>
		private Keyboard Keyboard
		{
			get { return (_keyboard ?? (_keyboard = _element.ParentWindow.Keyboard)); }
		}

		/// <summary>
		///     The mouse providing the mouse input.
		/// </summary>
		private Mouse Mouse
		{
			get { return (_mouse ?? (_mouse = _element.ParentWindow.Mouse)); }
		}

		/// <summary>
		///     Gets  a value indicating whether the logical input device provides text input.
		/// </summary>
		internal bool TextInputEnabled
		{
			get { return Keyboard.FocusedElement is ITextInputControl; }
		}

		/// <summary>
		///     Raised when a key has been pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed;

		/// <summary>
		///     Raised when a key has been released.
		/// </summary>
		public event Action<KeyEventArgs> KeyReleased;

		/// <summary>
		///     Raised when a mouse button was pressed.
		/// </summary>
		public event Action<MouseButtonEventArgs> MouseButtonPressed;

		/// <summary>
		///     Raised when a mouse button was released.
		/// </summary>
		public event Action<MouseButtonEventArgs> MouseButtonReleased;

		/// <summary>
		///     Raised when the mouse was moved.
		/// </summary>
		public event Action<MouseEventArgs> MouseMoved;

		/// <summary>
		///     Raised when the mouse wheel changed.
		/// </summary>
		public event Action<MouseWheelEventArgs> MouseWheel;

		/// <summary>
		///     Raises the mouse wheel event.
		/// </summary>
		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (MouseWheel != null)
				MouseWheel(e);
		}

		/// <summary>
		///     Raises the mouse move event.
		/// </summary>
		private void OnMouseMoved(object sender, MouseEventArgs e)
		{
			if (MouseMoved != null)
				MouseMoved(e);
		}

		/// <summary>
		///     Updates the mouse button state of the mouse button the event has been raised for.
		/// </summary>
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			_mouseButtonStates[(int)e.Button].Pressed();
			_doubleClicked[(int)e.Button] |= e.DoubleClick;

			if (MouseButtonPressed != null)
				MouseButtonPressed(e);
		}

		/// <summary>
		///     Updates the mouse button state of the mouse button the event has been raised for.
		/// </summary>
		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			_mouseButtonStates[(int)e.Button].Released();

			if (MouseButtonReleased != null)
				MouseButtonReleased(e);
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
		public void Update()
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
			_element.KeyDown -= OnKeyDown;
			_element.KeyUp -= OnKeyUp;
			_element.MouseDown -= OnMouseDown;
			_element.MouseUp -= OnMouseUp;
			_element.MouseWheel -= OnMouseWheel;
			_element.MouseMove -= OnMouseMoved;
		}

		/// <summary>
		///     Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsPressed(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].IsPressed && Keyboard.IsPressed(key);
		}

		/// <summary>
		///     Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].WentDown && Keyboard.WentDown(key);
		}

		/// <summary>
		///     Gets a value indicating whether the key was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].WentUp && Keyboard.WentUp(key);
		}

		/// <summary>
		///     Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///     when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			Assert.ArgumentInRange(key);
			return _keyStates[(int)key].IsRepeated && Keyboard.IsRepeated(key);
		}

		/// <summary>
		///     Gets a value indicating whether the button is currently being pressed down.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool IsPressed(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _mouseButtonStates[(int)button].IsPressed && Mouse.IsPressed(button);
		}

		/// <summary>
		///     Gets a value indicating whether the button is currently being double-clicked.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool IsDoubleClicked(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _doubleClicked[(int)button] && Mouse.IsDoubleClicked(button);
		}

		/// <summary>
		///     Gets a value indicating whether the button was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool WentDown(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _mouseButtonStates[(int)button].WentDown && Mouse.WentDown(button);
		}

		/// <summary>
		///     Gets a value indicating whether the button was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="button">The button that should be checked.</param>
		public bool WentUp(MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return _mouseButtonStates[(int)button].WentUp && Mouse.WentUp(button);
		}
	}
}