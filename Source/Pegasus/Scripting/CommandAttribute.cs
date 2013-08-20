using System;

namespace Pegasus.Scripting
{
	using Platform;

	/// <summary>
	///   Must be applied to a method in a registry specification interface in order to indicate that the method represents
	///   a command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[MeansImplicitUse]
	public class CommandAttribute : Attribute
	{
	}
}