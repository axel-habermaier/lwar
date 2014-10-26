namespace Pegasus.UserInterface.Input
{
	using System;

	/// <summary>
	///     Identifies a mouse button.
	/// </summary>
	public enum MouseButton
	{
		/// <summary>
		///     Represents an unknown mouse button.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///     Identifies the left mouse button.
		/// </summary>
		Left = 1,

		/// <summary>
		///     Identifies the right mouse button.
		/// </summary>
		Right = 2,

		/// <summary>
		///     Identifies the middle mouse button.
		/// </summary>
		Middle = 3,

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