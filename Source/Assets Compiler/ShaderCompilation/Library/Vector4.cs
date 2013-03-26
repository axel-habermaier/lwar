﻿using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.Library
{
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public partial struct Vector4
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector4(float x, float y, float z, float w)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector4(Vector3 v, float w)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Implicitely converts a three-component vector to a four-component one.
		/// </summary>
		public static implicit operator Vector4(Vector3 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector4 operator *(Vector4 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector4 operator *(float f, Vector4 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Divides a vector by a scalar value.
		/// </summary>
		public static Vector4 operator /(Vector4 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Adds two vectors.
		/// </summary>
		public static Vector4 operator +(Vector4 v1, Vector4 v2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Subtracts a vector from another.
		/// </summary>
		public static Vector4 operator -(Vector4 v1, Vector4 v2)
		{
			throw new NotImplementedException();
		}
	}
}