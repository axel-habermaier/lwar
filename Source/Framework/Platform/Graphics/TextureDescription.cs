using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Describes the metadata of a texture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct TextureDescription
	{
		/// <summary>
		///   The width of the texture.
		/// </summary>
		public uint Width;

		/// <summary>
		///   The height of the texture.
		/// </summary>
		public uint Height;

		/// <summary>
		///   The depth of the texture.
		/// </summary>
		public uint Depth;

		/// <summary>
		///   The number of elements of the texture array.
		/// </summary>
		public uint ArraySize;

		/// <summary>
		///   The type of the texture.
		/// </summary>
		public TextureType Type;

		/// <summary>
		///   The format of the texture.
		/// </summary>
		public SurfaceFormat Format;

		/// <summary>
		///   Indicates whether and how the texture uses mipmaps.
		/// </summary>
		public Mipmaps Mipmaps;
	}
}