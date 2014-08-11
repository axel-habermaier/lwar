namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
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
		///     Indicates whether all mouse input events are handled by an UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> HandlesAllInputProperty = new DependencyProperty<bool>();

		/// <summary>
		///     Stores whether a button is currently being double-clicked.
		/// </summary>
		private readonly bool[] _doubleClicked = new bool[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     A stack of hovered UI elements, with the topmost element in the visual tree at the bottom of the stack.
		/// </summary>
		private readonly Stack<UIElement> _hoveredElements = new Stack<UIElement>(32);

		/// <summary>
		///     The mouse button states.
		/// </summary>
		private readonly InputState[] _states = new InputState[Enum.GetValues(typeof(MouseButton)).Length];

		/// <summary>
		///     The window that generates the mouse events.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///     The UI element the mouse is currently over. Null if the mouse is not over any UI element.
		/// </summary>
		private UIElement _hoveredElement;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Mouse()
		{
			HandlesAllInputProperty.Changed += OnHandlesAllInput;
		}

		/// <summary>
		///     Initializes a new instance for testing purposes.
		/// </summary>
		internal Mouse()
		{
		}

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
			_window.NativeWindow.MouseWheel += OnWheel;
			_window.NativeWindow.MouseMoved += OnMove;
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
		///     Ensures that the UI element handles all keyboard input events.
		/// </summary>
		private static void OnHandlesAllInput(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var element = obj as UIElement;
			if (element == null)
				return;

			if (args.NewValue)
			{
				element.MouseDown += HandleEvent;
				element.MouseUp += HandleEvent;
				element.MouseLeftButtonDown += HandleEvent;
				element.MouseLeftButtonUp += HandleEvent;
				element.MouseRightButtonDown += HandleEvent;
				element.MouseRightButtonDown += HandleEvent;
			}
			else
			{
				element.MouseDown -= HandleEvent;
				element.MouseUp -= HandleEvent;
				element.MouseLeftButtonDown -= HandleEvent;
				element.MouseLeftButtonUp -= HandleEvent;
				element.MouseRightButtonDown -= HandleEvent;
				element.MouseRightButtonUp -= HandleEvent;
			}
		}

		/// <summary>
		///     Marks the event as handled.
		/// </summary>
		private static void HandleEvent(object sender, MouseButtonEventArgs e)
		{
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
		///     Raised when the mouse has been moved within the bounds of the window.
		/// </summary>
		public event Action<Vector2i> Moved
		{
			add { _window.NativeWindow.MouseMoved += value; }
			remove { _window.NativeWindow.MouseMoved -= value; }
		}

		/// <summary>
		///     Raised when the mouse wheel is scrolled within the bounds of the window.
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

			_states[(int)button].Pressed();
			_doubleClicked[(int)button] |= doubleClick;

			if (_hoveredElement == null)
				return;

			var args = MouseButtonEventArgs.Create(this, position, _states, button, doubleClick, _window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseDownEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseDownEvent, args);

			if (button == MouseButton.Left)
			{
				_hoveredElement.RaiseEvent(UIElement.PreviewMouseLeftButtonDownEvent, args);
				_hoveredElement.RaiseEvent(UIElement.MouseLeftButtonDownEvent, args);
			}

			if (button == MouseButton.Right)
			{
				_hoveredElement.RaiseEvent(UIElement.PreviewMouseRightButtonDownEvent, args);
				_hoveredElement.RaiseEvent(UIElement.MouseRightButtonDownEvent, args);
			}
		}

		/// <summary>
		///     Invoked when a button has been released.
		/// </summary>
		/// <param name="button">Identifies the mouse button that has been pressed.</param>
		/// <param name="position">The position of the mouse at the time of the button press.</param>
		private void OnButtonReleased(MouseButton button, Vector2i position)
		{
			Assert.ArgumentInRange(button);
			_states[(int)button].Released();

			if (_hoveredElement == null)
				return;

			var args = MouseButtonEventArgs.Create(this, position, _states, button, false, _window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseUpEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseUpEvent, args);

			if (button == MouseButton.Left)
			{
				_hoveredElement.RaiseEvent(UIElement.PreviewMouseLeftButtonUpEvent, args);
				_hoveredElement.RaiseEvent(UIElement.MouseLeftButtonUpEvent, args);
			}

			if (button == MouseButton.Right)
			{
				_hoveredElement.RaiseEvent(UIElement.PreviewMouseRightButtonUpEvent, args);
				_hoveredElement.RaiseEvent(UIElement.MouseRightButtonUpEvent, args);
			}
		}

		/// <summary>
		///     Invoked when the mouse wheel has been changed.
		/// </summary>
		/// <param name="delta">A value indicating the amount the mouse wheel has changed.</param>
		private void OnWheel(int delta)
		{
			if (_hoveredElement == null)
				return;

			var args = MouseWheelEventArgs.Create(this, Position, _states, delta, _window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseWheelEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseWheelEvent, args);
		}

		/// <summary>
		///     Invoked when the mouse has been moved.
		/// </summary>
		/// <param name="position">The new position of the mouse.</param>
		private void OnMove(Vector2i position)
		{
			UpdateHoveredElement(position);

			if (_hoveredElement == null)
				return;

			var args = MouseEventArgs.Create(this, position, _states, _window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseMoveEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseMoveEvent, args);
		}

		/// <summary>
		///     Updates the hovered UI element, if necessary.
		/// </summary>
		/// <param name="position">The position of the mouse.</param>
		private void UpdateHoveredElement(Vector2i position)
		{
			Assert.That((_hoveredElement == null && _hoveredElements.Count == 0) ||
						(_hoveredElement != null && _hoveredElements.Count != 0), "Invalid hovered elements state.");

			var hoveredElement = _window.HitTest(new Vector2d(position.X, position.Y));
			if (hoveredElement == _hoveredElement)
				return;

			var args = MouseEventArgs.Create(this, Position, _states, _window.Keyboard.GetModifiers());

			if (_hoveredElement != null)
				UnsetIsMouseOver(_hoveredElement, args);

			_hoveredElement = hoveredElement;

			if (_hoveredElement != null)
				SetIsMouseOver(args);
		}

		/// <summary>
		///     Sets the IsMouseOver property and raises the MouseEnter event of all currently hovered UI
		///     elements that are in the path of the hovered element to the root.
		/// </summary>
		/// <param name="args">The event arguments that should be passed to the MouseLeaveEvent.</param>
		private void SetIsMouseOver(MouseEventArgs args)
		{
			var hoveredElement = _hoveredElement;
			while (hoveredElement != null)
			{
				if (!hoveredElement.IsMouseOver)
				{
					hoveredElement.IsMouseOver = true;
					hoveredElement.RaiseEvent(UIElement.MouseEnterEvent, args);
				}

				_hoveredElements.Push(hoveredElement);
				hoveredElement = hoveredElement.Parent;
			}
		}

		/// <summary>
		///     Unsets the IsMouseOver property and raises the MouseLeave event of all currently hovered UI
		///     elements that are not in the path of the given UI element to the root.
		/// </summary>
		/// <param name="uiElement">The UI element that starts the path to the root.</param>
		/// <param name="args">The event arguments that should be passed to the MouseLeaveEvent.</param>
		private void UnsetIsMouseOver(UIElement uiElement, MouseEventArgs args)
		{
			if (uiElement.Parent != null)
				UnsetIsMouseOver(uiElement.Parent, args);

			if (_hoveredElements.Count == 0)
				return;

			var topmostElement = _hoveredElements.Pop();
			if (uiElement == topmostElement)
				return;

			topmostElement.IsMouseOver = false;
			foreach (var hoveredElement in _hoveredElements)
			{
				hoveredElement.IsMouseOver = false;
				hoveredElement.RaiseEvent(UIElement.MouseLeaveEvent, args);
			}

			_hoveredElements.Clear();
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

			// We have to check every frame for a new hovered element, as the UI might change at any time
			// (due to animations, UI elements becoming visible/invisible, etc.).
			UpdateHoveredElement(Position);
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
			_window.NativeWindow.MouseWheel -= OnWheel;
			_window.NativeWindow.MouseMoved -= OnMove;
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