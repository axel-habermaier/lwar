using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Indicates which blend function should be used.
	/// </summary>
	public enum BlendOption
	{
		Zero = 1101,
		One = 1102,
		SourceColor = 1103,
		InverseSourceColor = 1104,
		SourceAlpha = 1105,
		InverseSourceAlpha = 1106,
		DestinationAlpha = 1107,
		InverseDestinationAlpha = 1108,
		DestinationColor = 1109,
		InverseDestinationColor = 1110,
		SourceAlphaSaturate = 1111,
		BlendFactor = 1112,
		InverseBlendFactor = 1113,
	}
}