namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Describes the trigger mode of an input binding.
	/// </summary>
	public enum TriggerMode
	{
		/// <summary>
		///     The input binding is triggered when the input is activated.
		/// </summary>
		Activated,

		/// <summary>
		///     The input binding is triggered when the input is deactivated.
		/// </summary>
		Deactivated
	}
}