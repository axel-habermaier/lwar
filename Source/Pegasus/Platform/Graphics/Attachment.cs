namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Maps a texture to an attachment point of a render target.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Attachment
	{
		/// <summary>
		///   The attachment point the texture is attached to.
		/// </summary>
		public AttachmentPoint AttachmentPoint;

		/// <summary>
		///   The texture that is attached to a render target.
		/// </summary>
		public Texture Texture;
	}
}