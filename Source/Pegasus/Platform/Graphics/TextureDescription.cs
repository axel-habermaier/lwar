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
		///   The number of mipmap levels.
		/// </summary>
		public uint Mipmaps;

		/// <summary>
		///   The total number of surfaces, including all faces and mipmaps.
		/// </summary>
		/// <remarks>
		///   Cube maps have an array size of 1, hence the surface count is not necessarily equal to the array size times the
		///   levels of mipmaps.
		/// </remarks>
		public uint SurfaceCount;

		/// <summary>
		///   The texture flags that affect the texture.
		/// </summary>
		public TextureFlags Flags;
	}
}