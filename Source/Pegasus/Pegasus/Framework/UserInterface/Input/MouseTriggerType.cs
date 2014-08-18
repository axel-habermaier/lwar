namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Determines the type of a mouse input trigger.
	/// </summary>
	public enum MouseTriggerType
	{
		/// <summary>
		///     Indicates that the mouse trigger triggers when the mouse button is released.
		/// </summary>
		Released,

		/// <summary>
		///     Indicates that the mouse trigger triggers when the mouse button is pressed.
		/// </summary>
		Pressed,

		/// <summary>
		///     Indicates that the mouse trigger triggers when the mouse button went down.
		/// </summary>
		WentDown,

		/// <summary>
		///     Indicates that the mouse trigger triggers when the mouse button went up.
		/// </summary>
		WentUp,

		/// <summary>
		///     Indicates that the mouse trigger triggers when the mouse button has been double-clicked.
		/// </summary>
		DoubleClicked
	}
}