namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using Controls;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///     Represents the state of the keyboard.
	/// </summary>
	public class Keyboard : DisposableObject
	{
		/// <summary>
		///     Indicates whether all keyboard input events are handled by an UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> HandlesAllInputProperty = new DependencyProperty<bool>();

		/// <summary>
		///     The previously focused UI elements.
		/// </summary>
		private readonly Stack<UIElement> _focusedElements = new Stack<UIElement>();

		/// <summary>
		///     The key states.
		/// </summary>
		private readonly InputState[] _states = new InputState[Enum.GetValues(typeof(Key)).Length];

		/// <summary>
		///     The UI element that currently has the keyboard focus.
		/// </summary>
		private UIElement _focusedElement;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Keyboard()
		{
			UIElement.KeyUpEvent.Raised += HandleEvent;
			UIElement.KeyDownEvent.Raised += HandleEvent;
		}

		/// <summary>
		///     Initializes a new instance for testing purposes.
		/// </summary>
		internal Keyboard()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that generates the key events.</param>
		internal Keyboard(Window window)
		{
			Assert.ArgumentNotNull(window);

			Window = window;
			Window.NativeWindow.KeyPressed += OnKeyPressed;
			Window.NativeWindow.KeyReleased += OnKeyReleased;
			Window.NativeWindow.CharacterEntered += OnCharacterEntered;
			Window.NativeWindow.DeadCharacterEntered += OnDeadCharacterEntered;
		}

		/// <summary>
		///     Gets the window the keyboard is associated with.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///     Gets the UI element that currently has the keyboard focus. Unless the focus has been shifted to another UI
		///     element, it is the window itself.
		/// </summary>
		public UIElement FocusedElement
		{
			get
			{
				Assert.NotNull(_focusedElement);
				return _focusedElement;
			}
			internal set
			{
				if (_focusedElement == value)
					return;

				if (value == null)
					value = Window;

				if (_focusedElement != null)
					_focusedElement.IsFocused = false;

				_focusedElement = value;
				_focusedElement.IsFocused = true;

				if (_focusedElement != Window)
					_focusedElements.Push(value);

				Log.DebugIf(_focusedElements.Count > 32, "Unusually large focused elements history stack.");
			}
		}

		/// <summary>
		///     Marks the event as handled.
		/// </summary>
		private static void HandleEvent(object sender, KeyEventArgs e)
		{
			var element = sender as UIElement;
			if (element != null && GetHandlesAllInput(element))
				e.Handled = true;
		}

		/// <summary>
		///     Gets a value indicating whether all keyboard input events are handled by the UI element.
		/// </summary>
		/// <param name="element">The UI element that should be checked.</param>
		public static bool GetHandlesAllInput(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(HandlesAllInputProperty);
		}

		/// <summary>
		///     Sets a value indicating that all keyboard input events are handled by the UI element.
		/// </summary>
		/// <param name="element">The UI element that should handle all keyboard input events.</param>
		/// <param name="handlesAllInput">Indicates whether the UI element should handle all keyboard input events.</param>
		public static void SetHandlesAllInput(UIElement element, bool handlesAllInput)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(HandlesAllInputProperty, handlesAllInput);
		}

		/// <summary>
		///     Raised when a text character was entered.
		/// </summary>
		public event Action<char> CharacterEntered;

		/// <summary>
		///     Raised when a key was pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed;

		/// <summary>
		///     Raised when a key was released.
		/// </summary>
		public event Action<KeyEventArgs> KeyReleased;

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Window.NativeWindow.KeyPressed -= OnKeyPressed;
			Window.NativeWindow.KeyReleased -= OnKeyReleased;
			Window.NativeWindow.CharacterEntered -= OnCharacterEntered;
			Window.NativeWindow.DeadCharacterEntered -= OnDeadCharacterEntered;
		}

		/// <summary>
		///     Invoked when a character has been entered as the result of a dead key press.
		/// </summary>
		/// <param name="character">Identifies the character that has been entered.</param>
		/// <param name="cancel">
		///     If set to true, the dead character is removed such that the subsequently entered character is not
		///     influenced by the dead character.
		/// </param>
		private static void OnDeadCharacterEntered(CharacterEnteredEventArgs character, out bool cancel)
		{
			// Cancel the dead key if it is the result of the console key being pressed (happens, for instance, on German keyboard layouts)
			cancel = character.ScanCode == PlatformInfo.ConsoleKey;
		}

		/// <summary>
		///     Invoked when a character has been entered.
		/// </summary>
		/// <param name="character">Identifies the character that has been entered.</param>
		private void OnCharacterEntered(CharacterEnteredEventArgs character)
		{
			// Only raise the character entered event if the character is a printable ASCII character
			if (character.Character < 32 || character.Character > 255 || Char.IsControl(character.Character))
				return;

			// Do not raise the character entered event if the character is the result of the console key being pressed
			if (character.ScanCode == PlatformInfo.ConsoleKey)
				return;

			if (CharacterEntered != null)
				CharacterEntered(character.Character);

			FocusedElement.RaiseEvent(UIElement.TextInputEvent, TextInputEventArgs.Create(character.Character, character.ScanCode));
		}

		/// <summary>
		///     Invoked when a key has been released.
		/// </summary>
		/// <param name="key">Identifies the key that has been released.</param>
		/// <param name="scanCode">The scan code of the key that has been released.</param>
		private void OnKeyReleased(Key key, int scanCode)
		{
			Assert.ArgumentInRange(key);
			_states[(int)key].Released();

			var args = KeyEventArgs.Create(this, key, scanCode, _states[(int)key]);
			args.RoutedEvent = null;

			if (KeyReleased != null)
				KeyReleased(args);

			FocusedElement.RaiseEvent(UIElement.PreviewKeyUpEvent, args);
			FocusedElement.RaiseEvent(UIElement.KeyUpEvent, args);
		}

		/// <summary>
		///     Invoked when a key has been pressed.
		/// </summary>
		/// <param name="key">Identifies the key that has been pressed.</param>
		/// <param name="scanCode">The scan code of the key that has been pressed.</param>
		private void OnKeyPressed(Key key, int scanCode)
		{
			Assert.ArgumentInRange(key);
			_states[(int)key].Pressed();

			var args = KeyEventArgs.Create(this, key, scanCode, _states[(int)key]);
			args.RoutedEvent = null;

			if (KeyPressed != null)
				KeyPressed(args);

			FocusedElement.RaiseEvent(UIElement.PreviewKeyDownEvent, args);
			FocusedElement.RaiseEvent(UIElement.KeyDownEvent, args);
		}

		/// <summary>
		///     Updates the keyboard state.
		/// </summary>
		internal void Update()
		{
			for (var i = 0; i < _states.Length; ++i)
				_states[i].Update();

			// We have to check every frame whether the focused element must be reset; it could have been removed
			// or hidden since the last frame, among other things.
			if (FocusedElement != Window && !FocusedElement.CanBeFocused)
				ResetFocusedElement();
		}

		/// <summary>
		///     Resets the focused element to the first focusable previously focused element. If none exists, the
		///     closest focusable element in the tree is focused. Otherwise, the focus is set to the window.
		/// </summary>
		private void ResetFocusedElement()
		{
			// Focus the first previously focused element, if one exists
			while (_focusedElements.Count > 0)
			{
				var focusedElement = _focusedElements.Pop();
				if (!focusedElement.CanBeFocused)
					continue;

				FocusedElement = focusedElement;
				return;
			}

			// Check if the element is still a child of the window. If so, focus the closest focusable
			// element in the tree. Otherwise, focus the window.
			if (FocusedElement.ParentWindow == null)
				FocusedElement = Window;
			else
			{
				var uiElement = FocusedElement;
				while (uiElement != Window && !uiElement.CanBeFocused)
					uiElement = uiElement.LogicalParent;

				FocusedElement = uiElement;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsPressed(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].IsPressed;
		}

		/// <summary>
		///     Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].WentDown;
		}

		/// <summary>
		///     Gets a value indicating whether the key was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].WentUp;
		}

		/// <summary>
		///     Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///     when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].IsRepeated;
		}

		/// <summary>
		///     Gets the set of key modifiers that are currently pressed.
		/// </summary>
		public KeyModifiers GetModifiers()
		{
			var modifiers = KeyModifiers.None;

			if (IsPressed(Key.LeftAlt) || IsPressed(Key.RightAlt))
				modifiers |= KeyModifiers.Alt;

			if (IsPressed(Key.LeftControl) || IsPressed(Key.RightControl))
				modifiers |= KeyModifiers.Control;

			if (IsPressed(Key.LeftShift) || IsPressed(Key.RightShift))
				modifiers |= KeyModifiers.Shift;

			return modifiers;
		}
	}
}