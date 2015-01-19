namespace Pegasus.UserInterface.Input
{
	using System;

	/// <summary>
	///     Identifies a mouse button.
	/// </summary>
	public enum MouseButton : byte
	{
		/// <summary>
		///     Identifies the left mouse button.
		/// </summary>
		Left = 1,

		/// <summary>
		///     Identifies the middle mouse button.
		/// </summary>
		Middle = 2,

		/// <summary>
		///     Identifies the right mouse button.
		/// </summary>
		Right = 3,

		/// <summary>
		///     Identifies the first extra mouse button.
		/// </summary>
		XButton1 = 4,

		/// <summary>
		///     Identifies the second extra mouse button.
		/// </summary>
		XButton2 = 5
	};
}