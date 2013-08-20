using System;

namespace Pegasus.Scripting
{
	using Platform;

	/// <summary>
	///   When applied to a cvar property or command method in a registry specification interface, indicates that the cvar or
	///   command can only be set or invoked by the system and not via the console.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	[MeansImplicitUse]
	public class SystemOnlyAttribute : Attribute
	{
	}
}