using System;

namespace Pegasus.AssetsCompiler.Effects.Semantics
{
	/// <summary>
	///   Indicates that a shader argument or return value represents a position.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class PositionAttribute : Attribute
	{
	}
}