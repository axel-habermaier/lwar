using System;

namespace Pegasus.Framework.Platform.Input
{
	/// <summary>
	///   Provides information about mouse button press and release events.
	/// </summary>
	public struct MouseEventArgs
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="button">The mouse button that was pressed or released.</param>
		/// <param name="x">The mouse position along the X axis when the button was pressed or released.</param>
		/// <param name="y">The mouse position along the Y axis when the button was pressed or released.</param>
		public MouseEventArgs(MouseButton button, int x, int y)
			: this()
		{
			Button = button;
			X = x;
			Y = y;
		}

		/// <summary>
		///   Gets the mouse button that was pressed or released.
		/// </summary>
		public MouseButton Button { get; private set; }

		/// <summary>
		///   Gets the mouse position along the X axis when the button was pressed or released.
		/// </summary>
		public int X { get; private set; }

		/// <summary>
		///   Gets the mouse position along the Y axis when the button was pressed or released.
		/// </summary>
		public int Y { get; private set; }
	}
}