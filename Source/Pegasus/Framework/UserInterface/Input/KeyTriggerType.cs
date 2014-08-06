namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Determines the type of a key input trigger.
	/// </summary>
	internal enum KeyTriggerType
	{
		/// <summary>
		///     Indicates that the key trigger triggers when the key is released.
		/// </summary>
		Released,

		/// <summary>
		///     Indicates that the key trigger triggers when the key is pressed.
		/// </summary>
		Pressed,

		/// <summary>
		///     Indicates that the key trigger triggers when the key is repeated.
		/// </summary>
		Repeated,

		/// <summary>
		///     Indicates that the key trigger triggers when the key went down.
		/// </summary>
		WentDown,

		/// <summary>
		///     Indicates that the key trigger triggers when the key went up.
		/// </summary>
		WentUp
	}
}