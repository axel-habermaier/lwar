namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Math;

	/// <summary>
	///     Provides information about mouse wheel events.
	/// </summary>
	public sealed class MouseWheelEventArgs : MouseEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly MouseWheelEventArgs CachedInstance = new MouseWheelEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private MouseWheelEventArgs()
		{
		}

		/// <summary>
		///     Gets a value that indicates the amount that the mouse wheel has changed.
		/// </summary>
		public int Delta { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the mouse at the time the event was generated.</param>
		/// <param name="inputStates">The states of the mouse buttons.</param>
		/// <param name="delta">A value indicating the amount the mouse wheel has changed.</param>
		public static MouseWheelEventArgs Create(Vector2i position, InputState[] inputStates, int delta)
		{
			Assert.ArgumentNotNull(inputStates);

			CachedInstance.Reset();
			CachedInstance.Position = position;
			CachedInstance.InputStates = inputStates;
			CachedInstance.Delta = delta;

			return CachedInstance;
		}
	}
}