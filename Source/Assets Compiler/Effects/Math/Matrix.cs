using System;

namespace Pegasus.AssetsCompiler.Effects.Math
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a 4x4 matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix
	{
		/// <summary>
		///   The value at row 1 column 1 of the matrix.
		/// </summary>
		public float M11;

		/// <summary>
		///   The value at row 1 column 2 of the matrix.
		/// </summary>
		public float M12;

		/// <summary>
		///   The value at row 1 column 3 of the matrix.
		/// </summary>
		public float M13;

		/// <summary>
		///   The value at row 1 column 4 of the matrix.
		/// </summary>
		public float M14;

		/// <summary>
		///   The value at row 2 column 1 of the matrix.
		/// </summary>
		public float M21;

		/// <summary>
		///   The value at row 2 column 2 of the matrix.
		/// </summary>
		public float M22;

		/// <summary>
		///   The value at row 2 column 3 of the matrix.
		/// </summary>
		public float M23;

		/// <summary>
		///   The value at row 2 column 4 of the matrix.
		/// </summary>
		public float M24;

		/// <summary>
		///   The value at row 3 column 1 of the matrix.
		/// </summary>
		public float M31;

		/// <summary>
		///   The value at row 3 column 2 of the matrix.
		/// </summary>
		public float M32;

		/// <summary>
		///   The value at row 3 column 3 of the matrix.
		/// </summary>
		public float M33;

		/// <summary>
		///   The value at row 3 column 4 of the matrix.
		/// </summary>
		public float M34;

		/// <summary>
		///   The value at row 4 column 1 of the matrix.
		/// </summary>
		public float M41;

		/// <summary>
		///   The value at row 4 column 2 of the matrix.
		/// </summary>
		public float M42;

		/// <summary>
		///   The value at row 4 column 3 of the matrix.
		/// </summary>
		public float M43;

		/// <summary>
		///   The value at row 4 column 4 of the matrix.
		/// </summary>
		public float M44;

		/// <summary>
		///   Multiplies a matrix with a vector.
		/// </summary>
		public static Vector4 operator *(Matrix m, Vector4 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies two matrices.
		/// </summary>
		public static Matrix operator *(Matrix m1, Matrix m2)
		{
			throw new NotImplementedException();
		}
	}
}