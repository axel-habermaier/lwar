namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Math;

	/// <summary>
	///     Provides information about mouse button press and release events.
	/// </summary>
	public class MouseEventArgs : RoutedEventArgs<MouseEventArgs>
	{
		/// <summary>
		///     Gets the mouse button that was pressed or released.
		/// </summary>
		public MouseButton Button { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the mouse press was a double click.
		/// </summary>
		public bool DoubleClick { get; private set; }

		/// <summary>
		///     Gets the position of the mouse when the button was pressed or released.
		/// </summary>
		public Vector2i Position { get; private set; }

		/// <summary>
		///     Gets the state of the mouse button.
		/// </summary>
		public InputState State { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="button">The mouse button that was pressed or released.</param>
		/// <param name="doubleClick">Indicates whether the mouse press was a double click.</param>
		/// <param name="position">The position of the mouse at the time of the button press or release.</param>
		/// <param name="state">The state of the mouse button.</param>
		public static MouseEventArgs Create(MouseButton button, bool doubleClick, Vector2i position, InputState state)
		{
			Assert.ArgumentInRange(button);

			CachedInstance.Button = button;
			CachedInstance.DoubleClick = doubleClick;
			CachedInstance.Position = position;
			CachedInstance.State = state;

			return CachedInstance;
		}
	}
}