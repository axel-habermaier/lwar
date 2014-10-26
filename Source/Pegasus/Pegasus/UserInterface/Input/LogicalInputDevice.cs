namespace Pegasus.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using Controls;
	using Math;
	using Platform.Memory;
	using Utilities;

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
		///     Indicates whether the preview input events are used to check for user input.
		/// </summary>
		private readonly bool _usePreviewEvents;

		/// <summary>
		///     The keyboard providing the keyboard input.
		/// </summary>
		private Keyboard _keyboard;

		/// <summary>
		///     The mouse providing the mouse input.
		/// </summary>
		private Mouse _mouse;

		/// <summary>
		///     The current mouse position.
		/// </summary>
		private Vector2 _mousePosition;

		/// <summary>
		///     The current normalized mouse position.
		/// </summary>
		private Vector2 _normalizedMousePosition;

		/// <summary>
		///     The UI element that is associated with this logical device.
		/// </summary>
		private UIElement _uiElement;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="usePreviewEvents">Indicates whether the preview input events should be used to check for user input.</param>
		public LogicalInputDevice(bool usePreviewEvents = false)
		{
			_usePreviewEvents = usePreviewEvents;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="element">The UI element that should be associated with this logical device.</param>
		/// <param name="usePreviewEvents">Indicates whether the preview input events should be used to check for user input.</param>
		public LogicalInputDevice(UIElement element, bool usePreviewEvents = false)
		{
			Assert.ArgumentNotNull(element);

			_usePreviewEvents = usePreviewEvents;
			UIElement = element;
		}

		/// <summary>
		///     Gets or sets the UI element that is associated with this logical device.
		/// </summary>
		public UIElement UIElement
		{
			get { return _uiElement; }
			set
			{
				if (_uiElement == value)
					return;

				CleanupEvents();
				_uiElement = value;
				SetupEvents();
			}
		}

		/// <summary>
		///     Gets the position of the mouse.
		/// </summary>
		public Vector2 MousePosition
		{
			get
			{
				if (!Mouse.InputIsCaptured)
					_mousePosition = Mouse.Position;

				return _mousePosition;
			}
		}

		/// <summary>
		///     Gets the position of the mouse normalized in both directions to the range -1..1 such
		///     that the origin lies at the center of the window.
		/// </summary>
		public Vector2 NormalizedMousePosition
		{
			get
			{
				if (!Mouse.InputIsCaptured)
					_normalizedMousePosition = Mouse.NormalizedPosition;

				return _normalizedMousePosition;
			}
		}

		/// <summary>
		///     The keyboard providing the keyboard input.
		/// </summary>
		private Keyboard Keyboard
		{
			get
			{
				Assert.NotNull(UIElement);
				return _keyboard ?? (_keyboard = UIElement.ParentWindow.Keyboard);
			}
		}

		/// <summary>
		///     The mouse providing the mouse input.
		/// </summary>
		private Mouse Mouse
		{
			get
			{
				Assert.NotNull(UIElement);
				return _mouse ?? (_mouse = UIElement.ParentWindow.Mouse);
			}
		}

		/// <summary>
		///     Gets  a value indicating whether the logical input device provides text input.
		/// </summary>
		internal bool TextInputEnabled
		{
			get { return Keyboard.FocusedElement is ITextInputControl; }
		}

		/// <summary>
		///     Sets up the input event handling.
		/// </summary>
		private void SetupEvents()
		{
			if (UIElement != null && _usePreviewEvents)
			{
				UIElement.PreviewKeyDown += OnKeyDown;
				UIElement.PreviewKeyUp += OnKeyUp;
				UIElement.PreviewMouseDown += OnMouseDown;
				UIElement.PreviewMouseUp += OnMouseUp;
				UIElement.PreviewMouseWheel += OnMouseWheel;
				UIElement.PreviewMouseMove += OnMouseMoved;
			}
			else if (UIElement != null)
			{
				UIElement.KeyDown += OnKeyDown;
				UIElement.KeyUp += OnKeyUp;
				UIElement.MouseDown += OnMouseDown;
				UIElement.MouseUp += OnMouseUp;
				UIElement.MouseWheel += OnMouseWheel;
				UIElement.MouseMove += OnMouseMoved;
			}
		}

		/// <summary>
		///     Cleans up the input event handling.
		/// </summary>
		private void CleanupEvents()
		{
			if (UIElement != null && _usePreviewEvents)
			{
				UIElement.PreviewKeyDown -= OnKeyDown;
				UIElement.PreviewKeyUp -= OnKeyUp;
				UIElement.PreviewMouseDown -= OnMouseDown;
				UIElement.PreviewMouseUp -= OnMouseUp;
				UIElement.PreviewMouseWheel -= OnMouseWheel;
				UIElement.PreviewMouseMove -= OnMouseMoved;
			}
			else if (UIElement != null)
			{
				UIElement.KeyDown -= OnKeyDown;
				UIElement.KeyUp -= OnKeyUp;
				UIElement.MouseDown -= OnMouseDown;
				UIElement.MouseUp -= OnMouseUp;
				UIElement.MouseWheel -= OnMouseWheel;
				UIElement.MouseMove -= OnMouseMoved;
			}
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
			if (UIElement == null)
				return;

			// Update all logical inputs
			foreach (var input in _inputs)
				input.Update(this);

			// Update all key states
			for (var i = 0; i < _keyStates.Length; ++i)
			{
				// We might have missed a key up event for a pressed key, so update the input state accordingly
				if (_keyStates[i].IsPressed && !Keyboard.IsPressed((Key)i))
					_keyStates[i].Released();

				_keyStates[i].Update();
			}

			// Update all mouse button states
			for (var i = 0; i < _mouseButtonStates.Length; ++i)
			{
				// We might have missed a mouse up event for a pressed mouse button, so update the input state accordingly
				if (_mouseButtonStates[i].IsPressed && !Mouse.IsPressed((MouseButton)i))
					_mouseButtonStates[i].Released();

				_mouseButtonStates[i].Update();
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			CleanupEvents();
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