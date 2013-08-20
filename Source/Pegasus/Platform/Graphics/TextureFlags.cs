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
		///   Indicates that the texture can be attached as a color buffer of a render target.
		/// </summary>
		RenderTarget = 2,

		/// <summary>
		///   Indicates that the texture can be attached as a depth stencil buffer of a render target.
		/// </summary>
		DepthStencil = 4,
	}
}