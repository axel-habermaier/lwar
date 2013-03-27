using System;

namespace Pegasus.AssetsCompiler.Effects
{
	/// <summary>
	///   When applied to a method, indicates that the method should be cross-compiled into a fragment shader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class FragmentShaderAttribute : Attribute
	{
	}
}