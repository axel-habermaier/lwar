using System;

namespace Pegasus.AssetsCompiler.Effects.Semantics
{
	/// <summary>
	///   Describes the semantics of a shader argument or return value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public abstract class SemanticsAttribute : Attribute
	{
	}
}