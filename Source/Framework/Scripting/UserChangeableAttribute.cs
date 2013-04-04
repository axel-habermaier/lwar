using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   When applied to a cvar property in a registry specification interface, indicates that the cvar can be changed by the
	///   user using the in-app console.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	[MeansImplicitUse]
	public class UserChangeableAttribute : Attribute
	{
	}
}