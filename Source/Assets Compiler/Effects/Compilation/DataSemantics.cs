using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Describesthe semantics of a shader data object.
	/// </summary>
	internal enum DataSemantics
	{
		/// <summary>
		///   Indicates that the semantics of the data are unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///   Indicates that the data describes positions.
		/// </summary>
		Position,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 0.
		/// </summary>
		Color0,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 1.
		/// </summary>
		Color1,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 2.
		/// </summary>
		Color2,

		/// <summary>
		///   Indicates that the data describes colors at semantic index 3.
		/// </summary>
		Color3,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 0.
		/// </summary>
		TexCoords0,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 1.
		/// </summary>
		TexCoords1,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 2.
		/// </summary>
		TexCoords2,

		/// <summary>
		///   Indicates that the data describes texture coordinates at semantic index 3.
		/// </summary>
		TexCoords3,

		/// <summary>
		///   Indicates that the data describes normals.
		/// </summary>
		Normal
	}
}