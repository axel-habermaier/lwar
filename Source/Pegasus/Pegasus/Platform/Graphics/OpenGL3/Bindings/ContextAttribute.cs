namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents an attribute of an OpenGL context.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal enum ContextAttribute
	{
		RedSize,
		GreenSize,
		BlueSize,
		AlphaSize,
		BufferSize,
		Doublebuffer,
		DepthSize,
		StencilSize,
		AccumRedSize,
		AccumGreenSize,
		AccumBlueSize,
		AccumAlphaSize,
		Stereo,
		Multisamplebuffers,
		Multisamplesamples,
		AcceleratedVisual,
		RetainedBacking,
		MajorVersion,
		MinorVersion,
		ContextEgl,
		ContextFlags,
		ProfileMask,
		ShareWithCurrentContext,
		FramebufferSrgbCapable
	}
}