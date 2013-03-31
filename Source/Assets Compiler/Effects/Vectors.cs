using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a two-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial class Vector2
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector2(float x, float y)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///   Represents a three-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial class Vector3
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Vector3(float x, float y, float z)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///   Represents a four-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial class Vector4
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
		public Vector4(Vector3 xyz, float w)
		{
			throw new NotImplementedException();
		}
	}
}