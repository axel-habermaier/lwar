namespace Pegasus.AssetsCompiler.Textures
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents the surface of a texture, i.e., a single mipmap and/or face of a texture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Surface
	{
		/// <summary>
		///     The width of the surface.
		/// </summary>
		public int Width;

		/// <summary>
		///     The height of the surface.
		/// </summary>
		public int Height;

		/// <summary>
		///     The depth of the surface.
		/// </summary>
		public int Depth;

		/// <summary>
		///     The size of the surface data in bytes.
		/// </summary>
		public int Size;

		/// <summary>
		///     The stride between two rows of the surface in bytes.
		/// </summary>
		public int Stride;

		/// <summary>
		///     The surface data.
		/// </summary>
		public byte[] Data;
	}
}