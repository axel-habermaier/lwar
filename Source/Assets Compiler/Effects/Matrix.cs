using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a 4x4 matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class Matrix
	{
		/// <summary>
		///   Multiplies each element of the matrix with the scalar value.
		/// </summary>
		/// <param name="matrix">The matrix that should be multiplied.</param>
		/// <param name="value">The value each element of the matrix should be multiplied with.</param>
		public static Vector4 operator *(Matrix matrix, float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies each element of the matrix with the scalar value.
		/// </summary>
		/// <param name="matrix">The matrix that should be multiplied.</param>
		/// <param name="value">The value each element of the matrix should be multiplied with.</param>
		public static Vector4 operator *(float value, Matrix matrix)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies the matrix with the vector.
		/// </summary>
		/// <param name="matrix">The matrix that should be multiplied.</param>
		/// <param name="vector">The vector that should be multiplied.</param>
		public static Vector4 operator *(Matrix matrix, Vector4 vector)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies two matrices.
		/// </summary>
		/// <param name="matrix1">The first matrix that should be multiplied.</param>
		/// <param name="matrix2">The second matrix that should be multiplied.</param>
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Performs an element-wise addition.
		/// </summary>
		/// <param name="matrix1">The first matrix that should be added.</param>
		/// <param name="matrix2">The second matrix that should be added.</param>
		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Performs an element-wise subtraction.
		/// </summary>
		/// <param name="matrix1">The matrix the second matrix should be subtracted from.</param>
		/// <param name="matrix2">The matrix that should be subtracted.</param>
		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
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