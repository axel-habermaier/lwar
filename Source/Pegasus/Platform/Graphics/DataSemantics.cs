using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Describes the usage scenario of an input element.
	/// </summary>
	public enum DataSemantics
	{
		/// <summary>
		///   Indicates that the data describes positions.
		/// </summary>
		Position = 0,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 0.
		/// </summary>
		Color0 = 1,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 1.
		/// </summary>
		Color1 = 2,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 2.
		/// </summary>
		Color2 = 3,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 3.
		/// </summary>
		Color3 = 4,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 0.
		/// </summary>
		TexCoords0 = 5,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 1.
		/// </summary>
		TexCoords1 = 6,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 2.
		/// </summary>
		TexCoords2 = 7,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 3.
		/// </summary>
		TexCoords3 = 8,

		/// <summary>
		///   Indicates that the data describes normals.
		/// </summary>
		Normal = 9
	}
}