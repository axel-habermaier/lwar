﻿namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Indicates the direction of the data flow in a data binding.
	/// </summary>
	public enum BindingMode
	{
		/// <summary>
		///     Indicates that the data flows from the source to the target only.
		/// </summary>
		OneWay = 1,

		/// <summary>
		///     Indicates that the data flows from the target to the source only.
		/// </summary>
		OneWayToSource,

		/// <summary>
		///     Indicates that the data flows from the source to the target and vice versa.
		/// </summary>
		TwoWay
	}
}