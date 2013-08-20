using System;

namespace Pegasus.Scripting
{
	using Platform;

	/// <summary>
	///   Must be applied to a property in a registry specification interface in order to indicate that the property
	///   represents a cvar and to provide a default value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	[MeansImplicitUse]
	public class CvarAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="defaultValue">The default value of the cvar.</param>
		/// <param name="mode">The update mode of the cvar.</param>
		public CvarAttribute(object defaultValue, UpdateMode mode = UpdateMode.Immediate)
		{
		}
	}
}