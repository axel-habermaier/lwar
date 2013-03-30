using System;

namespace Pegasus.AssetsCompiler.Effects.Math
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a 4x4 matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class Matrix
	{
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

		/// <summary>
		///   Gets the element of the matrix at the given column and the given row.
		/// </summary>
		/// <param name="column">The column index of the element that should be returned.</param>
		/// <param name="row">The row index of the element that should be returned.</param>
		public float this[int column, int row]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}