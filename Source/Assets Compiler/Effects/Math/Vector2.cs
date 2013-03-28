﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Math
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a two-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial struct Vector2
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector2(float x, float y)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector2 operator *(Vector2 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Multiplies a vector by a scalar value.
		/// </summary>
		public static Vector2 operator *(float f, Vector2 v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Divides a vector by a scalar value.
		/// </summary>
		public static Vector2 operator /(Vector2 v, float f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Adds two vectors.
		/// </summary>
		public static Vector2 operator +(Vector2 v1, Vector2 v2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Subtracts a vector from another.
		/// </summary>
		public static Vector2 operator -(Vector2 v1, Vector2 v2)
		{
			throw new NotImplementedException();
		}
	}
}