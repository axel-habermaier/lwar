namespace Pegasus.AssetsCompiler.Textures
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Describes the metadata of a texture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct TextureDescription
	{
		/// <summary>
		///     The width of the texture.
		/// </summary>
		public int Width;

		/// <summary>
		///     The height of the texture.
		/// </summary>
		public int Height;

		/// <summary>
		///     The depth of the texture.
		/// </summary>
		public int Depth;

		/// <summary>
		///     The number of elements of the texture array.
		/// </summary>
		public int ArraySize;

		/// <summary>
		///     The type of the texture.
		/// </summary>
		public TextureType Type;

		/// <summary>
		///     The format of the texture.
		/// </summary>
		public SurfaceFormat Format;

		/// <summary>
		///     The number of mipmap levels.
		/// </summary>
		public int Mipmaps;

		/// <summary>
		///     The total number of surfaces, including all faces and mipmaps.
		/// </summary>
		/// <remarks>
		///     Cube maps have an array size of 1, hence the surface count is not necessarily equal to the array size times the
		///     levels of mipmaps.
		/// </remarks>
		public int SurfaceCount;
	}
}