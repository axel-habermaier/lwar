namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;
	using Utilities;

	/// <summary>
	///     Indicates the type of an OpenGL context.
	/// </summary>
	[Flags]
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal enum ContextProfile
	{
		Core = 0x1,
		Compatibility = 0x2,
		Es = 0x4
	}
}