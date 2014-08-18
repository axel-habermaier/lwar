namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Specifies a set of key modifiers.
	/// </summary>
	[Flags]
	public enum KeyModifiers
	{
		/// <summary>
		///     Indicates that no modifier keys are pressed.
		/// </summary>
		None = 0,

		/// <summary>
		///     Indicates that the left or right 'alt' keys are pressed.
		/// </summary>
		Alt = 1,

		/// <summary>
		///     Indicates that the left or right 'control' keys are pressed.
		/// </summary>
		Control = 2,

		/// <summary>
		///     Indicates that the left or right 'shift' keys are pressed.
		/// </summary>
		Shift = 4
	}
}