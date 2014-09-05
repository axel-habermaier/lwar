namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Security;
	using Controls;
	using Math;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents the state of the mouse.
	/// </summary>
	internal class Mouse : DisposableObject
	{
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
		///     The UI element the mouse is currently over. Null if the mouse is not over any UI element.
		/// </summary>
		private UIElement _hoveredElement;

		/// <summary>
		///     Initializes a new instance for testing purposes.
		/// </summary>
		internal Mouse()
		{
		}

		/// <summary>
		/// Gets a value indicating whether the mouse input is currently captured by an UI element.
		/// </summary>
		internal bool InputIsCaptured { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that generates the mouse events.</param>
		internal Mouse(Window window)
		{
			Assert.ArgumentNotNull(window);

			Window = window;
			Window.NativeWindow.MousePressed += OnButtonPressed;
			Window.NativeWindow.MouseReleased += OnButtonReleased;
			Window.NativeWindow.MouseWheel += OnWheel;
			Window.NativeWindow.MouseMoved += OnMove;
		}

		/// <summary>
		///     Gets the position of the mouse.
		/// </summary>
		public Vector2i Position
		{
			get
			{
				int x, y;
				NativeMethods.GetMousePosition(Window.NativeWindow.NativePtr, out x, out y);

				return new Vector2i(x, y);
			}
		}

		/// <summary>
		///     Gets the position of the mouse normalized in both directions to the range -1..1 such
		///     that the origin lies at the center of the window.
		/// </summary>
		public Vector2 NormalizedPosition
		{
			get { return NormalizePosition(Position); }
		}

		/// <summary>
		///     Gets or sets the window the mouse is associated with.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///     Normalizes the given mouse position relative to size of the mouse's window such that both directions lie within the
		///     range -1..1, with the origin lying at the center of the window.
		/// </summary>
		internal Vector2 NormalizePosition(Vector2i position)
		{
			var size = Window.Size;
			var x = position.X - size.Width / 2;
			var y = position.Y - size.Height / 2;

			// It's important to do the division and conversion to float now and not earlier; otherwise,
			// there would be some drift if the width or height of the window is uneven.
			// This implementation matches what the platform library does when resetting the cursor to the
			// center of the window when the cursor is captured.
			return new Vector2(x / (float)size.Width, y / (float)size.Height);
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

			var args = MouseButtonEventArgs.Create(this, position, _states, button, doubleClick, Window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseDownEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseDownEvent, args);
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

			var args = MouseButtonEventArgs.Create(this, position, _states, button, false, Window.Keyboard.GetModifiers());
			_hoveredElement.RaiseEvent(UIElement.PreviewMouseUpEvent, args);
			_hoveredElement.RaiseEvent(UIElement.MouseUpEvent, args);
		}

		/// <summary>
		///     Invoked when the mouse wheel has been changed.
		/// </summary>
		/// <param name="delta">A value indicating the amount the mouse wheel has changed.</param>
		private void OnWheel(int delta)
		{
			if (_hoveredElement == null)
				return;

			var args = MouseWheelEventArgs.Create(this, Position, _states, delta, Window.Keyboard.GetModifiers());
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

			var args = MouseEventArgs.Create(this, position, _states, Window.Keyboard.GetModifiers());
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

			var hoveredElement = Window.HitTest(new Vector2d(position.X, position.Y), boundsTestOnly: false);
			if (hoveredElement == _hoveredElement)
				return;

			InputIsCaptured = false;
			var args = MouseEventArgs.Create(this, Position, _states, Window.Keyboard.GetModifiers());

			if (_hoveredElement != null)
				UnsetIsMouseOver(_hoveredElement, args);

			_hoveredElement = hoveredElement;
			Log.DebugIf(false, "Hovered element: {0}", _hoveredElement == null ? "None" : _hoveredElement.GetType().Name);

			if (_hoveredElement == null)
				return;

			SetIsMouseOver(args);
				
			var element = _hoveredElement;
			do
			{
				InputIsCaptured = InputManager.GetCapturesInput(element);
				element = element.VisualParent;
			} while (element != null && !InputIsCaptured);
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
				hoveredElement = hoveredElement.LogicalParent;
			}
		}

		/// <summary>
		///     Unsets the IsMouseOver property and raises the MouseLeave event of all currently hovered UI
		///     elements that are not in the path of the given UI element to the root.
		/// </summary>
		/// <param name="element">The UI element that starts the path to the root.</param>
		/// <param name="args">The event arguments that should be passed to the MouseLeaveEvent.</param>
		private void UnsetIsMouseOver(UIElement element, MouseEventArgs args)
		{
			if (element.LogicalParent != null)
				UnsetIsMouseOver(element.LogicalParent, args);

			if (_hoveredElements.Count == 0)
				return;

			var topmostElement = _hoveredElements.Pop();
			if (element == topmostElement)
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
			Window.NativeWindow.MousePressed -= OnButtonPressed;
			Window.NativeWindow.MouseReleased -= OnButtonReleased;
			Window.NativeWindow.MouseWheel -= OnWheel;
			Window.NativeWindow.MouseMoved -= OnMove;
		}

		/// <summary>
		///     Draws the mouse cursor.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the cursor.</param>
		internal void DrawCursor(SpriteBatch spriteBatch)
		{
			Assert.ArgumentNotNull(spriteBatch);

			// Check if the hovered element or any of its parents override the default cursor
			Cursor cursor = null;
			var element = Window.HitTest(new Vector2d(Position.X, Position.Y), boundsTestOnly: true);

			while (element != null)
			{
				cursor = element.GetValue(Cursor.CursorProperty);
				element = element.LogicalParent;

				if (cursor != null)
					break;
			}

			cursor = cursor ?? Cursor.Arrow;
			cursor.Draw(spriteBatch, Position);
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