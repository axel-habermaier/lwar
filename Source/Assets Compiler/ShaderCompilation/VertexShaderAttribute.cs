using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation
{
	/// <summary>
	///   When applied to a method, indicates that the method should be cross-compiled into a vertex shader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class VertexShaderAttribute : Attribute
	{
	}
}