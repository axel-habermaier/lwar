namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Indicates the direction of the data flow in a data binding.
	/// </summary>
	public enum BindingMode
	{
		/// <summary>
		///     Indicates that the target dependency property's default binding mode is used.
		/// </summary>
		Default,

		/// <summary>
		///     Indicates that the data flows from the source to the target only.
		/// </summary>
		OneWay,

		/// <summary>
		///     Indicates that the data flows from the target to the source only.
		/// </summary>
		OneWayToSource,

		/// <summary>
		///     Indicates that the data flows from the source to the target and vice versa.
		/// </summary>
		TwoWay,

		/// <summary>
		///     Indicates that the binding occurs only once and subsequent changes to the source do not update the target.
		/// </summary>
		OneTime
	}
}