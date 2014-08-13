namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Math;

	/// <summary>
	///     Provides information about mouse events.
	/// </summary>
	public class MouseEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly MouseEventArgs CachedInstance = new MouseEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected MouseEventArgs()
		{
		}

		/// <summary>
		///     Gets or sets the states of the mouse buttons.
		/// </summary>
		protected InputState[] InputStates { get; set; }

		/// <summary>
		///     Gets the mouse device that raised the event.
		/// </summary>
		public Mouse Mouse { get; protected set; }

		/// <summary>
		///     Gets the position of the mouse at the time the event was generated.
		/// </summary>
		public Vector2i Position { get; protected set; }

		/// <summary>
		///     Gets the position of the mouse at the time the event was generated normalized in both directions to the range -1..1 such
		///     that the origin lies at the center of the window.
		/// </summary>
		public Vector2 NormalizedPosition
		{
			get
			{
				var size = Mouse.Window.Size;
				var x = Position.X - size.Width / 2;
				var y = Position.Y - size.Height / 2;

				// It's important to do the division and conversion to float now and not earlier; otherwise,
				// there would be some drift if the width or height of the window is uneven.
				// This implementation matches what the platform library does when resetting the cursor to the
				// center of the window when the cursor is captured.
				return new Vector2(x / (float)size.Width, y / (float)size.Height);
			}
		}

		/// <summary>
		///     Gets the set of key modifiers that was pressed when the event was raised.
		/// </summary>
		public KeyModifiers Modifiers { get; protected set; }

		/// <summary>
		///     Gets the state of the left mouse button.
		/// </summary>
		public InputState LeftButton
		{
			get { return InputStates[(int)MouseButton.Left]; }
		}

		/// <summary>
		///     Gets the state of the right mouse button.
		/// </summary>
		public InputState RightButton
		{
			get { return InputStates[(int)MouseButton.Right]; }
		}

		/// <summary>
		///     Gets the state of the middle mouse button.
		/// </summary>
		public InputState MiddleButton
		{
			get { return InputStates[(int)MouseButton.Middle]; }
		}

		/// <summary>
		///     Gets the state of the first extended mouse button.
		/// </summary>
		public InputState XButton1
		{
			get { return InputStates[(int)MouseButton.XButton1]; }
		}

		/// <summary>
		///     Gets the state of the second extended mouse button.
		/// </summary>
		public InputState XButton2
		{
			get { return InputStates[(int)MouseButton.XButton2]; }
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="mouse">The mouse device that raised the event.</param>
		/// <param name="position">The position of the mouse at the time the event was generated.</param>
		/// <param name="inputStates">The states of the mouse buttons.</param>
		/// <param name="modifiers">The key modifiers that were pressed when the event was raised.</param>
		public static MouseEventArgs Create(Mouse mouse, Vector2i position, InputState[] inputStates, KeyModifiers modifiers)
		{
			Assert.ArgumentNotNull(mouse);
			Assert.ArgumentNotNull(inputStates);

			CachedInstance.Reset();
			CachedInstance.Mouse = mouse;
			CachedInstance.Position = position;
			CachedInstance.InputStates = inputStates;
			CachedInstance.Modifiers = modifiers;

			return CachedInstance;
		}
	}
}