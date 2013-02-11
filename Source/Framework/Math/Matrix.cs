using System;

namespace Pegasus.Framework.Math
{
	using System.Runtime.InteropServices;
	using Math = System.Math;

	/// <summary>
	///   Represents a 4x4 matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix : IEquatable<Matrix>
	{
		/// <summary>
		///   The identity matrix.
		/// </summary>
		public static readonly Matrix Identity = new Matrix { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f, M44 = 1.0f };

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
		///   Gets or sets the first row of the matrix (M11, M12, M13, and M14).
		/// </summary>
		public Vector4 Row1
		{
			get { return new Vector4(M11, M12, M13, M14); }
			set
			{
				M11 = value.X;
				M12 = value.Y;
				M13 = value.Z;
				M14 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the second row of the matrix (M21, M22, M23, and M24).
		/// </summary>
		public Vector4 Row2
		{
			get { return new Vector4(M21, M22, M23, M24); }
			set
			{
				M21 = value.X;
				M22 = value.Y;
				M23 = value.Z;
				M24 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the third row of the matrix (M31, M32, M33, and M34).
		/// </summary>
		public Vector4 Row3
		{
			get { return new Vector4(M31, M32, M33, M34); }
			set
			{
				M31 = value.X;
				M32 = value.Y;
				M33 = value.Z;
				M34 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the fourth row of the matrix (M41, M42, M43, and M44).
		/// </summary>
		public Vector4 Row4
		{
			get { return new Vector4(M41, M42, M43, M44); }
			set
			{
				M41 = value.X;
				M42 = value.Y;
				M43 = value.Z;
				M44 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the first column of the matrix (M11, M21, M31, and M41).
		/// </summary>
		public Vector4 Column1
		{
			get { return new Vector4(M11, M21, M31, M41); }
			set
			{
				M11 = value.X;
				M21 = value.Y;
				M31 = value.Z;
				M41 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the second column of the matrix (M12, M22, M32, and M42).
		/// </summary>
		public Vector4 Column2
		{
			get { return new Vector4(M12, M22, M32, M42); }
			set
			{
				M12 = value.X;
				M22 = value.Y;
				M32 = value.Z;
				M42 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the third column of the matrix (M13, M23, M33, and M43).
		/// </summary>
		public Vector4 Column3
		{
			get { return new Vector4(M13, M23, M33, M43); }
			set
			{
				M13 = value.X;
				M23 = value.Y;
				M33 = value.Z;
				M43 = value.W;
			}
		}

		/// <summary>
		///   Gets or sets the fourth column of the matrix (M14, M24, M34, and M44).
		/// </summary>
		public Vector4 Column4
		{
			get { return new Vector4(M14, M24, M34, M44); }
			set
			{
				M14 = value.X;
				M24 = value.Y;
				M34 = value.Z;
				M44 = value.W;
			}
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="value">The value that should be assigned to all components.</param>
		public Matrix(float value)
		{
			M11 = M12 = M13 = M14 = value;
			M21 = M22 = M23 = M24 = value;
			M31 = M32 = M33 = M34 = value;
			M41 = M42 = M43 = M44 = value;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="m11">The value of row 1, column 1 of the matrix.</param>
		/// <param name="m12">The value of row 1, column 2 of the matrix.</param>
		/// <param name="m13">The value of row 1, column 3 of the matrix.</param>
		/// <param name="m14">The value of row 1, column 4 of the matrix.</param>
		/// <param name="m21">The value of row 2, column 1 of the matrix.</param>
		/// <param name="m22">The value of row 2, column 2 of the matrix.</param>
		/// <param name="m23">The value of row 2, column 3 of the matrix.</param>
		/// <param name="m24">The value of row 2, column 4 of the matrix.</param>
		/// <param name="m31">The value of row 3, column 1 of the matrix.</param>
		/// <param name="m32">The value of row 3, column 2 of the matrix.</param>
		/// <param name="m33">The value of row 3, column 3 of the matrix.</param>
		/// <param name="m34">The value of row 3, column 4 of the matrix.</param>
		/// <param name="m41">The value of row 4, column 1 of the matrix.</param>
		/// <param name="m42">The value of row 4, column 2 of the matrix.</param>
		/// <param name="m43">The value of row 4, column 3 of the matrix.</param>
		/// <param name="m44">The value of row 4, column 4 of the matrix.</param>
		public Matrix(float m11, float m12, float m13, float m14,
					  float m21, float m22, float m23, float m24,
					  float m31, float m32, float m33, float m34,
					  float m41, float m42, float m43, float m44)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		/// <summary>
		///   Creates a perspective projection matrix.
		/// </summary>
		/// <param name="left">The minimum x-value of the viewing volume.</param>
		/// <param name="right">The maximum x-value of the viewing volume.</param>
		/// <param name="bottom">The minimum y-value of the viewing volume.</param>
		/// <param name="top">The maximum y-value of the viewing volume.</param>
		/// <param name="znear">The minimum z-value of the viewing volume.</param>
		/// <param name="zfar">The maximum z-value of the viewing volume.</param>
		public static Matrix CreatePerspective(float left, float right, float bottom, float top, float znear, float zfar)
		{
			var width = right - left;
			var height = bottom - top;
			var depth = zfar - znear;

			var a = (right + left) / width;
			var b = (top + bottom) / height;
			var c = -((zfar + znear) / depth);
			var d = -((2 * zfar * znear) / depth);

			return new Matrix
			{
				M11 = 2 * znear / width,
				M13 = a,
				M22 = 2 * znear / height,
				M23 = b,
				M33 = c,
				M34 = d,
				M43 = -1
			};
		}

		/// <summary>
		///   Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fieldOfView">The field of view in radians.</param>
		/// <param name="aspect">The aspect ratio, defined as view space width divided by view space height.</param>
		/// <param name="znear">The minimum z-value of the viewing volume.</param>
		/// <param name="zfar">The maximum z-value of the viewing volume.</param>
		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspect, float znear, float zfar)
		{
			var depth = znear - zfar;
			var f = 1.0f / (float)Math.Tan(fieldOfView * 0.5);

			return new Matrix
			{
				M11 = f / aspect,
				M22 = f,
				M33 = (zfar + znear) / depth,
				M34 = 2 * zfar * znear / depth,
				M43 = -1
			};
		}

		/// <summary>
		///   Creates a orthographic projection matrix.
		/// </summary>
		/// <param name="left">The minimum x-value of the viewing volume.</param>
		/// <param name="right">The maximum x-value of the viewing volume.</param>
		/// <param name="bottom">The minimum y-value of the viewing volume.</param>
		/// <param name="top">The maximum y-value of the viewing volume.</param>
		/// <param name="znear">The minimum z-value of the viewing volume.</param>
		/// <param name="zfar">The maximum z-value of the viewing volume.</param>
		public static Matrix CreateOrthographic(float left, float right, float bottom, float top, float znear,
												float zfar)
		{
			var zRange = 1.0f / (zfar - znear);

			var result = Identity;
			result.M11 = 2.0f / (right - left);
			result.M22 = 2.0f / (top - bottom);
			result.M33 = zRange;
			result.M41 = (left + right) / (left - right);
			result.M42 = (top + bottom) / (bottom - top);
			result.M43 = -znear * zRange;
			return result;
		}

		/// <summary>
		///   Creates a translation matrix.
		/// </summary>
		/// <param name="x">The translation offset in X-direction.</param>
		/// <param name="y">The translation offset in Y-direction.</param>
		/// <param name="z">The translation offset in Z-direction.</param>
		public static Matrix CreateTranslation(float x, float y, float z)
		{
			var matrix = Identity;
			matrix.M41 = x;
			matrix.M42 = y;
			matrix.M43 = z;
			return matrix;
		}

		/// <summary>
		///   Creates a translation matrix.
		/// </summary>
		/// <param name="translation">The translation offset in X, Y, and Z direction.</param>
		public static Matrix CreateTranslation(Vector3 translation)
		{
			return CreateTranslation(translation.X, translation.Y, translation.Z);
		}

		/// <summary>
		/// Creates a viewing matrix where the camera lies at the given position at looks at the given target.
		/// </summary>
		/// <param name="position">The position of the camera.</param>
		/// <param name="target">The target of the camera.</param>
		/// <param name="up">The up direction.</param>
		public static Matrix CreateLookAt(Vector3 position, Vector3 target, Vector3 up)
		{
			var f = (position - target).Normalize();
			var s = Vector3.Cross(up, f).Normalize();
			var u = Vector3.Cross(f, s);

			return new Matrix
			{
				M11 = s.X,
				M21 = s.Y,
				M31 = s.Z,
				M12 = u.X,
				M22 = u.Y,
				M32 = u.Z,
				M13 = f.X,
				M23 = f.Y,
				M33 = f.Z,
				M41 = -Vector3.Dot(s, position),
				M42 = -Vector3.Dot(u, position),
				M43 = -Vector3.Dot(f, position),
				M44 = 1
			};
		}

		/// <summary>
		///   Creates a matrix that rotates around the Z-axis.
		/// </summary>
		/// <param name="angle">The angle of the rotation, measured in radians.</param>
		public static Matrix CreateRotationX(float angle)
		{
			var matrix = Identity;
			var cos = (float)Math.Cos(angle);
			var sin = (float)Math.Sin(angle);

			matrix.M22 = cos;
			matrix.M23 = -sin;
			matrix.M32 = sin;
			matrix.M33 = cos;

			return matrix;
		}

		/// <summary>
		///   Creates a matrix that rotates around the Z-axis.
		/// </summary>
		/// <param name="angle">The angle of the rotation, measured in radians.</param>
		public static Matrix CreateRotationY(float angle)
		{
			var matrix = Identity;
			var cos = (float)Math.Cos(angle);
			var sin = (float)Math.Sin(angle);

			matrix.M11 = cos;
			matrix.M13 = sin;
			matrix.M31 = -sin;
			matrix.M33 = cos;

			return matrix;
		}

		/// <summary>
		///   Creates a matrix that rotates around the Z-axis.
		/// </summary>
		/// <param name="angle">The angle of the rotation, measured in radians.</param>
		public static Matrix CreateRotationZ(float angle)
		{
			var matrix = Identity;
			var cos = (float)Math.Cos(angle);
			var sin = (float)Math.Sin(angle);

			matrix.M11 = cos;
			matrix.M12 = -sin;
			matrix.M21 = sin;
			matrix.M22 = cos;

			return matrix;
		}

		/// <summary>
		///   Creates a matrix that uniformly changes the scale.
		/// </summary>
		/// <param name="scale">The scale in X-direction.</param>
		public static Matrix CreateScale(float scale)
		{
			return CreateScale(scale, scale, scale);
		}

		/// <summary>
		///   Creates a matrix that changes the scale.
		/// </summary>
		/// <param name="x">The scale in X-direction.</param>
		/// <param name="y">The scale in Y-direction.</param>
		/// <param name="z">The scale in Z-direction.</param>
		public static Matrix CreateScale(float x, float y, float z)
		{
			var matrix = Identity;
			matrix.M11 = x;
			matrix.M22 = y;
			matrix.M33 = z;
			return matrix;
		}

		/// <summary>
		///   Determines whether the given matrix is equal to this matrix.
		/// </summary>
		/// <param name="other">The other matrix to compare with this matrix.</param>
		public bool Equals(Matrix other)
		{
			return MathUtils.FloatEquality(M11, other.M11) &&
				   MathUtils.FloatEquality(M12, other.M12) &&
				   MathUtils.FloatEquality(M13, other.M13) &&
				   MathUtils.FloatEquality(M14, other.M14) &&
				   MathUtils.FloatEquality(M21, other.M21) &&
				   MathUtils.FloatEquality(M22, other.M22) &&
				   MathUtils.FloatEquality(M23, other.M23) &&
				   MathUtils.FloatEquality(M24, other.M24) &&
				   MathUtils.FloatEquality(M31, other.M31) &&
				   MathUtils.FloatEquality(M32, other.M32) &&
				   MathUtils.FloatEquality(M33, other.M33) &&
				   MathUtils.FloatEquality(M34, other.M34) &&
				   MathUtils.FloatEquality(M41, other.M41) &&
				   MathUtils.FloatEquality(M42, other.M42) &&
				   MathUtils.FloatEquality(M43, other.M43) &&
				   MathUtils.FloatEquality(M44, other.M44);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this matrix.
		/// </summary>
		/// <param name="value">The object to compare with this matrix.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Matrix)))
				return false;

			return Equals((Matrix)value);
		}

		/// <summary>
		///   Returns a hash code for this matrix.
		/// </summary>
		public override int GetHashCode()
		{
			return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() +
				   M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() +
				   M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() +
				   M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this matrix.
		/// </summary>
		public override string ToString()
		{
			return string.Format("[M11: {0}, M12: {1}, M13: {2}, M14: {3}] [M21: {4}, M22: {5}, M23: {6}, M24: {7}] " +
								 "[M31: {8}, M32: {9}, M33: {10}, M34: {11}] [M41: {12}, M42: {13}, M43: {14}, M44: {15}]",
								 M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
		}

		/// <summary>
		///   Tests for equality between two matrices.
		/// </summary>
		/// <param name="left">The first matrix to compare.</param>
		/// <param name="right">The second matrix to compare.</param>
		public static bool operator ==(Matrix left, Matrix right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two matrices.
		/// </summary>
		/// <param name="left">The first matrix to compare.</param>
		/// <param name="right">The second matrix to compare.</param>
		public static bool operator !=(Matrix left, Matrix right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a matrix addition.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		public static Matrix operator +(Matrix left, Matrix right)
		{
			return new Matrix(left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13, left.M14 + right.M14,
							  left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23, left.M24 + right.M24,
							  left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33, left.M34 + right.M34,
							  left.M41 + right.M41, left.M42 + right.M42, left.M43 + right.M43, left.M44 + right.M44);
		}

		/// <summary>
		///   Performs a matrix subtraction.
		/// </summary>
		/// <param name="left">The first matrix.</param>
		/// <param name="right">The second matrix.</param>
		public static Matrix operator -(Matrix left, Matrix right)
		{
			return new Matrix(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M14 - right.M14,
							  left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M24 - right.M24,
							  left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33, left.M34 - right.M34,
							  left.M41 - right.M41, left.M42 - right.M42, left.M43 - right.M43, left.M44 - right.M44);
		}

		/// <summary>
		///   Multiplies two matricies.
		/// </summary>
		/// <param name="left">The first matrix to multiply.</param>
		/// <param name="right">The second matrix to multiply.</param>
		public static Matrix operator *(Matrix left, Matrix right)
		{
			return new Matrix
			{
				M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41),
				M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42),
				M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43),
				M14 = (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44),
				M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41),
				M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42),
				M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43),
				M24 = (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44),
				M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41),
				M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42),
				M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43),
				M34 = (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44),
				M41 = (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41),
				M42 = (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42),
				M43 = (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43),
				M44 = (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44)
			};
		}
	}
}