using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Specifies the attachment point where a texture is attached to a render target.
	/// </summary>
	public enum AttachmentPoint
	{
		/// <summary>
		///   Indicates that the texture acts as the depth stencil buffer of the render target.
		/// </summary>
		DepthStencil = 3001,

		/// <summary>
		///   Indicates that the texture acts as the first color buffer of the render target.
		/// </summary>
		Color0 = 3002,

		/// <summary>
		///   Indicates that the texture acts as the second color buffer of the render target.
		/// </summary>
		Color1 = 3003,

		/// <summary>
		///   Indicates that the texture acts as the third color buffer of the render target.
		/// </summary>
		Color2 = 3004,

		/// <summary>
		///   Indicates that the texture acts as the fourth color buffer of the render target.
		/// </summary>
		Color3 = 3005
	}
}