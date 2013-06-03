using System;

namespace Pegasus.Framework.Scripting
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
		/// <param name="mode">The update mode of the cvar.</param>
		public CvarAttribute(UpdateMode mode = UpdateMode.Immediate)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="defaultValue">The default value of the cvar.</param>
		/// <param name="mode">The update mode of the cvar.</param>
		public CvarAttribute(object defaultValue, UpdateMode mode = UpdateMode.Immediate)
		{
		}

		/// <summary>
		///   Gets or sets the cvar's default value as a string. This property should only be used if the default value cannot be
		///   specified as a C# constant.
		/// </summary>
		public string DefaultExpression { get; set; }
	}
}