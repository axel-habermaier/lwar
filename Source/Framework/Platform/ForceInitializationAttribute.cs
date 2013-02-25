using System;

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   When applied to a class, forces the class' type initializer to be executed at application startup.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ForceInitializationAttribute : Attribute
	{
	}
}