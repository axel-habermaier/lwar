using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a mipmap of a texture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Mipmap
	{
		/// <summary>
		///   The width of the mipmap.
		/// </summary>
		public int Width;

		/// <summary>
		///   The height of the mipmap.
		/// </summary>
		public int Height;

		/// <summary>
		///   The size of the mipmap in bytes.
		/// </summary>
		public int Size;

		/// <summary>
		///   The mipmap data.
		/// </summary>
		public byte[] Data;

		/// <summary>
		///   The mipmap level, where 0 represents the base texture.
		/// </summary>
		public int Level;
	}
}