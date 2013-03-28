using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Represents a data type.
	/// </summary>
	internal enum DataType
	{
		/// <summary>
		///   Indicates that the data type is unknown or unsupported.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///   Represents a Boolean value.
		/// </summary>
		Boolean,

		/// <summary>
		///   Represents a 32-bit signed integer value.
		/// </summary>
		Integer,

		/// <summary>
		///   Represents a single floating point value.
		/// </summary>
		Float,

		/// <summary>
		///   Represents a two-component floating point vector.
		/// </summary>
		Vector2,

		/// <summary>
		///   Represents a three-component floating point vector.
		/// </summary>
		Vector3,

		/// <summary>
		///   Represents a four-component floating point vector.
		/// </summary>
		Vector4,

		/// <summary>
		///   Represents a 4x4 floating point matrix.
		/// </summary>
		Matrix,

		/// <summary>
		///   Represents a two-dimensional texture object.
		/// </summary>
		Texture2D,

		/// <summary>
		///   Represents a cubemap object.
		/// </summary>
		CubeMap
	}
}