namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes a set of properties of an OpenGL context.
	/// </summary>
	[Flags]
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal enum ContextFlags
	{
		Debug = 0x1,
		ForwardCompatible = 0x2,
		RobustAccess = 0x4,
		ResetIsolation = 0x8
	}
}