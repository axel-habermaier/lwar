namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates which blend function should be used.
	/// </summary>
	public enum BlendOption
	{
		Zero,
		One,
		SourceColor,
		InverseSourceColor,
		SourceAlpha,
		InverseSourceAlpha,
		DestinationAlpha,
		InverseDestinationAlpha,
		DestinationColor,
		InverseDestinationColor,
		SourceAlphaSaturate,
		BlendFactor,
		InverseBlendFactor,
	}
}