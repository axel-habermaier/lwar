using System;

namespace Pegasus.AssetsCompiler.Effects.Math
{
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public partial struct Vector3
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector3(float x, float y, float z)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Implicitely converts a four-component vector to a three-component one.
		/// </summary>
		public static implicit operator Vector3(Vector4 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector3 operator *(Vector3 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector3 operator *(float f, Vector3 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Divides a vector by a scalar value.
		/// </summary>
		public static Vector3 operator /(Vector3 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Adds two vectors.
		/// </summary>
		public static Vector3 operator +(Vector3 v1, Vector3 v2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Subtracts a vector from another.
		/// </summary>
		public static Vector3 operator -(Vector3 v1, Vector3 v2)
		{
			throw new NotImplementedException();
		}
	}
}