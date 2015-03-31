namespace Pegasus.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using Controls;
	using Math;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents the state of the keyboard.
	/// </summary>
	internal class Keyboard : DisposableObject
	{
		/// <summary>
		///     The number of scan codes supported by the platform.
		/// </summary>
		internal const int ScanCodeCount = 512;

		/// <summary>
		///     Indicates whether all keyboard input events are handled by an UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> HandlesAllInputProperty = new DependencyProperty<bool>();

		/// <summary>
		///     The previously focused UI elements.
		/// </summary>
		private readonly List<UIElement> _focusedElements = new List<UIElement>();

		/// <summary>
		///     The key states.
		/// </summary>
		private readonly InputState[] _states = new InputState[ScanCodeCount];

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
			Window.NativeWindow.TextEntered += OnTextEntered;
		}

		/// <summary>
		///     Get or sets a value indicating whether text input is enabled for the currently focused window.
		/// </summary>
		internal static bool TextInputEnabled
		{
			get { return NativeMethods.IsTextInputEnabled(); }
			set { NativeMethods.EnableTextInput(value); }
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
				{
					_focusedElements.Add(value);
					CleanFocusedElements();
				}

				Log.DebugIf(false, "Focused element: {0}", _focusedElement.GetType().Name);
				Log.DebugIf(_focusedElements.Count > 32, "Unusually large focused elements history stack.");
			}
		}

		/// <summary>
		///     Changes the text input area.
		/// </summary>
		/// <param name="area">The new text input area.</param>
		internal static void ChangeTextInputArea(Rectangle area)
		{
			NativeMethods.SetTextInputArea(MathUtils.RoundIntegral(area.Left), MathUtils.RoundIntegral(area.Top),
				MathUtils.RoundIntegral(area.Width), MathUtils.RoundIntegral(area.Height));
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
		///     Raised when text was entered.
		/// </summary>
		public event Action<string> TextEntered;

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
			Window.NativeWindow.TextEntered -= OnTextEntered;
		}

		/// <summary>
		///     Invoked when text has been entered.
		/// </summary>
		/// <param name="text">The text that has been entered.</param>
		private void OnTextEntered(string text)
		{
			if (TextEntered != null)
				TextEntered(text);

			FocusedElement.RaiseEvent(UIElement.TextInputEvent, TextInputEventArgs.Create(text));
		}

		/// <summary>
		///     Invoked when a key has been released.
		/// </summary>
		/// <param name="key">Identifies the key that has been released.</param>
		/// <param name="scanCode">The scan code of the key that has been released.</param>
		private void OnKeyReleased(Key key, ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			_states[(int)scanCode].Released();

			var args = KeyEventArgs.Create(this, key, scanCode, _states[(int)scanCode]);
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
		private void OnKeyPressed(Key key, ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			_states[(int)scanCode].Pressed();

			var args = KeyEventArgs.Create(this, key, scanCode, _states[(int)scanCode]);
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
				var last = _focusedElements.Count - 1;
				var focusedElement = _focusedElements[last];
				_focusedElements.RemoveAt(last);

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
				var element = FocusedElement;
				while (element != Window && !element.CanBeFocused)
					element = element.LogicalParent;

				FocusedElement = element;
			}
		}

		/// <summary>
		///     Removes all previously focused elements that are no longer part of the visual tree.
		/// </summary>
		private void CleanFocusedElements()
		{
			for (var i = _focusedElements.Count; i > 0; --i)
			{
				if (!_focusedElements[i - 1].IsAttachedToRoot)
					_focusedElements.RemoveAt(i - 1);
			}
		}

		/// <summary>
		///     Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsPressed(Key key)
		{
			return IsPressed(key.ToScanCode());
		}

		/// <summary>
		///     Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			return WentDown(key.ToScanCode());
		}

		/// <summary>
		///     Gets a value indicating whether the key was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			return WentUp(key.ToScanCode());
		}

		/// <summary>
		///     Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///     when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			return IsRepeated(key.ToScanCode());
		}

		/// <summary>
		///     Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="scanCode">The scan code of the key that should be checked.</param>
		public bool IsPressed(ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			return _states[(int)scanCode].IsPressed;
		}

		/// <summary>
		///     Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="scanCode">The scan code of the key that should be checked.</param>
		public bool WentDown(ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			return _states[(int)scanCode].WentDown;
		}

		/// <summary>
		///     Gets a value indicating whether the key was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="scanCode">The scan code of the key that should be checked.</param>
		public bool WentUp(ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			return _states[(int)scanCode].WentUp;
		}

		/// <summary>
		///     Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///     when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="scanCode">The scan code of the key that should be checked.</param>
		public bool IsRepeated(ScanCode scanCode)
		{
			Assert.ArgumentInRange(scanCode);
			return _states[(int)scanCode].IsRepeated;
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