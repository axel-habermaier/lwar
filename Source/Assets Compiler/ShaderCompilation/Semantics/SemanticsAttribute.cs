using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.Semantics
{
	/// <summary>
	///   Describes the semantics of a shader argument or return value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public abstract class SemanticsAttribute : Attribute
	{
	}
}