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
		///     Gets the position of the mouse at the time the event was generated.
		/// </summary>
		public Vector2i Position { get; protected set; }

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
		/// <param name="position">The position of the mouse at the time the event was generated.</param>
		/// <param name="inputStates">The states of the mouse buttons.</param>
		public static MouseEventArgs Create(Vector2i position, InputState[] inputStates)
		{
			Assert.ArgumentNotNull(inputStates);

			CachedInstance.Position = position;
			CachedInstance.InputStates = inputStates;

			return CachedInstance;
		}
	}
}