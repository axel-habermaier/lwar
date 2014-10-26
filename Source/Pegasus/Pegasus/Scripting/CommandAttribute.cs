namespace Pegasus.Scripting
{
	using System;
	using Utilities;

	/// <summary>
	///     Must be applied to a method in a registry specification interface in order to indicate that the method represents
	///     a command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[MeansImplicitUse]
	public class CommandAttribute : Attribute
	{
	}
}