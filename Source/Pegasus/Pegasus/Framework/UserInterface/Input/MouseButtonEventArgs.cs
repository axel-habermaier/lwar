namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Math;

	/// <summary>
	///     Provides information about mouse button press and release events.
	/// </summary>
	public sealed class MouseButtonEventArgs : MouseEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly MouseButtonEventArgs CachedInstance = new MouseButtonEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private MouseButtonEventArgs()
		{
		}

		/// <summary>
		///     Gets the mouse button that was pressed or released.
		/// </summary>
		public MouseButton Button { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the mouse press was a double click.
		/// </summary>
		public bool DoubleClick { get; private set; }

		/// <summary>
		///     Gets the state of the mouse button that was pressed or released.
		/// </summary>
		public InputState ButtonState
		{
			get { return InputStates[(int)Button]; }
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="mouse">The mouse device that raised the event.</param>
		/// <param name="position">The position of the mouse at the time the event was generated.</param>
		/// <param name="inputStates">The states of the mouse buttons.</param>
		/// <param name="button">The mouse button that was pressed or released.</param>
		/// <param name="doubleClick">Indicates whether the mouse press was a double click.</param>
		/// <param name="modifiers">The key modifiers that were pressed when the event was raised.</param>
		internal static MouseButtonEventArgs Create(Mouse mouse, Vector2 position, InputState[] inputStates, MouseButton button, bool doubleClick,
													KeyModifiers modifiers)
		{
			Assert.ArgumentNotNull(mouse);
			Assert.ArgumentInRange(button);
			Assert.ArgumentNotNull(inputStates);

			CachedInstance.Reset();
			CachedInstance.Position = position;
			CachedInstance.NormalizedPosition = mouse.NormalizePosition(position);
			CachedInstance.InputStates = inputStates;
			CachedInstance.Button = button;
			CachedInstance.DoubleClick = doubleClick;
			CachedInstance.Modifiers = modifiers;

			return CachedInstance;
		}
	}
}