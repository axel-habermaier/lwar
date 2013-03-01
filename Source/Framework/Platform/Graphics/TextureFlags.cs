using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Specifies what operations are supported on a texture.
	/// </summary>
	[Flags]
	public enum TextureFlags
	{
		/// <summary>
		///   Indicates that the mipmap levels below the base mipmap can be automatically generated.
		/// </summary>
		GenerateMipmaps = 1,

		/// <summary>
		///   Indicates that the texture can be attached to a render target.
		/// </summary>
		Renderable = 2
	}
}