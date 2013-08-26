using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	/// <summary>
	///   When applied to a property of an UI class, specifies that the property should be ignored at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class IgnoreAtRuntimeAttribute : Attribute
	{
	}
}